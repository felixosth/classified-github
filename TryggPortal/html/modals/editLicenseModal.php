    <div class="modal fade" id="editLicense" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">Ändra licens</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <form class="needs-validation" name="modifyForm" action="../api/modifyLicense.php" method="post" novalidate>
                        <div class="form-group">
                            <label>Licens:</label>
                            <input type="text" id="showLicenseField" name="licenseguid" class="form-control" readonly>
                        </div>
                        <div class="form-group">
                            <label>Kund:<sup>*</sup></label>
                            <input type="text" id="editCustomerField" name="customer" class="form-control" required>
                            <div class="invalid-feedback">
                                Kundnamn saknas.
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Site:</label>
                            <input type="text" id="editSiteField" name="site" class="form-control" required>
                            <div class="invalid-feedback">
                                Sitenamn saknas.
                            </div>
                        </div>
                        <div class="form-group" style="float:left">
                            <label>Utgångsdatum:<sup>*</sup></label>
                            <input type="text" id="editExpirationField" name="expirationdate" class="form-control">
                        </div>
                        <div class="form-group" style="float:right">
                            <label>Användargräns:<sup>*</sup></label>
                            <input type="text" name="maxClients" id="maxClientsField" class="form-control">
                        </div>
                        <div class="modal-footer">
                            <div class="form-group text-left">
                                <button type="button" onclick="deleteRow()" class="btn btn-danger">Ta bort</button>
                            </div>
                            <div class="form-group text-right">
                                <!--<input type="submit" class="btn btn-primary" value="Ändra">-->
                                <input type="button" onclick="confirmSelection('Vill du verkligen modifiera denna licens?', 'modifyForm')" class="btn btn-primary" value="Ändra">
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <script>
        function confirmSelection(text, form)
        {
            if(window.confirm(text))
            {
                form = document.forms[form];
                form.submit();
            }
        }
        function deleteRow()
        {
            if (window.confirm("Vill du verkligen ta bort licensen?")) {
                form = document.forms["modifyForm"];
                form.action = "../api/deleteLicense.php";
                form.submit();
            }
            else
            {
                $('#editLicense').modal('hide');
            }
        }
    </script>

    <?php include "areYouSureModal.php"; ?>
    