<?php

require_once __DIR__ . "/../../lib/config.php";

$action = isset($_REQUEST["action"]) ? $_REQUEST["action"] : null;
$o3c_name = isset($_SERVER["HTTP_X_STSERVER_ID"]) ? $_SERVER["HTTP_X_STSERVER_ID"] : null;
$o3c_host = $_SERVER["REMOTE_ADDR"];
$client_id = isset($_REQUEST["client_id"]) ? explode(".", $_REQUEST["client_id"])[1] : null;

mysqli_report(MYSQLI_REPORT_OFF);


switch($action)
{
    case "server_up":
        onServerUp();
        break;
    case "server_down":
        onServerDown();
        break;
    case "client_hello":
        onClientHello();
        break;
    case "client_connect":
        onClientConnect();
        break;
    case "client_disconnect":
        onClientDisconnect();
        break;
    default:
        http_response_code(400);
        break;
}


function onServerUp()
{
    if($stmt = $GLOBALS["conn"]->prepare("INSERT INTO o3c_servers (name, host, failover_o3c_id) VALUES (?, ?, (SELECT value FROM `global_settings` WHERE setting =  'o3c_entry_point')) ON DUPLICATE KEY UPDATE state=1;"))
    {
        $stmt->bind_param("ss", $GLOBALS["o3c_name"], $GLOBALS["o3c_host"]);
        $stmt->execute();
    }
}

function onServerDown()
{
    if($stmt = $GLOBALS["conn"]->prepare("UPDATE o3c_servers SEt state=0 WHERE name=?;"))
    {
        $stmt->bind_param("s", $GLOBALS["o3c_name"]);
        $stmt->execute();
    }
}

function onClientHello()
{
    // insert or update devices_dynamic
    if($stmt = $GLOBALS["conn"]->prepare(
        "INSERT INTO devices_dynamic 
        (o3c_server, client_id) VALUES 
        ((SELECT id FROM o3c_servers WHERE name=?), ?) 
        ON DUPLICATE KEY 
        UPDATE state=1, o3c_server=VALUES(o3c_server);"))
    {
        $stmt->bind_param("ss", $GLOBALS["o3c_name"], $GLOBALS["client_id"]);
        $stmt->execute();
    }

    http_response_code(204); // OK client can connect
}

function onClientConnect()
{
    $model = isset($_REQUEST["product"]) ? $_REQUEST["product"] : null;
    $firmware = isset($_REQUEST["firmwareVersion"]) ? $_REQUEST["firmwareVersion"] : null;
    $source = isset($_REQUEST["client_srcaddr"]) ? $_REQUEST["client_srcaddr"] : null;

    if($stmt = $GLOBALS["conn"]->prepare("UPDATE devices_dynamic SET o3c_server=(SELECT id FROM o3c_servers WHERE name=?), model=?, firmware=?, source_addr=?, state=1 WHERE client_id=?;"))
    {
        $stmt->bind_param("sssss", $GLOBALS["o3c_name"], $model, $firmware, $source, $GLOBALS["client_id"]);
        $stmt->execute();
    }

    global $serverManager;

    $static_device = $serverManager->getStaticDevice($GLOBALS["client_id"]);
    $o3c_server = $serverManager->getServerByName($GLOBALS["o3c_name"]);

    if(!isset($static_device))
    {
        if($stmt = $GLOBALS["conn"]->prepare("INSERT IGNORE INTO devices_static (client_id, o3c_server_dst) VALUES (?, ?);"))
        {
            $stmt->bind_param("si", $GLOBALS["client_id"], $o3c_server["id"]);
            $stmt->execute();
        }

        $static_device = array("o3c_server_dst" => $o3c_server["id"]);
    }

    $o3c_server_dst = $serverManager->getServerWithServerlist($static_device["o3c_server_dst"]);

    $serverList = explode("=", 
    trim(
        $serverManager->webRequest(
            $GLOBALS["client_id"] . "/axis-cgi/param.cgi?action=list&group=root.RemoteService.ServerList", 
            $o3c_server["admin_host"])
        )
    )[1];

    if($serverList != $o3c_server_dst["serverlist"])
    {
        $serverManager->webRequest($GLOBALS["client_id"] . "/axis-cgi/param.cgi?action=update&root.RemoteService.ServerList=" . $o3c_server_dst["serverlist"], $o3c_server["admin_host"]);

        http_response_code(302);
        header("Location: " . $o3c_server_dst["client_host"]);
    }
        

}


function onClientDisconnect()
{
    if($stmt = $GLOBALS["conn"]->prepare("UPDATE devices_dynamic SET state=0 WHERE client_id=?;"))
    {
        $stmt->bind_param("s", $GLOBALS["client_id"]);
        $stmt->execute();
    }
}

?>