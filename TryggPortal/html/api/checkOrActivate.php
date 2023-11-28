<?php
require_once "../functions/newConfig.php";

if(isset($_POST["license"]) && isset($_POST["mguid"]) && isset($_POST["product"]))
{
    $machineGuid = $_POST["mguid"];
    $product = $_POST["product"];
    $license = $_POST["license"];
    $row = null;
    $result = array("result" => "", "error" => null, "license" => null);

    if($conn->connect_error)
    {
        die("DB connection failed: " . $conn->connect_error);
    }

    if($stmt = $conn->prepare("SELECT * FROM licenses WHERE LicenseGUID=? AND Product=? LIMIT 1"))
    {
        $stmt->bind_param("ss", $license, $product);
        $stmt->execute();
        $row = $stmt->get_result()->fetch_assoc();
        $stmt->free_result();
        $stmt->close();
    }

    if (!$result) {
        trigger_error('Invalid query: ' . $conn->error);
        echo "-1";
    }
    else if(isset($row))
    {
        if(strtotime($row["ExpirationDate"]) - time() < 0)
        {
            $result["result"] = "License expired bro";
            $result["error"] = "License expired";
        }
        else if($row["MachineGUID"] != "") // It is activated
        {
            if($row["MachineGUID"] == $machineGuid) {
                $result["result"] = "Here you go";
                $result["license"] = $row;
            }
            else
            {
                $result["result"] = "Not your license bro";
                $result["error"] = "No license found with provided mguid";
            }
        }
        else // activate it
        {
            if($stmt = $conn->prepare("UPDATE licenses SET MachineGUID=?, Product=? WHERE ID=?;"))
            {
                $stmt->bind_param("ssi", $machineGuid, $product, $row["ID"]);
                if($stmt->execute())
                {
                    $row["MachineGUID"] = $machineGuid;
                    $result["result"] = "License activated";
                    $result["license"] = $row;
                }
                else
                {
                    $result["error"] = "Something went wrong during activation";
                }
            }
        }
    }

    echo json_encode($result);

    $conn->close();
}
else
{
    http_response_code(400);
}


?>