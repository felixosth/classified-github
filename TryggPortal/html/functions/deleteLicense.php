<?php
require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";

if($_SERVER["REQUEST_METHOD"] == "POST"){
    if(isset($_POST["licenseguid"]))
    {
        //$query = "DELETE FROM licenses WHERE LicenseGUID='" .$_POST["licenseguid"] . "'"; 
        $query = "DELETE FROM licenses WHERE LicenseGUID=?";
        if($stmt = $conn->prepare($query))
        {
            $stmt->bind_param("s", $_POST["licenseguid"]);
            if($stmt->execute())
            {
                $_SESSION["successResult"] = "Licens borttagen.";
                include "log.php";
                addToLog("LICENSE DELETED", $_POST["licenseguid"], $conn);
            }
            else
                $_SESSION["errorResult"] = "Borttagning av licens misslyckades...";
        } 
        header("location: ../license");
    }
}
?>