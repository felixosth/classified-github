<?php
$manageLic = $register = "";

if($activePage === "manageLic")
    $manageLic = "active";
else if($activePage === "register")
    $register = "active";
else if($activePage === "sms")
    $sms = "active";
else if($activePage === "bankid")
    $bankid = "active";
else if($activePage === "updater")
    $updater = "active";

?>

<nav class="navbar navbar-expand-lg navbar-light bg-light navborder">
    <a class="navbar-brand" href="/">Tryggportalen</a>
    <button class="navbar-toggler" type="button" data-toggle="collapse" data-target="#navbarNavAltMarkup" aria-controls="navbarNavAltMarkup" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="navbarNavAltMarkup">
        <div class="navbar-nav">
        <a class="nav-item nav-link <?php echo $manageLic; ?>" href="/license">Licenshantering</a>
        <a class="nav-item nav-link <?php echo $sms; ?>" href="/sms">SMS</a>
        <a class="nav-item nav-link <?php echo $bankid; ?>" href="/bankid">BankID</a>
        <a class="nav-item nav-link <?php echo $updater; ?>" href="/updater">Uppdaterare</a>
        <a class="nav-item nav-link <?php echo $register; echo $isAdmin ? "" : "disabled"; ?>" <?php echo $isAdmin ? "href='/register'" : ""; ?>>Registrera ny användare</a>
        </div>
        <div class="navbar-nav ml-auto">
            <a class="nav-item nav-link">Inloggad som <strong><?php echo $_SESSION['username']; ?></strong></a>
            <a class="btn btn-outline-danger" href="/logout/">Logga ut</a>
        </div>
    </div>
</nav>
