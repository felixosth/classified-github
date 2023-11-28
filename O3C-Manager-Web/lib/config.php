<?php

ini_set('display_errors', 1);
ini_set('display_startup_errors', 1);
error_reporting(E_ALL);
mysqli_report(MYSQLI_REPORT_STRICT);

define('DB_SERVER', 'localhost');
define('DB_NAME', 'o3c_manager');
define('DB_USERNAME', 'o3cman');
define('DB_PASSWORD', 'Camera21!');

define('ADP_USERNAME', 'adp_insupport_102');
define('ADP_PASSWORD', 'WQouKibHgkSMVOdYLHch');

$conn = new mysqli(DB_SERVER, DB_USERNAME, DB_PASSWORD, DB_NAME);
$conn->set_charset("utf8");
if($conn->connect_error)
{
    die("DB connection failed: " . $conn->connect_error);
}


function getNavbar($activeLink)
{
    $GLOBALS["activeLink"] = $activeLink;
    include __DIR__ . "/navbar.php";
}

function getHead($title)
{
    $GLOBALS["title"] = $title;
    include __DIR__ . "/head.php";
}

require_once __DIR__ . "/ServerManager.php";

$serverManager = new ServerManager($conn);

require_once __DIR__ . "/Folders.php";
$folderManager = new Folders($conn);

?>