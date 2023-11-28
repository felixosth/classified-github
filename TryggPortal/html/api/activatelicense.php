<?php

if(isset($_POST["license"]) && isset($_POST["mguid"]) && isset($_POST["product"]))
{
    $machineGuid = $_POST["mguid"];
    $product = $_POST["product"];
    $license = $_POST["license"];
    require_once "../functions/newConfig.php";

    //$conn = new mysqli("localhost", "licenseinfo", "rS2c5dTUEfyRrc1A", "plugin_licenses");
    if($conn->connect_error)
    {
        die("DB connection failed: " . $conn->connect_error);
    }
    $query = "SELECT * FROM licenses WHERE LicenseGUID=? AND Product=? LIMIT 1";
    //$query = "SELECT * FROM licenses WHERE LicenseGUID='" . $license . "' AND Product='" . $product . "' LIMIT 1";
    //echo $query;
    if($stmt = $conn->prepare($query))
    {
        $stmt->bind_param("ss", $license, $product);
        $stmt->execute();
        $result = $stmt->get_result();
        $stmt->free_result();
        $stmt->close();
    }

    if (!$result) {
        trigger_error('Invalid query: ' . $conn->error);
        echo "-1";
    }
    else if($result->num_rows > 0)
    {
        while($row = $result->fetch_assoc()) {
            if(strtotime($row["ExpirationDate"]) - time() < 0)
            {
                echo "2";
            }
            else if($row["MachineGUID"] != "")
            {
                if($row["MachineGUID"] == $machineGuid)
                    echo "0";
                else
                    echo "1";
            }
            else
            {
                $query = "UPDATE licenses SET MachineGUID='" . $machineGuid . "', Product='" . $product . "' WHERE ID='" . $row["ID"] . "'";
                $newResult = $conn->query($query);
                if (!$newResult) {
                trigger_error('Invalid query: ' . $conn->error);
                }
                else
                    echo "0";
            }
        }
    }
    else
    {
        echo "3";
    }
    $conn->close();

    function fred($val)
    {
    echo '<pre>';
    print_r( $val );
    echo '</pre>';
    }
}


?>