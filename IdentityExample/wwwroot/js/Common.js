




function showPopup(url, callback) {
    
    $.ajax({
        type: "GET",
        url: url,
        success: function (data) {
            $(".modal-backdrop").remove();
            var popupWrapper = $("#PopupWrapper");
            popupWrapper.empty();
            popupWrapper.html(data);
            var popup = $(".modal", popupWrapper);
            $(".modal", popupWrapper).modal();
            callback(popup);
        }
    });
}

function init() {
    $("#LoginPopup").click(function () {
        _this.showPopup("/admin/Account/Ajax", function (modal) {
        });
    });
}


document.getElementById('LoginPopup').addEventListener('click', () => {

        this.showPopup("/admin/Account/Ajax", function (modal) {
        });

    this.showPopup("/admin/Account/Ajax", initLoginPopup);
});


function init() {
    $("#LoginPopup").click(function () {
        _this.showPopup("/admin/Account/Ajax", initLoginPopup);
    });
}
function initLoginPopup(modal) {
    $("#LoginButton").click(function () {
        $.ajax({
            type: "POST",
            url: "/admin/Account/Ajax",
            data: $("#LoginForm").serialize(),
            success: function (data) {
                showModalData(data);
                initLoginPopup(modal);
            }
        });
    });
}

function showModalData(data, callback) {
    $(".modal-backdrop").remove();
    var popupWrapper = $("#PopupWrapper");
    popupWrapper.empty();
    popupWrapper.html(data);
    var popup = $(".modal", popupWrapper);
    $(".modal", popupWrapper).modal();
    if (callback != undefined) {
        callback(popup);
    }
}
