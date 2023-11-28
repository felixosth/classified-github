<?php

require_once __DIR__ . "/../../lib/config.php";

header("Content-Type: application/json");
set_time_limit(60 * 60);


ob_end_flush();
apache_setenv('no-gzip', '1');
header('Content-Encoding: none');

switch($_SERVER["REQUEST_METHOD"])
{
    case "POST":
            $data = json_decode(file_get_contents('php://input'), true);
            if(isset($data["action"]))
            {
                switch($data["action"])
                {
            
                    case "changeServer":
            
                        if(isset($data["cameras"]) && isset($data["server"]))
                        {
                            $o3c_server = $serverManager->getServerWithServerlist($data["server"]);
                            if(isset($o3c_server))
                            {
                                foreach($data["cameras"] as $cameraId)
                                {
                                    $serverManager->moveDevice($cameraId, $o3c_server);
                                    echo json_encode(array("device" => $cameraId ,"output" => "Done"));

                                    flush();
                                }
                            }
                        }
            
                        break;

                    case "redirectDevice":
                        if(isset($data["cameras"]) && is_array($data["cameras"]) && isset($data["server"]))
                        {
                            $o3c_server = $serverManager->getServerWithServerlist($data["server"]);
                            if(isset($o3c_server))
                            {

                                foreach($data["cameras"] as $cameraId)
                                {
                                    $o = $serverManager->redirectDevice($cameraId, $o3c_server);
                                    echo json_encode(array("device" => $cameraId ,"output" => $o));

                                    flush();
                                }
                            }
                        }
                        break;
            
                    case "registerDevice":
                        if(isset($data["server"]) && isset($data["mac"]) && isset($data["oak"]))
                        {
                            $o3c_server = $serverManager->getServer($data["server"]);
                            if(isset($o3c_server))
                            {
                                echo $serverManager->registerDevice($data["mac"], $data["oak"], $data["folder"] ?? 1, $data["name"] ?? null, $o3c_server);
                            }
                        }
                        break;

                    case "reconnectDevice":
                        if(isset($data["cameras"]) && isset($data["server"]))
                        {
                            $o3c_server = $serverManager->getServer($data["server"]);
                            if(isset($o3c_server))
                            {
                                foreach($data["cameras"] as $cameraId)
                                {
                                    $o = $serverManager->reconnectDevice($cameraId, $o3c_server);
                                    echo json_encode(array("device" => $cameraId ,"output" => $o));

                                    flush();
                                }
                            }
                        }
                        break;
                    case "restartDevice":
                        if(isset($data["cameras"]) && isset($data["server"]))
                        {
                            $o3c_server = $serverManager->getServer($data["server"]);
                            if(isset($o3c_server))
                            {
                                foreach($data["cameras"] as $cameraId)
                                {
                                    $o = $serverManager->restartDevice($cameraId, $o3c_server);
                                    echo json_encode(array("device" => $cameraId ,"output" => $o));

                                    flush();
                                }
                            }
                        }
                        break;

                    case "saveServerSettings":
                        if(isset($data["server"]) && isset($data["display_name"]) && isset($data["host_external"]) && 
                            isset($data["client_port"]) && isset($data["admin_port"]))
                        {
                            $o3c_server = $serverManager->getServer($data["server"]);
                            if(isset($o3c_server))
                            {
                                // ob_implicit_flush(true);

                                $updateDevices = $o3c_server["host_external"] !== (empty(trim($data["host_external"])) ? null : trim($data["host_external"])) || 
                                                $o3c_server["failover_o3c_id"] !== $data["failover_o3c_id"];

                                $o3c_server["display_name"] = empty(trim($data["display_name"])) ? null : trim($data["display_name"]);
                                $o3c_server["host_external"] = empty(trim($data["host_external"])) ? null : trim($data["host_external"]);
                                $o3c_server["failover_o3c_id"] = isset($data["failover_o3c_id"]) ? $data["failover_o3c_id"] : null;
                                $o3c_server["port"] = $data["client_port"];
                                $o3c_server["admin_port"] = $data["admin_port"];

                                
                                if($serverManager->updateServerSettings($o3c_server))
                                {
                                    $newServer = $serverManager->getServerWithServerlist($o3c_server["id"]);
                                    if($updateDevices)
                                    {
                                        // Get affected servers
                                        $affectedServers = $serverManager->getRelatedServers($newServer["name"]);

                                        $affectedServersServerlists = array();
                                        $affectedServerIds = array();
                                        foreach($affectedServers as $affectedServer)
                                        {
                                            $affectedServersServerlists[$affectedServer["id"]] = $affectedServer["serverlist"];
                                            array_push($affectedServerIds, $affectedServer["id"]);
                                        }


                                        // Get devices from servers
                                        $affectedDevices = $serverManager->getDevicesFromArray($affectedServerIds);
                                        $response = array("progress" => 0, "max_progress" => count($affectedDevices), "fails" => array());

                                        foreach($affectedDevices as $device)
                                        {
                                            $response["progress"] += 1;
                                            $newServerList = $affectedServersServerlists[$device["o3c_server_dst"]];

                                            $setResult = trim($serverManager->webRequest(
                                                $device["client_id"] . "/axis-cgi/param.cgi?action=update&root.RemoteService.ServerList=" . $newServerList, 
                                                $device["o3c_admin_host"]
                                            ));

                                            if($setResult != "OK")
                                            {
                                                array_push($response["fails"], $device["client_id"]);
                                            }

                                            echo json_encode($response);
                                            flush();
                                        }
                                    }
                                    $response["server"] = $newServer;
                                    echo json_encode($response);
                                }
                            }
                        }
                        break;
            
                }
            }
            else
            {
                http_response_code(400);
            }
        break;

        case "GET":

            if(isset($_REQUEST["action"]))
            {
                switch($_REQUEST["action"])
                {
                    case "streamDebug":
                        if(isset($_REQUEST["server"]))
                        {
                            $o3c_server = $serverManager->getServer($_REQUEST["server"]);

                            if(isset($o3c_server))
                            {
                                set_time_limit(0); // run the delay as long as the user stays connected
                                ignore_user_abort(true);
                                header("Content-Type: text/plain");

                    
                                $ch = curl_init();
                                curl_setopt($ch, CURLOPT_URL, "http://" . $o3c_server["host"] . ":" . $o3c_server["admin_port"] . "/admin/debug.cgi?level=4");
                    
                                curl_setopt($ch, CURLOPT_WRITEFUNCTION, function($curl, $data) {
                    
                                    echo $data;
                                    flush();
                                    if(connection_aborted()) {
                                        return -1;
                                    }
                                    return strlen($data);
                                });
                                curl_exec($ch);
                                curl_close($ch);
                            }
                        }
                        break;

                    case "getDevicesFromFolder":
                        if(isset($_REQUEST["folder"]))
                        {
                            echo json_encode($folderManager->getDevicesFromFolder(intval($_REQUEST["folder"])));
                        }
                        break;
                }
            }
            else 
            {
                http_response_code(400);
            }
            
        
        break;
}






?>