// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// TIPOS DE ALERTAS
const TypeAlert = Object.freeze({
    success: "success",
    danger: "danger",
    warning: "warning",
    info: "info"
});
function AppShowAlert(typeAlert, content, width = 600) {

    var type = "info";

    switch (typeAlert) {
        case "success":
            type = "success";
            break;

        case "danger":
            type = "danger";
            break;

        case "warning":
            type = "warning";
            break;

        default:
            break;
    }

    if (width == undefined || width.length > 0) {
        width = 600
    }

    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;

    const idDiv = `AppAlert-${Math.floor(Math.random() * 1000)}`;

    $('#alert-position').append(`<div id="${idDiv}" class="alert alert-${type} appAlert" role="alert" style="width:${width}px; animation:aparecer 0.5s; scroll-behavior:smooth;">

        <button type = "button" class="btn-close" style="float: right;" data-bs-dismiss="alert" aria-label="Close"></button >

            ${content}

     </div > `)


    let ejecutado = false;
    setTimeout(function () {
        if (!ejecutado) {
            $(`#${idDiv}`).remove();
            ejecutado = true;
        }
    }, 20000);

}

// Función para abrir un modal con contenido dinámico
function OpenModal(encabezado, contenido) {
    // Asigna el contenido y el encabezado al modal
    $('#AppModalLabel').text(encabezado);
    $('#AppModalBody').html(contenido);

    // Abre el modal sin permitir cierre por clic fuera o tecla Esc
    $('#AppModal').modal({ backdrop: 'static', keyboard: false });
    $('#AppModal').modal('show');
}

function CloseModal() {

    setInterval(function () {

        // Limpia el contenido del modal
        //$('#AppModalLabel').text('');
        //$('#AppModalBody').html('');
        // Cierra el modal
        $('#AppModal').modal('hide');

    }, 1000);


}

function obtenerDatosFormulario(idForm) {

    const formulario = document.getElementById(idForm);

    if (formulario == undefined) {
        console.log("No se encontro formulario con el id", idForm)
    }

    const datos = {};

    for (let i = 0; i < formulario.elements.length; i++) {
        const elemento = formulario.elements[i];
        if (["INPUT", "SELECT"].includes(elemento.tagName)) {
            datos[elemento.name] = elemento.value;
        }
    }

    return datos;
}

function GenerateDataTable(urlGetElementos, idTable, idFormFiltro, columnsData, rowCallbackData, orderData = null, lengthMenuData = null) {

    var formData = obtenerDatosFormulario(idFormFiltro);
    var order2 = orderData ?? [[0, 'desc']];
    var lengthMenuData2 = lengthMenuData ?? [10, 25, 50, 100, -1]

    var $tb_rol = $(`table#${idTable}`);
    var table = $tb_rol.DataTable({
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.5/i18n/es-ES.json',
            "emptyTable": "No hay información",
            "processing": "Procesando..."
        },
        searching: false,
        "lengthMenu": lengthMenuData2,
        scrollY: 410,
        deferRender: true,
        scroller: true,
        ajax: {
            url: urlGetElementos,
            type: 'GET',
            processing: true,
            serverSide: true,
            dataSrc: "",
            data: function (d) {

                Object.keys(formData).forEach(propiedad => {
                    d[propiedad] = formData[propiedad];
                });

            },
            error: function (err) {
                // FUNCION ALERT ERROR.
                console.error(err);
                AppShowAlert(TypeAlert.danger, "Se presento un error, vuelve y cargue la pagina", 600);
            },
            complete: function (xhr, status) {

            }
        },
        columns: columnsData,
        dom: 'Bfrtip',
        select: true,
        order: order2,
        rowCallback: rowCallbackData,
        "drawCallback": function (settings) {

            $('.dt-buttons').css('float', 'right');

            const option = lengthMenuData2;
            var optionHtml = "";
            const pageLen = table.page.info().length;
            for (var i = 0; i < option.length; i++) {
                var elemen = "";
                if (option[i] == -1) {
                    elemen = `<option selected value="${option[i]}">Todos</option> `;
                } else {
                    elemen = `<option selected value="${option[i]}">${option[i]}</option> `;
                }

                if (option[i] != pageLen) {
                    elemen = elemen.replace("selected", '');
                }

                optionHtml += elemen
            }

            const idSelect = `select-${idTable}`;
            $(".dataTables_info").append(`
                <select style="float: right;" id="${idSelect}">
                    ${optionHtml}
                </select>`);
            $('#' + idSelect).change(function () {

                var length = $(this).val();
                $tb_rol.DataTable().page.len(length).draw();

            });

        },
        buttons: [
            {
                text: 'Buscar',
                className: 'btn btn-default',
                "attr": {
                    "id": "btn_Search_Dt"
                },
                action: function (e, dt, node, config) {
                    formData = obtenerDatosFormulario(idFormFiltro);
                    dt.ajax.reload(null, false);
                }
            },
            {
                text: 'Csv',
                className: 'btn btn-primary',
                "attr": {
                    "id": "btn_Csv"
                },
                action: function (e, dt, node, config) {
                    GenerateCSV(urlGetElementos, idFormFiltro);
                }
            }
        ]
    });

    return table;

}

function password_show_hide() {
    var x = document.getElementById("id-password");
    var show_eye = document.getElementById("show_eye");
    var hide_eye = document.getElementById("hide_eye");
    hide_eye.classList.remove("d-none");
    if (x.type === "password") {
        x.type = "text";
        show_eye.style.display = "none";
        hide_eye.style.display = "block";
    } else {
        x.type = "password";
        show_eye.style.display = "block";
        hide_eye.style.display = "none";
    }
}

function mayus(e) {
    e.value = e.value.toUpperCase();
}

function AppConfirm(url) {
    const text = "Seguro que quiere ejecutar esta acción";
    if (confirm(text) == true) {
        window.location.href = url;
    }
}

function GenerateCSV(url, idFilterForm) {
    var formData = obtenerDatosFormulario(idFilterForm);
    formData["IsReporteCsv"] = true;

    const contenido = `<div class="text-center">
                <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>`
    OpenModal("Generando archivo csv", contenido);

    $.ajax({
        type: 'GET',
        url: url,
        data: formData,
        beforeSend: function () {

        },
        success: function (response) {

            var blob = new Blob([response], { type: 'application/octet-stream' });
            // Crea un enlace temporal para la descarga del archivo
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = document.title + ".csv";
            link.click();

            // Elimina el enlace temporal
            window.URL.revokeObjectURL(link.href);

        },
        error: function (xhr, status, error) {
            // Acción a realizar en caso de error
            console.error('Error en la petición:', status, error);
            AppShowAlert(TypeAlert.danger, "Se presento un error, vuelve y cargue la pagina", 600);
        },
        complete: function () {

            CloseModal();

        }
    });
}