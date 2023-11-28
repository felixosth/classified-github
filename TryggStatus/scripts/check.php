

<?php

// block all access that doesn't know this SuperSecret password
$superSecretCode = "fdsad501ikfaa14k217s6fkjs72apofksapofkdsa";
$superSecretValue = "4847154125486461515615648964351";

if(!isset($_GET[$superSecretCode]))
{
    header("HTTP/1.1 401 Unauthorized");
    exit;
}
else
{
    if($_GET[$superSecretCode] !== $superSecretValue)
    {
        header("HTTP/1.1 401 Unauthorized");
        exit;
    }
}
// end of supersecret
?>

<html>
<head>
    <title>TryggStatus</title>
</head>
<body>

<?php

error_reporting(E_ALL);
ini_set('display_errors', 1);

require "sql.php";
require "email.php";
require "sms.php";

$sqlConn = getSql($_SERVER['SERVER_NAME']);

if(!isset($sqlConn))
{
    die("SQL is null");
}

$sites = array();
if($stmt = $sqlConn->prepare("SELECT * FROM sites;"))
{
    $stmt->execute();
    $result = $stmt->get_result();

    while($row = $result->fetch_assoc())
    {
        array_push($sites, $row);
    }
}

foreach($sites as $site)
{
    text("<b>" . $site["url"] . ":" . $site["port"] . "</b>");
    text("Last check: " . $site["lastCheck"]);
    text("Last online: " . $site["lastOnline"]);
    text();
    
    $result = check($site);
    if($result)
    {
        text("Site is online.");

        if($site["alarmSent"] === 1)
        {
            sendUpAgainNotification($site, $sqlConn);
        }

        updateOnline($site["id"], $sqlConn);   
    }
    else
    {
        updateOffline($site["id"], $sqlConn);

        $now = new DateTime("now");
        $lastOnlineDateTime = new DateTime($site["lastOnline"]);
        $dateDif = ($now->getTimestamp() - $lastOnlineDateTime->getTimestamp());
        text("Site offline for " . round(($dateDif/60), 1) . " minutes.");
        if($dateDif >= $site["alarmThreshold"] && $site["alarmSent"] == 0)
        {
            text("Sending alarm...");
            sendAlarm($site, $sqlConn);
        }
    }

    text();
}


// -------------------- FUNCTIONs ----------------

function text($txt = "")
{
    echo $txt . "<br>\r\n";
}

function check($site)
{
    if($socket =@ fsockopen($site["url"], $site["port"], $errno, $errstr, 30)) 
    {
        fclose($socket);
        return true;
    }
    else
    {
        return false;
    }
    return false;
}

function updateOnline($id, $sqlConn)
{
    if($stmt = $sqlConn->prepare("UPDATE sites SET lastCheck=CURRENT_TIMESTAMP, lastOnline=CURRENT_TIMESTAMP, alarmSent=0 WHERE id=?;"))
    {
        $stmt->bind_param("i", $id);
        return $stmt->execute();
    }
}

function updateOffline($id, $sqlConn)
{
    if($stmt = $sqlConn->prepare("UPDATE sites SET lastCheck=CURRENT_TIMESTAMP WHERE id=?;"))
    {
        $stmt->bind_param("i", $id);
        return $stmt->execute();
    }
}

function sendAlarm($site, $sqlConn)
{
    $sub = $site["url"] . " is DOWN";
    $bod = $sub . " since " . $site["lastOnline"];
    sendMail(getRecipients("email", $sqlConn), $sub, $bod);
    sendSms(getRecipients("sms", $sqlConn), $bod);

    if($stmt = $sqlConn->prepare("UPDATE sites SET alarmSent=1 WHERE id=?;"))
    {
        $stmt->bind_param("i", $site["id"]);
        $stmt->execute();
    }
}

function sendUpAgainNotification($site, $sqlConn)
{
    $sub = $site["url"] . " is UP";
    $bod = $site["url"] . " was down at " . $site["lastOnline"] . " but it's fine now! =)";
    sendMail(getRecipients("email", $sqlConn), $sub, $bod);
}

function getRecipients($type, $sqlConn)
{
    if($stmt = $sqlConn->prepare("SELECT data FROM recipients WHERE type=?;"))
    {
        $stmt->bind_param("s", $type);
        $stmt->execute();
        $result = $stmt->get_result();
    
        $recipients = array();
        while($row = $result->fetch_assoc())
        {
            array_push($recipients, $row["data"]);
        }
        return $recipients;
    }
    return array();
}

?>
</body>
</html>