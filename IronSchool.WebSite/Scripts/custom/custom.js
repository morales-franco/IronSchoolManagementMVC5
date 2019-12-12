function prepareJSonFilter(items) {
    var jsonString = "";

    items.forEach(function (currentValue, index, arr) {
        jsonString += '"' + currentValue + '": "' + $("#" + currentValue).val() + '"';
        if (index + 1 < items.length)
            jsonString += ",";
    }
    );

    return JSON.parse("{" + jsonString + "}");
}

function filterGrid() {
    $("#grid").data("kendoGrid").dataSource.read();
    $("#grid").data("kendoGrid").refresh();
}

function getFilter() {
    var IDs = [];
    $('#filterContainer :input.form-control').each(function () { IDs.push(this.id); });
    return prepareJSonFilter(IDs);
}

function downloadGridAsExcel() {
    $("#grid").data("kendoGrid").saveAsExcel();
}

(function ($) {
    var defaultOptions = {
        errorClass: 'has-error',
        validClass: 'has-success',
        highlight: function (element, errorClass, validClass) {
            $(element).closest(".form-group")
                .addClass(errorClass)
                .removeClass(validClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element).closest(".form-group")
                .removeClass(errorClass)
                .addClass(validClass);
        }
    };

    $.validator.setDefaults(defaultOptions);
    $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
    }
    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };
})(jQuery);

/*
 * Notifications
 */
function notify(message, type) {
    $.growl({
        message: message,
        url: ''
    }, {
            element: 'body',
            type: type,
            allow_dismiss: true,
            offset: {
                x: 20,
                y: 85
            },
            spacing: 10, 
            z_index: 10005,
            delay: 2500,
            timer: 1000,
            url_target: '_blank',
            mouse_over: false,
            icon_type: 'class',
            template: '<div data-growl="container" class="alert" role="alert">' +
                '<button type="button" class="close" data-growl="dismiss">' +
                '<span aria-hidden="true">&times;</span>' +
                '<span class="sr-only">Close</span>' +
                '</button>' +
                '<span data-growl="icon"></span>' +
                '<span data-growl="title"></span>' +
                '<span data-growl="message"></span>' +
                '<a href="#" data-growl="url"></a>' +
                '</div>'
        });
}


function confirmDelete(url, id) {
    var conf = confirm('¿Confirma que desea eliminar el elemento seleccionado?');
    if (conf) {
        $.ajax({
            url: url,
            type: "post",
            dataType: "json",
            traditional: true,
            async: false,
            data: { 'id': id },
            success: function (result, responseText) {

                if (result === true) {
                    notify('El elemento se ha eliminado exitosamente.', 'success');
                    filterGrid();
                } else {
                    notify(result, 'warning');
                }
                return false;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                notify('Se produjo un error al intentar eliminar el elemento.', 'danger');
                return false;
            }
        });
    }
    else {
        return false;
    }
}


function editSelectEntity(url, element) {
    var id = $(element).parent().parent().find("select").val();
    if (id.length > 0) {
        var newUrl = url.replace("elementId", id);
        window.open(newUrl, 'Editar');
    }
    else {
        notify('Debe seleccionar un elemento para editar', 'warning');
    }
}

function reloadSelectFromModal(elementId, values) {

    var $select = $('#' + elementId);
    var url = $select.data("url");
    var returnObject = JSON.parse(values);

    $select.empty().append('<option selected="selected" value="0" disabled = "disabled">Loading.....</option>');


    $.ajax({
        url: url,
        type: 'POST',
        success: function (response) {
            $select.empty();
            var items = '<option>Seleccione</option>';

            $.each(response, function (i, item) {
                items += "<option value='" + item.Value + "'>" + item.Text + "</option>";
            });
            $select.html(items);

            if (returnObject != null && JSON.parse(values)[elementId] != null)
                $select.val(JSON.parse(values)[elementId]);
        },
        error: function (xhr) {
            notify('Se ha producido un error al intentar obtener los elementos del campo ' + elementId, 'danger');
        }
    });
}

function setCreatedElement(elementId, values) {
    //No hago nada, espero que el usuario lo busque con el browser
}

function showBrowser(browserUrl, sender) {
    $("#browserContent").empty();
    $('#browserContent script').empty();

    var associatedFiledId = $(sender).parent().parent().find(".browser-id").attr('id');
    $("#browserContent").kendoWindow({
        width: "700px",
        height: "550px",
        title: "Seleccione un elemento",
        content: browserUrl,
        modal: true
    }).data("kendoWindow").center().open();
    $("#browserContent").data("associatedFiledId", associatedFiledId);
}

function browserGetByCode(url, input) {
    var code = $(input).val();
    var prevValue = $(input).data("previous-value");
    if (prevValue == undefined)
        prevValue = "";

    var ig = $(input).parent().parent().parent().parent();

    if (code != prevValue) {
        url = url + "?code=" + code;
        $.ajax({
            url: url,
            type: 'GET',
            async: false,
            success: function (response) {
                $(input).data("previous-value", code);
                if (response != "") {
                    ig.find('.browser-id').val(response.id);
                    ig.find('.browser-code').val(response.code);
                    ig.find('.browser-desc').val(response.description);
                }
                else {
                    ig.find('.browser-id').val('');
                    ig.find('.browser-code').val('');
                    ig.find('.browser-desc').val('');
                    notify('No se ha encontrado ningún elemento para el código ingresado', 'warning');
                }

                ig.find('.browser-id').trigger('change');
            },
            error: function (xhr) {
                notify('Se ha producido un error al intentar buscar por el código ingresado', 'danger');
            }
        });
    }
    else {
        if (code == "") {
            ig.find('.browser-id').val('');
            ig.find('.browser-code').val('');
            ig.find('.browser-desc').val('');

            ig.find('.browser-id').trigger('change');
        }
    }
}

function browserItemSelected(id, code, description) {
    var associatedFiledId = $("#browserContent").data("associatedFiledId");
    var idField = $("#" + associatedFiledId);
    idField.val(id);
    
    idField.parent().find(".browser-code").val(code);
    idField.parent().find(".browser-desc").val(description);
    idField.parent().find(".browser-id").trigger('change');

    idField.parent().find(".browser-id").trigger('on-browser-item-selected');
    $("#browserContent").data("kendoWindow").close();
    $("#browserContent").empty();
}

function deleteRowItem(object, listName) {
    var r = confirm("¿Estás seguro que querés eliminar el elemento seleccionado?");
    if (r == true) {
        var link = $(object);
        var table = link.closest("td").closest("tr").closest("table");
        link.closest("td").closest("tr").remove();

        //Reindexo la tabla
        var newIdx = 0;
        var rows = table.find('tbody > tr:has(td):not(:has(th))');

        rows.each(function () {
            var td = $(this).find('td');
            td.each(function () {
                var controls = $(this).find('[name^="' + listName + '"]');
                controls.each(function () {
                    var attr = $(this).attr('name');
                    attr = attr.split(']')[1];
                    $(this).attr('name', '' + listName + '[' + newIdx + ']' + attr);
                });
            });
            newIdx++;
        });

        return true;
    }
    return false;
}

function onlyNumbers(e) {
    var prevent = true;
    var keyCode = window.event.keyCode;

    if ((keyCode >= 48 && keyCode <= 57) ||
        (keyCode >= 96 && keyCode <= 105) ||
        keyCode == 8 || keyCode == 9 || keyCode == 37 ||
        keyCode == 39 || keyCode == 46 || keyCode == 188) {

    } else {
        prevent = false;
    }

    if (keyCode == 110 && $(e).val().indexOf(',') == -1) {
        $(e).val($(e).val().replace('.', ','));
    }
    if ($(e).val().indexOf(',') !== -1 && keyCode == 188) {
        prevent = false;
    }

    if (!prevent) {
        window.event.preventDefault();
    }
}



function getFormatDecimal(stringNumber) {
    return stringNumber.replace(".", ",");
}

function stringToDecimal(stringNumber, decimals) {
    var result = 0;

    if (stringNumber.trim() == "")
        return result;

    result = parseFloat(stringNumber.replace(",", ".")).toFixed(decimals);
    return parseFloat(result);
}

function decimalToString(decimalNumber) {
    var aux = "";

    if (typeof decimalNumber === 'string')
        aux = decimalNumber;
    else
        aux = decimalNumber.toFixed(2).toString();

    return getFormatDecimal(aux);
}

function addRuleValidator(selector, messageRequired) {
    $(selector).rules("add", {
        required: true,
        messages: {
            required: messageRequired
        }
    });
}

function removeRules(selector) {
    $(selector).rules('remove');
}

function configureChildrenManagerStyle(manager) {
    manager.addIndexTableButtonSeparator = function () {
        return '';
    };

    manager.addIndexTableEditText = function () {
        return '<i class="grid-action-icon zmdi-edit"></i>';
    };

    manager.addIndexTableViewText = function () {
        return '<i class="grid-action-icon zmdi-view-headline"></i>';
    };

    manager.addIndexTableDeleteText = function () {
        return '<i class="grid-action-icon zmdi-delete"></i>';
    };

    manager.clearAbmValidation = function () {
        var errorClass = manager.getValidationErrorClass();
        var validClass = manager.getValidationValidClass();

        var inputErrorClass = manager.getInputErrorClass();
        var inputValidClass = manager.getInputValidClass();

        this.getValidationControls().each(function (idx, element) {
            $(element).removeClass(errorClass).addClass(validClass).empty();
        });

        this.getControls().each(function (idx, element) {
            $(element).removeClass(inputErrorClass).addClass(inputValidClass);
        });

        //además limpio los colores verdes y rojos de los campos
        var groups = $('.has-success', this.getAbmForm());
        groups.each(function (idx, element) {
            $(element).removeClass('has-success');
        });

        groups = $('.has-error', this.getAbmForm());
        groups.each(function (idx, element) {
            $(element).removeClass('has-error');
        });
    }
}

function disableChildrenManager(manager) {
    manager.addIndexTableEditButton = function () { return "" };
    manager.addIndexTableDeleteButton = function () { return "" };
    $('.addChild').remove();
}

function convertToBoolean(value) {
    var result = value == true || value == "True" || value == 1 || value == "1" || value == "true";

    return result;
}

function fillRelatedSelect(baseUrl, urlParams, relatedElement) {
    $.ajax({
        url: baseUrl + urlParams,
        type: 'GET',
        success: function (response) {
            relatedElement.empty();
            var items = '<option>Seleccione</option>';

            $.each(response, function (i, item) {
                items += "<option value='" + item.Value + "'>" + item.Text + "</option>";
            });
            relatedElement.html(items);

        },
        error: function (xhr) {
            notify('Se ha producido un error al intentar obtener los elementos del campo relacionado', 'danger');
        }
    });
}

function handlerError(jqXHR, textStatus, errorThrown) {
    console.log(jqXHR);
}