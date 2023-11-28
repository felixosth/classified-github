<?php
session_start();
require_once  __DIR__ . "/config.php";

if(!isset($_SESSION['username']) || empty($_SESSION['username'])){
  header("location: /login/");
  exit;
}


define("LOGINTIMEOUT", 3600); // def 3600

if (isset($_SESSION['LAST_ACTIVITY']) && (time() - $_SESSION['LAST_ACTIVITY'] > LOGINTIMEOUT)) {  //def 3600= 1 hour
    $loc = "location: /login/" . (isset($thisPage) ? "?dest=$thisPage" : "");
    //var_dump($loc);

    //die;
    // last request was more than 20 minutes ago
    session_unset();     // unset $_SESSION variable for the run-time 
    session_destroy();   // destroy session data in storage
    header($loc);
    exit;
}
$_SESSION['LAST_ACTIVITY'] = time(); // update last activity time stamp

if (!isset($_SESSION['CREATED'])) {
    $_SESSION['CREATED'] = time();
} else if (time() - $_SESSION['CREATED'] > 1200) {
    // session started more than 20 minutes ago
    session_regenerate_id(true);    // change session ID for the current session and invalidate old session ID
    $_SESSION['CREATED'] = time();  // update creation time
}

$isAdmin = false;
if(isset($_SESSION['role']) || !empty($_SESSION['role'])){
  if($_SESSION["role"] === "admin")
    $isAdmin = true;
}
?>