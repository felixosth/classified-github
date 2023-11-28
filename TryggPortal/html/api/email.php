<?php

require __DIR__ . '/../functions/newConfig.php';
require __DIR__ . '/../functions/phpmailer/Exception.php';
require __DIR__ . '/../functions/phpmailer/PHPMailer.php';
require __DIR__ . '/../functions/phpmailer/SMTP.php';
use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;

// http://tryggportal.local:8011/api/email.php?recipient[]=felix.osth@insupport.se&recipient[]=dev@insupport.se&from=Felix&subject=Hej&message=Hall%C3%A5%C3%A5%C3%A5%C3%A5%C3%A5

if (!empty($_SERVER['HTTP_CLIENT_IP'])) {
    $ip = $_SERVER['HTTP_CLIENT_IP'];
} elseif (!empty($_SERVER['HTTP_X_FORWARDED_FOR'])) {
    $ip = $_SERVER['HTTP_X_FORWARDED_FOR'];
} else {
    $ip = $_SERVER['REMOTE_ADDR'];
}

if($_SERVER["REQUEST_METHOD"] == "GET")
{
    if(isset($_GET["recipient"]) && isset($_GET["from"]) && isset($_GET["subject"]) && isset($_GET["message"]) && isset($_GET["license"]))
    {
        $license = $_GET["license"];
        $recipient = $_GET["recipient"];
        $from = $_GET["from"];
        $subject = $_GET["subject"];
        $message = $_GET["message"];
        
        $licenseId = checkLicense($license, $conn);

        if($licenseId !== null && $licenseId !== false)
        {
            email($recipient, $from, $subject, $message);

            $recipients = is_array($recipient) ?  implode(", ", $recipient) : $recipients;
            if($stmt = $conn->prepare("INSERT INTO emails (recipient, subject, message, license, ip) VALUES (?, ?, ?, ?, ?);"))
            {
                $stmt->bind_param("sssis", $recipients, $subject, $message, $licenseId, $ip);
                $stmt->execute();
            }
        }
        else
            echo "invalid license";
    }
}
else if ($_SERVER["REQUEST_METHOD"] == "POST")
{
    if(isset($_POST["recipient"]) && isset($_POST["from"]) && isset($_POST["subject"]) && isset($_POST["message"]) && isset($_POST["license"]))
    {
        $license = $_POST["license"];
        $recipient = $_POST["recipient"];
        $from = $_POST["from"];
        $subject = $_POST["subject"];
        $message = $_POST["message"];
        $useHtml = isset($_POST["useHTML"]) ? isBoolean($_POST["useHTML"]) : false;
        
        $licenseId = checkLicense($license, $conn);

        if($licenseId !== null && $licenseId !== false)
        {
            email($recipient, $from, $subject, $message, $useHtml);

            $recipients = is_array($recipient) ?  implode(", ", $recipient) : $recipients;
            if($stmt = $conn->prepare("INSERT INTO emails (recipient, subject, message, license, ip) VALUES (?, ?, ?, ?, ?);"))
            {
                $stmt->bind_param("sssis", $recipients, $subject, $message, $licenseId, $ip);
                $stmt->execute();
            }
        }
        else
            echo "invalid license";
    }
}

function isBoolean($value) {
    if ($value && strtolower($value) !== "false") {
       return true;
    } else {
       return false;
    }
 }

function email($recipient, $from, $subject, $message, $useHtml = false)
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
    $mail->IsHTML($useHtml);
    $mail->setFrom("no-reply@tryggconnect.se", $from);

    $mail->Subject   = $subject;
    $mail->Body      = $message;

    if(is_array($recipient))
    {
        foreach($recipient as $rec)
        {
            $mail->AddAddress($rec);
            $mail->Send();
            $mail->ClearAllRecipients();
        }
    }
    else
    {
        $mail->AddAddress($recipient);
        $mail->Send();
    }

}

function checkLicense($license, $conn)
{
    if($stmt = $conn->prepare("SELECT id FROM licenses WHERE LicenseGUID=?;"))
    {
        $stmt->bind_param("s", $license);
        $stmt->execute();
        $result = $stmt->get_result();
        $stmt->free_result();
        $stmt->close();

        $row = $result->fetch_assoc();

        if(isset($row["id"]))
        {
            return $row["id"];
        }
        else
            return false;
    }
}




?>