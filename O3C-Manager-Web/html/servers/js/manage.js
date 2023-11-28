var selectedCameras = [];
            var $chkboxes = null;
            var lastChecked = null;

            $(document).ready(function() {

                // ---- Change server
                $(".device-checkbox").change(function() {
                    $(".device-btn").prop("disabled", $(".device-checkbox:checked").length == 0);

                    selectedCameras = [];

                    $(".device-checkbox:checked").each(function() {
                        selectedCameras.push(parseInt($(this).parents().eq(1).attr("camid")));
                    });
                });
                $("td.checkbox-col").click(function(d) {

                    if(d.target == this) {
                        $checkbox = $(this).find('input[type="checkbox"]');
                        if($checkbox.prop("disabled") == false)
                        {
                            $checkbox.prop("checked", !$checkbox.prop("checked"));
                            chkboxClick(d, $checkbox[0]);
                            $checkbox.trigger("change");
                        }
                    }
                });

                $("#change-server-button").click(function() {

                    $("#change-server-modal .modal-title").html($(this).html());

                    $("#change-server-modal .save-changes").unbind('click').click(function() {
                        // $("#change-server-modal .save-changes .ico").html('<i class="fas fa-spinner fa-pulse"></i>');

                        var me = this;
                        $(me).prop("disabled", true);
                        $("#change-server-modal").modal("hide");

                        selectedCameras.forEach(function(i) {
                            $('tr[camid="'+i+'"] .fa-stack').html('<i class="fas fa-spinner fa-pulse fa-stack-1x"></i>');
                        });

                        apiCallStream( {
                            action: "changeServer",
                            cameras: selectedCameras,
                            server: parseInt($("#server-select").val())
                        }, function(data) {

                            if(data == "" | data == null)
                                return;
                            // var response = JSON.parse(data);

                            getJsonObjects(data).forEach(function(response){
                                $('tr[camid="'+response.device+'"] .fa-stack')
                                .html('<i class="fas fa-circle fa-stack-2x text-primary" aria-hidden="true"></i><i class="fas fa-people-carry fa-stack-1x fa-inverse" aria-hidden="true"></i>')
                                .attr("title", "Moved")
                                .parents().eq(1).find('input[type="checkbox"]').prop("checked", false).prop("disabled", true);
                            });

                            
                            
                            // if(data != "")
                                // alert(data);

                            // $(me).prop("disabled", false);
                            // $(".device-checkbox:checked").each(function() { $(this).parents().eq(1).remove();});
                            // $("#change-server-modal .save-changes .ico").html('<i class="fas fa-save"></i>');
                        });
                    });

                    $("#change-server-modal").modal("show");
                });

                
                // ---- /Change server

                // ---- Redirect
                $("#redirect-device-button").click(function(){

                    $("#change-server-modal .modal-title").html($(this).html());

                    $("#change-server-modal .save-changes").unbind('click').click(function() {
                        // $("#change-server-modal .save-changes .ico").html('<i class="fas fa-spinner fa-pulse"></i>');

                        var me = this;
                        var btnTxt = $("#change-server-modal .save-changes .text").text();
                        $(me).prop("disabled", true);
                        $("#change-server-modal").modal("hide");


                        selectedCameras.forEach(function(i) {
                            $('tr[camid="'+i+'"] .fa-stack').html('<i class="fas fa-spinner fa-pulse fa-stack-1x"></i>');
                        });


                        apiCallStream( {
                            action: "redirectDevice",
                            cameras: selectedCameras,
                            server: parseInt($("#server-select").val())
                        }, function(data) {
                            
                            if(data == "" | data == null)
                                return;
                            getJsonObjects(data).forEach(function(response){
                                $('tr[camid="'+response.device+'"] .fa-stack')
                                .html('<i class="fas fa-circle fa-stack-2x text-secondary" aria-hidden="true"></i><i class="fas fa-directions fa-stack-1x fa-inverse" aria-hidden="true"></i>')
                                .attr("title", "Redirected")
                                .parents().eq(1).find('input[type="checkbox"]').prop("checked", false).prop("disabled", true);
                            });

                        });
                    });

                    $("#change-server-modal").modal("show");
                });
                // ---- /Redirect

                // ---- Server settings
                $("#server-settings-modal .save-changes").click(function() {
                    $("#server-settings-modal .save-changes .ico").html('<i class="fas fa-spinner fa-pulse"></i>');
                    var $me = $(this);
                    $me.prop("disabled", true);

                    var client_port = parseInt($("#server-client-port-input").val());
                    var admin_port = parseInt($("#server-admin-port-input").val());

                    if(isNaN(client_port))
                    {
                        alert("Client port is not a number");
                        return;
                    }
                    if(isNaN(admin_port))
                    {
                        alert("Client port is not a number");
                        return;
                    }

                    if(confirm("Are you sure you want to modify these settings? If the serverlist is modified, this change will be pushed out to ALL devices with this server as primary."))
                    {
                        apiCallStream( {
                                action: "saveServerSettings",
                                display_name: $("#server-name-input").val(),
                                host_external: $("#server-external-host-input").val(),
                                failover_o3c_id: $("#server-failover-select").val() == "null" ? null : parseInt($("#server-failover-select").val()),
                                client_port,
                                admin_port,
                                server: parseInt($("#o3c_server").val())
                            }, function(data) {

                                if(data == null || data == "")
                                    return;

                                try
                                {
                                    var jsonObjs = getJsonObjects(data);
                                    var response = jsonObjs[jsonObjs.length -1];
                                    
                                    $("#server-settings-modal .save-changes .text").text("Updating devices (" + response.progress + "/" + response.max_progress + ")");

                                    if(response.server !== undefined) {

                                        var server = response.server;

                                        $("#server-name").text(server.display_name != null ? server.display_name + " (" + server.name + ")" : server.name);
                                        $("#serverlist").text(server.serverlist);

                                        $("#server-settings-modal").modal("hide");

                                        $me.prop("disabled", false);
                                        $("#server-settings-modal .save-changes .ico").html('<i class="fas fa-save"></i>');
                                        $("#server-settings-modal .save-changes .text").text("Save");
                                    }
                                }
                                catch(err)
                                {
                                    console.error("Error parsing data: " + data);
                                }
                            });
                    }
                });
                // ---- /Server settings


                // ---- Checkboxes

                $chkboxes = $('.device-checkbox:not(:disabled)');
                lastChecked = null;

                $chkboxes.click(chkboxClick);

                // ---- /Checkboxes

                // ---- Reconnect device
                $("#reconnect-device-button").click(function() {

                    if(confirm("Do you want to reconnect the selected cameras?")) {

                        selectedCameras.forEach(function(i) {
                            $('tr[camid="'+i+'"] .fa-stack').html('<i class="fas fa-spinner fa-pulse fa-stack-1x"></i>');
                        });

                        apiCallStream( {
                            action: "reconnectDevice",
                            cameras: selectedCameras,
                            server: parseInt($("#o3c_server").val())
                        }, function(data) {

                            if(data == "" | data == null)
                                return;
                            
                            try{

                                getJsonObjects(data).forEach(function(response) {
                                    $('tr[camid="'+response.device+'"] .fa-stack')
                                    .html('<i class="fas fa-circle fa-stack-2x text-warning" aria-hidden="true"></i><i class="fas fa-unlink fa-stack-1x" aria-hidden="true"></i>')
                                    .attr("title", "Reconnected")
                                    .parents().eq(1).find('input[type="checkbox"]').prop("checked", false).prop("disabled", true);
                                });
                                
                               
                            }
                            catch(err){
                                console.error("Error parsing data: " + data);
                            }
                        });
                    }
                });

                // ---- /Reconnect device

                // ---- Restart device
                $("#restart-device-button").click(function() {

                    if(confirm("Do you want to restart the selected cameras?")) {
                        
                        selectedCameras.forEach(function(i) {
                            $('tr[camid="'+i+'"] .fa-stack').html('<i class="fas fa-spinner fa-pulse fa-stack-1x"></i>');
                        });

                        apiCallStream({
                            action: "restartDevice",
                            cameras: selectedCameras,
                            server: parseInt($("#o3c_server").val())
                        }, function(data) {
                            if(data == "" | data == null)
                                return;

                                getJsonObjects(data).forEach(function(response) {
                                    $('tr[camid="'+response.device+'"] .fa-stack')
                                    .html('<i class="fas fa-circle fa-stack-2x text-danger" aria-hidden="true"></i><i class="fas fa-power-off fa-stack-1x fa-inverse" aria-hidden="true"></i>')
                                    .attr("title", "Restarted")
                                    .parents().eq(1).find('input[type="checkbox"]').prop("checked", false).prop("disabled", true);
                                });
                            // var response = JSON.parse(data);
                            
                        });
                    }
                });
                // ---- /Restart device



                // ---- Register device
                $("#register-device-modal .save-changes").click(function() {

                    var me = this;
                    $(me).prop("disabled", true);
                    apiCall( {
                        action: "registerDevice",
                        mac: $("#device-mac-input").val(),
                        oak: $("#device-oak-input").val(),
                        server: parseInt($("#o3c_server").val())
                    }, function(data) {
                        alert(data);
                        $(me).prop("disabled", false);
                        $("#register-device-modal").modal('hide');
                    });
                });

                $("#register-device-button").click(function() {
                    $("#register-device-modal").modal('show');
                });
                // ---- /Register device


                // ---- Debug frame
                $("#open-debug-button").click(function() {
                    
                    $("#debug-log-iframe").attr("src", "/api/api.php?action=streamDebug&server=" + $("#o3c_server").val());
                    $("#debug-log-modal").modal('show');

                });

                $("#debug-log-modal").on("hidden.bs.modal", function() {
                    $("#debug-log-iframe").attr("src", "");
                });

                $("#open-debug-dialog-button").click(function() {
                    window.open($("#debug-log-iframe").attr("src"), "_blank");
                    $("#debug-log-modal").modal('hide');
                });
                // ---- /Debug frame

            });

            function chkboxClick(e, context = null) {


                if(context == null)
                    context = this;
                
                if (!lastChecked) {
                    lastChecked = this;
                    return;
                }

                if (e.shiftKey) {
                    // e.preventDefault();

                    var start = $chkboxes.index(context);
                    var end = $chkboxes.index(lastChecked);

                    $chkboxes.slice(Math.min(start,end), Math.max(start,end)+ 1).prop('checked', lastChecked.checked);
                    window.getSelection().removeAllRanges();
                }

                lastChecked = context;
            }

            function getJsonObjects(text)
            {
                var arr = [];
                var split = text.split("}{");

                for (let i = 0; i < split.length; i++) {
                    const element = split[i];
                    
                    arr.push( JSON.parse( (i > 0 ? "{" : "") + split[i] + (i < split.length -1 ? "}" : "") ) );
                }
                return arr;
            }