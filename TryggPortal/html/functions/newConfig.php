<?php

require_once __DIR__ . "/config.php";

$conn = new mysqli(DB_SERVER, DB_USERNAME, DB_PASSWORD, DB_NAME);
$conn->set_charset("utf8");
if($conn->connect_error)
{
    die("DB connection failed: " . $conn->connect_error);
}


?>