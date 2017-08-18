$(function () {
    var options = {
        dateFormat: 'yy-mm-dd',
        yearRange: '-1:+0'
    };

    // Setup

    $.tablesorter.addWidget({
        id: 'hover',
        format: function (table) {
            $('tr', table).hover(
		        function () { $(this).addClass('hover'); },
		        function () { $(this).removeClass('hover'); }
            );
        }
    });
    $.datepicker.setDefaults({
        dateFormat: options.dateFormat,
        yearRange: options.yearRange
    });

    $('.button, input[type="submit"], input[type="button"]').hover(
		function () { $(this).addClass('hover'); },
		function () { $(this).removeClass('hover'); }
    );

    $('.datepicker').datepicker();

    // Menu

    var menu = $('div#Menu');

    if (menu.length) {
        menu.find('a').hover(
		    function () { $(this).addClass('hover'); },
		    function () { $(this).removeClass('hover'); }
        );
    }

    // Cuentas

    var cuentas = $('table#Cuentas');

    if (cuentas.length) {
        cuentas.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover', 'details'],
            headers: {
                0: { sorter: 'text' },
                1: { sorter: 'text' },
                2: { sorter: 'text' },
                3: { sorter: 'digit' },
                4: { sorter: false }
            }
        });
    }

    // Detalles Cuenta

    var detalles = $('table#Detalles');

    if (detalles.length) {
        detalles.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {
                0: { sorter: 'text' },
                1: { sorter: 'text' },
                2: { sorter: 'text' },
                3: { sorter: 'text' },
                4: { sorter: 'text' }
            }
        });
    }

    // Pago

    var pago = $('table#Pago');

    if (pago.length) {
        $('#Moneda').change(function () {
            var $this = $(this);

            $.ajax({
                dataType: 'text',
                url: '../ObtenerCambio/' + $this.val(),
                success: function (data, status, xhr) {
                    data = eval('(' + data + ')');
                    $('#TipoCambio').val(data.Cambio);
                }
            });

        });
    }
});