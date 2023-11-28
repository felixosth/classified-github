<?php
require_once __DIR__ . "/../../Login/Login.php";

// ini_set('display_errors', 1);
// ini_set('display_startup_errors', 1);
// error_reporting(E_ALL);
require __DIR__ . '/../phpmailer/new/Exception.php';
require __DIR__ . '/../phpmailer/new/PHPMailer.php';
require __DIR__ . '/../phpmailer/new/SMTP.php';
use PHPMailer\PHPMailer\PHPMailer;
use PHPMailer\PHPMailer\Exception;

// Checking for a POST request 
if ($_SERVER["REQUEST_METHOD"] == "POST") {

    $name = test_input($_POST["name"]);
    $email = test_input($_POST["email"]);
    $tel = test_input($_POST["tel"]);
    $store = test_input($_POST["store"]);
    $scope = test_input($_POST["scope"]);
    $message = test_input($_POST["message"]);
    try
    {
        $ip = getRealIpAddr();
    }
    catch(\Exception $ex)
    {
        $ip = "N/A";
    }

    try
    {
        $mail = new PHPMailer(true);
        $mail->SMTPDebug  = 2;
        $mail->Debugoutput = function($str, $level) {
            file_put_contents('smtp.log', gmdate('Y-m-d H:i:s'). "\t$level\t$str\n", FILE_APPEND | LOCK_EX);
        };

        $mail->isSMTP();// Set mailer to use SMTP
        $mail->Host = "mailcluster.loopia.se";// Specify main and backup SMTP servers
        $mail->SMTPAuth = true;// Enable SMTP authentication
        $mail->Username = "no-reply@tryggconnect.se";// SMTP username
        $mail->Password = "In\$upp0rt2018!";// SMTP password
        $mail->Port = 587;
        $mail->CharSet = 'UTF-8'; 
        $mail->setFrom("no-reply@tryggconnect.se", 'TryggSupport');

        $mail->addAddress("dev@insupport.se");
        $mail->isHTML(true); // Set email format to HTML
        $mail->Subject = "Felanmälan från " . $store;

        $mail->Body = 
        "<h4>Felanmälan via Bergendahls Supportsida</h4>
        <p>Namn: $name</p>
        <p>E-post: $email</p>
        <p>Telefonnummer: <a href=\"tel:$tel\">$tel</a></p>
        <p>Butik/Objekt: $store</p>
        <p>Omfattning: $scope</p>
        <p>Meddelande: $message</p>
        <p>Avsändare: $ip</p>";

        $sent = $mail->send();
    }
    catch(Exception $ex)
    {
        $sent = false;
    }
} 

// Removing the redundant HTML characters if any exist. 
function test_input($data) { 
    $data = trim($data); 
    $data = stripslashes($data); 
    $data = htmlspecialchars($data); 
    return $data; 
}

function getRealIpAddr(){
    if ( !empty($_SERVER['HTTP_CLIENT_IP']) ) {
     // Check IP from internet.
     $ip = $_SERVER['HTTP_CLIENT_IP'];
    } elseif (!empty($_SERVER['HTTP_X_FORWARDED_FOR']) ) {
     // Check IP is passed from proxy.
     $ip = $_SERVER['HTTP_X_FORWARDED_FOR'];
    } else {
     // Get IP address from remote address.
     $ip = $_SERVER['REMOTE_ADDR'];
    }
    return $ip;
   }
?>
<!DOCTYPE html>
<html>
    <head>
        <!--
            Code by InSupport Nätverksvideo AB for Bergendahls Food AB (2020).
            Contact: dev@insupport.se
        -->

        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">

        <link rel="stylesheet" href="/bootstrap-4.5.2/css/bootstrap.min.css">
        <link rel="stylesheet" href="/style.css">

        <style>
            form{
                padding: 20px;
            }
            .form-check{
                margin-bottom: 1rem;
            }
        </style>
    </head>
    <body>
        <script src="/bootstrap-4.5.2/js/jquery-3.5.1.min.js"></script>
        <script src="/bootstrap-4.5.2/js/bootstrap.min.js"></script>
        <form method="post" action="<?=htmlspecialchars($_SERVER["PHP_SELF"]);?>">

            <?php
            if(isset($sent))
            {
                if($sent)
                {
                    echo "<div class=\"alert alert-success\" role=\"alert\">Din felamälan har skickats!</div>";
                }
                else
                {
                    echo "<div class=\"alert alert-danger\" role=\"alert\">
                    <p>Det gick inte att skicka din felanmälan! Testa att skicka igen eller maila direkt till support@insupport.se.</p>
                    <hr>
                    <a href=\"#mailError\" data-toggle=\"collapse\" aria-expanded=\"false\" class=\"dropdown-toggle\">Felmeddelande</a>
                    <p id=\"mailError\" class=\"collapse\">$ex</p>
                    </div>";
                }
            }
            ?>
            <h2>Felanmälan till InSupport Nätverksvideo</h2>
            <p>Innan du felanmäler, läs dokumentet "Frågor och svar", det du undrar kanske finns där!</p>
            <p>För felanmälan via telefon, ring 08 459 00 66.</p>

            <div class="form-group">
                <label for="nameInput">Namn</label>
                <input name="name" type="text" class="form-control" id="nameInput" aria-describedby="emailHelp" placeholder="För- och efternamn" required>
              </div>

            <div class="form-group">
              <label for="emailInput">E-postadress</label>
              <input name="email" type="email" class="form-control" id="emailInput" aria-describedby="emailHelp" placeholder="namn@exempel.se" required>
            </div>

            <div class="form-group">
              <label for="telInput">Telefonnummer</label>
              <input name="tel" type="tel" class="form-control" id="telInput" placeholder="Telefonnummer" required>
            </div>

            <div class="form-group">
                <label for="storeInput">Butik/Objekt</label>
                <input name="store" type="text" class="form-control" id="storeInput" placeholder="Butik/Objekt" required>
            </div>

            <div class="form-group">
                <label for="scopeInput">Omfattning</label>
                <input name="scope" type="text" class="form-control" id="scopeInput" placeholder="T ex. Kamera 2, Klienten i förbutiken" required>
            </div>

            <div class="form-group">
                <label for="messageInput">Omfattning</label>
                <textarea name="message" class="form-control" id="messageInput" placeholder="Summering av problem" rows="3" required></textarea>
            </div>
            <div class="form-check">
                <input class="form-check-input" type="checkbox" value="" id="agreeCheck" required>
                <label class="form-check-label" for="agreeCheck">
                    Jag är medveten om att detta är en beställning och kan komma att faktureras.
                </label>
            </div>

            <button id="submitBtn" type="submit" class="btn btn-primary">Skicka felanmälan</button>
          </form>

          <script>
            $(document).ready(function()
            {
                $("form").submit(function()
                {
                    $("#submitBtn").attr("disabled", true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Skickar...');
                });
            });
          </script>
    </body>
</html>