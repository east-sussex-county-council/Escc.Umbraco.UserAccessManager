$("#menuitemuser").click(function () {
    $("#searchinput").html("User <span class='caret'></span>");
    $("#searchterm").prop("type", "text").prop("name", "Username").prop("placeholder", "Username").val("");
});

$("#menuitememail").click(function () {
    $("#searchinput").html("Email <span class='caret'></span>");
    $("#searchterm").prop("type", "email").prop("name", "emailAddress").prop("placeholder", "Email Address").val("");
});

$("#menuitemurl").click(function () {
    $("#searchinput").html("URL <span class='caret'></span>");
    $("#searchterm").prop("type", "text").prop("name", "pageUrl").prop("placeholder", "Page URL").val("");
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
        selectMode: 2,
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

$("#searchterm").keypress(function (e) {
    if (e.which == 13) {
        $("#finduser").trigger("click");
    }
});

updateFields = function (data) {
    if (data == false) {
        //alert("Error - Unable to find user");
        $("#result").html("<p><strong>User not found</strong></p>");
        $("#copyPerms").attr("disabled", "disabled");
    } else {
        $("#result").html("<p id='sourceId' class='" + data.UserId + "'><strong>Id:</strong> " + data.UserId + "</p><p><strong>Name:</strong> " + data.FullName + "</p><p><strong>Email:</strong> " + data.EmailAddress + "</p><p><strong>Logon ID:</strong> " + data.UserName + "</p>");
        $("#copyPerms").removeAttr("disabled");
    }
}

$("#copyPerms").click(function () {
    var token = $('[name=__RequestVerificationToken]').val();
    $.ajax({
        url: $("#apppath").html() + "/Permissions/CopyPermissionsForUser",
        type: "POST",
        data: { __RequestVerificationToken: token, sourceId: $("#sourceId").prop("class"), targetId: $("#userId").html() },
        dataType: "json",
        success: function (json) {
            if (json.isRedirect) {
                window.location.href = json.redirectUrl + "/Index/" + $("#userId").html();
            }
        }
    });
});

$("#myModalClose").click(function () {
    $("#searchterm").val("");
    $("#result").empty();
});

//$("#checkpage").click(function () {
//    var btn = $(this);
//    btn.prop("disabled", true);
//    var dest = $("#PageEditors");
//    dest.html("");
//    $.get($("#apppath").html() + "/Tools/CheckPagePermissions/", { url: $("#url").val() }, function (data) {
//        dest.html(data);
//        btn.prop("disabled", false);
//    });
//});

$("#lookupPermissions").click(function () {
    var btn = $(this);
    btn.prop("disabled", true);
    var dest = $("#PermissionsResults");
    dest.html("<img src=\"Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");
    var searchTerm = $("#searchterm").prop("name");

    switch (searchTerm) {
        case "emailAddress":
            var email = $("#searchterm").val();
            $.get($("#apppath").html() + "/Tools/CheckUserPermissions/", { EmailAddress: email }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        case "Username":
            var user = $("#searchterm").val();
            $.get($("#apppath").html() + "/Tools/CheckUserPermissions/", { UserName: user }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;

        case "pageUrl":
            var url = $("#searchterm").val();
            $.get($("#apppath").html() + "/Tools/CheckPagePermissions/", { url: url }, function (data) {
                dest.html(data);
                btn.prop("disabled", false);
            });
            break;
        default:
            dest.html("");
            btn.prop("disabled", false);
    }
});

$("#PagesWithoutAuthor").click(function () {
    var btn = $(this);
    btn.prop("disabled", true);
    var dest = $("#UnauthordPermissions");
    dest.html("<img src=\"Content/ajax-loader.gif\" class=\"loaderimg\" alt=\"Please wait...\" />");
    $.get($("#apppath").html() + "/Tools/CheckPagesWithoutAuthor/", function (data) {
        dest.html(data);
        btn.prop("disabled", false);
    });
});

$("#url").keypress(function (e) {
    if (e.which == 13) {
        $("#checkpage").trigger("click");
    }
});

$("#searchterm").keypress(function (e) {
    if (e.which == 13) {
        $("#lookupPermissions").trigger("click");
    }
});