<?php

function addToLog($action, $object, $conn)
{
    if($myStmt = $conn->prepare("INSERT INTO log (action, object, action_by, from_ip) VALUES (?,?,?,?)"))
    {
        $myStmt->bind_param("ssss", $action, $object, $_SESSION["username"], $_SERVER['REMOTE_ADDR']);
        if(!$myStmt->execute())
        {
            echo $myStmt->error;
            die;
        }
    }
}


?>