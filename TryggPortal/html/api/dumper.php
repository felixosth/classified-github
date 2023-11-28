<?php

$username = "testing";
$password = "testpassword";

ini_set("allow_url_fopen", true);

// https://gist.github.com/magnetikonline/650e30e485c0f91f2f40
class DumpHTTPRequestToFile {
	public function execute($targetFile) {
		$data = sprintf(
			"%s %s %s\n\nHTTP headers:\n",
			$_SERVER['REQUEST_METHOD'],
			$_SERVER['REQUEST_URI'],
			$_SERVER['SERVER_PROTOCOL']
		);
		foreach ($this->getHeaderList() as $name => $value) {
			$data .= $name . ': ' . $value . "\n";
		}
		$data .= "\nRequest body:\n";
		file_put_contents(
			$targetFile,
            $data . file_get_contents('php://input') . "\n\n\n\n",
            FILE_APPEND
		);
		//echo("Done!\n\n");
	}
	private function getHeaderList() {
		$headerList = [];
		foreach ($_SERVER as $name => $value) {
			if (preg_match('/^HTTP_/',$name)) {
				// convert HTTP_HEADER_NAME to Header-Name
				$name = strtr(substr($name,5),'_',' ');
				$name = ucwords(strtolower($name));
				$name = strtr($name,' ','-');
				// add to list
				$headerList[$name] = $value;
			}
		}
		return $headerList;
	}
}
(new DumpHTTPRequestToFile)->execute('./dumprequest.txt');


$method = $_SERVER['REQUEST_METHOD'];
if(isset($_GET))
{
    switch($method)
    {
        case "GET":
            switch($_GET["method"])
            {
                case "camera.ping":
                    echo '{"contact":true,"input":false,"license":false,"credentials":false,"camera-exist":false}';
                case "version.get":
                    echo '{"request":"OK","version":"2.5","time":' . strtotime("now") . '}';

            }
        case "POST":
        switch($_GET["method"])
        {
            case "camera.ping":
                if($_GET["encrypted"] === "true")
                    echo '{"contact":true,"input":true,"license":true,"credentials":true,"camera-exist":true}';
            case "decrypt":
            echo _readFromInputAndDecrypt();
        }
    }
}


function _readFromInputAndDecrypt()
{
    $inputStream = fopen('php://input', 'r');
    $encrypted = '';
    while (!feof($inputStream)) {
        $encrypted .= fgets($inputStream);
    }
    fclose($inputStream);

    $encrypted = base64_decode(str_replace(" ", "+", $encrypted));
    return $encrypted;
}