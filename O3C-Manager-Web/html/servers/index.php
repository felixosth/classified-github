<?php

require_once __DIR__ . "/../../lib/config.php";

$o3c_servers = array();

if($stmt = $conn->prepare("SELECT concat(ifnull(s.host_external, s.host), ':', s.port) client_host, s.*, sum(if(d.state=1, 1, 0)) as online_devices, sum(if(d.state=0, 1, 0)) as offline_devices FROM o3c_servers AS s LEFT JOIN devices_dynamic AS d ON s.id = d.o3c_server GROUP BY s.id ORDER BY s.name;"))
// if($stmt = $conn->prepare("SELECT s.*, count(distinct d.id) devices FROM o3c_servers s LEFT JOIN devices_dynamic d ON s.id = d.o3c_server GROUP BY s.id ORDER BY s.name;"))
{
    $stmt->execute();
    $result = $stmt->get_result();
    while($row = $result->fetch_assoc())
    {
        array_push($o3c_servers, $row);
    }
}

?>

<!DOCTYPE html>
<html>
    <head>
        <?php getHead("Servers"); ?>

        <style>

        .progress {
            height: 1.5rem;
        }
        </style>
    </head>
    <body>
        <?php getNavbar("servers"); ?>

        <div class="container">

            <h2>Servers</h2>
            <table class="table">
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Name</th>
                        <th>Host</th>
                        <th>Online / Offline devices</th>
                        <th>State</th>
                    </tr>
                </thead>
                <tbody>
                <?php
                    foreach($o3c_servers as $server)
                    {
                        echo "<tr>";
                        echo "<td>".$server["id"]."</td>";
                        echo '<td><a href="manage.php?server=' . $server["id"] . '">' . ($server["display_name"] != null ? "{$server["display_name"]} ({$server["name"]})" : $server["name"]) . "</a></td>";
                        echo "<td>" . $server["client_host"] . "</td>";

                        // echo "<td>" . $server["online_devices"] . " / " . $server["offline_devices"] . "</td>";
                        $totalDevices = $server["online_devices"] + $server["offline_devices"];
                        $percentUp = $totalDevices > 0 ? intval($server["online_devices"] / $totalDevices * 100) : 0;
                        $percentDown = $totalDevices > 0 ? intval($server["offline_devices"] / $totalDevices * 100) : 0;

                        $percentDown += $totalDevices > 0 ? 100 - $percentDown - $percentUp : 0;

                        echo /* bg-success? */
                        "<td>
                            <div class=\"progress devices-progress\" title=\"".$server["online_devices"] . " / " . $server["offline_devices"]."\">
                                <div class=\"progress-bar\" role=\"progressbar\" style=\"width: {$percentUp}%\" aria-valuenow=\"{$percentUp}\" aria-valuemin=\"0\" aria-valuemax=\"100\">{$server["online_devices"]}</div>
                                <div class=\"progress-bar progress-bar-striped bg-danger\" role=\"progressbar\" style=\"width: {$percentDown}%\" aria-valuenow=\"{$percentDown}\" aria-valuemin=\"0\" aria-valuemax=\"100\">{$server["offline_devices"]}</div>
                            </div>
                        </td>";

                        $isUp = $server["state"] == 1;
                        echo '<td>' . ($isUp ? '<span class="fa-stack" title="Online"><i class="fas fa-plug fa-stack-1x"></i></span>' : '<span class="fa-stack" title="Offline"><i class="fas fa-plug fa-stack-1x"></i><i class="fas fa-ban fa-stack-2x" style="color:Red"></i></span>') . "</td>";
                        
                        echo "</tr>";
                    }
                ?>
                </tbody>
            </table>

        </div>
    </body>
</html>