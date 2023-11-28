<?php

require_once "../vendor/autoload.php";
require_once "../functions/newConfig.php";

/*
openssl cmd:
openssl pkcs12 -in insupport-crt.p12 -out cert.pem -nodes
*/

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);
// error_reporting(0);

if($_SERVER["REQUEST_METHOD"] == "POST"){

    $crtFile = __DIR__."/../../crt/finalcert.pem";
    $testCrt = __DIR__."/crt/testcert2.pem";
    // $testCrt = __DIR__."/crt/testcert.pem";

    // echo "Readable: " . is_readable($crtFile);

    $bankIDService = null;

    $rawData = file_get_contents('php://input');

    $data = json_decode($rawData, true);

    if(isset($data["environment"]))
    {
        switch($data["environment"])
        {
            case "test":
                $bankIDService = new Dimafe6\BankID\Service\BankIDService(
                    'https://appapi2.test.bankid.com/rp/v5.1/',
                    $_SERVER["REMOTE_ADDR"],
                    [
                        'verify' => false,
                        'cert'   => $testCrt
                    ]
                );
            break;
            case "live":
                $bankIDService = new Dimafe6\BankID\Service\BankIDService(
                    'https://appapi2.bankid.com/rp/v5.1/',
                    $_SERVER["REMOTE_ADDR"],
                    [
                        'verify' => false,
                        'cert'   => $crtFile
                    ]
                );
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
    
    if(isset($data["license"]) && isset($data["method"]) && isset($bankIDService))
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
                            $cancelled = $bankIDService->cancelOrder($data["orderRef"]);
                            // updateRequest($conn, $req, $collectResponse);
                            echo json_encode(array("cancelled" => $cancelled));
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
                            // "message" => $e->getMessage(),
                            // "code" => $e->getCode()
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
                            $collectResponse = $bankIDService->collectResponse($data["orderRef"]);
                            updateRequest($conn, $req, $collectResponse);
                            echo json_encode($collectResponse);
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
                            // "message" => $e->getMessage(),
                            // "code" => $e->getCode()
                        ]);
                        exit;
                    }
                }
            break;

            case "auth":
                if(isset($data["personalNumber"]))
                {
                    try
                    {
                        $response = $bankIDService->getAuthResponse($data["personalNumber"]);

                        $responseStr = json_encode($response);
                        if(addBankIdRequest($conn, $data["license"], $data["method"], $data["personalNumber"], $responseStr, $response->orderRef, $data["environment"]))
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
                            // "message" => $e->getMessage(),
                            // "code" => $e->getCode()
                        ]);
                        exit;
                    }
                }
                else
                {
                    try
                    {
                        $response = $bankIDService->getEmptyAuthResponse();

                        $responseStr = json_encode($response);
                        if(addBankIdRequest($conn, $data["license"], $data["method"], "", $responseStr, $response->orderRef, $data["environment"]))
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
                            // "message" => $e->getMessage(),
                            // "code" => $e->getCode()
                        ]);
                        exit;
                    }
                }

                case "sign":
                if(isset($data["personalNumber"]) && isset($data["userData"]))
                {
                    try
                    {
                        $response = $bankIDService->getSignResponse($data["personalNumber"], $data["userData"]);
                        $responseStr = json_encode($response);
                        addBankIdRequest($conn, $data["license"], $data["method"], $data["personalNumber"], $responseStr, $response->orderRef, $data["environment"], $data["userData"]);
                        echo $responseStr;
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
        if($bankIDService === null)
        {
            echo json_encode([ "response" => "invalid environment"]);
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
        $stmt->bind_param("isi", $request["collects"], $collectResponse->status, $request["id"]);
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