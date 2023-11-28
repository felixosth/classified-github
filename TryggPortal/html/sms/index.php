<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);

$thisPage = "sms";
require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";

$query = "SELECT * FROM sms ORDER BY ID DESC LIMIT 400;";

if($stmt = $conn->prepare($query))
{
    $stmt->execute();
    $result = $stmt->get_result();
    $smsArray = array();
    while($row = $result->fetch_assoc())
    {
        array_push($smsArray, $row);
    }
    $stmt->free_result();
    $stmt->close();
}
else
    echo "Unable to fetch SMS";

$conn->close();
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>ISTP - SMS</title>
    <?php include "../functions/getbt.php"; ?>
    <link rel="stylesheet" type="text/css" href="../customStyles.css">
    <style type="text/css">
        body{ font: 14px sans-serif; }
        /*@media only screen and (min-device-width: 1600px) {
            .container-fluid
            {
                width:1600px;
            }
        }*/
    </style>
</head>
<body>
    
    <?php
    $activePage = "sms";
    include "../functions/navbar.php";
    include "../modals/sendSmsModal.php"; 
    include "../modals/filterSmsModal.php"; ?>
    <div class="container-fluid mx-auto">
        <div class="page-header">
            <h1>SMS</h1>
            <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#sendSms" <?php echo ($isAdmin ? "" : "disabled") ?>>Skicka SMS</button>
            <button type="button" class="btn btn-secondary" data-toggle="modal" data-target="#filterSms">Filtrera</button>
            <!--<a href="newlicense.php" class="btn btn-success" >Ny licens</a>-->
        </div>
        <!--<input type="button" class="btn btn-default" value="Hem" onclick="location.href = 'index.php';">-->
        <br><br>
        <div class="table-responsive-xl">
            <table class="table table-striped table-bordered">
                <thead>
                    <tr>
                    <th scope="col">ID</th><th scope="col">Avsändare</th><th scope="col">Mottagare</th><th scope="col">Meddelande</th><th scope="col">Licens</th><th scope="col">Datum</th>
                    </tr>
                </thead>
                <tbody>
                    <?php
                        $i = 0;
                        foreach($smsArray as $sqlRow) {
                            echo "<tr>";
                            echo "<td>".$sqlRow['id']."</td><td>".$sqlRow['sender']."</td><td>".$sqlRow['reciever']."</td><td>".$sqlRow['message']."</td>";
                            echo "<td><a href='#filter' class='licenseCol'>" . $sqlRow['licenseKey'] . "</a></td>";
                            echo "<td>".$sqlRow['date']."</td>";
                            echo "</tr>";
                        $i++;
                    }
                    ?>
                </tbody>
                <caption><?php echo count($smsArray); ?> sms</caption>
            </table>
        </div>
    </div>
    <script>
    var elements = document.querySelectorAll(".licenseCol");
    //console.log(elements);
    for (var i = 0; i < elements.length; i++) {
        elements[i].onclick = fillModal;
    }
    function fillModal()
    {
        lic = document.getElementById("licBox");
        lic.value = this.innerHTML;
        $("#filterSms").modal();
    }

    $(function () {
    $('[data-toggle="popover"]').popover()
    })
    $(function () {
    $('[data-toggle="tooltip"]').tooltip()
    })
    $('.pop').on('shown.bs.popover', function () {
        var $pop = $(this);
        setTimeout(function () {
            $pop.popover('hide');
        }, 800);
    });

    </script>
</body>
</html>