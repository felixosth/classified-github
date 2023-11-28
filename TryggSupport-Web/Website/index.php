<?php
require_once __DIR__ . "/../Login/Login.php";

$files = dirToArray(__DIR__."/files");

function buildList($files, $path = "", $first = true)
{
    foreach($files as $file)
    {
        if(is_array($file))
        {
            $clean = strtolower(str_replace(".", "", str_replace(" ", "", $file["name"])));
            echo "<li" . ($first ? " class=\"active\"" : " class=\"dropdown\"") .  ">";
            echo '<a href="#' . $clean . 'Submenu" data-toggle="collapse" aria-expanded="false" class="dropdown-toggle">' . $file["name"] . '</a>';
            echo '<ul class="collapse list-unstyled" id="' . $clean . 'Submenu">';

            buildList($file["files"], $path . "/" . $file["name"], false);
            echo '</ul>
            </li>';
        }
        else
        {
            if($file === "Felanmälan.php" || $file === "Kontakt.pdf")
                continue;

            $filetime = filemtime("Files/".$path."/".$file);
            $filename = basename(basename(basename(basename(basename(basename(basename($file, '.txt'), ".pdf"), ".jpg"), ".png"), ".webm"), ".html"), ".php");
            echo '<li>
            <a file="' . $path ."/". $file . '" time="'.$filetime.'" href="#'.$path."/".$file.'">'.$filename.'</a>
            </li>
            ';
        }
    }
}

function dirToArray($dir)
{  
    $result = array();

    $cdir = scandir($dir);
    foreach ($cdir as $key => $value)
    {
        if (!in_array($value,array(".","..")))
        {
            if (is_dir($dir . DIRECTORY_SEPARATOR . $value))
            {
                array_push($result, array("name" => $value, "files" => dirToArray($dir . DIRECTORY_SEPARATOR . $value)));
            }
            else
            {
                $result[] = $value;
            }
        }
    }

    return $result;
}

?>

<!DOCTYPE html>
<html>
    <head>
        <!--
            Code by InSupport Nätverksvideo AB for Bergendahls Food AB (2020)
        -->
        <meta charset="utf-8">
        <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">

        <link rel="stylesheet" href="./bootstrap-4.5.2/css/bootstrap.min.css">
        <link rel="stylesheet" href="./style.css">

        <title>Kamerasystem</title>
    </head>
    <body>
        <div class="wrapper">

            <!-- Sidebar  -->
            <nav id="sidebar">
                <div class="sidebar-header">
                    <h3>Kamerasystem</h3>
                </div>
    
                <!-- File tree -->
                <ul class="components">
                    <li><a href="#home">Kontakt</a></li>
                    <li><a href="#/Felanmälan.php" file="/Felanmälan.php">Felanmälan</a></li>
                    <?php
                        /* ----- Build file tree ------ */
                        buildList($files);
                    ?>
                </ul>

                <!-- Contact -->
                <p class="fixed-bottom bottom">support@insupport.se</p>
            </nav>

            <!-- Content -->
            <div id="content-for-file">
                <!-- <div id="pre-content">
                    <h2>Webbsida för kamerasystem</h2>
                    <p>Utvecklas av InSupport Nätverksvideo AB</p>
                    <p>Använd sidomenyn till vänster för att navigera webbsidan.</p>
                </div> -->
                <!-- <iframe style="display: none" id="file-frame" src=""></iframe> -->
                <iframe src="/files/Kontakt.pdf" id="file-frame" src=""></iframe>
            </div>
        </div>
    
        <!-- Scripts -->
        <script src="./bootstrap-4.5.2/js/jquery-3.5.1.min.js"></script>
        <script src="./bootstrap-4.5.2/js/bootstrap.min.js"></script>
    
        <script type="text/javascript">

            var firstClick = true;

            $(document).ready(function () {
                var loc = unescape(encodeURIComponent(window.location.hash)).substr(1);
                
                if(loc === "home")
                    toggleHome(true);
                else if(loc !== "")
                    setFileSrc(loc);

                $('#sidebarCollapse').on('click', function () {
                    $('#sidebar').toggleClass('active');
                });

                $("a[file]").click(function()
                {
                    var file = $(this).attr("file");
                    var time = $(this).attr("time");
                    if(time === undefined)
                        time = new Date().getTime();
                    setFileSrc(file + "?" + time);
                });

                $('a[href="#home"]').click(function()
                {
                    toggleHome(true);
                    firstClick = true;
                });
            });

            function setFileSrc(file)
            {
                
                $("#file-frame").attr("src", "./files" + file + "#toolbar=0");
            }

            function toggleHome()
            {
                setFileSrc("/files/Kontakt.pdf");
            }
        </script>
    </body>
</html>