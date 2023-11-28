<?php

if(!isset($o3c_servers))
  $o3c_servers = $serverManager->getServers();

?>

<div class="modal fade" id="change-server-modal" tabindex="-1" aria-hidden="true">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title"><i class="fas fa-people-carry"></i> Change server</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <div class="form-group">
            <label>Destination O3C server</label>
            <select class="form-select" id="server-select">
                <?php
                foreach($o3c_servers as $server)
                {
                    if($o3c_server["id"] != $server["id"])
                      echo "<option value=\"{$server["id"]}\">{$server["name"]}</option>";
                }
                ?>
            </select>
        </div>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
        <button type="button" class="btn btn-primary save-changes"><span class="ico"><i class="fas fa-save"></i></span> Save changes</button>
      </div>
    </div>
  </div>
</div>