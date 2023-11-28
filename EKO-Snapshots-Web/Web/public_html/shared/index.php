
<?php
// error_reporting(E_ALL);
// ini_set("display_errors", 1);
?>

<!DOCTYPE html>
<html lang="sv">
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-BmbxuPwQa2lc/FVzBcNJ7UAyJxM6wuqIj61tLrc4wSX0szH/Ev+nYRRuWlolflfl" crossorigin="anonymous">
        <style>
            div.container {
                margin-top: 1rem;
            }
            .zoom {
                display:inline-block;
                position: relative;
            }
            .zoom img::selection { background-color: transparent; }
            .zoom img { display: block; }

            footer {
                padding-bottom: .5rem;
            }

            footer img {
                padding-bottom: .5rem;
            }
        </style>
    </head>
    <body class="d-flex flex-column min-vh-100">
        <div class="container">
            <div class="row">
                <div class="col-md-auto">
                    <ul class="list-group" id="site-list">
                        <a href="#" class="list-group-item list-group-item-action disabled">Sajter</a>
                        <a href="/fikaplats/" class="list-group-item list-group-item-action">Fikaplats</a>
                        <a href="/tartor/" class="list-group-item list-group-item-action">Tårtor</a>
                    </ul>
                </div>
                <div class="col-md-auto">
                    <ul class="list-group" id="stores-list"></ul>
                </div>
                <div class="col-md-auto">
                    <ul class="list-group" id="images-list"></ul>
                    <!-- <button style="margin-top: .5rem;" class="btn btn-outline-secondary" id="update-list-btn">Uppdatera lista</button> -->
                </div>
                <div class="col">
                    <span class="zoom" id="zoom-span">
                        <img id="image" class="img-fluid" src=""/>
                    </span>
                </div>
            </div>
        </div>

        <footer class="mt-auto text-muted text-center text-small">
            <img src="/shared/logo_black.png"/>
            <p class="mb-1">Webapplikation av InSupport Nätverksvideo AB</p>
        </footer>

        <script src="https://code.jquery.com/jquery-3.6.0.min.js" integrity="sha256-/xUj+3OJU5yExlq6GSYGSHk7tPXikynS7ogEvDej/m4=" crossorigin="anonymous"></script>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta2/dist/js/bootstrap.bundle.min.js" integrity="sha384-b5kHyXgcpbZJO/tY9Ul7kGkf1S0CWuKcCD38l8YkeH8z8QjE0GmW1gYU5S9FOnJ0" crossorigin="anonymous"></script>
        <script src="/shared/js/jquery.zoom.js"></script>
        <script src="/shared/js/script.js?ver=20210326"></script>
    </body>
</html>