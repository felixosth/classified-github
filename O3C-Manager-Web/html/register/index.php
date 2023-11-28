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
<html lang="en">
    <head>
        <?php getHead("Manage"); ?>
        <link href="/lib/css/treeview.css" rel="stylesheet"/>

        <style>
            ol#todo li > select {
                width: 33%!important;
            }
            ol#todo  > li {
                margin-top: 1rem;
            }

            ol#todo li > * {
                margin-left: .5rem;
            }

            td[status="complete"] {
                color: #198754;
            }

            td[status="failed"] {
                color: #dc3545;
            }

            i.fa-pause-circle {
                opacity: .5;
            }

            .selected-folder {
                border: 1px solid gray;
                font-weight: bold;
                /* border-radius: .5rem; */
                padding: .1rem;
            }
        </style>
    </head>
    <body>
        <?php getNavbar("register"); ?>
        
        <script src="/lib/js/jquery.csv.min.js"></script>

        <div class="container">
            <ol id="todo">
                <li>
                    <label class="btn btn-outline-secondary">
                        Load CSV file
                        <input id="csv-file-input" type="file" accept=".csv" hidden>
                    </label>
                </li>
                <li>
                    <label>Select MAC column</label>
                    <select id="mac-col-select" class="form-select" disabled></select>
                </li>
                <li>
                    <label>Select OAK column</label>
                    <select id="oak-col-select" class="form-select" disabled></select>
                </li>
                <li>
                    <label>Select name column (optional)</label>
                    <select id="name-col-select" class="form-select" disabled></select>
                </li>
                <li>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>MAC</th>
                                <th>OAK</th>
                                <th>Name</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody id="device-table-body">
                        </tbody>
                    </table>
                </li>
                <li>
                    <label>Select O3C server</label>
                    <select id="server-select" class="form-select">
                        <?php
                            $servers = $serverManager->getServers();

                            foreach($servers as $server)
                            {
                                echo "<option value=\"{$server["id"]}\">{$server["name"]}</option>";
                            }
                            
                        ?>
                    </select>
                </li>
                <li>
                    <label>Selected folder: <span id="selected-folder-label"></span></label>

                    <ul id="devices-ul">
                        <?php
                            $folders = $folderManager->getFolderTree();
                            drawFolder($folders[0]);
                        ?>
                    </ul>
                    <!-- <select id="folder-select" class="form-select">
                        <?php
                            $folders = $folderManager->getFolders();

                            foreach($folders as $folder)
                            {
                                echo "<option value=\"{$folder["id"]}\">{$folder["name"]}</option>";
                            }
                            
                        ?>
                    </select> -->
                </li>
                <li>
                    <button type="button" id="register-devices-btn" class="btn btn-primary" disabled>Register devices</button>
                </li>
            </ol>
        </div>

        <script>

            var rows = [];
            var table = document.getElementById("device-table-body");
            var selectedGroup = null;

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


                $(".folder-link").click(function() {

                    if(selectedGroup != null)
                    {
                        selectedGroup.classList.toggle("selected-folder");
                    }

                    this.classList.toggle("selected-folder");
                    selectedGroup = this;
                    document.getElementById("selected-folder-label").innerText = this.innerText;

                }).first().click();
                

                $("#mac-col-select").on("change", selectChange);
                $("#oak-col-select").on("change", selectChange);
                $("#name-col-select").on("change", selectChange);

                $("#csv-file-input").on("change", function() {

                    var reader = new FileReader();
                    reader.onload = function(event) {

                        try {
                            var csv = event.target.result;

                            if(csv.trim() == "") {
                                alert("Empty file!");
                                return;
                            }

                            var separator = determineMost(event.target.result.split(/[\n\r]+/g)[0]);
                            rows = $.csv.toObjects(csv, { separator });

                            if(rows.length > 0) {
                                $("#csv-file-input").val("");

                                var firstRow = rows[0];
                                var propNames = Object.getOwnPropertyNames(firstRow);

                                if(propNames.length >= 2) {

                                    $("#mac-col-select").html("");
                                    $("#oak-col-select").html("");
                                    $("#name-col-select").html('<option value="null">N/A</option>');

                                    propNames.forEach(function(propName) {

                                        var option = "<option>" + propName + "</option>";

                                        $("#oak-col-select").append(option).prop("disabled", false);
                                        $("#mac-col-select").append(option).prop("disabled", false);
                                        $("#name-col-select").append(option).prop("disabled", false);

                                    });

                                    $("#mac-col-select").val(propNames[0]);
                                    $("#oak-col-select").val(propNames[1]);
                                    $("#name-col-select").val("null");
                                    $("#register-devices-btn").prop("disabled", false);

                                    selectChange();
                                }
                                else
                                    alert("Invalid csv");
                            }
                            else
                                alert("Invalid csv");

                        }
                        catch(ex) {
                            alert("Error reading file!\r\n" + ex);
                        }

                    };
                    reader.readAsText($("#csv-file-input")[0].files[0]);
                });

                $("#register-devices-btn").click(function() {
                    if(confirm("Are you sure you want to register these devices?")){

                        registerRow( 0, 
                            parseInt($("#server-select").val()), 
                            parseInt($(".selected-folder").attr("folder"))
                        );
                    }
                });
            });

            function registerRow(rowNumber, serverId, folder)
            {
                var row = table.rows[rowNumber];

                if(row === undefined)
                    return;

                var mac = row.cells[0].innerText.replace(/ /g,'');
                var oak = row.cells[1].innerText.replace(/ /g,'');
                var name = row.cells[2].innerText.replace(/ /g,'');
                if(name == "N/A")
                    name = null;

                row.cells[3].setAttribute("status", "inprogress");
                row.cells[3].innerHTML = '<i class="fas fa-spinner fa-pulse fa-2x"></i>';

                apiCall( {
                        action: "registerDevice",
                        mac,
                        oak,
                        name,
                        folder,
                        server: serverId
                    }, function(data) {

                        if(data.includes("error") == false) {
                            row.cells[3].setAttribute("status", "complete");
                            row.cells[3].title = "Complete";
                            row.cells[3].innerHTML = '<i class="far fa-check-circle fa-2x"></i>';
                        }
                        else {
                            row.cells[3].setAttribute("status", "failed");
                            row.cells[3].innerHTML = '<i class="far fa-times-circle fa-2x"></i>';
                            row.cells[3].title = data;
                        }
                        console.log(data);
                    });
                registerRow(rowNumber + 1, serverId, folder);
            }

            function selectChange()
            {
                if(rows.length > 0) {

                    table.innerHTML = "";

                    var macPropName = $("#mac-col-select").val();
                    var oakPropName = $("#oak-col-select").val();
                    var namePropName = $("#name-col-select").val();

                    rows.forEach(function(r) {
                        var row = table.insertRow();
                        row.insertCell().innerText = r[macPropName];
                        row.insertCell().innerText = r[oakPropName];
                        row.insertCell().innerText = r[namePropName] ?? "N/A";


                        var statusCell = row.insertCell();
                        statusCell.title = "In queue";
                        statusCell.innerHTML = '<i class="far fa-pause-circle fa-2x"></i>';
                        statusCell.setAttribute("status", "pause");

                    });

                }
            }

            function determineMost (chunk) {
                var ignoreString = false
                var itemCount = {}
                var maxValue = 0
                var maxChar
                var currValue
                [',', ';', '\t', '|'].forEach(function (item) {
                    itemCount[item] = 0
                })
                for (var i = 0; i < chunk.length; i++) {
                    if (chunk[i] === '"') ignoreString = !ignoreString
                    else if (!ignoreString && chunk[i] in itemCount) {
                    currValue = ++itemCount[chunk[i]]
                    if (currValue > maxValue) {
                        maxValue = currValue
                        maxChar = chunk[i]
                    }
                    }
                }
                return maxChar
                }

        </script>

    </body>
</html>