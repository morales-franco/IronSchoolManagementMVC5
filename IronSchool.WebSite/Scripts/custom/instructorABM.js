/************* Global Variable Section *************/
var _instructor = null;


/************* Init Section *************/
function InstructorJs() {
    this.Constant = {};
    this.Resources = {};
    this.Url = {};
}

function init() {
    setEvents();
}

function setEvents() {
    $(".browser-id").on("change", onChangeUser);

}

function onChangeUser() {
    var userName = $('#UserName').val();

    if (userName == "") {
        $('#ContactMail').val("");
        return false;
    }

    $.ajax({
        url: _instructor.Url.GetByCodeUser,
        type: "get",
        dataType: "json",
        traditional: true,
        async: true,
        data: { 'code': userName },
        success: function (userData) {
            $('#ContactMail').val(userData.email);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            notify('Se produjo un error al buscar el usuario', 'danger');
            handlerError(jqXHR, textStatus, errorThrown);
            return false;
        }
    });

}