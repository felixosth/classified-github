<?php
    ini_set('display_errors',0); 
    //error_reporting(E_ALL);

    if(isset($_POST["uid"]))
    {
        require_once "../dbconfig.php";

        $uid = $_POST["uid"];

        if($_POST["action"] == "acknowledge" || $_POST["action"] == "unacknowledge")
        {
            $query = "UPDATE alarms SET acknowledged=? WHERE uid=?";
            if($stmt = $conn->prepare($query))
            {
                $isAck = $_POST["action"] == "acknowledge" ? "1" : "0";
                $stmt->bind_param("ss", $isAck, $uid);
                $stmt->execute();
                
                echo $isAck;
            }
        }
        else if($_POST["action"] == "alarm")
        {
            $alarmStatus = 1;

            $query = "SELECT alarm FROM alarms WHERE uid=?";
            if($stmt = $conn->prepare($query))
            {
                $stmt->bind_param("s", $uid);
                $stmt->execute();
                $result = $stmt->get_result();
                $row = $result->fetch_assoc();
                $alarmStatus = $row["alarm"];
            }

            if($alarmStatus == 0)
            {
                try
                {
                    $port = "13344";
                    $ip = "192.168.2.64";
                    set_time_limit(20);

                    $socket = socket_create(AF_INET, SOCK_STREAM, 0) or die("Could not create socket");
                    $connection = socket_connect($socket, $ip, $port) or die("Kunde ej kontakta larmcentral.");

                    $msg = "SKICKA LARM FIFAN!!!";
                    socket_write($socket, $msg, strlen($msg)) or die("Could not send data");
                    $response = socket_read($socket, 1024) or die("Could not read response");
                    socket_close($socket);

                    $query = "UPDATE alarms SET alarm=?, alarmresponse=? WHERE uid=?";
                    if($stmt = $conn->prepare($query))
                    {
                        $alarmStatus = 1;
                        $stmt->bind_param("sss", $alarmStatus, $response, $uid);
                        $stmt->execute();
                    }
                }
                catch(\Exception $ex)
                {
                    echo "fel :(";
                }

                echo "Larm skickat!";
            }
            else
                echo "Larm har redan skickats.";
        }
    }


?>