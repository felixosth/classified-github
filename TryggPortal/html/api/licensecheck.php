<?php

if(isset($_POST["mguid"]) && isset($_POST["product"]))
{
    require_once "../functions/newConfig.php";
    $machineGuid = $_POST["mguid"];
    $product = $_POST["product"];
    //$conn = new mysqli("localhost", "licenseinfo", "rS2c5dTUEfyRrc1A", "plugin_licenses");
    if($conn->connect_error)
    {
        die("DB connection failed: " . $conn->connect_error);
    }

    $query = "SELECT ID, ExpirationDate FROM licenses WHERE MachineGUID=? and Product=? LIMIT 1";
    //$query = "SELECT ID, ExpirationDate FROM licenses WHERE MachineGUID='" . $machineGuid . "' LIMIT 1";
    if($stmt = $conn->prepare($query))
    {
        $stmt->bind_param("ss", $machineGuid, $product);
        $stmt->execute();
        $result = $stmt->get_result();
        $stmt->free_result();
        $stmt->close();
    }
    //$result = $conn->query($query);
    if (!$result) {
    trigger_error('Invalid query: ' . $conn->error);
    }
    else if($result->num_rows > 0)
    {
        while($row = $result->fetch_assoc()) {
            if(strtotime($row["ExpirationDate"]) - time() < 0)
                echo "1";
            else
                echo "0";
        }
    }
    else
    {
        echo "2";
    }

    $conn->close();
}
else
    echo "no post";



?>