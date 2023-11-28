<?php
// error_reporting(E_ALL);
// ini_set("display_errors", 1);
// ini_set("log_errors", 1);
// ini_set("error_log", __DIR__ . "/php-error.log");

// echo "<pre>";
// var_dump($_SERVER);
// echo "</pre>";
// die;

define("MAX_IMG_COUNT", 35);

$imgFolder = dirname($_SERVER["SCRIPT_FILENAME"]) . "/images";

if($_SERVER["REQUEST_METHOD"] === "POST")
{
    if(isset($_REQUEST["myuploadfile"]))
    {
        if(isset($_SERVER["HTTP_CONTENT_DISPOSITION"]))
        {
            $attachment = getFilename($_SERVER["HTTP_CONTENT_DISPOSITION"]);

            $split = explode("-", $attachment);
            $mac = $split[0];
            $filename = $split[1];

            $thisDir = $imgFolder . "/" . $mac;

            if (!file_exists($thisDir)) {
                mkdir($thisDir, 0777, true);
            }

            $existingImages = getDirContents($thisDir);

            // Cleanup
            while(count($existingImages) >= MAX_IMG_COUNT)
            {
                unlink(array_shift($existingImages));
            }

            file_put_contents($thisDir . "/" . $filename, file_get_contents('php://input'));
            $data = file_get_contents('php://input');
        }
    }
    exit;
}
else if($_SERVER["REQUEST_METHOD"] === "GET" && isset($_GET["action"]))
{
    switch($_GET["action"])
    {
        case "list":
            if(isset($_GET["folder"]))
            {
                $dir = dirname($_SERVER["SCRIPT_FILENAME"]) . "/images/" . $_GET["folder"];
                $files = getDirContents($dir);
                $images = array();
                foreach($files as $file)
                {
                    if(is_dir($file))
                        continue;

                    $image = new stdClass();
                    $image->isFolder = is_dir($file);
                    $image->filename =  str_replace($_SERVER["DOCUMENT_ROOT"], "", $file);
                    $image->originalFilename = $file;
                    $image->docroot = $_SERVER["DOCUMENT_ROOT"];
                    $image->createdate = filemtime($file);
                    array_push($images, $image);
                }
            
                sort($images);
                echo json_encode($images);
            }
            exit;
        break;
        case "listfolders":

            $files = array();
            $scannedFiles = array_diff(scandir(dirname($_SERVER["SCRIPT_FILENAME"]) . "/images"), array(".", ".."));
            foreach($scannedFiles as $file)
            {
                array_push($files, $file);
            }
            sort($files);
            echo json_encode($files);
            exit;
        break;
    }

}

function getDirContents($dir, &$results = array()) {
    $files = scandir($dir);

    foreach ($files as $key => $value) {
        $path = realpath($dir . DIRECTORY_SEPARATOR . $value);
        if (!is_dir($path)) {
            $results[] = $path;
        } else if ($value != "." && $value != "..") {
            getDirContents($path, $results);
            $results[] = $path;
        }
    }

    return $results;
}


function getFilename($header) {
    if (preg_match('/.*?filename="(.+?)"/', $header, $matches)) {
        return $matches[1];
    }
    if (preg_match('/.*?filename=([^; ]+)/', $header, $matches)) {
        return rawurldecode($matches[1]);
    }
    throw new Exception(__FUNCTION__ .": Filename not found");
}


?>