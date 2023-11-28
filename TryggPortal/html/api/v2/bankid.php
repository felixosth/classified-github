<?php

require_once "../../vendor/autoload.php";
require_once "../../functions/newConfig.php";

/*
openssl cmd:
openssl pkcs12 -in insupport-crt.p12 -out cert.pem -nodes
*/

// ini_set('display_errors', 1);
// ini_set('display_startup_errors', 1);
// error_reporting(E_ALL);
// error_reporting(0);

if($_SERVER["REQUEST_METHOD"] == "POST"){

    $crtFile = __DIR__."/../../../crt/finalcert.pem";
    $testCrt = __DIR__."/../../../crt/test.pem";

    $bankID = null;

    $rawData = file_get_contents('php://input');

    $data = json_decode($rawData, true);

    if(isset($data["environment"]))
    {
        switch($data["environment"])
        {
            case "test":
                $bankID = new LJSystem\BankID\BankID("test", $testCrt, null, null, null);
            break;
            case "live":
                $bankID = new LJSystem\BankID\BankID("prod", $crtFile, false, null, null);
            break;
        }

    }

    if(isset($data["mguid"]) && !isset($data["license"]))
    {
        $data["license"] = validateLicenseFromMguid($conn, $data["mguid"]);
    }
    else
    {
        $data["license"] = validateLicense($conn, $data["license"]);
    }
    
    if(isset($data["license"]) && isset($data["method"]) && isset($bankID))
    {
        switch($data["method"])
        {
            case "cancel":
                if(isset($data["orderRef"]))
                {
                    try
                    {
                        $req = getRequestFromDb($conn, $data["license"], $data["orderRef"]);
                        if(isset($req))
                        {
                            $cancelled = $bankID->cancel($data["orderRef"]);
                            echo json_encode(array("cancelled" => $cancelled->getStatus() == "OK"));
                            exit;
                        }
                        else
                        {
                            echo json_encode(array("response" => "No request found"));
                            exit;
                        }
                    }
                    catch(Exception $e)
                    {
                        echo json_encode([
                            "error" => "Unable to create request"
                        ]);
                        exit;
                    }
                }
            break;
            case "collect":
                if(isset($data["orderRef"]))
                {
                    try
                    {
                        $req = getRequestFromDb($conn, $data["license"], $data["orderRef"]);
                        if(isset($req))
                        {
                            $collectResponse = $bankID->collect($data["orderRef"]);
                            updateRequest($conn, $req, $collectResponse);
                            echo json_encode($collectResponse->getBody());
                            exit;
                        }
                        else
                        {
                            echo json_encode(array("response" => "No request found"));
                            exit;
                        }
                    }
                    catch(Exception $e)
                    {
                        echo json_encode([
                            "error" => "Unable to create request"
                        ]);
                        exit;
                    }
                }
            break;

            case "auth":

                if(isset($data["personalNumber"]) == false)
                    $data["personalNumber"] = "";

                try
                {
                    $response = $bankID->authenticate($data["personalNumber"], $_SERVER['REMOTE_ADDR']);

                    $responseStr = json_encode($response->getBody());
                    $orderRef = $response->getOrderRef();

                    if($orderRef == null || addBankIdRequest($conn, $data["license"], $data["method"], $data["personalNumber"], $responseStr, $orderRef, $data["environment"]))
                    {
                        echo $responseStr;
                    }
                    else
                    {
                        echo json_encode(array("response" => "Unable to add request to db"));
                    }
                    exit;
                }
                catch(Exception $e)
                {
                    echo json_encode([
                        "error" => "Unable to create request"
                    ]);
                    exit;
                }

                case "sign":

                if(isset($data["personalNumber"]) == false)
                    $data["personalNumber"] = "";

                if(isset($data["userData"]) == false)
                    $data["userData"] = "";

                try
                {
                    $response = $bankID->sign($data["personalNumber"], $_SERVER['REMOTE_ADDR'], $data["userData"]);
                    $orderRef = $response->getOrderRef();
                    $responseStr = json_encode($response->getBody());
                    
                    if($orderRef == null || addBankIdRequest($conn, $data["license"], $data["method"], $data["personalNumber"], $responseStr, $orderRef, $data["environment"], $data["userData"]))
                    {
                        echo $responseStr;
                    }
                    else
                    {
                        echo json_encode(array("response" => "Unable to add request to db"));
                    }

                    exit;
                }
                catch(Exception $e)
                {
                    echo json_encode([
                        "message" => $e->getMessage(),
                        "code" => $e->getCode()
                    ]);
                    exit;
                }
                break;
            break;
            default:
                echo json_encode([ "response" => "invalid method"]);
                exit;
            break;
        }
    }
    else
    {
        if($data["license"] === null)
        {
            echo json_encode([ "response" => "invalid license"]);
        }
        if($bankID === null)
        {
            echo json_encode([ "response" => "invalid environment"]);
        }
        else
        {
            echo json_encode([ "response" => "Missing params"]);
        }
    }
}
// var_dump($data);
// echo "\r\n";
// echo "Something went wrong";

function addBankIdRequest($conn, $lic, $method, $pnr, $responseStr, $orderRef, $environment, $userData = null)
{
    if($stmt = $conn->prepare("INSERT INTO bankidrequests (license, method, pnr, orderRef, response, enviroment, userData) VALUES (?, ?, ?, ?, ?, ?, ?);"))
    {
        $stmt->bind_param("sssssss", $lic, $method, $pnr, $orderRef, $responseStr, $environment, $userData);
        if(!$stmt->execute())
        {
            echo $stmt->error;
            return false;
        }
        return true;
    }
}

function getRequestFromDb($conn, $license, $orderRef)
{
    if($stmt = $conn->prepare("SELECT * FROM bankidrequests WHERE license=? AND orderRef=?;"))
    {
        $stmt->bind_param("ss", $license, $orderRef);
        $stmt->execute();
        return $stmt->get_result()->fetch_assoc();
    }
}

function updateRequest($conn, $request, $collectResponse)
{
    $request["collects"] += 1;

    if($stmt = $conn->prepare("UPDATE bankidrequests SET collects=?, lastcollect=NOW(), lastcollectstatus=? WHERE id=?;"))
    {
        $status = $collectResponse->getStatus();
        $stmt->bind_param("isi", $request["collects"], $status, $request["id"]);
        $stmt->execute();
    }
}

function validateLicenseFromMguid($conn, $mguid)
{
    if($stmt = $conn->prepare("SELECT * FROM licenses WHERE MachineGUID=? AND BankID=1;"))
    {
        $stmt->bind_param("s", $mguid);
        $stmt->execute();
        $license = $stmt->get_result()->fetch_assoc();

        if(isset($license))
        {
            return strtotime($license["ExpirationDate"]) > time() ? $license["LicenseGUID"] : null; 
        }
    }
    return null;
}

function validateLicense($conn, $licenseStr)
{
    if($stmt = $conn->prepare("SELECT * FROM licenses WHERE LicenseGUID=? AND BankID=1;"))
    {
        $stmt->bind_param("s", $licenseStr);
        $stmt->execute();
        $license = $stmt->get_result()->fetch_assoc();

        if(isset($license))
        {
            return strtotime($license["ExpirationDate"]) > time() ? $license["LicenseGUID"] : null; 
        }
    }
    return null;
}

?>