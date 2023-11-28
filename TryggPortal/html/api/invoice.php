<?php

ini_set('display_errors',1); 
error_reporting(E_ALL);

if(!isset($_GET["mackaper"]))
{
    echo "no you don't";
    exit;
}
else if($_GET["mackaper"] !== "bananer")
{
    echo "no no no";
    exit;
}

// $recipients = [];
// if(isset($_GET["recipients"]))
// {
//     $recipients = $_GET["recipients"];
// }
// else
// {
//     echo "noooooooooooooooooo";
//     exit;
// }

require __DIR__ . '/../functions/phpmailer/Exception.php';
require __DIR__ . '/../functions/phpmailer/PHPMailer.php';
require __DIR__ . '/../functions/phpmailer/SMTP.php';
require __DIR__ . '/../functions/fpdf/fpdf.php';
require __DIR__ . '/../functions/newConfig.php';

use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;

$smsLicenses = [];
$bankidLicenses = [];

$pdfDocuments = [];

if($stmt = $conn->prepare("SELECT * FROM licenses WHERE SMS=1;"))
{
    $stmt->execute();
    $result = $stmt->get_result();
    while($row = $result->fetch_assoc())
    {
        array_push($smsLicenses, $row);
    }
}

foreach($smsLicenses as $smsLicense)
{
    $pdf = new FPDF();
    $pdf->AddPage();
    $pdf->SetFont('Arial','',12);
    $pdf->SetTitle($smsLicense["LicenseGUID"]);
    $pdf->Cell(0, 8, "InSupport TryggPortalen - " . $smsLicense["Product"], 0, 1);
    $pdf->Cell(0, 8, $smsLicense["LicenseGUID"], 0, 1);
    $pdf->Cell(0, 8, $smsLicense["Customer"] . " - " . $smsLicense["Site"], 0, 1);
    $pdf->Ln(4);

    $smses = [];
    
    if($stmt = $conn->prepare("SELECT * FROM sms WHERE licenseKey=? AND YEAR(date) = YEAR(CURRENT_DATE - INTERVAL 1 MONTH) AND MONTH(date) = MONTH(CURRENT_DATE - INTERVAL 1 MONTH);"))
    {
        $stmt->bind_param("s", $smsLicense["LicenseGUID"]);
        $stmt->execute();
        $result = $stmt->get_result();
        while($row = $result->fetch_assoc())
        {
            array_push($smses, $row);
        }
    }

    if(count($smses) > $smsLicense["SMSLimit"])
    {
        $pdf->Cell(0, 8, count($smses) . " sms", 0, 1);

        $i = 1;
        foreach($smses as $sms)
        {
            $pdf->Cell(0, 5, "[{$i}] {$sms["date"]} - {$sms["reciever"]} - {$sms["message"]}", 0, 1);
            $i++;
        }
        array_push($pdfDocuments, [
            "name" => "SMS: {$smsLicense["Customer"]} - {$smsLicense["Site"]}.pdf",
            "file" => $pdf->Output("S",'Report.pdf'),
        ]);
    }
}

if($stmt = $conn->prepare("SELECT * FROM licenses WHERE BankID=1;"))
{
    $stmt->execute();
    $result = $stmt->get_result();
    while($row = $result->fetch_assoc())
    {
        array_push($bankidLicenses, $row);
    }
}

foreach($bankidLicenses as $bankidLicense)
{
    $pdf = new FPDF();
    $pdf->AddPage();
    $pdf->SetFont('Arial','',12);
    $pdf->SetTitle($bankidLicense["LicenseGUID"]);
    $pdf->Cell(0, 8, "InSupport TryggPortalen - " . $bankidLicense["Product"], 0, 1);
    $pdf->Cell(0, 8, $bankidLicense["LicenseGUID"], 0, 1);
    $pdf->Cell(0, 8, $bankidLicense["Customer"] . " - " . $bankidLicense["Site"], 0, 1);
    $pdf->Ln(4);

    $requests = [];
    
    if($stmt = $conn->prepare("SELECT * FROM bankidrequests WHERE license=? AND enviroment='live' AND YEAR(creationdate) = YEAR(CURRENT_DATE - INTERVAL 1 MONTH) AND MONTH(creationdate) = MONTH(CURRENT_DATE - INTERVAL 1 MONTH);"))
    {
        $stmt->bind_param("s", $bankidLicense["LicenseGUID"]);
        $stmt->execute();
        $result = $stmt->get_result();
        while($row = $result->fetch_assoc())
        {
            array_push($requests, $row);
        }
    }

    if(count($requests) > $bankidLicense["BankIDLimit"])
    {
        $pdf->Cell(0, 8, count($requests) . " requests", 0, 1);

        $i = 1;
        foreach($requests as $request)
        {
            $request["lastcollectstatus"] == "" ? $request["lastcollectstatus"] = "N/A" : $request["lastcollectstatus"];
            $pdf->Cell(0, 5, "[{$i}] {$request["creationdate"]} - {$request["method"]} - {$request["lastcollectstatus"]}", 0, 1);
            $i++;
        }
        array_push($pdfDocuments, [
            "name" => "BankID: {$bankidLicense["Customer"]} - {$bankidLicense["Site"]}.pdf",
            "file" => $pdf->Output("S",'Report.pdf'),
        ]);
    }
}

if(count($pdfDocuments) > 0)
{
    $mail = new PHPMailer();
    $mail->SMTPDebug = 2;
    $mail->isSMTP();
    $mail->Host = "mailcluster.loopia.se";
    $mail->SMTPAuth = true;
    $mail->Username = "no-reply@tryggconnect.se";
    $mail->Password = "In\$upp0rt2018!";
    $mail->SMTPSecure = 'tls';
    $mail->Port = 587;
    $mail->CharSet = 'UTF-8';
    $mail->setFrom("no-reply@tryggconnect.se", 'TryggPortalen');

    $mail->Subject   = "TryggPortal fakturor";
    $mail->Body      = "Hej. Här kommer förra månadens fakturor.";
    
    $mail->AddAddress("fredrik.westin@insupport.se");
    $mail->AddAddress("jens.lovgren@insupport.se");
    $mail->AddAddress("support@insupport.se");

    foreach($pdfDocuments as $pdfDoc)
    {
        $mail->addStringAttachment($pdfDoc["file"], $pdfDoc["name"], $encoding = 'base64', $type = 'application/pdf');
    }
    $mail->Send();
}

?>