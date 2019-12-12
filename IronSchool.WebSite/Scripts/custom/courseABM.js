/************* Global Variable Section *************/
var _course = null;


/************* Init Section *************/
function CourseJs() {
    this.Constant = {};
    this.Resources = {};
    this.Url = {};
}

function init() {
    setEvents();
}

function setEvents() {
    $('#add-student-btn-id').click(showListStudent);

}

function showListStudent() {
    $("#browserContent").kendoWindow({
        width: "800px",
        height: "550px",
        title: "Seleccione un elemento",
        content: _course.Url.BrowserStudent,
        modal: true
    }).data("kendoWindow").center().open();
}

function browserInstructorItemSelected(instructorId, instructorName) {
    browserItemSelected(instructorId, null, instructorName);
}

function closePopUp(popUpID) {
    $(popUpID).data("kendoWindow").close();
    $(popUpID).empty();
}

function browserStudentItemSelected(studentId, firstName, lastName) {
    if (studentIsRepetead(studentId)) {
        notify(_course.Resources.ValidationStudentSeleccionadoRepetido, 'danger');
        return false;
    }

    addRowStudent(studentId, firstName, lastName);
    closePopUp("#browserContent");
}

function studentIsRepetead(studentId) {
    var isRepetead = $("#student-table-id tbody tr input.student-id").filter(function (index, input) {
        return $(input).val() == studentId;
    }).length > 0;

    return isRepetead;
}

function addRowStudent(studentId, firstName, lastName) {
    var index = $("#student-table-id tbody tr").length;
    var tr = '<tr>';
    tr += '<td>';
    tr += '<input type="hidden" value="' + studentId + '" name="Students[__index__].StudentId" class="student-id"/>';
    tr += '<input type="hidden" value="' + firstName + '" name="Students[__index__].FirstName" />';
    tr += '<input type="hidden" value="' + lastName + '" name="Students[__index__].LastName" />';
    tr += lastName + '</td>';
    tr += '<td>' + firstName + '</td>';
    tr += '<td><ul class="actions pull-right">' +
        '<li><button type="button" formnovalidate="formnovalidate" class="btn btn-danger waves-effect" onclick="removeRowStudent(this)"><i class="zmdi zmdi-delete"></i></button></li>' +
        '</ul></td>';
    tr += '</tr>';

    tr = tr.replace(/__index__/g, index);
    $('#student-table-id tbody').append(tr);
}

function removeRowStudent(btn) {
    deleteRowItem(btn, 'Students');
}