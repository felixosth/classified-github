function apiCall(data, callback = null)
{
    var xmlhttp = new XMLHttpRequest();

    if(callback != null)
    {
        xmlhttp.onreadystatechange = function() {
            if (this.readyState == 4 && this.status == 200) {
                callback(this.responseText);
            }
        };
    }

    xmlhttp.open("POST", "/api/api.php", true);
    xmlhttp.send(JSON.stringify(data));
}

function apiCallStream(data, callback) {
    var xmlhttp = new XMLHttpRequest();
    var lastLength = -1;

    if(callback != null)
    {
        xmlhttp.onreadystatechange = function() {
            
            if(this.responseText.length > lastLength) {
                callback(this.responseText.substring(lastLength));
                lastLength = this.responseText.length;
            }
        };
    }

    xmlhttp.open("POST", "/api/api.php", true);
    xmlhttp.send(JSON.stringify(data));
}

function customApiCall(url, data, callback = null)
{
    var xmlhttp = new XMLHttpRequest();

    if(callback != null)
    {
        xmlhttp.onreadystatechange = function() {
            if (this.readyState == 4 && this.status == 200) {
                callback(this.responseText);
            }
        };
    }

    xmlhttp.open("POST", url, true);
    xmlhttp.send(data);
}

function apiCallGet(url, callback)
{
    var xmlhttp = new XMLHttpRequest();

    if(callback != null)
    {
        xmlhttp.onreadystatechange = function() {
            if (this.readyState == 4 && this.status == 200) {
                callback(this.responseText);
            }
        };
    }

    xmlhttp.open("GET", url);
    xmlhttp.send();
}