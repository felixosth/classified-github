<?php
header('Access-Control-Allow-Origin: *'); 
// setcookie('samesite-test', '1', 0, '/; samesite=strict');

?>

<!DOCTYPE html>
<html>
    <head>
        <style>

            iframe {
                width: 100%;
                height: 100vh;
                padding:0;
                display:block;
                position: relative;
            }

            .img-container
            {
                width:  100%;
                height: 100%;
                white-space: nowrap;
                text-align: center;
            }

            .helper {
                display: inline-block;
                height: 100%;
                vertical-align: middle;
            }

            #videoImage{
                max-height: 100%;
                max-width: 100%;
                width: auto;
                height: auto;
                position: absolute;
                top: 0;
                bottom: 0;
                left: 0;
                right: 0;
                margin: auto;
                z-index: 98;
                border: 2px solid black;
            }

            html, body
            {
                width: 100%;
                height: 100%;
                overflow: hidden;
            }
            #center
            {
                opacity: 0.75;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
                position: absolute;
                width: 100%;
                height: 100%;
                /* border: 1px solid red;
                border-radius: 50%; */
            }

            div.particle{
                /* background-color: red; */
                border: 2px solid red;
                border-radius: 50%;
                width: 20px;
                height: 20px;
                z-index: 101;
                transform: translate(-50%, -50%);
            }

            #circle
            {
                position: absolute;
                top: 50%;
                left: 50%;
                width: 100px;
                transform: translate(-50%, -50%);
                height: 100px;
                border: 1px solid #FF0010;
                border-radius: 50%;
                z-index: 99;
            }

            #bullet.transition
            {
                transition-duration: .33s;
            }

            #bullet
            {
                z-index: 100;
                top: 50%;
                left: 50%;
                position: relative;
                transform: translate(-50%, -50%);
                border-radius: 50%;
                width: 16px;
                height: 16px;
                background-color: #FF0010;
                cursor: move;
            }

            #footer
            {
                position: absolute;
                bottom: 0;
                left: 0;
                right: 0;
                height: 50px;
                vertical-align: middle;
            }

            #footer > input
            {
                width: 50%;
                transform: translate(50%,0);
            }

        </style>
    </head>
    <body>
        <div id="center">
            <img id="videoImage"/>
            <div id="circle"></div>
            <div id="bullet"></div>
            <div id="footer">
                <input type="range" min="1" max="9999" value="1" class="slider" id="zoomSlider">
            </div>
        </div>

        <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
        <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
        <script>

        // http://195.60.68.14:11050/axis-cgi/com/ptz.cgi?camera=1&continuouspantiltmove=-2,-65 x y
        //Request URL: http://195.60.68.14:11024/axis-cgi/com/ptz.cgi?camera=1&center=118,112&imagewidth=1920&imageheight=1080
        // ptz.cgi?camera=1&zoom=1-9999

            $(document).ready(function()
            {
                httpGetAsync("http://localhost:8124/getptz", function(r) // Hämta zoom för att sätta slidern till rätt existerande värde
                {
                    var json = JSON.parse(r);
                    $("#zoomSlider").val(json.zoom);
                });

                $("#zoomSlider").on("input", function() // zooma
                {
                    // todo: Bromsa antalet calls till bridge så som det är gjort i setContinous(), detta skickar varje gång detta event avfyras
                    httpGetAsync("http://localhost:8124/ptz?zoom=" + this.value);
                });

                $("#videoImage").on("click", function(event) { // Centrera på musklick
                    var x = event.pageX - this.offsetLeft;
                    var y = event.pageY - this.offsetTop;
                    createParticle(event);
                    httpGetAsync("http://localhost:8124/ptz?center=" + x + "," + y + "&imagewidth=1920&imageheight=1080");
                });

                refreshImage();

                $("#bullet").draggable({
                    start: function()
                    {
                        $(this).removeClass("transition");
                    },
                    drag: function(event, ui){

                        var $center = $("#center");
                        var cLeft = $center.offset().left + $center.outerWidth()/2;
                        var cTop = $center.offset().top + $center.outerHeight()/2;

                        var $bullet = $("#bullet");
                        var bLeft = $bullet.offset().left + $bullet.outerWidth()/2;
                        var bTop = $bullet.offset().top + $bullet.outerHeight()/2;

                        var wWidth = $(window).width();
                        var wHeight = $(window).height();

                        var leftVal = Math.min(100, Math.max(-100, Math.round(((bLeft - wWidth/2) / wWidth)*2*100)));
                        var topVal = Math.min(100, Math.max(-100, Math.round(((bTop - wHeight/2) / wHeight)*2*100*-1)));

                        setContinous(leftVal, topVal);
                    },
                    stop: function() {
                        setContinous(0,0);
                        centerBullet();
                    },
                    cursorAt: { top: 8, left: 8}
                });
            });

            function refreshImage()
            {
                $("#videoImage").attr("src", "http://localhost:8124/image?ts=" + new Date().getTime()).one("load", () => 
                {
                    setTimeout(refreshImage, 100);
                });
            }

            var lastCall = new Date().getTime();
            function setContinous(x, y){
                var now  = new Date().getTime();
                if(now - lastCall > 100 || (x === 0 && y === 0))
                {
                    var cap = 20;
                    if(x > cap)
                        x = cap;
                    else if(x < -cap)
                        x = -cap;

                    if(y > cap)
                        y = cap;
                    else if(y < -cap)
                        y = -cap;

                    lastCall = now;
                    httpGetAsync("http://localhost:8124/ptz?continuouspantiltmove=" + x + "," + y);
                }
            }

            function centerBullet()
            {
                $("#bullet").addClass("transition").css({top: "50%", left: "50%"});
            }

            function httpGetAsync(theUrl, callback)
            {
                var xmlHttp = new XMLHttpRequest();
                xmlHttp.onreadystatechange = function() { 
                    if (xmlHttp.readyState == 4 && xmlHttp.status == 200)
                        if (callback != null)
                            callback(xmlHttp.responseText);
                }
                xmlHttp.open("GET", theUrl, true); // true for asynchronous 
                xmlHttp.send(null);
            }

            function createParticle(event)
            {
                var newthing = document.createElement("div");                
                newthing.classList = "particle";
                document.getElementById("center").appendChild(newthing); // Your existing code


                // get the coordinates of the mouse
                var x = event.clientX;     // get the horizontal coordinate
                var y = event.clientY;   // get the vertical coordinate

                // position newthing using the coordinates
                newthing.style.position = "fixed"; // fixes el relative to page. Could use absolute.
                newthing.style.left = x + "px";
                newthing.style.top = y + "px";

                $(newthing).animate({width:'0px', height: '0px', opacity: '0'}, 800, "swing", function(){
                    $(this).remove();
                });
            }
        </script>
    </body>
</html>