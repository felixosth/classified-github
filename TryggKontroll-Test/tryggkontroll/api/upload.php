<?php

require_once "../dbconfig.php";

$dir = "../case/vid/";

//var_dump($_FILES);

$id = $_POST["id"];
$mguid = $_POST["mguid"];

$lic = checkLicense($conn, $mguid);

if($lic != false)
{
    $dir = $dir . "/" . $lic;
    if(!file_exists($dir))
        mkdir($dir);
}
else
    die;

//echo "id: " . $id;

if (is_uploaded_file($_FILES["file"]["tmp_name"])) 
{
    //$uploadfile = $dir . basename($_FILES["file"]["name"]);
    $uploadfile = $dir . "/" . $id . ".mp4";
    //echo "File ". $_FILES["file"]["name"] . " uploaded successfully. ";
    if (move_uploaded_file($_FILES["file"]["tmp_name"], $uploadfile)) 
    {
        echo "File uploaded.";
    }
    else
        print_r($_FILES);
}
else 
{
    echo "Upload Failed with error " . $_FILES["file"]["error"];
    print_r($_FILES);
}

?>