$(document).ready(function(){

    $("#image").bind("load", function() {
        $('#zoom-span').trigger('zoom.destroy').zoom();
    });

    // $("#update-list-btn").click(refreshImages);

    var $activeSite = $('#site-list a[href="' + window.location.pathname + '"]');
    $activeSite.toggleClass("active");
    document.title = "EKO Snapshots - " + $activeSite.text();

    // $('#site-list a[href="' + window.location.pathname + '"]').toggleClass("active");

    refreshFolders();

    setInterval(() => {
        refreshImages(false);
    }, 5000);
});

function getSelectedFolder() {
    return $("#stores-list .active").first().text();
}

function refreshFolders() {
    $.get("upload.php?action=listfolders", function(data) {
        var folders = JSON.parse(data);
        var $foldersListGrp = $("#stores-list").html("");

        var header = document.createElement("a");
        header.classList = "list-group-item list-group-item-action disabled";
        header.innerText = "Butiker";
        header.href = "#";
        $foldersListGrp.append(header);

        var first = true;
        for(var i in folders) {
            var listItem = document.createElement("a");
            listItem.classList = "list-group-item list-group-item-action" + (first ? " active" : "");
            if(first) {
                lastFolderListItemClicked = listItem;
            }
            
            first = false;
            listItem.innerText = folders[i];

            listItem.href = "#";

            listItem.onclick = folderListItemClick;

            $foldersListGrp.append(listItem);
        }
        refreshImages(false);
        
    });
}

function refreshImages(clickLast) {
    var folder = getSelectedFolder();
    $.get("upload.php?action=list&folder=" + folder, function(data) {

        var images = JSON.parse(data);
        var $imgListGrp = $("#images-list").html("");

        var header = document.createElement("a");
        header.classList = "list-group-item list-group-item-action disabled";
        header.innerText = "Ögonblicksbilder";
        header.href = "#";
        $imgListGrp.append(header);

        var lastItem = null;

        for(var i in images)
        {
            if(images[i].isFolder)
                continue;
            
            var listItem = document.createElement("a");

            listItem.classList = "list-group-item list-group-item-action image-item";
            if(lastImageListItemClicked !== null && lastImageListItemClicked.getAttribute("imglink") === images[i].filename) {
                listItem.classList += " active";
                lastImageListItemClicked = listItem;
            }

            listItem.innerText = new Date(images[i].createdate * 1000).toLocaleString();
            listItem.setAttribute("imglink", images[i].filename);

            listItem.href = "#";

            listItem.onclick = imageListItemClick;
            listItem.onfocus = imageListItemFocus;
            listItem.onblur = imageListItemBlur;

            $imgListGrp.append(listItem);
            lastItem = listItem;
        }

        if(clickLast || lastImageListItemClicked === null)
            lastItem.click();
        else
            lastImageListItemClicked.focus({preventScroll:true});

    });

}

function imageListItemFocus() {
    // console.log("Got focus", this);
    this.onkeydown = imageListItemKeyDown;

}

function imageListItemKeyDown(evt) {
    // console.log(evt);
    if(evt.keyCode === 38) // Arrow up
    {
        $(this).prev().click().focus({preventScroll:true});
    }
    else if(evt.keyCode === 40) // Arrow down
    {
        $(this).next().click().focus({preventScroll:true});
    }

}

function imageListItemBlur() {
    // console.log("Got blur", this);
    this.onkeydown = null;

}

var lastImageListItemClicked = null;
function imageListItemClick() {

    if(lastImageListItemClicked !== null) {
        $(lastImageListItemClicked).toggleClass("active");
    }

    $(this).toggleClass("active");
    lastImageListItemClicked = this;
    $("#image").attr("src", $(this).attr("imglink"));
};


var lastFolderListItemClicked = null;
function folderListItemClick() {

    if(lastFolderListItemClicked !== null) {
        $(lastFolderListItemClicked).toggleClass("active");
    }

    $(this).toggleClass("active");
    lastFolderListItemClicked = this;
    refreshImages(true);
}
