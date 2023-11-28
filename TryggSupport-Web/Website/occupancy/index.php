<?php 
ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);

require_once __DIR__ . "/../../Login/Login.php";

if($_SERVER["HTTP_HOST"] === "supportsida.local:8011")
{
    $tryggStores = array(
        "http://localhost:1266"
    );
}
else
{
    $tryggStores = array(
        "http://172.62.1.201:1266",
        "http://172.62.14.201:1266",
        "http://172.62.18.203:1266", // Länna
        "http://172.23.5.201:1266",
        "http://172.62.3.201:1266",
        "http://172.62.5.201:1266",
        "http://172.23.17.201:1266",
        "http://172.62.8.201:1266",
        "http://172.62.19.201:1266",
        "http://172.62.32.201:1266"
    );
}

if($_SERVER['REQUEST_METHOD'] === "POST")
{
    header('Content-type: application/json; charset=UTF-8');

    if(isset($_POST["action"]))
    {
        switch($_POST["action"])
        {
            case "list":
                echo "{ \"count\": " . count($tryggStores). "}";
            break;
            case "pollList":
                $results = array();
                foreach($tryggStores as $store)
                {
                    $output = curl($store . "/api?action=getData");

                    $result = json_decode($output, true);
                    $result["StoreUrl"] = $store;

                    array_push($results, $result);
                }

                echo json_encode($results);
                break;
        }
    }

    exit();
}

function curl($url)
{
    $ch = curl_init();

    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_HEADER,0);
    curl_setopt($ch, CURLOPT_CONNECTTIMEOUT_MS, 1000);
    curl_setopt($ch, CURLOPT_ENCODING , "");
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($ch, CURLOPT_FOLLOWLOCATION,1);

    $output = curl_exec($ch);

    if($output === false)
        return "{ \"Error\": \"" . htmlspecialchars(curl_error($ch)) . "\"}";

    curl_close($ch);    
    return $output;
}

?>

<!DOCTYPE html>
<html lang="sv">
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">
    
        <style>
            .counter-container {
                margin-top: 1rem;
                margin-bottom: 1rem;
            }

            /* .header-count[status="under-threshold"] {
                color: #28a745!important;
            }
            .header-count[status="close-threshold"] {
                color: #ffc107!important;
            }
            .header-count[status="above-threshold"] {
                color: #dc3545!important;
            } */

            td[status="under-threshold"] {
                color: #fff;
                background-color: #28a745!important;
            }
            td[status="close-threshold"] {
                background-color: #ffc107!important;
            }
            td[status="above-threshold"] {
                color: #fff;
                background-color: #dc3545!important;
            }

            a.header {
                color: inherit;
            }

            div.counter-container:not(.got-data) {
                display: none;
            }

            table {
                font-size: 18px;
            }

            .hover-image {
                position: absolute;
                width: 480px;
                height: 270px;
                border: 1px solid black;
            }
            .hover-image:not(.visible) { 
                display: none;
            }

            body {
                padding-top: 1rem;
            }
        </style>
        <title>TryggSTORE - Dashboard</title>

        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1" crossorigin="anonymous">
    </head>
    <body>
        <script src="https://code.jquery.com/jquery-3.5.1.min.js" integrity="sha256-9/aliU8dGd2tb6OSsuzixeV4y/faTqgFtohetphbbj0=" crossorigin="anonymous"></script>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/js/bootstrap.bundle.min.js" integrity="sha384-ygbV9kiqUc6oa4msXn9868pTtWMgiQaeYH7/t7LECLbyPA2x65Kgf80OJFdroafW" crossorigin="anonymous"></script>

        <div id="main" class="container">
            <h1>💙TryggSTORE Live Dashboard</h1>
            <h4 id="clock"></h4>

            <table class="table">
                <thead>
                    <tr>
                        <th>Butik</th><th>Besökare just nu</th><th>Gränsvärde gult (80%)</th><th>Gränsvärde rött (max)</th>
                    </tr>
                </thead>
                <tbody id="table-body">
                </tbody>
            </table>
        </div>

        <img class="hover-image" src="/occupancy/assets/buffer.gif">
        <img class="hover-image stream-image" src="/occupancy/assets/buffer.gif">

        <script>
            var isLocal = document.location.hostname == "172.18.0.90";
            $clock = null;
            var counters = -1;

            $(document).ready(function(){
                $clock = $("#clock").text(new Date().toLocaleString());

                setInterval(() => {
                    $clock.text(new Date().toLocaleString());
                }, 333);

                $.ajax({ 
                    url: "/occupancy/index.php",
                    type: "POST",
                    data: { action: "list" },
                    dataType: "json"
                }).done(function(data) {
                    console.log("Stores: " + data.count);
                    counters = data.count;

                    for(var i = 0; i < counters; i++) {

                        var table = document.getElementById("table-body");

                        var row = table.insertRow();
                        row.id = "row-" + i;

                        var headerRow = row.insertCell();
                        var headerLink = document.createElement("a");
                        headerLink.classList = "header",
                        headerRow.appendChild(headerLink);

                        row.insertCell();
                        var closeRow = row.insertCell();
                        closeRow.setAttribute("status", "close-threshold");
                        
                        var maxRow = row.insertCell();
                        maxRow.setAttribute("status", "above-threshold");
                    }

                    if(isLocal)
                    {
                        $("a.header").hover(function(e){
                            $(".hover-image").css({left:e.pageX + 1, top:e.pageY + 1}).toggleClass("visible");
                            $(".stream-image").attr("src", $(this).attr("href") + "/stream.mjpeg");

                        }, function(e) {
                            $(".hover-image").toggleClass("visible");
                            $(".stream-image").attr("src", "");
                        });
                    }

                    pollList();
                    setInterval(pollList, 10 * 1000);

                }).fail(function() {
                    alert("Failed to get list!");
                });
            });
            
            function pollList() {
                $.ajax({ 
                    url: "/occupancy/index.php",
                    type: "POST",
                    data: { action: "pollList" },
                    dataType: "json"
                }).done(function(dataList) {

                    for(var i in dataList)
                    {
                        if(dataList[i].Error !== undefined)
                        {
                            console.log(dataList[i].StoreUrl + ": " + dataList[i].Error);
                        }
                        else
                        {
                            var row = document.getElementById("row-" + i);
                            var siteLink = row.querySelector(".header");
                            siteLink.classList = "header";
                            siteLink.innerText = dataList[i].SiteName == "" ? "N/A" : dataList[i].SiteName;
                            siteLink.target = "_blank";
                            if(isLocal)
                                siteLink.href = dataList[i].StoreUrl;

                            var statusClass = "under-threshold";
                            if(dataList[i].EmergencyStop || dataList[i].CurrentCount >= dataList[i].MaxOccupancyThreshold)
                                statusClass = "above-threshold";
                            else if(dataList[i].CurrentCount >= dataList[i].CloseOccupancyThreshold)
                                statusClass = "close-threshold";

                            row.cells[1].setAttribute("status", statusClass);


                            row.cells[1].innerText = dataList[i].CurrentCount + (dataList[i].EmergencyStop ? " (Manuellt stopp)" : "");
                            // row.cells[1].setAttribute("status", statusClass);
                            row.cells[1].classList = "header-count";

                            row.cells[2].innerText = dataList[i].CloseOccupancyThreshold;

                            row.cells[3].innerText = dataList[i].MaxOccupancyThreshold;
                        }
                    }
                    sortTable();

                }).fail(function() {
                    console.error("Failed to fetch data!");
                });
            }


            function sortTable() {
                var table, rows, switching, i, x, y, shouldSwitch;
                table = document.getElementById("table-body");
                switching = true;
                /*Make a loop that will continue until
                no switching has been done:*/
                while (switching) {
                    //start by saying: no switching is done:
                    switching = false;
                    rows = table.rows;
                    /*Loop through all table rows (except the
                    first, which contains table headers):*/
                    for (i = 0; i < (rows.length - 1); i++) {
                    //start by saying there should be no switching:
                    shouldSwitch = false;
                    /*Get the two elements you want to compare,
                    one from current row and one from the next:*/
                    x = rows[i].getElementsByTagName("TD")[0].children[0];
                    y = rows[i + 1].getElementsByTagName("TD")[0].children[0];
                    //check if the two rows should switch place:
                    if (x.innerHTML.toLowerCase() > y.innerHTML.toLowerCase()) {
                        //if so, mark as a switch and break the loop:
                        shouldSwitch = true;
                        break;
                    }
                    }
                    if (shouldSwitch) {
                    /*If a switch has been marked, make the switch
                    and mark that a switch has been done:*/
                    rows[i].parentNode.insertBefore(rows[i + 1], rows[i]);
                    switching = true;
                    }
                }
            }

        </script>

    </body>
</html>