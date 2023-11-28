<?php

require_once __DIR__ . "/../../lib/config.php";
// mysqli_report(MYSQLI_REPORT_ALL);
if(isset($_GET["address"]))
{
    $expl = explode(":", $_GET["address"]);
    if($stmt = $conn->prepare("SELECT id FROM o3c_servers WHERE ifnull(host_external, host) = ? AND port=? LIMIT 1;"))
    {
        $stmt->bind_param("ss", $expl[0], $expl[1]);
        if($stmt->execute())
        {
            $server = $stmt->get_result()->fetch_assoc();
            $stmt->free_result();

            if(isset($server))
            {
                header("Location: /servers/manage.php?server=" . $server["id"]);
            }
            else
            {
                http_response_code(400);
            }
        }
    }
    else
    {

    }
}

?>