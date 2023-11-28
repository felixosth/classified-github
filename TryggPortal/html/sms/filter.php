<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);

$month = $_POST["month"];
$year = $_POST["year"];

$query = "SELECT * FROM sms WHERE YEAR(date)=?";

$stringbinds = "s";
$bindArray = array($year);

if (isset($_POST["license"]) && $_POST["license"] != "") {
    $query = $query . " AND licenseKey=?";
    $stringbinds = $stringbinds . "s";
    $license = trim($_POST["license"]);
    array_push($bindArray, $license);
}

if ($month != "0") {
    $query = $query . " AND MONTH(date)=?";
    $stringbinds = $stringbinds . "s";
    array_push($bindArray, $month);
}

require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";

if($stmt = $conn->prepare($query))
{
    $stmt->bind_param($stringbinds, ...$bindArray);
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
?>

<html>
    <head>
    <?php include "../functions/getbt.php"; ?>
    <link rel="stylesheet" type="text/css" href="../customStyles.css">
    <title>ISTP - SMS Filter </title>
    </head>
    <body>
        <div class="container-fluid mx-auto">
        <br>
            <button type="button" class="btn btn-primary" onclick='location.href="<?=LOC?>/sms/"'>Tillbaka</button>
            <div class="table-responsive-xl">
                <h4>Licens: <?php echo (isset($license) ? $license : "Ej specificerad"); ?> </h4>
                <table class="table table-striped table-bordered">
                    <thead>
                        <tr>
                        <th scope="col">Nr</th><th scope="col">Avsändare</th><th scope="col">Mottagare</th><th scope="col">Meddelande</th>
                        <?php echo (!isset($license) ? "<th scope='col'>Licens</th>" : ""); ?>
                        <th scope="col">Datum</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php
                            if(isset($smsArray))
                            {
                                $i = 0;
                                foreach($smsArray as $sqlRow) {
                                    echo "<tr>";
                                    echo "<td>". ($i + 1) ."</td><td>".$sqlRow['sender']."</td><td>".$sqlRow['reciever']."</td><td>".$sqlRow['message']."</td>";
                                    echo (!isset($license) ? "<td>".$sqlRow['licenseKey']."</td>" : "");
                                    
                                    echo "<td>".$sqlRow['date']."</td>";
                                    echo "</tr>";
                                $i++;
                            }
                        }
                        ?>
                    </tbody>
                    <caption><?php echo (isset($smsArray) ? count($smsArray) : 0); ?> sms</caption>
                </table>
            </div>
        </div>
    </body>
</html>