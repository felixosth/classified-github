<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);
// Initialize the session
require_once "functions/checklogin.php";

// If session variable is not set it will redirect to login page
?>
 
<!DOCTYPE html>
<html lang="en">
<head>
    <meta property="og:image" content="img/logo.png" />
    <meta property="og:description" content="InSupport Tryggportal." />
    <meta property="og:url" content="https://portal.tryggconnect.se" />
    <meta property="og:title" content="Tryggportalen" />

    <title>ISTP</title>
    <?php require_once "functions/getbt.php"; ?>
    <link rel="stylesheet" type="text/css" href="customStyles.css">

    <style type="text/css">
        body{ font: 14px sans-serif; text-align: center; }
    </style>
</head>
<body>
    <?php
    $activePage = "none";
    require_once "functions/navbar.php";?>

    <div class="container-fluid">
        <div class="page-header">
            <h1>Hej, <b><?php echo $_SESSION['username']; ?></b>. Välkommen till Tryggportalen.</h1>
        </div>
        <h4>Här kommer det finnas info och sånt sen. Använd menyn högst upp för att navigera.</h4>
    </div>
</body>
</html>