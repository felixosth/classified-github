<?php
    ini_set('display_errors',1); 
    error_reporting(E_ALL);

    require_once "../dbconfig.php";
    $data = json_decode(file_get_contents('php://input'), true);

    if (isset($data["alarmText"]) && isset($data["alarmImage"]) && isset($data["mguid"]))
    {
        $alarmText = $data["alarmText"];
        $encodedImage = $data["alarmImage"];
        $mguid = $data["mguid"];

        $licCheck = checkLicense($conn, $mguid);

        if($licCheck != false)
        {
            $uid = uniqid();
            while(!checkUID($uid, $conn))
            {
                $uid = uniqid();
            }

            if($encodedImage != "")
            {
                $image = base64_decode($encodedImage);
                file_put_contents("../case/img/$uid.png", $image);
            }

            $query = "INSERT INTO alarms (uid, text, license, media) VALUES (?, ?, ?, ?)";
            if($stmt = $conn->prepare($query))
            {
                $media = $encodedImage != "" ? "img" : "vid";
                $stmt->bind_param("ssss", $uid, $alarmText, $licCheck, $media);
                $stmt->execute();
                echo $uid;
            }
        }
        else {
            echo "invalid license";
        }
    }


    function checkUID($uid, $conn)
    {
        $rows = 0;
        if($stmt = $conn->prepare("SELECT uid FROM alarms WHERE uid=?"))
        {
            $stmt->bind_param("s", $uid);
            $stmt->execute();
            $stmt->store_result();
            $rows = $stmt->num_rows;
        }

        return $rows < 1;
    }



?>