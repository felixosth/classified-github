<div class="modal fade" id="sendSms" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Skicka SMS</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <form class="needs-validation" name="sendSmsForm" action="../api/sms.php" method="post" novalidate>
                        <div class="form-group">
                            <label>Från:</label>
                            <input type="text" name="sender" maxlength="11" class="form-control" required>
                            <div class="invalid-feedback">Var god skriv en avsändare.</div>
                        </div>
                        <div class="form-group">
                            <label>Till:</label>
                            <input type="text" name="reciever" class="form-control" required>
                            <div class="invalid-feedback">Var god skriv en mottagare.</div>
                        </div>
                        <div class="form-group">
                            <label>Meddelande:</label>
                            <!--<input type="text" name="message" class="form-control" required>-->
                            <textarea class="form-control" name="message" rows="3" required></textarea>
                            <div class="invalid-feedback">Var god skriv ett meddelande.</div>
                        </div>
                        <div class="modal-footer">
                            <div class="form-group text-right">
                                <input type="submit" class="btn btn-primary" value="Skicka">
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function() {
        'use strict';
        window.addEventListener('load', function() {
            // Fetch all the forms we want to apply custom Bootstrap validation styles to
            var forms = document.getElementsByClassName('needs-validation');
            // Loop over them and prevent submission
            var validation = Array.prototype.filter.call(forms, function(form) {
            form.addEventListener('submit', function(event) {
                if (form.checkValidity() === false) {
                event.preventDefault();
                event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
            });
        }, false);
        })();
    </script>