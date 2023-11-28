<?php


if (session_status() == PHP_SESSION_NONE) {
    session_start();
}

$user = "citygross";
$pwd = "5252";


if(isset($_GET["request"]))
{
    if($_GET["request"] === "/")
        $_SESSION["request"] = "/index.php";
    else
        $_SESSION["request"] = $_GET["request"];
}

if(isset($_SESSION["CREATED"]))
{
    header("Location: " . $_SESSION["request"]);
}

if($_SERVER['REQUEST_METHOD'] === "POST")
{
    if(isset($_POST["username"]) && isset($_POST["password"]))
    {
        if($_POST["username"] === $user && $_POST["password"] === $pwd)
        {
            $_SESSION["CREATED"] = time();
            $_SESSION['LAST_ACTIVITY'] = time();
            header("Location: " . (isset($_SESSION["request"]) ? $_SESSION["request"] : "/index.php"));
        }
    }
}

?>

<!DOCTYPE html>
<html>
    <head>
        <meta charset="utf-8">
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-giJF6kkoqNQ00vy+HMDP7azOuL0xtbfIcaT9wjKHr8RbDVddVHyTfAAsrekwKmP1" crossorigin="anonymous">
    </head>
    <body>
        <script src="https://code.jquery.com/jquery-3.5.1.min.js" integrity="sha256-9/aliU8dGd2tb6OSsuzixeV4y/faTqgFtohetphbbj0=" crossorigin="anonymous"></script>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.0.0-beta1/dist/js/bootstrap.bundle.min.js" integrity="sha384-ygbV9kiqUc6oa4msXn9868pTtWMgiQaeYH7/t7LECLbyPA2x65Kgf80OJFdroafW" crossorigin="anonymous"></script>
        <div class="container">
            <h2>Login</h2>
            <form action="<?=htmlspecialchars($_SERVER["PHP_SELF"]);?>" method="post"> 
                <input type="hidden" name="request" value="<?=isset($_GET["request"]) ? $_GET["request"] : "";?>"/>
                <div class="mb-3">
                    <label for="username">Username</label>
                    <input class="form-control" type="text" name="username"/>
                </div>
                <div class="mb-3">
                    <label for="username">Password</label>
                    <input  class="form-control" type="password" name="password"/>
                </div>
                <button type="submit" class="btn btn-primary">Submit</button>
            </form>
        </div>
    </body>
</html>