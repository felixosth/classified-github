<?php

function sendSms($to, $msg)
{
    if(!isset($to) || count($to) == 0)
    {
        return;
    }

    $data = array(
        "From" => "TryggStatus",
        "To" => $to,
        "Text" => $msg
    );

    $user = "900257";
    $pass = "DWIEBGDA";

    $url = "https://api.genericmobile.se/SmsGateway/api/v1/Message/";

    $curl = curl_init();
    curl_setopt($curl, CURLOPT_POST, 1);
    curl_setopt($curl, CURLOPT_POSTFIELDS, json_encode($data));

    // Optional Authentication:
    curl_setopt($curl, CURLOPT_HTTPAUTH, CURLAUTH_BASIC);
    curl_setopt($curl, CURLOPT_USERPWD, "$user:$pass");

    $headers = array(
        'Content-Type: application/json'
    );
    curl_setopt($curl, CURLOPT_HTTPHEADER, $headers);

    curl_setopt($curl, CURLOPT_URL, $url);
    curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);

    $result = curl_exec($curl);

    curl_close($curl);
}

?>