<?php 

class Folders {

    protected $conn;
    
    function __construct($conn)
    {
        $this->conn = $conn;
    }



    function getFolderTree($folderId = 0, $folders = null)
    {
        $tree = array();

        if(!isset($folders))
        {
            $folders = $this->getFolders();
        }

        foreach($folders as $folder)
        {
            if($folder["parent_folder"] == $folderId)
            {
                $folder["children"] = $this->getFolderTree($folder["id"], $folders);
                array_push($tree, $folder);
            }
        }

        return $tree;

    }

    function getFolders()
    {
        $folders = array();

        if($stmt = $this->conn->prepare("SELECT * FROM folders;"))
        {
            $stmt->execute();
            $result = $stmt->get_result();
            while($row = $result->fetch_assoc())
            {
                array_push($folders, $row);
            }
            $stmt->free_result();
        }
        return $folders;
    }

    function getDevicesFromFolder(int $folder)
    {
        $devices = array();

        if($stmt = $this->conn->prepare(
            "SELECT s.id static_id, d.id dynamic_id, s.*, d.*, o.name o3c_name 
                FROM devices_static s 
            INNER JOIN devices_dynamic d 
                ON d.client_id=s.client_id 
            INNER JOIN o3c_servers o 
                ON d.o3c_server=o.id 
            WHERE folder=? 
            ORDER BY o.name, s.display_name, s.client_id;
            "
            ))
        {
            $stmt->bind_param("i", $folder);
            $stmt->execute();
            $result = $stmt->get_result();
            while($row = $result->fetch_assoc())
            {
                array_push($devices, $row);
            }
            $stmt->free_result();
        }
        return $devices;
    }

}
?>