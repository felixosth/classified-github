<?php

$conn = new mysqli("192.168.2.64", "trygguser", "T9DG0Iz7nuqpCbPu", "tryggconnect_se");
if($conn->connect_error)
{
    die("DB connection failed: " . $conn->connect_error);
}



function checkLicense($conn, $mguid)
{
    $rows = 0;
    if($stmt = $conn->prepare("SELECT LicenseGUID, ExpirationDate FROM licenses WHERE Product=? AND MachineGUID=?"))
    {
        $product = "trygglarm";
        $stmt->bind_param("ss", $product, $mguid);
        $stmt->execute();
        $result = $stmt->get_result();
        while($row = $result->fetch_assoc())
        {
            if (time() <= strtotime($row["ExpirationDate"])) {
                return $row["LicenseGUID"];
            }
        }
    }

    return false;
}

?>