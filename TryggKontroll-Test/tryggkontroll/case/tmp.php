<html>
    <head>
        <title>Video</title>
    </head>
    <body>
        <video src="http://192.168.2.64:8011/tryggkontroll/case/vid/video.mkv" type="video/mp4" autoplay controls onerror="failed(event)" ></video>
        <script>
            function failed(event)
            {
                console.log(event);
            }
        </script>
    </body>
</html>