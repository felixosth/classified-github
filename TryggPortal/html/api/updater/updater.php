<?php
// App updater for c#
// ini_set('display_errors',1); 
// error_reporting(E_ALL);

require __DIR__ . '/../../functions/newConfig.php';
require __DIR__ . "/../../functions/log.php";

if($_SERVER["REQUEST_METHOD"] == "GET"){
    if(isset($_GET["branch"]) && isset($_GET["action"]))
    {
        $branch = $_GET["branch"];
        switch($_GET["action"])
        {
            case "getVersion":
                echo json_encode(getApp($branch, $conn));
                exit();
                break;
            case "getFile":
                if(isset($_GET["downloadKey"]))
                {
                    $dlk = $_GET["downloadKey"];
                    getFile($branch, $dlk, $conn);
                    exit;
                }
                break;
        }
    }
}
else if($_SERVER["REQUEST_METHOD"] == "POST")
{
    require_once __DIR__ . "/../../functions/checklogin.php";

    if(isset($_POST["action"]))
    {
        switch($_POST["action"])
        {
            case "createProduct":
                if(isset($_POST["productName"]) && isset($_POST["fileName"]) && isset($_POST["version"]))
                {
                    $insertId = createProduct($_POST["productName"], $_POST["version"], $_POST["fileName"], $conn);
                    if($insertId !== null)
                    {
                        addToLog("PRODUCT CREATED", $insertId, $conn);
                        header('Location: ' . $_SERVER['HTTP_REFERER']);
                    }
                }
                break;
                case "editProduct":
                    if(isset($_POST["delete"]) && isset($_POST["productId"]))
                    {
                        if(removeProduct($_POST["productId"], $conn))
                        {
                            addToLog("PRODUCT DELETED", $_POST["productId"], $conn);
                            header('Location: ' . $_SERVER['HTTP_REFERER']);
                        }
                    }
                    else if(isset($_POST["productId"]) && isset($_POST["productName"]) && isset($_POST["fileName"]) && isset($_POST["version"]))
                        if(editProduct($_POST["productId"], $_POST["productName"], $_POST["version"], $_POST["fileName"], $conn))
                            {
                            addToLog("PRODUCT EDITED", $_POST["productId"], $conn);
                            header('Location: ' . $_SERVER['HTTP_REFERER']);
                            }
                    break;
        }
    }
}

function removeProduct(int $id, $conn)
{
    if($stmt = $conn->prepare("DELETE FROM updater WHERE productId=?;"))
    {
        $stmt->bind_param("i", $id);
        return $stmt->execute();
    }
}

function editProduct(int $id, string $productName, string $version, string $fileName, $conn)
{
    if($stmt = $conn->prepare("UPDATE updater SET productName=?, `version`=?, fileName=? WHERE productId=?;"))
    {
        $stmt->bind_param("sssi", $productName, $version,$fileName, $id);
        return $stmt->execute();
    }
}

function createProduct(string $productName, string $version, string $fileName, $conn)
{
    $branch = GUID();
    $dlk = GUID();
    if($stmt = $conn->prepare("INSERT INTO updater (productName, `version`, branchGuid, downloadKey, fileName) VALUES (?, ?, ?, ?, ?);"))
    {
        $stmt->bind_param("sssss", $productName, $version, $branch, $dlk, $fileName);
        if($stmt->execute())
            return $stmt->insert_id;
    }
}

function GUID()
{
    if (function_exists('com_create_guid') === true)
    {
        return trim(com_create_guid(), '{}');
    }

    return sprintf('%04X%04X-%04X-%04X-%04X-%04X%04X%04X', mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(16384, 20479), mt_rand(32768, 49151), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535));
}

function getApp(string $branch, $conn)
{
    if($stmt = $conn->prepare("SELECT productName, `version` FROM updater WHERE branchGuid=?;"))
    {
        $stmt->bind_param("s", $branch);
        $stmt->execute();
        $result = $stmt->get_result();
        $stmt->free_result();
        $stmt->close();

        $row = $result->fetch_assoc();
        return $row;
    }
    return null;
}

function getFile(string $branch, string $downloadKey, $conn)
{
    $row = null;
    if($stmt = $conn->prepare("SELECT fileName FROM updater WHERE branchGuid=? AND downloadKey=?;"))
    {
        $stmt->bind_param("ss", $branch, $downloadKey);
        $stmt->execute();
        $result = $stmt->get_result();
        $stmt->free_result();
        $stmt->close();

        $row = $result->fetch_assoc();
    }

    $attachment_location = __DIR__."/files/" . $row["fileName"];
    if (file_exists($attachment_location)) {
        header($_SERVER["SERVER_PROTOCOL"] . " 200 OK");
        header("Cache-Control: public"); // needed for internet explorer
        header("Content-Type: application/zip");
        header("Content-Transfer-Encoding: Binary");
        header("Content-Length:".filesize($attachment_location));
        header("Content-Disposition: attachment; filename=file.zip");
        readfile($attachment_location);
        die();        
    } else {
        die("Error: File not found.");
    } 
}

?>