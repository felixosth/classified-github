<?php
//ini_set('display_errors',1); 
//error_reporting(E_ALL);

require_once '../functions/config.php';
session_start();
if(isset($_SESSION['username']) || !empty($_SESSION['username'])){
    header("location: " . $_SERVER["REQUEST_SCHEME"]  . "://" . $_SERVER['HTTP_HOST']);
}
// Define variables and initialize with empty values
$username = $password = "";
$username_err = $password_err = "";

if(isset($_GET["dest"]))
{
    setcookie("dest", $_GET["dest"], 0);
}

// Processing form data when form is submitted
if($_SERVER["REQUEST_METHOD"] == "POST"){
 
    // Check if username is empty
    if(empty(trim($_POST["username"]))){
        $username_err = 'Please enter username.';
    } else{
        $username = trim($_POST["username"]);
    }
    
    // Check if password is empty
    if(empty(trim($_POST['password']))){
        $password_err = 'Please enter your password.';
    } else{
        $password = trim($_POST['password']);
    }
    
    // Validate credentials
    if(empty($username_err) && empty($password_err)){
        // Prepare a select statement
        $sql = "SELECT username, password, role FROM users WHERE username = ?";
        
        if($stmt = mysqli_prepare($link, $sql)){
            // Bind variables to the prepared statement as parameters
            mysqli_stmt_bind_param($stmt, "s", $param_username);
            
            // Set parameters
            $param_username = $username;
            
            // Attempt to execute the prepared statement
            if(mysqli_stmt_execute($stmt)){
                // Store result
                mysqli_stmt_store_result($stmt);
                
                // Check if username exists, if yes then verify password
                if(mysqli_stmt_num_rows($stmt) == 1){                    
                    // Bind result variables
                    mysqli_stmt_bind_result($stmt, $username, $hashed_password, $role);
                    if(mysqli_stmt_fetch($stmt)){
                        if(password_verify($password, $hashed_password)){
                            /* Password is correct, so start a new session and
                            save the username to the session */
                            //session_start();
                            $_SESSION['username'] = $username;
                            $_SESSION['role'] = $role;

                            $loc = "location: " . (isset($_COOKIE["dest"]) ? "/" . $_COOKIE["dest"] : $_SERVER["REQUEST_SCHEME"]."://".$_SERVER['HTTP_HOST']);
                            header($loc);
                        } else{
                            // Display an error message if password is not valid
                            $password_err = 'The password you entered was not valid.';
                        }
                    }
                } else{
                    // Display an error message if username doesn't exist
                    $username_err = 'No account found with that username.';
                }
            } else{
                echo "Oops! Something went wrong. Please try again later.";
            }
        }
        
        // Close statement
        mysqli_stmt_close($stmt);
    }
    
    // Close connection
    mysqli_close($link);
}
?>
 
<!DOCTYPE html>
<html lang="en">
<head>
    <title>ISTP - Login</title>
    <?php include "../functions/getbt.php"; ?>
    <link rel="stylesheet" type="text/css" href="../customStyles.css">
    <style type="text/css">
        body{ font: 14px sans-serif; }
        .wrapper{ width: 350px; padding: 20px; }
        @media only screen and (min-device-width: 1300px) {
            .container-fluid
            {
                width:500px;
            }
            .invalid-feedback {
            display: block;
            }
        }
    </style>
</head>
<body>
<br>
    <div class="container-fluid mx-auto">
        <center><img src="../img/logo.png" class="img-fluid border rounded d-block"/></center>
        <br><br>
        <!--<p>Var god fyll i inloggningsuppgifter för att logga in.</p>-->
        <form action="<?php echo htmlspecialchars($_SERVER["PHP_SELF"]); ?>" method="post" class="px-3 py-3 border rounded">
            <h2 class="text-center">Tryggportalen</h2>
            <div class="form-group <?php echo (!empty($username_err)) ? 'invalid-feedback' : ''; ?>">
                <label>Användarnamn:<sup>*</sup></label>
                <input type="text" name="username"class="form-control" value="<?php echo $username; ?>">
                <span class="help-block"><?php echo $username_err; ?></span>
            </div>    
            <div class="form-group <?php echo (!empty($password_err)) ? 'invalid-feedback' : ''; ?>">
                <label>Lösenord:<sup>*</sup></label>
                <input type="password" name="password" class="form-control">
                <span class="help-block"><?php echo $password_err; ?></span>
            </div>
            <div class="form-group text-right">
                <input type="submit" class="btn btn-primary" value="Logga In">
            </div>
        </form>
    </div>
</body>
</html>