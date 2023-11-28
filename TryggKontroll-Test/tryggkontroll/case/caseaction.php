<?php
    ini_set('display_errors',1); 
    error_reporting(E_ALL);

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
                
                header("Location: view.php?uid=$uid");
            }
        }
    }


?>