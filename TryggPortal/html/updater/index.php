<?php
ini_set('display_errors',1); 
error_reporting(E_ALL);

$thisPage = "updater";
require_once "../functions/checklogin.php";
require_once "../functions/newConfig.php";

$query = "SELECT * FROM updater;";
$products = array();

if($stmt = $conn->prepare($query))
{
    $stmt->execute();
    $result = $stmt->get_result();
    while($row = $result->fetch_assoc())
    {
        array_push($products, $row);
    }
    $stmt->free_result();
    $stmt->close();
}
else
    echo "Unable to fetch products";

$conn->close();
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>ISTP - Uppdaterare</title>
    <?php include "../functions/getbt.php"; ?>
    <link rel="stylesheet" type="text/css" href="../customStyles.css">
    <style type="text/css">
        body{ font: 14px sans-serif; }
        /*@media only screen and (min-device-width: 1600px) {
            .container-fluid
            {
                width:1600px;
            }
        }*/
    </style>
</head>
<body>
    
    <?php
    $activePage = "updater";
    include "../functions/navbar.php";?>
    <div class="container-fluid mx-auto">
        <div class="page-header">
            <h1>Uppdaterare</h1>
            <button type="button" id="newProductButton" class="btn btn-primary">Ny produkt</button>
        </div>
        <br>
        <div class="table-responsive-xl">
            <table class="table table-striped table-bordered">
                <thead>
                    <tr>
                    <th scope="col">Produkt</th>
                    <th scope="col">Filnamn</th>
                    <th scope="col">Version</th>
                    <th scope="col">Branch</th>
                    <th scope="col">Dlk</th>
                    <th scope="col"></th>
                    </tr>
                </thead>
                <tbody>
                    <?php
                        $i = 0;
                        foreach($products as $sqlRow) {
                            echo "<tr productId=\"".$sqlRow["productId"]."\">";
                            echo "<td>".$sqlRow['productName']."</td>";
                            echo "<td>".$sqlRow['version']."</td>";
                            echo "<td>".$sqlRow['fileName']."</td>";
                            echo "<td>".$sqlRow['branchGuid']."</td>";
                            echo "<td>".$sqlRow['downloadKey']."</td>";
                            echo "<td><button class=\"btn btn-secondary editProductButton\">Ändra</button></td>";
                            echo "</tr>";
                        $i++;
                    }
                    ?>
                </tbody>
                <caption><?php echo count($products); ?> produkter</caption>
            </table>
        </div>
    </div>

    <div class="modal fade" id="newProduct" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Ny produkt</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <form class="needs-validation" name="createProductForm" action="/api/updater/updater.php" method="post" novalidate>
                        <input type="hidden" id="formAction" value="createProduct" name="action"/>
                        <input type="hidden" id="formProductId" value="-1" name="productId"/>
                        <div class="form-group">
                            <label>Namn:</label>
                            <input type="text" name="productName" maxlength="11" class="form-control" placeholder="Produktnamn" required/>
                            <div class="invalid-feedback">Var god skriv ett namn.</div>
                        </div>
                        <div class="form-group">
                            <label>Version:</label>
                            <input type="text" name="version" class="form-control" value="1.0" required/>
                            <div class="invalid-feedback">Var god skriv en version.</div>
                        </div>
                        <div class="form-group">
                            <label>Filnamn:</label>
                            <input type="text" class="form-control" name="fileName" value="file.zip" required/>
                            <div class="invalid-feedback">Var god skriv ett filnamn.</div>
                        </div>
                        <div class="modal-footer">
                            <div class="form-group text-right">
                                <input type="submit" class="btn btn-primary" value="Spara">
                                <input type="submit" style="display: none" name="delete" id="deleteButton" class="btn btn-danger" value="Ta bort">
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>

    <script>
    $(document).ready(function(){

        $("#newProductButton").click(function() {
            $("#formAction").attr("value", "createProduct");
            $("#formProductId").val("-1");
            $("input[name=\"productName\"]").val("");
            $("input[name=\"version\"]").val("1.0");
            $("input[name=\"fileName\"]").val("file.zip");
            $("#deleteButton").hide();

            $("#newProduct").modal();
        });

        $(".editProductButton").click(function() {

            $("#deleteButton").show();
            var $row = $(this).parents().eq(1);
            var productId = $row.attr("productId");
            $("#formProductId").val(productId);

            var properties = $row.children("td");

            $("#formAction").attr("value", "editProduct");
            $("input[name=\"productName\"]").val(properties[0].innerText);
            $("input[name=\"version\"]").val(properties[1].innerText);
            $("input[name=\"fileName\"]").val(properties[2].innerText);
            $("#newProduct").modal();
        });

        $("#deleteButton").click(function(e) {

            if(!confirm("Är du säker på att du vill ta bort denna produkt?"))
            {
                e.preventDefault();
                e.stopPropagation();
            }
        });

    });

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
    </script>
</body>
</html>