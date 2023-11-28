<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);
if(isset($_POST["mguid"]) && isset($_POST["product"]))
{
    require_once "../functions/newConfig.php";
    //$conn = new mysqli("localhost", "licenseinfo", "rS2c5dTUEfyRrc1A", "plugin_licenses");
    if($conn->connect_error)
    {
        die("DB connection failed: " . $conn->connect_error);
    }
    $query = "SELECT * FROM licenses WHERE MachineGUID=? AND Product=?";
    //$query = "SELECT * FROM licenses WHERE MachineGUID='" . $_POST["mguid"] . "'";
    //echo $query;
    if($stmt = $conn->prepare($query))
    {
        $stmt->bind_param("ss", $_POST["mguid"], $_POST["product"]);
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
        if(isset($_POST["format"]) && $_POST["format"] == "json")
        {
            $row = $result->fetch_assoc();
            echo json_encode($row);
            header('Content-type: application/json');
        }
        else
        {
            $xml = new XMLWriter();

            $xml->openURI("php://output");
            $xml->startDocument();
            $xml->setIndent(true);

            //$xml->startElement('LicenseInfo');
            while($row = $result->fetch_assoc()) {
                $xml->startElement("LicenseInfo");
                $xml->writeAttribute("MachineGUID", $row["MachineGUID"]);
                $xml->writeAttribute("Product", $row["Product"]);
                $xml->writeAttribute('Customer', $row["Customer"]);
                $xml->writeAttribute('Site', $row["Site"]);
                $xml->writeAttribute('ExpirationDate', $row["ExpirationDate"]);
                $xml->writeAttribute('MaxCurrentUsers', $row["MaxCurrentUsers"]);
                $xml->writeRaw($row['LicenseGUID']);

                $xml->endElement();
            }
            $xml->flush();

            header('Content-type: text/xml');
        }
        //$xml->endElement();
    }


}


?>