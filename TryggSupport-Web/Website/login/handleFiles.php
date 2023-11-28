<?php
require_once __DIR__ . "/../../Login/Login.php";

if(!isset($_GET["file"]) || $_GET["file"] == "")
{
    header("HTTP/1.0 404 Not Found");
    exit;
}

$filePath = __DIR__ . "/../files/" . $_GET["file"];
if(!file_exists($filePath))
{
    $filePath = __DIR__ . "/../" . $_GET["file"];
    if(!file_exists($filePath))
    {
        header("HTTP/1.0 404 Not Found");
        exit;
    }
}

if(substr($filePath, -3) === "php")
{
    include $filePath;
    exit;
}

header('Content-Type: ' . mime_content_type($filePath));
// header('Content-Type: application/octet-stream');
header("Content-Length: " . filesize($filePath));
// header("Content-disposition: attachment; filename=\"" . $_GET["file"] . "\""); 
readfile($filePath);
exit;

?>