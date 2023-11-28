<?php
require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";

include "log.php";


if($_SERVER["REQUEST_METHOD"] == "POST"){
        if(isset($_POST["customer"]) && isset($_POST["site"]) && isset($_POST["product"]) && isset($_POST["expirationdate"]) & isset($_POST["maxClients"]))
        {
            if (isset($_POST["allowSms"])) {
                $sms = 1;
            }
            else
            {
                $sms = 0;
            }

            $product = $_POST["product"];
            $customer = $_POST["customer"];
            $site = $_POST["site"];
            $expDate = $_POST["expirationdate"];
            $maxClients = intval($_POST["maxClients"]);

            if($chkClientsStmt = $conn->prepare("SELECT Name FROM products WHERE Name=?"))
            {
                $chkClientsStmt->bind_param("s", $product);
                $chkClientsStmt->execute();
                //$productsInDb = array();
                //$result = $stmt->get_result();
                $chkClientsStmt->store_result();
                
                if($chkClientsStmt->num_rows < 1)
                {
                    $_SESSION["errorResult"] = "Produkten finns ej.";
                }
            }

            if($maxClients < 1)
            {
                $_SESSION["errorResult"] = "Felaktigt format på Användargräns.";
            }

            $date = DateTime::createFromFormat('Y-m-d', $_POST["expirationdate"]);
            //$_SESSION["expDateError"] = false;
            if($date === FALSE)
            {
                //$_SESSION["expDateError"] = true;
                $_SESSION["errorResult"] = "Felaktigt format på Utgångsdatum.";
            }
            else if(checkdate($date->format("m"), $date->format("d"), $date->format("Y")) === false)
            {
                //$_SESSION["expDateError"] = true;
                $_SESSION["errorResult"] = "Felaktigt format på Utgångsdatum.";
            }
            else if(date_timestamp_get($date) - time() < 0)
            {
                //$_SESSION["expDateError"] = true;
                $_SESSION["errorResult"] = "Utgångsdatum har redan vart.";
            }

            if(empty($customerError) && !isset($_SESSION["errorResult"]))
            {
                $query = "INSERT INTO licenses (Product, Customer, Site, LicenseGUID, DateAdded, ExpirationDate, AddedBy, MaxCurrentUsers, SMS) VALUES (?,?,?,?,?,?,?,?,?)";
                if($stmt = $conn->prepare($query))
                {
                    $guid = GUID();
                    $stmt->bind_param("ssssssssi", $product, $customer, $site, $guid, date("Y-m-d"), $expDate, $_SESSION["username"], $maxClients, $sms);
                    
                    if($stmt->execute())
                    {
                        addToLog("LICENSE ADDED", $guid, $conn);
                        $_SESSION["successResult"] = "Licens tillagd!";
                    }
                }
            }

        header("location: ".LOC."/license");
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


?>