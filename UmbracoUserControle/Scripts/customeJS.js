$("#menuitemuser").click(function () {
    $("#searchinput").html("User <span class='caret'></span>");
    $("#searchterm").prop("type", "text").prop("name", "Username").prop("placeholder", "Username");
});

$("#menuitememail").click(function () {
    $("#searchinput").html("Email <span class='caret'></span>");
    $("#searchterm").prop("type", "email").prop("name", "emailAddress").prop("placeholder", "Email Address");
});

$(function () {
    $("#tree").fancytree({
        source: {
            url: $("#apppath").html() + "/Permissions/PopTreeRootResult",
            cache: false,
            data: { UserId: $("#userId").html() }
        },
        lazyLoad: function (event, data) {
            var node = data.node;
            data.result = {
                url: $("#apppath").html() + "/Permissions/PopTreeChildResult",
                cache: false,
                data: { RootId: node.key, UserId: data.node.data.UserId }
            };
        },
        checkbox: true,
        autoScroll: true,
        generateIds: true,
        idPrefix: "",
        loadChildren: function (event, data) {
            if (data.node.hasChildren()) {
                data.node.folder = true;
            };
        },
        select: function (event, data) {
            $.getJSON($("#apppath").html() + "/Permissions/ChangePermissionsResult", { PageId: data.node.key, UserId: data.node.data.UserId, selected: data.node.selected, PageName: data.node.title }, function (returnData) {
                if (!returnData) {
                    if (data.node.selected == true) {
                        alert("Error setting permissions");
                        data.node.selected = false;
                    }
                    else {
                        alert("Error removing permissions");
                        data.node.selected = true;
                    }
                }
            });
        }
    });
});

$("#finduser").click(function () {
    if ($("#searchterm").prop("name") == "emailAddress") {
        var email = $("#searchterm").val();
        $.getJSON($("#apppath").html() + "/Permissions/CheckDestinationUser", { EmailAddress: email }, updateFields);
    }
    else {
        var user = $("#searchterm").val();
        $.getJSON($("#apppath").html() + "/Permissions/CheckDestinationUser", { UserName: user }, updateFields);
    };
});

updateFields = function (data) {
    if (data.length == 0) {
        alert("Error finding user");
    }
    $(".result").append("<p id='targetId' class='" + data.UserId + "'>Id: " + data.UserId + "</p><p>Name: " + data.FullName + "</p><p>Email: " + data.EmailAddress + "</p><p>Username: " + data.UserName + "</p>");
}

$("#postAjax").click(function () {
    $.ajax({
        url: $("#apppath").html() + "/Permissions/CopyPermissionsForUser",
        type: "POST",
        data: { sourceId: $("#userId").html(), targetId: $("#targetId").prop("class") },
        dataType: "json",
        success: function (json) {
            if (json.isRedirect) {
                window.location.href = json.redirectUrl + "/Index/" + $("#targetId").prop("class");
            }
        }
    });
});

$("#checkpage").click(function () {
    alert("click");
    var dest = $("#editors");
    $.get($("#apppath").html() + "/Tools/CheckPagePermissions/", { url: $("#url").val() }, function (data) {
        dest.replaceWith(data);
    });
});