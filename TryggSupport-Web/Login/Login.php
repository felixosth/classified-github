<?php

$timeout = 60 * 60;

if (session_status() == PHP_SESSION_NONE) {
    session_start();
}

if($_SERVER["SERVER_NAME"] !== "172.18.0.90" && !isset($_SESSION["CREATED"]))
{
    header("Location: /login/?request=" . $_SERVER["REQUEST_URI"]);
    exit();
}


if(isset($_SESSION["CREATED"]))
{
    if (isset($_SESSION['LAST_ACTIVITY']) && (time() - $_SESSION['LAST_ACTIVITY'] > $timeout)) {
        session_unset();     // unset $_SESSION variable for the run-time 
        session_destroy();   // destroy session data in storage
        header("Location: /login/?request=" . $_SERVER["REQUEST_URI"]);
        exit;
    }
    else if (time() - $_SESSION['CREATED'] > $timeout) {
        // session started more than 30 minutes ago
        session_regenerate_id(true);    // change session ID for the current session and invalidate old session ID
        $_SESSION['CREATED'] = time();  // update creation time
    }

    $_SESSION['LAST_ACTIVITY'] = time(); // update last activity time stamp
}


?>