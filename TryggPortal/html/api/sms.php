<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);

require_once "../functions/newConfig.php";
require_once "../vendor/autoload.php";


$user = "900257";
$pass = "DWIEBGDA";


/*

    OBS! VAR FÖRSIKTIG MED DENNA FIL!
    TESTA ATT SKICKA SMS MED POST EFTER MODIFIERING!

*/

if($_SERVER['REQUEST_METHOD'] === "GET" && isset($_GET["sender"]) && isset($_GET["reciever"]) && isset($_GET["license"]) && isset($_GET["message"]))
{
    $from = $_GET["sender"];
    $to = $_GET["reciever"];
    $msg = $_GET["message"];
    $lic = $_GET["license"];
    
    $sentSms = true;
    SendSMS_GET($conn, $user, $pass, $from, $to, $msg, $lic);
}
else if(isset($_POST["sender"]) && isset($_POST["reciever"]) && isset($_POST["license"]) && isset($_POST["message"]) && isset($_POST["mguid"]))
{
    $from = $_POST["sender"];
    $to = $_POST["reciever"];
    $msg = $_POST["message"];
    $lic = $_POST["license"];
    $mguid = $_POST["mguid"];

    SendSMS($conn, $user, $pass, $from, $to, $msg, $lic, $mguid);
}
else if (isset($_POST["sender"]) && isset($_POST["reciever"]) && isset($_POST["message"]))
{
    require "../functions/checklogin.php";

    if($isAdmin) // require admin
    {
        $from = $_POST["sender"];
        $to = $_POST["reciever"];
        $msg = $_POST["message"];

        SendSMS($conn, $user, $pass, $from, $to, $msg, $_SESSION["username"], null, false);
    }

    header("location: " . LOC . "/sms");
}
else
    echo "no";

    function SendSMS($conn, $user, $pass, $from, $to, $msg, $lic, $mguid, $checkLicense = true)
    {
        $to = trim($to);

        if(substr($to, 0, 1) === "0")
            $to = "+46" . substr($to, 1);
        
        if(substr($to, 0, 2) === "46")
            $to = "+" . $to;
        
        if ($checkLicense) {
            $query = "SELECT * FROM licenses WHERE MachineGUID=? and LicenseGUID=? and SMS=1 LIMIT 1";
            if($stmt = $conn->prepare($query))
            {
                $stmt->bind_param("ss", $mguid, $lic);
                $stmt->execute();
                $result = $stmt->get_result();
                $licenseArray = array();
                while($row = $result->fetch_assoc())
                {
                    if(strtotime($row["ExpirationDate"]) - time() > 0)
                        array_push($licenseArray, $row);
                }
                $stmt->free_result();
                $stmt->close();
            }
        }
        else
            $licenseArray = array(1);
    
        if (count($licenseArray) > 0 || !$checkLicense) {
    
            $data = array(
                "From" => $from,
                "To" => array($to),
                "Text" => $msg
            );

            try
            {
                $res = zendSms($user, $pass, $data);
                $body = $res->getBody();
                echo $body;
        
                $query = "INSERT INTO sms (sender, reciever, licenseKey, message, response, endpoint) VALUES (?,?,?,?,?,?)";
                if($stmt2 = $conn->prepare($query))
                {
                    $stmt2->bind_param("ssssss", $from, $to, $lic, $msg, $body, $_SERVER['REMOTE_ADDR']);
                    $stmt2->execute();
                }
            }
            catch(Exception $ex){
                echo "<pre>";
                var_dump($ex);
                echo "</pre>";
            }
            
        }
        else 
            echo "no valid license";
    }

    function SendSMS_GET($conn, $user, $pass, $from, $to, $msg, $lic)
    {
        $to = trim($to);

        if(substr($to, 0, 1) === "0")
            $to = "+46" . substr($to, 1);
        
        if(substr($to, 0, 2) === "46")
            $to = "+" . $to;
        
        $query = "SELECT * FROM licenses WHERE LicenseGUID=? and SMS=1 LIMIT 1";
        if($stmt = $conn->prepare($query))
        {
            $stmt->bind_param("s", $lic);
            $stmt->execute();
            $result = $stmt->get_result();
            $licenseArray = array();
            while($row = $result->fetch_assoc())
            {
                if(strtotime($row["ExpirationDate"]) - time() > 0)
                    array_push($licenseArray, $row);
            }
            $stmt->free_result();
            $stmt->close();
        }
    
        if (count($licenseArray) > 0) {
    
            $data = array(
                "From" => $from,
                "To" => array($to),
                "Text" => $msg
            );
    
            try
            {
                $res = zendSms($user,$pass,$data);
                $body = $res->getBody();
                echo $body;

                $query = "INSERT INTO sms (sender, reciever, licenseKey, message, response, endpoint) VALUES (?,?,?,?,?,?)";
                if($stmt2 = $conn->prepare($query))
                {
                    $stmt2->bind_param("ssssss", $from, $to, $lic, $msg, $body, $_SERVER['REMOTE_ADDR']);
                    $stmt2->execute();
                }
            }
            catch(Exception $ex){
                echo "<pre>";
                var_dump($ex);
                echo "</pre>";
            }
        }
        else 
            echo "no valid license";
    }

    function zendSms($user, $pass, $data)
    {
        $client  = new \Zend\Http\Client("https://api.genericmobile.se/SmsGateway/api/v1/Message");
        $client->setAuth($user, $pass, \Zend\Http\Client::AUTH_BASIC);
        $client->setMethod(\Zend\Http\Request::METHOD_POST);
        $client->setRawBody(json_encode($data));
        $client->setEncType('application/json');
        return $client->send();
    }

?>