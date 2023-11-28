<?php

// ini_set('display_errors', 1);
// ini_set('display_startup_errors', 1);
// error_reporting(E_ALL);

// Initialize the session
session_start();
 
// Unset all of the session variables
$_SESSION = array();
session_unset();
// Destroy the session.
session_destroy();

require_once __DIR__."/../functions/config.php";
 
// Redirect to login page
header("location: " . $_SERVER["REQUEST_SCHEME"]  . "://" . $_SERVER['HTTP_HOST']);
exit;
?>