<?php

require_once __DIR__ . "/../../lib/config.php";


function drawFolder($folder, $level = 0)
{
    $haveChildren = count($folder["children"]) > 0;

    $folderNameLink = '<a href="#" folder="'.$folder["id"].'" class="link-primary folder-link">' . $folder["name"] . ' <span class="ico"></span></a>'; 

    if($haveChildren)
    {
        if($level == 0) // Top parent
            echo '<li>'.$folderNameLink.'<ul>';
        else
        {
            echo '<li><span class="caret" tabindex="0"><i class="fas fa-caret-right fa-lg"></i></span>'.$folderNameLink;
            echo '<ul class="nested">';
        }

        foreach($folder["children"] as $childFolder)
        {
            drawFolder($childFolder, $level += 1);
        }
        echo '</ul></li>';
    }
    else
    {
        echo "<li>" . $folderNameLink . "</li>";
    }
}

?>

<!DOCTYPE html>
<html>
    <head>
        <?php getHead("Folders"); ?>
        <link href="/lib/css/treeview.css" rel="stylesheet"/>
    </head>
    <body>
        <?php getNavbar("folders"); ?>

        <div class="container">

            <h2>Folders</h2>

            <div class="row">
                <div class="col col-md-auto border">
                    <ul id="devices-ul">
                        <?php
                            $folders = $folderManager->getFolderTree();
                            drawFolder($folders[0]);
                        ?>
                    </ul>
                </div>
                
                <div class="col border">

                    <table class="table">
                        <thead>
                            <tr>
                                <th>Client ID</th>
                                <th>Name</th>
                                <th>Model</th>
                                <th>Firmware</th>
                                <th>O3C Server</th>
                                <th>State</th>
                            </tr>
                        </thead>
                        <tbody id="devices-tbody">
                        </tbody>
                    </table>
                </div>
            </div>

        </div>

        <script>


            var tbody = document.getElementById("devices-tbody");

            $(document).ready(function() {

                var toggler = document.getElementsByClassName("caret");
                var i;

                for (i = 0; i < toggler.length; i++) {
                    toggler[i].addEventListener("click", function() {
                        console.log(this);
                        this.parentElement.querySelector(".nested").classList.toggle("active");
                        this.querySelector("i").classList.toggle("fa-rotate-90");
                    });
                }

                $("a.folder-link").click(function() {
                    var folder = this.getAttribute("folder");

                    var $ico = $(this).children(".ico").html('<i class="fas fa-spinner fa-pulse"></i>');

                    apiCallGet("/api/api.php?action=getDevicesFromFolder&folder=" + folder, function(data) {

                        var devices = JSON.parse(data);
                        tbody.innerHTML = "";

                        if(devices.length == 0) {
                            var row = tbody.insertRow();
                            var cell = row.insertCell();

                            cell.setAttribute("colspan", "5");
                            cell.innerText = "There are no devices in this folder.";

                        }
                        else {
                            devices.forEach(function(device) {

                                var row = tbody.insertRow();

                                row.insertCell().innerHTML = '<a href="#">' + device.client_id.toUpperCase() + '</a>';
                                row.insertCell().innerText = device.display_name || "N/A";
                                row.insertCell().innerText = device.model;
                                row.insertCell().innerText = device.firmware;
                                row.insertCell().innerText = device.o3c_name;
                                row.insertCell().innerHTML = device.state == 1 ? '<span class="fa-stack" title="Online"><i class="fas fa-plug fa-stack-1x"></i></span>' : '<span class="fa-stack" title="Offline"><i class="fas fa-plug fa-stack-1x"></i><i class="fas fa-ban fa-stack-2x" style="color:Red"></i></span>';
                            });
                        }

                        $ico.html("");
                    });

                });

                $("*[tabindex]").keyup(function(event) {
                    if (event.keyCode === 13) {
                        (this).click();
                    }
                });

                $("#devices-ul a").first().click();
            });

        </script>
    </body>
</html>