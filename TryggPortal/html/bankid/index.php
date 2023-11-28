<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);

require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";

$query = "SELECT * FROM bankidrequests ORDER BY ID DESC LIMIT 200;";

if($stmt = $conn->prepare($query))
{
    $stmt->execute();
    $result = $stmt->get_result();
    $requestsArray = array();
    while($row = $result->fetch_assoc())
    {
        array_push($requestsArray, $row);
    }
    $stmt->free_result();
    $stmt->close();
}

$conn->close();
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>ISTP - BankID</title>
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
    $activePage = "bankid";
    include "../functions/navbar.php";
    include __DIR__."/../modals/filterBankidModal.php"; ?>
    <div class="container-fluid mx-auto">
        <div class="page-header">
            <h1>BankID</h1>
            <button id="filterBtn" type="button" class="btn btn-info">Filtrera</button>
        </div>
        <br><br>
        <div class="table-responsive-xl">
            <table class="table table-striped table-bordered">
                <thead>
                    <tr>
                    <th scope="col">ID</th><th scope="col">Licens</th><th scope="col">Metod</th><th scope="col">Collects</th><th scope="col">Status</th><th scope="col">Datum</th>
                    </tr>
                </thead>
                <tbody>
                    <?php
                        $i = 1;
                        foreach($requestsArray as $sqlRow) {
                            echo "<tr>";
                            echo "<td>".$i."</td><td><a href='#filter' class='licenseCol'>" . $sqlRow['license'] . "</a></td>";
                            echo "<td>".$sqlRow['method']."</td><td>".$sqlRow['collects']."</td><td>".$sqlRow['lastcollectstatus']."</td>";
                            echo "<td>".$sqlRow['creationdate']."</td>";
                            echo "</tr>";
                            $i++;
                        }
                    ?>
                </tbody>
                <caption><?php echo count($requestsArray); ?> förfrågningar</caption>
            </table>
        </div>
    </div>
    <script>


    $(document).ready(function()
    {
        var elements = document.querySelectorAll(".licenseCol");
        //console.log(elements);
        for (var i = 0; i < elements.length; i++) {
            elements[i].onclick = fillModal;
        }

        $("#filterBtn").click(function()
        {
            $("input[name='license']").val("");
            $("select[name='month']").val(new Date().getMonth() + 1);
            $("input[name='year']").val(new Date().getFullYear());

            $("#filterBankid").modal();
        });
    });


    function fillModal()
    {
        $("input[name='license']").val(this.innerHTML);
        $("select[name='month']").val(new Date().getMonth() + 1);
        $("input[name='year']").val(new Date().getFullYear());
        $("#filterBankid").modal();
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