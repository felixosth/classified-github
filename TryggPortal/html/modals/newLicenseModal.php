    <div class="modal <?php echo ($showModal) ? "" : "fade";?>" id="newLicense" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="exampleModalLabel">Ny licens</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <form class="needs-validation" action="../api/addlicense.php" method="post" novalidate>
                        <div class="form-group">
                            <label>Produkt:<sup></sup></label>
                            <select class="form-control" name="product">
                                <?php
                                    foreach($productsArray as $sqlRow) {
                                        echo "<option value='" . $sqlRow["Name"] . "'>" . $sqlRow["DisplayName"] . "</option>";
                                    }
                                ?>
                            </select>
                        </div>    
                        <div class="form-group">
                            <label>Kund:<sup>*</sup></label>
                            <input type="text" name="customer" class="form-control" placeholder="Kundnamn" required>
                            <div class="invalid-feedback">
                                Kundnamn saknas.
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Site:</label>
                            <input type="text" name="site" class="form-control" placeholder="Sitenamn" required>
                            <div class="invalid-feedback">
                                Sitenamn saknas.
                            </div>
                        </div>
                        <div class="form-group" style="float:left">
                            <label>Utgångsdatum:<sup>*</sup></label>
                            <input type="text" name="expirationdate" class="form-control" value="<?php echo date("Y-m-d"); ?>">
                        </div>
                        <div class="form-group" style="float:right">
                            <label>Användargräns:<sup>*</sup></label>
                            <input type="text" name="maxClients" class="form-control" value="1">
                        </div>
                        <div class="input-group mb-3">
                            <div class="input-group-prepend">
                                <div class="input-group-text">
                                    <p style="vertical-align: middle; margin: 0; margin-right: .5rem">SMS</p>
                                    <input type="checkbox" name="allowSms">
                                </div>
                            </div>
                            <input disabled type="number" class="form-control" value="250" name="smsLimit">
                            <label style="vertical-align: middle; margin: 0; display: block; line-height: 36px">SMS/Månad</label>
                        </div>
                        <div class="input-group mb-3">
                            <div class="input-group-prepend">
                                <div class="input-group-text">
                                    <p style="vertical-align: middle; margin: 0; margin-right: .5rem">BankID</p>
                                    <input type="checkbox" name="allowBankId">
                                </div>
                            </div>
                            <input disabled type="number" class="form-control" value="250" name="bankIdLimit">
                            <label style="vertical-align: middle; margin: 0; display: block; line-height: 36px">Req/Månad</label>
                        </div>
                        <div class="modal-footer form-group text-right">
                            <input type="submit" class="btn btn-primary" value="Skapa">
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>