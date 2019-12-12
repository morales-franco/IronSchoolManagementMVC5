(function ($) {

    instanceId = 0;

    $.fn.childrenManager = function (collectionName, collectionItems, options) {

        var defaultOptions = { indexFormId: 'partialIndex', abmFormId: 'partialAbm', controlsSelector: 'textarea,input[type=text],input[type=number],input[type=date],input[type=hidden],select,input[type=checkbox]', validationSelector: '.text-danger,.field-validation-error', validationValidClass: 'field-validation-valid', validationErrorClass: 'field-validation-error', inputValidClass: 'valid', inputErrorClass: 'input-validation-error' };

        var indexDataSource = [];

        var currentDataSourceItem = null;

        var current = this;

        var abortHide = false;

        this.getCollectionItems = function () {
            return collectionItems;
        }

        this.getIndexDataSource = function () {
            return indexDataSource;
        }

        this.setIndexDataSource = function (collection) {
            return indexDataSource = collection;
        }

        this.getCurrentDataSourceItem = function () {
            return currentDataSourceItem;
        }

        this.isEditAction = function () {
            return currentDataSourceItem != null;
        }

        this.setAbortHide = function (value) {
            abortHide = value;
        }

        this.getOptions = function () {
            return options;
        }

        this.getCollectionName = function () {
            return collectionName;
        }

        this.getIndexFormId = function () {
            return this.getOptions().indexFormId;
        }

        this.getIndexForm = function () {
            return $('#' + this.getOptions().indexFormId);
        }

        this.getAbmFormId = function () {
            return this.getOptions().abmFormId;
        }

        this.getAbmForm = function () {
            return $('#' + this.getOptions().abmFormId);
        }

        this.getControlsSelector = function () {
            return this.getOptions().controlsSelector;
        }

        this.getValidationValidClass = function () {
            return this.getOptions().validationValidClass;
        }

        this.getValidationErrorClass = function () {
            return this.getOptions().validationErrorClass;
        }

        this.getInputValidClass = function () {
            return this.getOptions().inputValidClass;
        }

        this.getInputErrorClass = function () {
            return this.getOptions().inputErrorClass;
        }

        this.getControls = function () {

            var controlsToValidate = null;

            if (this.getControlsSelector()) {
                if (this.getControlsSelector() != '') {
                    controlsToValidate = $(this.getControlsSelector(), this.getAbmForm());
                }
                else {
                    controlsToValidate = $(defaultOptions.controlsSelector, this.getAbmForm());
                }
            } else {
                controlsToValidate = $(defaultOptions.controlsSelector, this.getAbmForm());
            }

            return controlsToValidate;
        }

        this.getValidationControls = function () {
            return $(this.getOptions().validationSelector, this.getAbmForm());
        }

        this.getAbmCancelButton = function () {
            return current.getAbmForm().find('[data-childaction="cancel"]');
        }

        this.getAbmAcceptButton = function () {
            return current.getAbmForm().find('[data-childaction="accept"]');
        }

        this.validateAbm = function () {
            var isValidDiv = true;

            var validator = $('form').validate();

            this.getControls().each(function (idx, element) {
                var isValidField = $(element).valid();
                isValidDiv = isValidDiv && isValidField;
            });

            return isValidDiv;
        }

        this.clearAbmValidation = function () {
            var errorClass = this.getValidationErrorClass();
            var validClass = this.getValidationValidClass();

            var inputErrorClass = this.getInputErrorClass();
            var inputValidClass = this.getInputValidClass();

            this.getValidationControls().each(function (idx, element) {
                $(element).removeClass(errorClass).addClass(validClass).empty();
            });

            this.getControls().each(function (idx, element) {
                $(element).removeClass(inputErrorClass).addClass(inputValidClass);
            });
        }

        this.clearAbmFields = function () {
            this.getControls().each(function (idx, element) {
                $(element).val('');

                if ($(element).is("input[type = checkbox]")) {
                    $(element).attr("checked", false);
                }

                if ($(element).is("select")) {
                    $(element).find("option").removeAttr("selected");
                    $(element).find("option").eq(0).prop("selected", true);
                }

            });

            current.clearAbmFieldsChildCollection();
        }

        this.clearAbmFieldsChildCollection = function () {

        }

        this.resetAbmFieldsStyle = function () {

            this.getControls().each(function (idx, element) {
                $(element).removeAttr('disabled');
            });

            current.getAbmAcceptButton().css('display', '');
        }

        this.setAbmFields = function (indexDataSourceItem, disable) {

            $(indexDataSourceItem).each(function (idx, item) {

                var abmField = $('[name="' + item.name + '"]', current.getAbmForm());

                if (abmField.length > 0) {

                    if (item.value == null) {
                        return null;
                    }

                    current.setFieldValue(abmField, item);

                    if (disable) {
                        current.getAbmAcceptButton().css('display', 'none');
                        $(abmField).attr('disabled', 'disabled');
                    } else {
                        current.getAbmAcceptButton().css('display', '');
                        $(abmField).removeAttr('disabled');
                    }
                }

            });
        };

        this.setFieldValue = function (abmField, item) {
            if (item.value.toString().toLowerCase() == 'true' || item.value.toString().toLowerCase() == 'false') {
                var newValue = item.value.toString().toLowerCase().replace('t', 'T').replace('f', 'F');
                $(abmField).val(newValue);
            } else {
                $(abmField).val(item.value);
            }
        };

        this.saveToCollection = function () {

            if (current.validateAbm()) {
                current.addToCollection();
                abortHide = false;
            } else {
                abortHide = true;
            }

        }

        this.cancelAdd = function () {
            abortHide = false;
            currentDataSourceItem = null;
        }

        this.addToCollection = function () {
            var indexDataSourceItem = [];

            if (currentDataSourceItem == null) {

                var dataItemId = {};
                dataItemId.name = 'Id';
                dataItemId.value = (indexDataSource.length + 1) * (-1);

                indexDataSourceItem.push(dataItemId);

                this.getControls().each(function (idx, element) {

                    var dataItem = {};

                    dataItem.name = $(element).attr('name');
                    dataItem.value = $(element).val();

                    indexDataSourceItem.push(dataItem);
                });


                indexDataSource.push(indexDataSourceItem);
                this.trySetChildCollectionData(indexDataSourceItem);
                this.addToIndex(indexDataSourceItem);

            } else {

                var aux = [];

                $(indexDataSource).each(function (i, elem) {
                    if (elem[0].value != currentDataSourceItem[0].value) {
                        aux.push(elem);
                    }
                });

                indexDataSource = aux;

                var dataItemId = {};
                dataItemId.name = 'Id';
                dataItemId.value = currentDataSourceItem[0].value;

                indexDataSourceItem.push(dataItemId);

                this.getControls().each(function (idx, element) {

                    var dataItem = {};

                    dataItem.name = $(element).attr('name');
                    dataItem.value = $(element).val();

                    indexDataSourceItem.push(dataItem);
                });

                indexDataSource.push(indexDataSourceItem);
                this.trySetChildCollectionData(indexDataSourceItem);
                this.changeInIndex(indexDataSourceItem);
            }

            currentDataSourceItem = null;
        }

        this.trySetChildCollectionData = function (indexDataSourceItem) {
            var dataChildren = this.getChildCollectionData();

            if (dataChildren == null)
                return;

            $(dataChildren).each(function (idx, dataItem) {
                indexDataSourceItem.push(dataItem); 
            });

        }

        this.getChildCollectionData = function () {
            return null;
        }

        this.addToIndex = function (indexDataSourceItem) {
            var indexTable = this.getIndexForm();

            var indexTableColumns = $('tr th', indexTable);

            var currentId = indexDataSourceItem[0].value;

            var indexTableRow = $('<tr data-itemid="' + currentId + '"></tr>');

            $(indexTableColumns).each(function (idx, column) {

                $(indexDataSourceItem).each(function (i, item) {

                    if ($(column).data('boundto') == item.name) {

                        var indexTableCell = current.addIndexTableCell(item.name, item.value, currentId, idx);
                        $(indexTableRow).append(indexTableCell);

                    }
                });
            });

            $(indexTableRow).append(current.addIndexTableActionCell(currentId));

            indexTable.append(indexTableRow);
        }

        this.addIndexTableCell = function (name, value, itemId, columnIdx) {
            var indexTableCell = $('<td data-boundto="' + name + '"></td>').html(current.setIndexTableCellContent(name, value, itemId));
            return indexTableCell;
        }

        this.setIndexTableCellContent = function (name, value, itemId) {

            if (value == null) {
                return '';
            }

            return value.toString();
        }

        this.addIndexTableActionCell = function (currentId) {

            var indexTableActionCell = $('<td></td>');

            var editLink = $(current.addIndexTableEditButton()).click(function () {
                current.editDataSourceItem(currentId);
            });

            var detailsLink = $(current.addIndexTableViewButton()).click(function () {
                current.viewDataSourceItem(currentId);
            });

            var deleteLink = $(current.addIndexTableDeleteButton()).click(function () {
                current.deleteDataSourceItem(currentId);
            });

            indexTableActionCell.append(editLink);

            indexTableActionCell.append(current.addIndexTableButtonSeparator());

            indexTableActionCell.append(detailsLink);

            indexTableActionCell.append(current.addIndexTableButtonSeparator());

            indexTableActionCell.append(deleteLink);

            return indexTableActionCell;
        }

        this.addIndexTableEditButton = function () {
            return $('<a data-toggle="modal" href="#' + current.getAbmFormId() + '">' + current.addIndexTableEditText() + '</a>');
        }

        this.addIndexTableViewButton = function () {
            return $('<a data-toggle="modal" href="#' + current.getAbmFormId() + '">' + current.addIndexTableViewText() + '</a>');
        }

        this.addIndexTableDeleteButton = function () {
            return $('<a href="#">' + current.addIndexTableDeleteText() + '</a>');
        }

        this.addIndexTableEditText = function () {
            return 'Edit';
        }

        this.addIndexTableViewText = function () {
            return 'View';
        }

        this.onModalShow = function () {
        }

        this.addIndexTableDeleteText = function () {
            return 'Delete';
        }

        this.addIndexTableButtonSeparator = function () {
            return $('<span> | </span>');
        }

        this.getJSONArrayFromDataSource = function (dataSourceArray) {

            if (dataSourceArray == undefined || dataSourceArray.length == 0) return '[]';
            var result = '[';

            $(dataSourceArray).each(function (idx, dataSourceItem) {
                var element = '{';
                $(dataSourceItem).each(function (fieldIdx, field) {
                    var size = Object.keys(field).length;
                    if (Array.isArray(dataSourceItem)) {
                        element += '"' + field.name + '":"' + field.value + '",';
                    } else {
                        if (size > 1) {
                            $(Object.keys(field)).each(function (j, key) {
                                element += '"' + key + '":"' + field[key] + '",';
                            });
                        }
                    }
                });

                element = element.substr(0, element.length - 1); //removes coma

                element += '}';
                result += element + ",";
            });

            result = result.substr(0, result.length - 1); //removes coma

            result += ']';
            return result;
        }

        this.changeInIndex = function (indexDataSourceItem) {

            var indexTable = this.getIndexForm();

            var currentId = indexDataSourceItem[0].value;

            var indexTableRow = $('tr[data-itemid="' + currentId + '"]', indexTable);

            $(indexDataSourceItem).each(function (idx, item) {

                $(indexTableRow).children('td[data-boundto="' + item.name + '"]').html(current.setIndexTableCellContent(item.name, item.value, currentId));

            });
        }

        this.editDataSourceItem = function (id) {

            var indexDataSourceItem = null;
            var indexCollection = 0;

            $(indexDataSource).each(function (i, elem) {

                if (elem[0].value == id) {
                    indexDataSourceItem = elem;

                    currentDataSourceItem = elem;
                    indexCollection = i;
                }

            });

            this.setAbmFields(indexDataSourceItem);
            current.setDataForChildCollection(id, indexCollection);
            current.afterInitEditItem(indexDataSourceItem);
        }

        this.afterInitEditItem = function (dataSourceItem) {

        }

        this.setDataForChildCollection = function (id, indexCollection) {

        }

        this.viewDataSourceItem = function (id) {
            var indexCollection = 0;
            var indexDataSourceItem = null;

            $(indexDataSource).each(function (i, elem) {

                if (elem[0].value == id) {
                    indexDataSourceItem = elem;
                    currentDataSourceItem = elem;
                    indexCollection = i;
                }

            });

            this.setAbmFields(indexDataSourceItem, true);
            this.afterInitDetailsItem(indexDataSourceItem);
            current.setDataForChildCollection(id, indexCollection);
        }

        this.afterInitDetailsItem = function (dataDetailsObject) {

        };

        this.deleteDataSourceItem = function (id) {

            var indexTableColumns = $('tr[data-itemid="' + id + '"]', this.getIndexForm());

            $(indexTableColumns).remove();

            var aux = [];

            $(indexDataSource).each(function (i, elem) {

                if (elem[0].value != id) {
                    aux.push(elem);
                }
            });

            indexDataSource = aux;
            current.onRemoveItem();
        }

        this.onRemoveItem = function () {

        };

        this.appendToSubmitForm = function () {

            $(indexDataSource).each(function (i, elem) {
                current.appendItemToSubmitForm(i, elem);
            });

        }

        this.cleanFields = function () {
            $('input:hidden .children-field').remove();
        }

        this.appendItemToSubmitForm = function (idx, item) {

            $(item).each(function (i, elem) {
                var hiddenField = null;
                if (elem != null && elem != undefined && Array.isArray(elem.value)) {
                    $(elem).each(function (j, subElement) {
                        current.appendItemToSubmitForm(j, subElement.value);
                    });
                } else {
                    hiddenField = current.createFieldToSubmit(idx, elem.name, elem.value);
                }

                if (hiddenField != null)
                    $('form').append(hiddenField);

            });

        }

        this.createFieldToSubmit = function (idx, name, value) {

            var hiddenField = $('<input type="hidden" class="children-field" />');
            hiddenField.attr('name', collectionName + '[' + idx + '].' + name);
            hiddenField.val(value);

            return hiddenField;
        }

        this.createIndex = function () {

            $.each(indexDataSource, function (idx, item) {

                current.addToIndex(item);

            });
        }

        this.ready(function () {

            var loadedItems = JSON.parse(collectionItems);

            $.each(loadedItems, function (idx, item) {

                var indexDataSourceItem = [];

                //insert a temporal id to identify the row
                var dataItemId = {};
                dataItemId.name = 'Id';
                dataItemId.value = (indexDataSource.length + 1) * (-1);
                indexDataSourceItem.push(dataItemId);

                current.generateIndexDataSourceItem(item, indexDataSourceItem, null, null);

                indexDataSource.push(indexDataSourceItem);
            });

            current.createIndex();

            current.getAbmCancelButton().click(function () {
                current.cancelAdd()
            });

            current.getAbmAcceptButton().click(function () {
                current.saveToCollection()
            });

            current.getAbmForm().on('show.bs.modal', function () {
                current.onModalShow();
            });

            current.getAbmForm().on('hide.bs.modal', function () {
                return !abortHide;
            });

            current.getAbmForm().on('hidden.bs.modal', function () {
                current.clearAbmFields();
                current.resetAbmFieldsStyle();
                current.clearAbmValidation();
            });

            current.clearAbmFields();

            var form = $('form');

            form.submit(function () {
                if ($(this).valid()) {
                    current.removeForm();
                    current.appendToSubmitForm();
                }


            });

        });

        this.removeForm = function () {
            $('#' + this.getOptions().abmFormId).remove();
        };

        this.generateIndexDataSourceItem = function (item, indexDataSourceItem, partialMemberName, index) {

            var keys = Object.keys(item);

            keys.forEach(function (key) {

                if (item[key] instanceof Array) //Children collection
                {
                    $.each(item[key], function (idx, subItem) {
                        current.generateIndexDataSourceItem(subItem, indexDataSourceItem, key, idx);
                    })
                }
                else if (item[key] instanceof Object) //related item
                {
                    current.generateIndexDataSourceItem(item[key], indexDataSourceItem, key, null);
                }
                else {
                    var dataItem = {};
                    if (partialMemberName != null) {
                        dataItem.name = partialMemberName;
                        if (index != null && index >= 0)
                            dataItem.name = dataItem.name + "[" + index.toString() + "]";
                        dataItem.name = dataItem.name + "." + key;
                    }
                    else {
                        dataItem.name = key;
                    }
                    dataItem.value = item[key];
                    indexDataSourceItem.push(dataItem);
                }
            });
        }

        this.getDataSourceItems = function () {
            return indexDataSource;
        }

        return this.each(function () {

            options = $.extend({}, defaultOptions, options);
            $.childrenManagerInstanceCreator(this, collectionName, collectionItems, options);

        });
    }

    $.childrenManagerInstanceCreator = function (target, collectionName, collectionItems, options) {
        return target.childrenManager || (target.childrenManager = new $.childrenManagerInstance(target, collectionName, collectionItems, options));
    }

    $.childrenManagerInstance = function (target, collectionName, collectionItems, options) {
        instanceId++;
    }

})(jQuery);