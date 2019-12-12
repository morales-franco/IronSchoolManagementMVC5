/************* Global Variable Section *************/
var _muestra = null;


/************* Init Section *************/
function MuestraJS() {
    this.Constant = {};
    this.Resources = {};
    this.Url = {};

    this.Constant.Readonly = false; 
}

function init() {
    setCustomConfigurationProductoChildrenManager();
    setCustomConfigurationLogoChildrenManager();
    setCustomConfigurationProcesoProductivoChildrenManager();
    setCustomConfigurationTareaDeMaquinaChildrenManager();

    setEventsMuestraProducto();
    setEventsCalculoTotales();
}

/************* Init Section - Muestra Producto *************/
function setEventsMuestraProducto() {
    $("#muestra-producto-edit-id").on("change", ".browser-id", onChangeProducto);
}

function setCustomConfigurationProductoChildrenManager() {
    muestraProductoManager.getControls = function () {
        var controlsToValidate = $(muestraProductoManager.getControlsSelector(), muestraProductoManager.getAbmForm());
        return controlsToValidate;
    }

    muestraProductoManager.clearAbmFieldsChildCollection = function () {
        //clear child grid
        $('#corte-producto-container-id #corte-producto-table-id tbody tr').remove();
    }

    muestraProductoManager.getChildCollectionData = function () {
        
        if (corteProductoManager == null)
            return;

        var cortes = corteProductoManager.getIndexDataSource();

        if (cortes == null || cortes.length == 0)
            return;

        var indexDataSourceItem = [];
        $.each(cortes, function (idx, corte) {

            $.each(corte, function (idxProperty, corteProperty) {
                var dataProperty = {};
                dataProperty.name = "CorteProducto[" + idx + "]." + corteProperty.name;
                dataProperty.value = corteProperty.value;
                indexDataSourceItem.push(dataProperty);

            })

        })

        return indexDataSourceItem;
    }

    muestraProductoManager.setDataForChildCollection = function (id, indexCollection) {
        var corteDataSource = [];
        var lastIndex = null;
        var corteDataItemSource = [];
        var index = "";

        $(muestraProductoManager.getCurrentDataSourceItem()).each(function (idx, element) {

            if (element.name.indexOf("CorteProducto[") >= 0) {
                //Child collection
                index = element.name.substring(element.name.indexOf("CorteProducto[") + 14);
                index = index.substring(0, index.indexOf("]"));

                if (lastIndex != null && index != lastIndex) {
                    corteDataSource.push(corteDataItemSource);
                    corteDataItemSource = [];
                }

                var name = element.name.substring(element.name.indexOf("].") + 2);
                var value = element.value;

                var dataProperty = {};
                dataProperty.name = name;
                dataProperty.value = value;
                corteDataItemSource.push(dataProperty);
                lastIndex = index;
            }
        });

        if (lastIndex != null) {
            corteDataSource.push(corteDataItemSource);
            corteDataItemSource = [];
        }

        initCorteProductoManager(indexCollection, corteDataSource != null ? muestraProductoManager.getJSONArrayFromDataSource(corteDataSource) : '[]');
    }

    muestraProductoManager.saveToCollection = function () {
        if (muestraProductoManager.validateAbm() && validateItemRepetead('#muestra-producto-edit-id #ProductoId', muestraProductoManager, 'ProductoId')) {
            beforeAddProductToCollection();
            muestraProductoManager.addToCollection();
            muestraProductoManager.setAbortHide(false);
            afterAddProductToCollection();
        } else {
            muestraProductoManager.setAbortHide(true);
        }
    }

    muestraProductoManager.afterInitEditItem = function (dataSourceItem) {
        var admiteCorteIndex = getPropertyIndex(dataSourceItem, "AdmiteCorte");
        var admiteCorte = convertToBoolean(dataSourceItem[admiteCorteIndex].value);
        var multiplicarPorCantidad = convertToBoolean(getPropertyIndex(dataSourceItem, "MultiplicarPorCantidad"));

        if (admiteCorte)
            enableCortes();
        else
            disableCortes();
    }

    muestraProductoManager.onRemoveItem = function (dataObjectDeleted) {
        calculateSubTotalMuestra('muestra-producto-table-id', 'SubTotal', 'SubTotalProductoMuestra');
        calculateCostoMateriaPrima();
    }

    muestraProductoManager.afterInitDetailsItem = function (dataSourceItem) {
        var admiteCorteIndex = getPropertyIndex(dataSourceItem, "AdmiteCorte");
        var admiteCorte = convertToBoolean(dataSourceItem[admiteCorteIndex].value);

        if (admiteCorte)
            enableCortes();
        else
            disableCortes();
    }

}

function beforeAddProductToCollection() {
    calculateSubTotalItem('#muestra-producto-edit-id #Cantidad', '#muestra-producto-edit-id #Costo', '#muestra-producto-edit-id #SubTotal');
}

function afterAddProductToCollection() {
    calculateSubTotalMuestra('muestra-producto-table-id', 'SubTotal', 'SubTotalProductoMuestra');
    calculateCostoMateriaPrima();
    disableCortes();
}

function initCorteProductoManager(index, dataSource) {

    if (index < 0) {
        //Alta
        dataSource = '[]';
        var cortesCount = $('#corte-producto-table-id tbody tr').length;

        if (cortesCount <= 1) {
            var lastMuestraProductoRow = $('#muestra-producto-table-id tbody tr:last')[0];

            if (typeof lastMuestraProductoRow == "undefined") {
                index = 0;
            } else {
                index = $(lastMuestraProductoRow).attr('data-itemid') * (-1);
            }
        }

    }

    //Remove old event's click --> corte producto is a dynamic children manager then generate dynamic events 
    $('#corte-producto-edit-id').find('[data-childaction="accept"]').off('click');
    corteProductoManager = $('#corte-producto-container-id').childrenManager('MuestraProducto[' + index + '].CorteProducto', dataSource, { indexFormId: 'corte-producto-table-id', abmFormId: 'corte-producto-edit-id' });
    configureChildrenManagerStyle(corteProductoManager);
    setCustomConfigurationCorteProductoManager(corteProductoManager);

    if (_muestra.Constant.Readonly)
        disableChildrenManager(corteProductoManager);
}

function setCustomConfigurationCorteProductoManager(corteProductoManager) {
    corteProductoManager.saveToCollection = function () {
        if (corteProductoManager.validateAbm()) {
            corteProductoManager.addToCollection();
            corteProductoManager.setAbortHide(false);
            updateCantidadProducto();
        } else {
            corteProductoManager.setAbortHide(true);
        }
    }

    corteProductoManager.onRemoveItem = function () {
        updateCantidadProducto();
    }
}

function updateCantidadProducto() {
    var multipicaPorCantidad = convertToBoolean($('#muestra-producto-edit-id #MultiplicarPorCantidad').val());
    var items = corteProductoManager.getDataSourceItems();

    var cantidadAux = 0;
    $.each(items, function (index, itemProperties) {
        var cantidadIndex = getPropertyIndex(itemProperties, "Cantidad");
        var consumoIndex = getPropertyIndex(itemProperties, "Consumo");

        var cantidad = stringToDecimal(itemProperties[cantidadIndex].value, 2);
        var consumo = stringToDecimal(itemProperties[consumoIndex].value, 2);

        if (multipicaPorCantidad)
            cantidadAux = cantidadAux + (cantidad * consumo);
        else
            cantidadAux = cantidadAux + consumo 
    });

    $('#muestra-producto-edit-id #Cantidad').val(decimalToString(cantidadAux));

}

function onChangeProducto() {
    var productoId = $('#muestra-producto-edit-id #ProductoCodigo').val();

    $.ajax({
        url: _muestra.Url.GetByCodeProducto,
        type: "get",
        dataType: "json",
        traditional: true,
        async: true,
        data: { 'code': productoId },
        success: function (product) {
            if (!admiteCorte(product))
                disableCortes();
            else
                enableCortes();

            setDataProduct(product);

            return false;
        },
        error: function (jqXHR, textStatus, errorThrown) {
            notify('Se produjo un error al buscar el producto', 'danger');
            return false;
        }
    });

}

function setDataProduct(product) {
    $('#muestra-producto-edit-id #MultiplicarPorCantidad').val(product.multiplicarPorCantidad);
    $('#muestra-producto-edit-id #AdmiteCorte').val(admiteCorte(product));
    $('#muestra-producto-edit-id #Costo').val(product.precio);
}

function disableCortes() {
    $('#muestra-producto-edit-id #Cantidad').attr('readonly', false);
    $('#corte-producto-container-id').hide();
}

function enableCortes() {
    $('#muestra-producto-edit-id #Cantidad').attr('readonly', true);
    $('#corte-producto-container-id').show();
    
}

function admiteCorte(product) {

    if (product.tipoDeCorteId == null)
        return false;

    if (product.tipoDeCorteId == _muestra.Constant.TipoCorteNoSeCortaId)
        return false;

    return true;
}


/************* Init Section - Muestra Logo *************/

function setCustomConfigurationLogoChildrenManager() {
    muestraLogoManager.saveToCollection = function () {

        if (muestraLogoManager.validateAbm() && validateItemRepetead('#muestra-logo-edit-id #LogoId', muestraLogoManager, 'LogoId')) {
            beforeAddLogoToCollection();
            muestraLogoManager.addToCollection();
            muestraLogoManager.setAbortHide(false);
            afterAddLogoToCollection();
        } else {
            muestraLogoManager.setAbortHide(true);
        }
    }

    muestraLogoManager.onRemoveItem = function (dataObjectDeleted) {
        calculateSubTotalMuestra('muestra-logo-table-id', 'SubTotal', 'SubTotalMuestraLogo');
        calculateCostoMateriaPrima();
    }
}

function browserLogoItemSelected(logoId, nombre, costo, esLogoGenerico) {
    $('#muestra-logo-edit-id #LogoId').val(logoId);
    $('#muestra-logo-edit-id #LogoNombre').val(nombre);
    $("#browserContent").data("kendoWindow").close();
    $("#browserContent").empty();
}

function beforeAddLogoToCollection() {
    calculateSubTotalItem('#muestra-logo-edit-id #Cantidad', '#muestra-logo-edit-id #Costo', '#muestra-logo-edit-id #SubTotal');
}

function afterAddLogoToCollection() {
    calculateSubTotalMuestra('muestra-logo-table-id', 'SubTotal', 'SubTotalMuestraLogo');
    calculateCostoMateriaPrima();
}

/************* Init Section - Muestra Proceso Productivo *************/

function setCustomConfigurationProcesoProductivoChildrenManager() {
    muestraProcesoProductivoManager.saveToCollection = function () {

        if (muestraProcesoProductivoManager.validateAbm() && validateItemRepetead('#muestra-proceso-productivo-edit-id #ProcesoProductivoId', muestraProcesoProductivoManager, 'ProcesoProductivoId')) {
            beforeAddProcesoProductivoToCollection();
            muestraProcesoProductivoManager.addToCollection();
            muestraProcesoProductivoManager.setAbortHide(false);
            afterAddProcesoProductivoToCollection();
        } else {
            muestraProcesoProductivoManager.setAbortHide(true);
        }
    }
}

function browserProcesoProductivoItemSelected(procesoProductivoId, descripcion, costo, requeridoEnProduccion) {
    $('#muestra-proceso-productivo-edit-id #ProcesoProductivoId').val(procesoProductivoId);
    $('#muestra-proceso-productivo-edit-id #ProcesoProductivoDescripcion').val(descripcion);
    $("#browserContent").data("kendoWindow").close();
    $("#browserContent").empty();
}

function beforeAddProcesoProductivoToCollection() {
    calculateSubTotalItem('#muestra-proceso-productivo-edit-id #Cantidad', '#muestra-proceso-productivo-edit-id #Costo', '#muestra-proceso-productivo-edit-id #SubTotal');
}

function afterAddProcesoProductivoToCollection() {
    calculateSubTotalMuestra('muestra-proceso-productivo-table-id', 'SubTotal', 'SubTotalMuestraProcesoProductivo');
}


/************* Init Section - Muestra Tareas De Maquina *************/
function setCustomConfigurationTareaDeMaquinaChildrenManager() {
    muestraTareaDeMaquinaManager.saveToCollection = function () {

        if (muestraTareaDeMaquinaManager.validateAbm() && validateItemRepetead('#muestra-tarea-maquina-edit-id #TareaDeMaquinaId', muestraTareaDeMaquinaManager, 'TareaDeMaquinaId')) {
            muestraTareaDeMaquinaManager.addToCollection();
            muestraTareaDeMaquinaManager.setAbortHide(false);
            afterAddTareaDeMaquinaToCollection();
        } else {
            muestraTareaDeMaquinaManager.setAbortHide(true);
        }
    }

    muestraTareaDeMaquinaManager.onRemoveItem = function (dataObjectDeleted) {
        calculateSubTotalMuestra('muestra-tarea-maquina-table-id', 'Tiempo', 'SubTotalMuestraTareaDeMaquina');
        calculateTiempoCostura();
    }
}

function browserTareasDeMaquinaItemSelected(tareaDeMaquinaId, descripcion) {
    $('#muestra-tarea-maquina-edit-id #TareaDeMaquinaId').val(tareaDeMaquinaId);
    $('#muestra-tarea-maquina-edit-id #TareaDeMaquinaDescripcion').val(descripcion);
    $("#browserContent").data("kendoWindow").close();
    $("#browserContent").empty();
}


function afterAddTareaDeMaquinaToCollection() {
    calculateSubTotalMuestra('muestra-tarea-maquina-table-id', 'Tiempo', 'SubTotalMuestraTareaDeMaquina');
    calculateTiempoCostura();
}


/************* Init Section - Common Functions *************/

function calculateSubTotalItem(cantidadInputId, costoInputId, subTotalInputId) {
    //Calcula Subtotal del Producto - Logo - Proceso Productivo que se esta generando o editando --> a nivel row 
    var cantidad = $(cantidadInputId).val();
    var costo = $(costoInputId).val();
    var subtotal = stringToDecimal(cantidad, 2) * stringToDecimal(costo, 2);
    $(subTotalInputId).val(decimalToString(subtotal));
}

function calculateSubTotalMuestra(tableId, columnId, subtotalId) {
    //Calcula Subtotal de los Productos - Logos --> a nivel grupo
    var total = 0;
    $.each($('#' + tableId + ' tbody tr td[data-boundto="' + columnId + '"]'), function (index, cell) {
        total += stringToDecimal($(cell).text(), 2);
    });

    $('#' + subtotalId).val(decimalToString(total));
}

function validateItemRepetead(currentItemDomId, manager, keyPropertyName) {
    var isRepetead = false;
    var currentItemId = $(currentItemDomId).val();
    var items = manager.getDataSourceItems();
    var itemEdit = manager.getCurrentDataSourceItem();
    if (items.length == 0)
        return !isRepetead;


    var itemKeyIndex = getPropertyIndex(items[0], keyPropertyName);

    if (itemEdit == null) {
        //Alta
        isRepetead = items.filter(function (item) {
            var itemAddedId = item[itemKeyIndex].value;
            return currentItemId == itemAddedId
        }).length > 0;
    } else {
        //Edit
        if (itemEdit[itemKeyIndex].value != currentItemId) {
            //Se modifico el Item, tenemos que validar repetidos
            isRepetead = items.filter(function (item) {
                var itemAddedId = item[itemKeyIndex].value;
                return currentItemId == itemAddedId
            }).length > 0;
        }
    }

    if (isRepetead)
        notify("El item seleccionado ya ha sido ingresado", 'danger');

    //If not repetead --> is valid
    return !isRepetead;
}

function getPropertyIndex(propertiesArray, keyName) {
    var indexObject = propertiesArray.find(function (item) {
        return item.name == keyName;
    })

    return propertiesArray.indexOf(indexObject);


}

/************* Init Section - Totales Functions *************/

function calculateTiempoCostura() {
    var total = 0;
    $.each($('#muestra-tarea-maquina-table-id tbody tr td[data-boundto="Tiempo"]'), function (index, cell) {
        total += stringToDecimal($(cell).text(), 0);
    });

    $('#TiempoCostura').val(parseInt(total));
    $('#TiempoCostura').trigger('change');
}

function calculateCostoMateriaPrima() {
    var subTotalProductoMuestra = stringToDecimal($('#SubTotalProductoMuestra').val(), 2);
    var subTotalMuestraLogo = stringToDecimal($('#SubTotalMuestraLogo').val(), 2);

    var total = subTotalProductoMuestra + subTotalMuestraLogo;
    $('#CostoMateriaPrima').val(decimalToString(total));
    $('#CostoMateriaPrima').trigger('change');
}

function calculateProduccionMensual() {
    var tiempoCostura = stringToDecimal($('#TiempoCostura').val(), 0);
    var capacidadDeProduccion = stringToDecimal($('#CapacidadDeProduccion').val(), 0);

    if (tiempoCostura == 0) {
        $('#ProduccionMensual').val(0);
        return;
    }

    var produccionMensual = capacidadDeProduccion / tiempoCostura;
    //integer division
    produccionMensual = Math.floor(produccionMensual);
    $('#ProduccionMensual').val(produccionMensual);
}

function calculateCostoFabricacion() {
    var tiempoCostura = stringToDecimal($('#TiempoCostura').val(), 0);
    var capacidadDeProduccion = stringToDecimal($('#CapacidadDeProduccion').val(), 2);
    var gastoFijo = stringToDecimal($('#GastoFijo').val(), 2);

    if (capacidadDeProduccion == 0) {
        $('#CostoFabricacion').val(0);
        return;
    }

    var costoFabricacion = (gastoFijo / capacidadDeProduccion) * tiempoCostura;
    $('#CostoFabricacion').val(decimalToString(costoFabricacion));
    $('#CostoFabricacion').trigger('change');
}

function calculateCostoBruto() {
    var costoMateriaPrima = stringToDecimal($('#CostoMateriaPrima').val(), 2);
    var costoFabricacion = stringToDecimal($('#CostoFabricacion').val(), 2);
    var costoBruto = costoMateriaPrima + costoFabricacion;
    $('#CostoBruto').val(decimalToString(costoBruto));
    $('#CostoBruto').trigger('change');
}

function calculatePrecioCalculado() {
    var indiceVenta = stringToDecimal($('#IndiceVenta').val(), 2);
    var costoBruto = stringToDecimal($('#CostoBruto').val(), 2);
    var precioCalculado = indiceVenta * costoBruto;

    $('#PrecioCalculado').val(decimalToString(precioCalculado));
}


function setEventsCalculoTotales() {
    $("#CostoMateriaPrima,#CostoFabricacion").on("change", calculateCostoBruto);
    $("#TiempoCostura").on("change", calculateProduccionMensual);
    $("#TiempoCostura").on("change", calculateCostoFabricacion);
    $("#CostoBruto").on("change", calculatePrecioCalculado);
}