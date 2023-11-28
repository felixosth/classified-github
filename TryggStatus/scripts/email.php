<?php

require __DIR__ . '/../phpmailer/Exception.php';
require __DIR__ . '/../phpmailer/PHPMailer.php';
require __DIR__ . '/../phpmailer/SMTP.php';
use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;

function sendMail($recipients, $sub, $msg)
{
    if(count($recipients) < 1)
        return;

    $mail = new PHPMailer(true);

    try
    {
        // $mail->SMTPDebug = 2;                                 // Enable verbose debug output
        $mail->isSMTP();                                      // Set mailer to use SMTP
        $mail->Host = "mailcluster.loopia.se";  // Specify main and backup SMTP servers
        $mail->SMTPAuth = true;                               // Enable SMTP authentication
        $mail->Username = "no-reply@tryggconnect.se";                 // SMTP username
        $mail->Password = "In\$upp0rt2018!";                           // SMTP password
        $mail->Port = 587;               
        $mail->CharSet = 'UTF-8';                     // TCP port to connect to

        $mail->setFrom("no-reply@tryggconnect.se", 'TryggStatus');
        
        foreach($recipients as $rec)
        {
            if($rec != "" && isset($rec))
                $mail->addAddress($rec); 
        }
        $mail->isHTML(true); 
        $mail->Subject = $sub;
        $mail->Body    = $msg;

        $mail->send();
    } catch (Exception $e) {
        echo 'Message could not be sent. Mailer Error: ', $mail->ErrorInfo;
    }
}

?>