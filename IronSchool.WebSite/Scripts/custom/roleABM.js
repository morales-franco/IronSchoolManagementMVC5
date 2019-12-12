$().ready(function () {
    $(":checkbox").after("<i class='input-helper'></i>");

    $("#selectAllRules").change(function () {
        if ($(this).is(":checked")) {
            $("#rulesTable tbody tr input[type=checkbox]").prop('checked', true);
        }
        else {
            $("#rulesTable tbody tr input[type=checkbox]").prop('checked', false);
        }
    });

    $("#selectAllUsers").change(function () {
        if ($(this).is(":checked")) {
            $("#usersTable tbody tr input[type=checkbox]").prop('checked', true);
        }
        else {
            $("#usersTable tbody tr input[type=checkbox]").prop('checked', false);
        }
    });
});