<?php
// ini_set('display_errors',1); 
// error_reporting(E_ALL);

$thisPage = "managelicenses";
require_once __DIR__ . "/../functions/checklogin.php";
require_once __DIR__ . "/../functions/newConfig.php";

if($_SERVER["REQUEST_METHOD"] == "POST"){
    if(isset($_POST["action"]))
    {
        if($_POST["action"] == "getProducts")
        {
            $productsArray = array();
            if($stmt = $conn->prepare("SELECT * FROM products;"))
            {
                $stmt->execute();
                $result = $stmt->get_result();
                while($row = $result->fetch_assoc())
                {
                    array_push($productsArray, $row);
                }
            }
    
            echo json_encode($productsArray);
        }
        else if($_POST["action"] == "addProduct")
        {
            if($stmt = $conn->prepare("INSERT INTO products (Name, DisplayName) VALUES (?, ?);"))
            {
                $stmt->bind_param("ss", $_POST["name"], $_POST["displayname"]);
                $stmt->execute();
            }
        }
        else if($_POST["action"] == "deleteProduct" && isset($_POST["product"]))
        {
            if($stmt = $conn->prepare("DELETE FROM products WHERE ID=?;"))
            {
                $stmt->bind_param("i", $_POST["product"]);
                $stmt->execute();
            }
        }
    }
    
    exit();
}

$query = "SELECT * FROM licenses";
if(!$isAdmin)
{
    $query .= " WHERE AddedBy=?";
}

if($stmt = $conn->prepare($query))
{
    if(!$isAdmin)
    {
        $stmt->bind_param("s", $_SESSION["username"]);
    }

    $stmt->execute();
    $result = $stmt->get_result();
    $licenseArray = array();
    while($row = $result->fetch_assoc())
    {
        array_push($licenseArray, $row);
    }
    $stmt->free_result();
    $stmt->close();
}
else
    echo "Unable to fetch licenses";

if($stmt = $conn->prepare("SELECT * FROM products"))
{
    $stmt->execute();
    $result = $stmt->get_result();
    $productsArray = array();
    while($row = $result->fetch_assoc()) {
        array_push($productsArray, $row);
    }
}
else
    echo "Unable to fetch products";

$conn->close();
?>

<!DOCTYPE html>
<html lang="en">
<head>
    <title>ISTP - Licenser</title>
    <?php include __DIR__ . "/../functions/getbt.php"; ?>
    <link rel="stylesheet" type="text/css" href="../customStyles.css">
    <style type="text/css">
        body{ font: 14px sans-serif; }
    </style>
    <!--<error>
        <?=$_SESSION["errorResult"]?>
    </error>-->
</head>
<body>
    
    <?php
    $activePage = "manageLic";
    include __DIR__ . "/../functions/navbar.php";?>
    <div class="container-fluid mx-auto">
        <div class="page-header">
            <h1>Licenshantering</h1>
            <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#newLicense">Ny licens</button>
            <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#productsModal">Produkter</button>
        </div>
        <br><br>
        <div class="table-responsive-xl">
            <table class="table table-striped table-bordered">
                <thead>
                    <tr>
                        <th scope="col">ID</th><th scope="col">Produkt</th><th scope="col">Kund</th><th scope="col">Site</th><th scope="col">LicenseGUID</th>
                        <th scope="col">MachineGUID</th><th scope="col">Användargräns</th><th scope="col">SMS</th><th>BankID</th><th scope="col">Datum tillagd</th>
                        <th scope="col">Utgångsdatum</th><?php echo $isAdmin ? "<th>Användare</th>" : ""; ?><th>Åtgärd</th>
                    </tr>
                </thead>
                <tbody>
                    <?php
                        $i = 0;
                        foreach($licenseArray as $sqlRow) {
                            echo "<tr>";
                            echo "<td>".$sqlRow['ID']."</td><td>".$sqlRow['Product']."</td><td class='customerCol'>".$sqlRow['Customer']."</td><td class='siteCol'>" . $sqlRow["Site"] . "</td>";
                            echo "<td class='pop' data-toggle='popover' data-content='Licens kopierad urklipp!' data-placement='top'><a href='#copy' class='licenseCol' data-toggle='tooltip' title='Kopiera till urklipp' data-placement='right'>" . $sqlRow["LicenseGUID"] . "</a></td>";
                            echo "<td>" . $sqlRow["MachineGUID"] . "</td><td class='maxCurUsersCol'>" . $sqlRow["MaxCurrentUsers"] . "</td><td class='allowSmsCol'>" . ($sqlRow["SMS"] === 1 ? "Ja" : "Nej") . "</td><td>" . ($sqlRow["BankID"] === 1 ? "Ja" : "Nej") . "</td><td>" . $sqlRow["DateAdded"] . "</td><td class='expirationDateCol'>" . $sqlRow["ExpirationDate"] . "</td>";
                            echo $isAdmin ? "<td>" . $sqlRow["AddedBy"] . "</td>" : "";
                            echo '<td><button type="button" class="btn btn-secondary" onclick="openEditLicenseModal(this)" data-toggle="modal" data-target="#editLicense">Ändra</button>';
                            echo "</tr>";
                            $i++;
                        }
                    ?>
                </tbody>
                <caption><?php echo count($licenseArray); ?> licenses</caption>
            </table>
        </div>
    </div>
    <?php include "../modals/newLicenseModal.php"; include "../modals/editLicenseModal.php"; include "../modals/productsModal.php"; ?>

    <input id="copyBox" style="display:none"></input>
    <script>
    var elements = document.querySelectorAll(".licenseCol");
    //console.log(elements);
    for (var i = 0; i < elements.length; i++) {
        elements[i].onclick = copyText;
    }
    function copyText()
    {
        copyBox = document.getElementById("copyBox");
        copyBox.style.display = "block";
        copyBox.value = this.innerHTML;
        copyBox.select();
        document.execCommand("Copy");
        copyBox.style.display = "none";
    }

        $(function () {
    $('[data-toggle="popover"]').popover()
    })
    $(function () {
    $('[data-toggle="tooltip"]').tooltip()
    })
    $('.pop').on('shown.bs.popover', function () {
        var $pop = $(this);
        setTimeout(function () {
            $pop.popover('hide');
        }, 800);
    });

    $(document).ready(function() {
        // show the alert
        setTimeout(function() {
            $(".alert").alert('close');
        }, 4000);

        $('#productsModal').on('shown.bs.modal', function () {
            fetchProducts();
        });

        $("#productsModal .add-product-btn").click(function()
        {
            addProduct();
        });

        $("input[name='allowSms']").change(function()
        {
            $("input[name='smsLimit']").attr("disabled", !this.checked);
        });

        $("input[name='allowBankId']").change(function()
        {
            $("input[name='bankIdLimit']").attr("disabled", !this.checked);
        });
    });
    // Example starter JavaScript for disabling form submissions if there are invalid fields
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

    function openEditLicenseModal(sender)
    {
        license = sender.parentElement.parentElement.getElementsByClassName("licenseCol")[0];
        customer = sender.parentElement.parentElement.getElementsByClassName("customerCol")[0];
        site = sender.parentElement.parentElement.getElementsByClassName("siteCol")[0];
        maxCurUsers = sender.parentElement.parentElement.getElementsByClassName("maxCurUsersCol")[0];
        expirationDate = sender.parentElement.parentElement.getElementsByClassName("expirationDateCol")[0];
        document.getElementById("editCustomerField").value = customer.innerText;
        document.getElementById("editSiteField").value = site.innerText;
        document.getElementById("editExpirationField").value = expirationDate.innerText;
        document.getElementById("maxClientsField").value = maxCurUsers.innerText;
        document.getElementById("showLicenseField").value = license.innerText;
    }

    function fetchProducts()
    {
        var formData = new FormData();
        formData.append("action", "getProducts");

        var request = new XMLHttpRequest();
        request.onreadystatechange = function(e)
        {
            if(this.readyState == 4)
            {
                var products = JSON.parse(this.responseText);

                var table = $("#productsModal .products-table tbody")[0];
                table.innerHTML = "";
                for(var i in products)
                {
                    var row = table.insertRow();
                    row.setAttribute("product", products[i].ID)
                    var cell1 = row.insertCell();
                    cell1.innerText = products[i].DisplayName;

                    var cell2 = row.insertCell();
                    cell2.innerText = products[i].Name;

                    var cell3 = row.insertCell();
                    cell3.innerHTML = '<button type="button" class="btn btn-danger delete-product-btn">Radera</button>';
                }

                $("#productsModal .delete-product-btn").click(function()
                {
                    var productId = this.parentElement.parentElement.getAttribute("product");
                    if(productId != null && confirm("Är du säker på att du vill ta bort produkten? Existerande licenser gäller fortfarande."))
                    {
                        var formData = new FormData();
                        formData.append("action", "deleteProduct");
                        formData.append("product", productId);

                        var request = new XMLHttpRequest();
                        request.onreadystatechange = function(e)
                        {
                            if(this.readyState == 4)
                            {
                                fetchProducts();
                            }
                        };
                        request.open("POST", "<?=$_SERVER['PHP_SELF']?>");
                        request.send(formData);
                    }
                });
            }
        };
        request.open("POST", "<?=$_SERVER['PHP_SELF']?>");
        request.send(formData);
    }

    function addProduct()
    {
        var displayname = $('#productsModal input[name="displayname"]').val();
        var name = $('#productsModal input[name="name"]').val();

        if(displayname == "" || name == "")
        {
            alert("Det saknas info.");
            return;
        }

        $('#productsModal input[name="displayname"]').val("");
        $('#productsModal input[name="name"]').val("");

        var formData = new FormData();
        formData.append("action", "addProduct");
        formData.append("displayname", displayname);
        formData.append("name", name);

        var request = new XMLHttpRequest();
        request.onreadystatechange = function(e)
        {
            if(this.readyState == 4)
            {
                fetchProducts();
            }
        };
        request.open("POST", "<?=$_SERVER['PHP_SELF']?>");
        request.send(formData);
    }

    </script>

    <?php
    if(isset($_SESSION["successResult"]))
    {
        if(!empty($_SESSION["successResult"]))
        {
            echo "<div class='alert alert-success'>";
            echo $_SESSION["successResult"];
            echo '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>';
            echo "</div>";
        }
        unset($_SESSION["successResult"]);
    }
    if(isset($_SESSION["errorResult"]))
    {
        if(!empty($_SESSION["errorResult"]))
        {
            echo "<div class='alert alert-danger'>";
            echo $_SESSION["errorResult"];
            echo '<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>';
            echo "</div>";
        }
        unset($_SESSION["errorResult"]);
        //unset($_SESSION["expDateErrorExplanation"]);
    }

    ?>
</body>
</html>