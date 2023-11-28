<?php

session_start();

if(isset($_SESSION["CREATED"]))
{
    session_unset();
    session_destroy();
    header("Location: /login/");
}

?>