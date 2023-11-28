<?php
// define('DB_SERVER', 'localhost');
define('DB_SERVER', 'localhost');

if($_SERVER["SERVER_NAME"] === "tryggportal.local")
{
    define('DB_USERNAME', 'tryggdrift');
    define('DB_PASSWORD', 'T9DG0Iz7nuqpCbPu');
}
else
{
    define('DB_USERNAME', 'root');
    define('DB_PASSWORD', 'In$upport2020!');
}
define('DB_NAME', 'portal');
// define("LOC", "http://localhost:8011/annat/tryggportal");
define("LOC", "https://portal.tryggconnect.se");
#define("LOC", "http://192.168.168.62");
 
/* Attempt to connect to MySQL database */
$link = mysqli_connect(DB_SERVER, DB_USERNAME, DB_PASSWORD, DB_NAME);
 
// Check connection
if($link === false){
    die("ERROR: Could not connect. " . mysqli_connect_error());
}
?>