<div class="modal fade" id="areYouSure" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
    <div class="modal-dialog" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="exampleModalLabel">Är du säker?</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <form name="areYouSureForm">
                    <div class="form-group text-left">
                        <button onclick="deleteRow()" type="button" class="btn btn-danger" value="Ja">
                    </div>
                    <div class="form-group text-right">
                        <button type="submit" class="btn btn-primary" data-dismiss="modal" value="Nej">
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>