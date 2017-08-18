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
//    $.tablesorter.addWidget({
//        id: 'details',
//        format: function (table) {
//            $('tr[data-details] > td', table).dblclick(function (ev) {
//                window.open('/Reports/CallDetailList/' + $(this).parent().data('details'), '_blank', 'width=480,height=400,status=0,toolbar=0,scrollbars=1,location=0,directories=0,menubar=0');
//                e.stopImmediatePropagation();
//            });
//        }
//    });
    $.datepicker.setDefaults({
        dateFormat: options.dateFormat,
        yearRange: options.yearRange
    });

    $.noty.defaults = {
        layout: 'topRight',
        theme: 'defaultTheme',
        type: 'error',
        text: '',
        dismissQueue: true, // If you want to use queue feature set this true
        template: '<div class="noty_message"><span class="noty_text"></span><div class="noty_close"></div></div>',
        animation: {
            open: { height: 'toggle' },
            close: { height: 'toggle' },
            easing: 'swing',
            speed: 500 // opening & closing animation speed
        },
        timeout: 3000, // delay for closing event. Set false for sticky notifications
        force: false, // adds notification to the beginning of queue when set to true
        modal: false,
        closeWith: ['click'], // ['click', 'button', 'hover']
        callback: {
            onShow: function () { },
            afterShow: function () { },
            onClose: function () { },
            afterClose: function () { }
        },
        buttons: false // an array of buttons
    };

    $('.button, input[type="submit"], input[type="button"]').hover(
		function () { $(this).addClass('hover'); },
		function () { $(this).removeClass('hover'); }
    );

    // First use report

    var firstUsesTable = $('table#FirstUses');

    if (firstUsesTable.length) {
        firstUsesTable.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {
                0: { sorter: 'text' },
                3: { sorter: 'digit' },
                5: { sorter: 'text' }
            }
        });
        $('#StartDate, #EndDate').datepicker();
    }

    // Menu

    var menu = $('div#Menu');

    if (menu.length) {
        menu.find('a').hover(
		    function () { $(this).addClass('hover'); },
		    function () { $(this).removeClass('hover'); }
        );
    }

    // Pin management

    var pins = $('table#Pins');

    if (pins.length) {
        var updatePinRange = function () {
            var start = $('#StartSerial').val(),
                end = $('#EndSerial').val(),
                valid = true;

            if (start === '' || end === '') {
                $('#Quantity').val('');
                return;
            }
            if (parseInt(start) != start || parseInt(end) != end) {
                valid = false;
            }
            else {
                start = parseInt(start);
                end = parseInt(end);
                if (start > end) valid = false;
            }
            $('#Quantity').val(!valid ? 'INVALID RANGE' : end - start + 1);
        };

        pins.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {}
        });

        $('#StartSerial, #EndSerial').blur(updatePinRange).keyup(updatePinRange);
    }

    // Users management

    var users = $('table#Users');

    if (users.length) {
        users.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {
                5: { sorter: false }
            }
        });
    }

    // Vendor management

    var vendors = $('table#Vendors');

    if (vendors.length) {
        vendors.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {}
        });
    }
    var vendorPins = $('table#VendorPins');

    if (vendorPins.length) {
        vendorPins.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {}
        });
    }

    // Pin series

    var series = $('table#Series');

    if (series.length) {
        series.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {}
        });
        $('#StartDate, #EndDate').datepicker();
    }

    // Pin search

    var foundPins = $('table#FoundPins');

    if (foundPins.length) {
        foundPins.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {}
        });
        $('#StartDate, #EndDate').datepicker();
    }

    // Customer service

    var pinOps = $('table#PinOps');

    if (pinOps.length) {
        pinOps.add('body').bind('selectstart, mousedown', function () { return false; }).attr('unselectable', 'on').css({
            '-moz-user-select': 'none',
            '-khtml-user-select': 'none',
            '-webkit-user-select': 'none',
            'user-select': 'none'
        });

        pinOps.addClass('tablesorter').tablesorter({
            widgets: ['zebra', 'hover'],
            headers: {
                0: { sorter: 'text' },
                1: { sorter: 'text' },
                2: { sorter: 'text' },
                4: { sorter: 'text' },
                5: { sorter: 'text' },
                7: { sorter: 'digit' },
                8: { sorter: 'digit' },
                9: { sorter: 'text' },
                10: { sorter: 'text' }
            }
        });
        $('#StartDate, #EndDate').datepicker();
    }

    // Product Management
    var productMenu = $('div#ProductMenu');

    if (productMenu.length) {
        productMenu.find('a').hover(
		    function () { $(this).addClass('hover'); },
		    function () { $(this).removeClass('hover'); }
        );
    }
});

$.fn.disable = function () {
    return this.each(function () {
        if (typeof this.disabled != "undefined") this.disabled = true;
    });
}

$.fn.enable = function () {
    return this.each(function () {
        if (typeof this.disabled != "undefined") this.disabled = false;
    });
}

String.prototype.lpad = function (padString, length) {
    var str = this;
    while (str.length < length)
        str = padString + str;
    return str;
}

String.prototype.rpad = function (padString, length) {
    var str = this;
    while (str.length < length)
        str = str + padString;
    return str;
}

$.fn.toHtmlString = function () {
    return $('<div></div>').html($(this).clone()).html();
};

/* --------------------------------- Data Type Functions --------------------------------- */
function addTimeToDate (time, unit, objDate, dateReference) {
    var dateTemp = (dateReference) ? objDate : new Date(objDate);

    switch (unit) {
        case 'y': dateTemp.setFullYear(objDate.getFullYear() + time); break;
        case 'M': dateTemp.setMonth(objDate.getMonth() + time); break;
        case 'w': dateTemp.setTime(dateTemp.getTime() + (time * 7 * 24 * 60 * 60 * 1000)); break;
        case 'd': dateTemp.setTime(dateTemp.getTime() + (time * 24 * 60 * 60 * 1000)); break;
        case 'h': dateTemp.setTime(dateTemp.getTime() + (time * 60 * 60 * 1000)); break;
        case 'm': dateTemp.setTime(dateTemp.getTime() + (time * 60 * 1000)); break;
        case 's': dateTemp.setTime(dateTemp.getTime() + (time * 1000)); break;
        default: dateTemp.setTime(dateTemp.getTime() + time); break;
    }

    return dateTemp;
}