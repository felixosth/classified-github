<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);

require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";

$month = $_GET["month"];
$year = $_GET["year"];

$query = "SELECT * FROM bankidrequests WHERE enviroment='live' AND YEAR(creationdate)=?";

$stringbinds = "s";
$bindArray = array($year);

if (isset($_GET["license"]) && $_GET["license"] !== "") {
    $query = $query . " AND license=?";
    $stringbinds = $stringbinds . "s";
    $license = trim($_GET["license"]);
    array_push($bindArray, $license);

    if($stmt = $conn->prepare("SELECT * FROM licenses WHERE LicenseGUID=?;"))
    {
        $stmt->bind_param("s", $license);
        $stmt->execute();
        $licenseObj = $stmt->get_result()->fetch_assoc();
    }
}

if ($month != "0") {
    $query = $query . " AND MONTH(creationdate)=?";
    $stringbinds = $stringbinds . "s";
    array_push($bindArray, $month);
}

if($stmt = $conn->prepare($query))
{
    $stmt->bind_param($stringbinds, ...$bindArray);
    $stmt->execute();
    $result = $stmt->get_result();
    $requestArray = array();
    while($row = $result->fetch_assoc())
    {
        array_push($requestArray, $row);
    }
    $stmt->free_result();
    $stmt->close();
}

?>

<html>
    <head>
    <?php include "../functions/getbt.php"; ?>
    <link rel="stylesheet" type="text/css" href="../customStyles.css">
    <title>ISTP - BankID Filter </title>
    </head>
    <body>
        <div class="container-fluid mx-auto">
        <br>
            <div class="table-responsive-xl">
                <?php
                    if(isset($licenseObj))
                    {
                        echo "<h3>{$licenseObj["Customer"]} - {$licenseObj["Site"]}</h3>";
                    }
                ?>
                <h5>[TryggLogin] <?=(isset($license) ? $license : "Licens ej specificerad")?></h5>
                <table class="table table-striped table-bordered">
                    <thead>
                        <tr>
                            <th scope="col">Nr</th>
                            <th scope="col">Tidpunkt</th>
                            <th scope="col">Metod</th>
                            <?=(!isset($license) ? "<th scope='col'>Licens</th>" : "")?>
                            <th scope="col">Status</th>
                            <th scope="col">Klartid</th>
                        </tr>
                    </thead>
                    <tbody>
                        <?php
                            if(isset($requestArray))
                            {
                                $i = 0;
                                foreach($requestArray as $sqlRow) {
                                    echo "<tr>";
                                    echo "<td>". ($i + 1) ."</td><td>".$sqlRow['creationdate']."</td><td>".$sqlRow['method']."</td>";
                                    echo (!isset($license) ? "<td>".$sqlRow['license']."</td>" : "");
                                    echo "<td>".$sqlRow['lastcollectstatus']."</td>";
                                    echo "<td>".($sqlRow['lastcollect'] !==  "0000-00-00 00:00:00" ? $sqlRow["lastcollect"] : "")."</td>";
                                    echo "</tr>";
                                    $i++;
                                }
                            }
                        ?>
                    </tbody>
                    <caption><?php echo (isset($requestArray) ? count($requestArray) : 0); ?> förfrågningar</caption>
                </table>
            </div>
        </div>
    </body>
</html>