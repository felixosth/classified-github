<?php

require_once __DIR__ . "/../../lib/config.php";

$o3c_server_id = isset($_REQUEST["server"]) ? intval($_REQUEST["server"]) : null;

if(isset($o3c_server_id) == false)
{
    http_response_code(400);
    exit;
}

$o3c_server = $serverManager->getServerWithServerlist($o3c_server_id);

if(isset($o3c_server) == false)
{
    http_response_code(400);
    exit;
}

$o3c_devices = $serverManager->getDevices($o3c_server["id"]);

?>

<!DOCTYPE html>
<html lang="en">
    <head>
        <?php getHead("Manage"); ?>

        <style>
        </style>
    </head>
    <body>
        <?php getNavbar("servers"); ?>
        <input type="hidden" id="o3c_server" value="<?=$o3c_server["id"]?>"/>

        <div class="container">

            <a href="/servers" class="link-primary">Go back to servers</a>

            <h1>[<?=$o3c_server["id"]?>] <span id="server-name"><?= $o3c_server["display_name"] != null ? "{$o3c_server["display_name"]} ({$o3c_server["name"]})" : $o3c_server["name"]?></span></h1>
            <p>Serverlist: <code id="serverlist">
            <?php
            // $o3c_server["serverlist"]
            $servers = explode(",",$o3c_server["serverlist"]);
            for ($i=0; $i < count($servers); $i++) { 
                echo ($i == 0 ? $servers[$i] :  '<a href="/servers/find.php?address=' . $servers[$i] . '" class="link-primary">' . $servers[$i] . '</a>' ) . ($i < count($servers) - 1 ? "," : "");
            }
            ?>
            </code></p>

            <div class="row">
                <label>Server actions</label>
                <div class="col">
                    <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#server-settings-modal"><i class="fas fa-cogs"></i> Server settings</button>
                    <button type="button" id="register-device-button" class="btn btn-secondary"><i class="fas fa-cloud-upload-alt"></i> Register device</button>
                    <button type="button" id="open-debug-button" class="btn btn-info"><i class="fas fa-list"></i> Open debug log</button>
                </div>
            </div>
            <div class="row" style="margin-top: 1rem">
                <label>Device actions</label>
                <div class="col">
                    <button type="button" id="change-server-button" class="device-btn btn btn-primary" disabled><i class="fas fa-people-carry"></i> Change server</button>
                    <button type="button" id="redirect-device-button" class="device-btn btn btn-secondary" disabled><i class="fas fa-directions"></i> Redirect</button>
                    <button type="button" id="reconnect-device-button" class="device-btn btn btn-warning" disabled><i class="fas fa-unlink"></i> Reconnect</button>
                    <button type="button" id="restart-device-button" class="device-btn btn btn-danger" disabled><i class="fas fa-power-off"></i> Restart</button>
                </div>
            </div>

            <table class="table">
                <thead>
                    <tr>
                        <th style="width: 1%"></th>
                        <th>Client ID</th>
                        <th>Name</th>
                        <th>Model</th>
                        <th>Firmware</th>
                        <th>Primary O3C</th>
                        <th>Source addr</th>
                        <th>State</th>
                    </tr>
                </thead>
                <tbody>
                <?php

                if(count($o3c_devices) == 0)
                    echo '<tr><td colspan="6">There are no devices in this server.</td></tr>';
                else
                    foreach($o3c_devices as $device)
                    {
                        echo "<tr camid='{$device["id"]}'>";
                        echo '<td class="checkbox-col"><input type="checkbox" class="device-checkbox"' . ($device["state"] == 0 ? "disabled" : "") . '/></td>';
                        echo "<td>" . strtoupper($device["client_id"]) . "</td>";
                        echo "<td>" . ($device["display_name"] ?? "N/A") . "</td>";
                        echo "<td>" . $device["model"] . "</td>";
                        echo "<td>" . ($device["firmware"] ?? "N/A") . "</td>";
                        echo "<td>" . $device["o3c_server_dst_name"] . "</td>";
                        echo "<td>" . ($device["source_addr"] ?? "N/A") . "</td>";
                        

                        $isUp = $device["state"] == 1;
                        echo '<td>' . ($isUp ? '<span class="fa-stack" title="Online"><i class="fas fa-plug fa-stack-1x"></i></span>' : '<span class="fa-stack" title="Offline"><i class="fas fa-plug fa-stack-1x"></i><i class="fas fa-ban fa-stack-2x" style="color:Red"></i></span>') . "</td>";
                        echo "</tr>";
                    }
                ?>
                </tbody>
            </table>

        </div>

        <script src="/servers/js/manage.js"></script>

        <?php
            require_once __DIR__ . "/modals/change-server-modal.php";
            require_once __DIR__ . "/modals/register-device-modal.php";
            require_once __DIR__ . "/modals/debug-log-modal.php";
            require_once __DIR__ . "/modals/server-settings-modal.php";
        ?>
    </body>
</html>