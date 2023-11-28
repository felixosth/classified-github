<?php 

class ServerManager {

    protected $conn;
    
    function __construct($conn)
    {
        $this->conn = $conn;
    }


    public function getServer(int $id)
    {
        if($stmt = $this->conn->prepare("SELECT *, concat(ifnull(host_external, host), ':', port) client_host, concat(host, ':', admin_port) admin_host FROM o3c_servers WHERE id=? LIMIT 1;"))
        {
            $stmt->bind_param("i", $id);
            if($stmt->execute())
            {
                $server = $stmt->get_result()->fetch_assoc();
                $stmt->free_result();
                return $server;
            }
        }
        return null;
    }

    public function getServerByName(string $name)
    {
        if($stmt = $this->conn->prepare("SELECT *, concat(ifnull(host_external, host), ':', port) client_host, concat(host, ':', admin_port) admin_host FROM o3c_servers WHERE name=? LIMIT 1;"))
        {
            $stmt->bind_param("s", $name);
            if($stmt->execute())
            {
                $server = $stmt->get_result()->fetch_assoc();
                $stmt->free_result();
                return $server;
            }
        }
        return null;
    }


    public function updateServerSettings($server)
    {
        if($stmt = $this->conn->prepare("UPDATE o3c_servers SET port=?, admin_port=?, failover_o3c_id=?, display_name=?, host_external=? WHERE id=?;"))
        {
            $stmt->bind_param("iiissi", $server["port"], $server["admin_port"], $server["failover_o3c_id"], $server["display_name"], $server["host_external"], $server["id"]);
            return $stmt->execute();
        }
    }

    public function getServerWithServerlist(int $id)
    {
        if($stmt = $this->conn->prepare("
            with recursive cte as (
                select id, name, display_name, host_external, failover_o3c_id, host, port, admin_port, 1 lvl from o3c_servers
                union all
                select c.id, c.name, c.display_name, t.host_external, t.failover_o3c_id, t.host, t.port, c.admin_port, lvl + 1
                from cte c
                inner join o3c_servers t on t.id = c.failover_o3c_id
            )
            select id, name, display_name, host_external, failover_o3c_id, group_concat(concat(ifnull(host_external, host), ':', port) order by lvl) serverlist, concat(ifnull(host_external, host), ':', port) client_host, concat(host, ':', admin_port) admin_host, port, admin_port
            from cte
            where id=?
            group by id;"))
        {
            $stmt->bind_param("i", $id);
            if($stmt->execute())
            {
                $server = $stmt->get_result()->fetch_assoc();
                $stmt->free_result();
                return $server;
            }
        }
    }

    public function getServerWithServerlistByName(string $name)
    {
        if($stmt = $this->conn->prepare(
            "
            with recursive cte as (
                select id, name, display_name, host_external, failover_o3c_id, host, port, admin_port, 1 lvl from o3c_servers
                union all
                select c.id, c.name, c.display_name, c.host_external, t.failover_o3c_id, t.host, t.port, c.admin_port, lvl + 1
                from cte c
                inner join o3c_servers t on t.id = c.failover_o3c_id
            )
            select id, name, display_name, host_external, failover_o3c_id, group_concat(concat(host, ':', port) order by lvl) serverlist, concat(host, ':', port) client_host, concat(host, ':', admin_port) admin_host, port, admin_port
            from cte
            where name=?
            group by id;"))
        {
            $stmt->bind_param("s", $name);
            if($stmt->execute())
            {
                $server = $stmt->get_result()->fetch_assoc();
                $stmt->free_result();
                return $server;
            }
        }
    }

    
    public function getRelatedServers(string $name)
    {
        $o3c_servers = array();
        if($stmt = $this->conn->prepare("
            with recursive cte as (
                select id, name, failover_o3c_id, host_external, host, port, 1 lvl from o3c_servers
                union all
                select c.id, t.name, t.failover_o3c_id, t.host_external, t.host, t.port, lvl + 1
                from cte c
                inner join o3c_servers t on t.id = c.failover_o3c_id
            )
            select id, group_concat(name order by lvl) as failoverlist, group_concat(concat(ifnull(host_external, host), ':', port) order by lvl) as serverlist
            from cte
            group by id
            having find_in_set(?, `failoverlist`);"))
        {
            $stmt->bind_param("s", $name);
            $stmt->execute();
            $result = $stmt->get_result();
            while($row = $result->fetch_assoc())
            {
                array_push($o3c_servers, $row);
            }
            $stmt->free_result();
        }
        return $o3c_servers;
    }

    public function getServers()
    {
        $o3c_servers = array();

        if($stmt = $this->conn->prepare("SELECT * FROM o3c_servers;"))
        {
            $stmt->execute();
            $result = $stmt->get_result();
            while($row = $result->fetch_assoc())
            {
                array_push($o3c_servers, $row);
            }
            $stmt->free_result();
        }
        return $o3c_servers;
    }

    public function getServerDefault()
    {
        if($stmt = $this->conn->prepare(
            "
            with recursive cte as (
                select id, name, failover_o3c_id, host, port, admin_port, 1 lvl from o3c_servers
                union all
                select c.id, c.name, t.failover_o3c_id, t.host, t.port, c.admin_port, lvl + 1
                from cte c
                inner join o3c_servers t on t.id = c.failover_o3c_id
            )
            select id, name,  group_concat(concat(ifnull(host_external, host), ':', port) order by lvl) serverlist, concat(ifnull(host_external, host), ':', port) client_host, concat(host, ':', admin_port) admin_host
            from cte
            where id=(SELECT value FROM global_settings WHERE setting='o3c_entry_point')
            group by id;"))
        {
            if($stmt->execute())
            {
                $server = $stmt->get_result()->fetch_assoc();
                $stmt->free_result();
                return $server;
            }
        }
        return null;
    }

    public function getDevice(int $id)
    {
        if($stmt = $this->conn->prepare("SELECT * FROM devices_dynamic WHERE id=?;"))
        {
            $stmt->bind_param("i", $id);
            if($stmt->execute())
            {
                $server = $stmt->get_result()->fetch_assoc();
                $stmt->free_result();
                return $server;
            }
        }
        return null;
    }

    public function getDevices(int $o3c_server_id)
    {
        $o3c_devices = array();

        if($stmt = $this->conn->prepare(
            "SELECT d.id, d.*, s.display_name, o.name as o3c_server_dst_name 
            FROM devices_dynamic d 
            INNER JOIN devices_static s 
                ON d.client_id = s.client_id
            INNER JOIN o3c_servers o
                ON o.id = s.o3c_server_dst
            WHERE o3c_server=? 
                ORDER BY client_id;"
            ))
        {
            $stmt->bind_param("i", $o3c_server_id);
            $stmt->execute();
            $result = $stmt->get_result();
            while($row = $result->fetch_assoc())
            {
                array_push($o3c_devices, $row);
            }
            $stmt->free_result();
        }
        return $o3c_devices;
    }

    // Dont call with user input
    public function getDevicesFromArray($o3c_server_array)
    {
        $o3c_devices = array();

        $idsStr = "(" . implode(",", $o3c_server_array) . ")";

        if($stmt = $this->conn->prepare(
            "SELECT d.id, d.*, s.display_name, s.o3c_server_dst, CONCAT(o.host, ':', o.admin_port) o3c_admin_host 
                FROM devices_dynamic d 
            INNER JOIN devices_static s 
                ON d.client_id = s.client_id 
            INNER JOIN o3c_servers o
                ON d.o3c_server = o.id
            WHERE s.o3c_server_dst IN {$idsStr} 
                ORDER BY client_id;"))
        {
            $stmt->execute();
            $result = $stmt->get_result();
            while($row = $result->fetch_assoc())
            {
                array_push($o3c_devices, $row);
            }
            $stmt->free_result();
        }
        return $o3c_devices;
    }

    public function getStaticDevice(string $client_id)
    {
        if($stmt = $this->conn->prepare("SELECT * FROM devices_static WHERE client_id=?;"))
        {
            $stmt->bind_param("s", $client_id);
            if($stmt->execute())
            {
                $server = $stmt->get_result()->fetch_assoc();
                $stmt->free_result();
                return $server;
            }
        }
        return null;
    }

    public function reconnectDevice(int $deviceId, $server)
    {
        $device = $this->getDevice($deviceId);
        $o3c_addr = $server["host"] .":". $server["admin_port"];
        return $this->webRequest($o3c_addr . "/admin/reconnect.cgi?client=" . $device["client_id"]);
    }

    public function restartDevice(int $deviceId, $server)
    {
        $device = $this->getDevice($deviceId);
        $o3c_addr = $server["host"] .":". $server["admin_port"];
        return $this->webRequest($o3c_addr . "/admin/restart.cgi?client=" . $device["client_id"]);
    }

    public function registerDevice(string $mac, string $oak, int $folder, $name, $server)
    {
        $mac = strtolower($mac);
        $defaultServer = $this->getServerDefault();

        $o3c_addr = $defaultServer["admin_host"];

        $url = $o3c_addr . "/admin/dispatch.cgi?action=register&user=".ADP_USERNAME."&pass=".ADP_PASSWORD."&mac=".$mac."&oak=".$oak."&server=".$defaultServer["serverlist"];

        $output = $this->webRequest($url);

        if (strpos($output, 'error') === false) {
            if($stmt = $this->conn->prepare("INSERT IGNORE INTO devices_static (client_id, o3c_server_dst, folder, display_name) VALUES (?, ?, ?, ?);"))
            {
                $stmt->bind_param("siis", $mac, $server["id"], $folder, $name);
                $stmt->execute();
                $stmt->free_result();
            }
        }


        return $output;

    }

    public function redirectDevice(int $deviceId, $toServer)
    {
        $device = $this->getDevice($deviceId);
        if(isset($device))
        {
            $existingServer = $this->getServer($device["o3c_server"]);
            $deviceAddr = "http://" . $device["client_id"];
            $o3c_addr = $existingServer["admin_host"];

            return $this->webRequest($o3c_addr . "/admin/redirect.cgi?client=" . $device["client_id"] . "&server=" . $toServer["client_host"]);
        }

    }

    public function moveDevice(int $deviceId, $newServer)
    {
        $device = $this->getDevice($deviceId);
        $static_device = $this->getStaticDevice($device["client_id"]);

        if(isset($static_device))
        {
            $existingServer = $this->getServer($device["o3c_server"]);

            $deviceAddr = "http://" . $device["client_id"];
            $o3c_addr = $existingServer["host"] .":". $existingServer["admin_port"];
            $newServerListEntry = $newServer["client_host"];
    
            if($stmt = $this->conn->prepare("UPDATE devices_static SET o3c_server_dst=? WHERE id=?;"))
            {
                $stmt->bind_param("ii", $newServer["id"], $static_device["id"]);
                if($stmt->execute())
                {
                    // Set new serverlist
                    $this->webRequest($device["client_id"] . "/axis-cgi/param.cgi?action=update&root.RemoteService.ServerList=" . $newServer["serverlist"], $existingServer["admin_host"]);
            
                    // redirect
                    $this->webRequest($o3c_addr . "/admin/redirect.cgi?client=" . $device["client_id"] . "&server=" . $newServerListEntry);
                }
            }
        }
        else
        {
            if($stmt = $GLOBALS["conn"]->prepare("INSERT IGNORE INTO devices_static (client_id, o3c_server_dst) VALUES (?, ?);"))
            {
                $stmt->bind_param("si", $device["client_id"], $newServer["id"]);
                if($stmt->execute())
                {
                    $this->moveDevice($device["id"], $newServer);
                }
                else
                {
                    // echo "Something went wrong! Try again!";
                }
            }
        }
    }

    public function webRequest($url, $proxy = null)
    {
        // create curl resource
        $ch = curl_init();

        // set url
        curl_setopt($ch, CURLOPT_URL, $url);

        // Proxy
        curl_setopt($ch, CURLOPT_PROXY, $proxy);

        //return the transfer as a string
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);

        // $output contains the output string
        $output = curl_exec($ch);

        // close curl resource to free up system resources
        curl_close($ch);
        return $output;
    }
}

?>