<div class="modal fade" id="server-settings-modal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title"><i class="fas fa-cogs"></i> Server settings</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">

        <div class="form-group">
            <label>Name</label>
            <input type="text" id="server-name-input" class="form-control" placeholder="Name" value="<?=$o3c_server["display_name"]?>"/>
        </div>

        <div class="form-group">
            <label>Client port</label>
            <input type="text" id="server-client-port-input" class="form-control" placeholder="8080" value="<?=$o3c_server["port"]?>"/>
        </div>

        <div class="form-group">
            <label>Admin port</label>
            <input type="text" id="server-admin-port-input" class="form-control" placeholder="3128" value="<?=$o3c_server["admin_port"]?>"/>
        </div>

        <div class="form-group">
            <label>External host</label>
            <input type="text" id="server-external-host-input" class="form-control" placeholder="External host" value="<?=$o3c_server["host_external"]?>"/>
        </div>

        <div class="form-group">
            <label>Failover server</label>
            <select class="form-select" id="server-failover-select">
                <option value="null">No failover</option>
                <?php

                if(!isset($o3c_servers))
                    $o3c_servers = $serverManager->getServers();

                foreach($o3c_servers as $server)
                {
                    $selected = $o3c_server["failover_o3c_id"] == $server["id"] ? " selected" : "";
                    if($o3c_server["id"] != $server["id"])
                      echo "<option value=\"{$server["id"]}\"{$selected}>{$server["name"]}</option>";
                }
                ?>
            </select>
        </div>

      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        <button type="button" class="btn btn-primary save-changes"><span class="ico"><i class="fas fa-save"></i></span> <span class="text">Save</span></button>
      </div>
    </div>
  </div>
</div>