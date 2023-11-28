<div class="modal fade" id="filterBankid" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Filtrera BankID</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <form class="needs-validation" action="filter.php" method="get" novalidate>
                        <div class="form-group">
                            <label>Licensnyckel:</label>
                            <input type="text" id="licBox" name="license" class="form-control">
                        </div>
                        <!--<div class="input-group">
                            <input type="checkbox" name="lastMonth">
                            <label>Visa endast föregående månad</label>
                        </div>-->
                        <div class="form-group">
                            <select class="form-control" name="month" style="width:49%;float:left;">
                                <option value="0">Alla månader</option>
                                <option value="1">Januari</option>
                                <option value="2">Februari</option>
                                <option value="3">Mars</option>
                                <option value="4">April</option>
                                <option value="5">Maj</option>
                                <option value="6">Juni</option>
                                <option value="7">Juli</option>
                                <option value="8">Augusti</option>
                                <option value="9">September</option>
                                <option value="10">Oktober</option>
                                <option value="11">November</option>
                                <option value="12">December</option>
                            </select>
                            <input type="text" value="<?php echo date('Y'); ?>" name="year" class="form-control" style="width:49%;float:right;">
                        </div>
                        <div class="modal-footer">
                            <div class="form-group text-right">
                                <input type="submit" class="btn btn-primary" value="Filtrera">
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>