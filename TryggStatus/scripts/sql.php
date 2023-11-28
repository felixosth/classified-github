<?php



function getSql($domain)
{
    $liveDbSrv = $domain == "localhost" ? "localhost" : "mysql680.loopia.se";

    $sqlConn = new mysqli($liveDbSrv, "statusdb@t248006", "rf5u5RnyPQ26Y9R", "tryggconnect_se_db_1");
    if($sqlConn->connect_error)
    {
        die("DB connection failed: " . $this->SQL->connect_error);
        return null;
    }
    
    $sqlConn->set_charset("utf8");
    return $sqlConn;
}


?>