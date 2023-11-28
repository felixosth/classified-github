<?php
require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";
require_once "../functions/log.php";

if($_SERVER["REQUEST_METHOD"] == "POST"){
    if(isset($_POST["customer"]) && isset($_POST["site"]) && isset($_POST["licenseguid"]) && isset($_POST["expirationdate"]))
    {
        $customer = $_POST["customer"];
        $site = $_POST["site"];
        $expDate = $_POST["expirationdate"];
        $licGuid = $_POST["licenseguid"];
        $maxClients = intval($_POST["maxClients"]);


        if($maxClients < 1)
        {
            $_SESSION["errorResult"] = "Felaktigt format på Användargräns.";
        }
        $date = DateTime::createFromFormat('Y-m-d', $expDate);
        
        //$_SESSION["expDateError"] = false;
        if($date === FALSE)
        {
            $_SESSION["errorResult"] = "Felaktigt format på Utgångsdatum.";
        }
        else if(checkdate($date->format("m"), $date->format("d"), $date->format("Y")) === false)
        {
            $_SESSION["errorResult"] = "Felaktigt format på Utgångsdatum.";
        }
        // else if(date_timestamp_get($date) - time() < 0)
        // {
        //     $_SESSION["errorResult"] = "Utgångsdatum har redan vart.";
        // }

        if(!isset($_SESSION["errorResult"]))
        {
            $query = "UPDATE licenses SET Customer=?, Site=?, ExpirationDate=?, MaxCurrentUsers=? WHERE LicenseGUID=?";
            if($stmt = $conn->prepare($query))
            {
                $stmt->bind_param("sssss", $customer, $site, $expDate, $maxClients, $licGuid);
                if($stmt->execute())
                {
                    $_SESSION["successResult"] = "Licens modifierad.";

                    addToLog("LICENSE MODIFIED", $licGuid, $conn);
                }
                else
                    $_SESSION["errorResult"] = "Modifiering av licens misslyckades...";
            }
        }
        //$query = "UPDATE licenses SET Customer='" .$_POST["customer"] . "', Site='" .$_POST["site"] . "', ExpirationDate='" . $_POST["expirationdate"] . "' WHERE LicenseGUID='" .$_POST["licenseguid"] . "'"; 

        header("location: ../license");
    }
}
?>