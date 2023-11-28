<?php
require_once "../dbconfig.php";

$mediaType = "none";
$alarmText = "No case found";
$encodedImage = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=";
$found = false;
$ack = 0;

if (isset($_GET["uid"]))
{
    if($stmt = $conn->prepare("SELECT * FROM alarms WHERE uid=?"))
    {
        $stmt->bind_param("s", $_GET["uid"]);
        $stmt->execute();

        $result = $stmt->get_result();
        $row = $result->fetch_assoc();

        if (isset($row)) {
            $uid = $row["uid"];
            $alarmText = $row["text"];
            $ack = $row["acknowledged"];
            $alarmStatus =  $row["alarm"];
            $lic = $row["license"];

            $alarmImage = "img/$uid.png";
            $mediaType = $row["media"];
            if($mediaType == "img")
            {
                $encodedImage = base64_encode(file_get_contents($alarmImage));
            }
            else if($mediaType == "vid")
            {
                $alarmVideo = "$lic/$uid.mp4";
            }
            else if(file_exists($alarmImage))
            {
                $mediaType = "img";
                $encodedImage = base64_encode(file_get_contents($alarmImage));
            }

            $found = true;
        }
    }
}

echo $_SERVER['DOCUMENT_ROOT'];
?>

<html>
    <head>
        <?php include "../functions/getbt.php"; ?>
        <link rel="stylesheet" type="text/css" href="style/slider.css">
        <link rel="stylesheet" type="text/css" href="style/style.css">

        <title>Case Inspector<?php echo $found ? " - " . $alarmText : ""; ?></title>
    </head>
    <body class="container-fluid mx-auto">
        <h1 style="margin-top:10px;margin-bot:10px;"><?php echo $alarmText; ?></h1>
        <div id="imageContainer">
            <?php
            if($mediaType == "img")
            {
                echo '<img id="alarmImage" class="img-fluid" src="data:image/png;base64,' . $encodedImage . '"/>';
            }
            else if($mediaType == "vid")
            {
                echo '<video src="http://192.168.2.64:8011/tryggkontroll/case/vid/' . $alarmVideo . '" type="video/mp4" autoplay controls muted loop onerror="failed(event)" controlsList="nodownload" oncontext="return false"></video>';
                //echo '<video type=\'video/webm;codecs="vp8, vorbis"\' src="http://192.168.2.64:8011/tryggkontroll/case/vid/' . $alarmVideo . '" autoplay controls muted loop onerror="failed(event)" ></video>';
            }

            ?>
            
        </div>
        <?php echo !$found ? "<!--" : ""; ?>

        <div id="buttonContainer">
            <button type="button" onclick="acknowledge()" id="ackBtn" class="btn btn-<?php echo $ack == 0 ? "primary" : "outline-secondary" ?>">
                <?php echo $found ? ($ack == 0 ? "Kvittera" : "Okvittera") : ""?>
            </button>
            <button type="button" onclick="showSlider()" class="btn btn-outline-danger" <?php echo $found ? ($alarmStatus == 1 ? "disabled" : "" ) : ""; ?>>Skicka Larm</button>
        </div>

        <?php echo !$found ? "-->" : ""; ?>

        <div class="alarm-slider" id="slider" style="display: none; margin-top: 15px">
            <input type="range" value="0" disabled></input>
            <span>Aktivera larm</span>
        </div>

        <script>
            var ack = "<?php echo $ack; ?>";
            var uid = "<?php echo $uid; ?>";
        </script>
        <script src="js/case.js"></script>

        <div class="footer">
            <div class="content">
                <p>Copyright © InSupport Nätverksvideo AB 2018</p>
                <a href="http://trygghetscenter.se" target="_blank"><img src="style/logowhite_smaller.bmp"></a>
            </div>
        </div>
    </body>
</html>