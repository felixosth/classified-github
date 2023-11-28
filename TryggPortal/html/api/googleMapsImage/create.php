<?php

$key = "&key=AIzaSyCCW7jaO279X57dtR6RaqCNFDnfqyntNuM";
$url = htmlspecialchars("https://maps.googleapis.com/maps/api/staticmap?");

if(isset($_GET["location"]))
{
    $loc = urlencode($_GET["location"]);
    $zoom = isset($_GET["zoom"]) ? $_GET["zoom"] : 10;
    $size = isset($_GET["size"]) ? $_GET["size"] : "400x300";
    $scale = isset($_GET["scale"]) ? $_GET["scale"] : "1";
    $remoteImage = htmlspecialchars_decode($url . "&zoom=" . $zoom . "&center=" . $loc . "&markers=" . $loc . "&size=" . $size . "&scale=" . $scale . $key);

    header("Content-type: image/png");
    readfile($remoteImage);
}


?>