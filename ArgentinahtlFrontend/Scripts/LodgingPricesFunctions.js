function verTarifasHotel(lodgingId, lodgingName) {
    debugger;
    $("#lodgingId").val(lodgingId);
    $("#lodging-prices-modalLabel").text("Tarifas para el hotel " + lodgingName)
    if ($("#checkinDate").val() != undefined) {
        var date = $("#checkinDate").val().split("-");
        var newDate = date[2] + "/" + date[1] + "/" + date[0];
        var values = {
            "LodgingId": lodgingId,
            "dateString": newDate
        }
    } else {
        var values = {
            "LodgingId": lodgingId
        };
    }
    
    $.ajax({
        url: '../Login/SearchLodgingWeeklyPrices',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        beforeSend: function () {
            debugger;
            $("#PricesTable").empty();
            $("#lodging-prices-modal").modal('show');
            $("#spinnerModal").show();
        },
        success: function (data) {
            debugger;
            $("#spinnerModal").fadeOut('fast', function () {
                buildDatesTable(data);
            });
        },
        error: function (xhr) {
            alert("Error");
        }
    });
}

function buildDatesTable(data) {
    var table = $("#PricesTable");
    var body = "";
    var colspan = 0;
    for (var i in data.Units) {
        var unit = data.Units[0];
        colspan = unit.Quota.length;
    }
    debugger;
    for (var key in data.Units) {
        var unit = data.Units[key];
        body = body + "<thead><tr style='background-color: #00125A; color: white;'><th colspan=" + colspan + " style='text-align:center; border-radius: 6px 6px 0px 0px;'>" + unit["NombreUnidad"] + "</th></tr></thead><tr style='background-color: #00125A; color: white;'>";
        for (var key3 in unit.Quota) {

            var quota2 = unit.Quota[key3];
            debugger;
            var src = quota2["Fecha"];
            src = src.replace(/[^0-9 +]/g, '');
            var date = new Date(parseInt(src));
            var day = date.getDate();
            var month = date.getMonth() + 1;
            var year = date.getFullYear();
            if (key3 == 0) {
                $("#firstWeekDay").val(month + "/" + day + "/" + year);
            }
            body = body + "<td>" + day + "/" + month + "/" + year + "</td>";
        }
        body = body + "</tr><tr>";
        for (var key2 in unit.Quota) {
            var quota = unit.Quota[key2];
            body = body + "<td>$" + quota["Monto"] + "</td>";
        }
        body = body + "</tr>";
    }
    table.append(body);
    table.fadeIn('fast');
}

function oneMoreWeek() {

    var table = $("#PricesTable");
    table.empty();
    lodgingId = $("#lodgingId").val();
    oldDate = $("#firstWeekDay").val();
    var parts = oldDate.split('/');
    var date = new Date(parts[2], parts[0] - 1, parts[1]);
    date.setDate(date.getDate() + 7);
    var newDate = date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear();
    var values = {
        "lodgingId": lodgingId,
        "dateString": newDate
    };
    $.ajax({
        url: '../Login/SearchLodgingWeeklyPrices',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        success: function (data) {
                buildDatesTable(data);
        },
        error: function (xhr) {
            alert("Error");
        }
    });
}

function oneLessWeek() {
    debugger;
    var table = $("#PricesTable");
    table.empty();
    lodgingId = $("#lodgingId").val();
    oldDate = $("#firstWeekDay").val();
    var parts = oldDate.split('/');
    var date = new Date(parts[2], parts[0] - 1, parts[1]);
    date.setDate(date.getDate() - 7);
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    var newDate = day + "/" + month + "/" + year;
    var values = {
        "lodgingId": lodgingId,
        "dateString": newDate
    };
    $.ajax({
        url: '../Login/SearchLodgingWeeklyPrices',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        success: function (data) {
                buildDatesTable(data);
        },
        error: function (xhr) {
            alert("Error");
        }
    });
}

function searchSpecificDate() {
    var table = $("#PricesTable");
    table.fadeOut('fast', function () {
        table.empty();
        $("#spinnerModal").fadeIn('fast');
    });
    lodgingId = $("#lodgingId").val();
    newDate = $("#searchDate").val();
    var values = {
        "lodgingId": lodgingId,
        "dateString": newDate
    };
    $.ajax({
        url: '../Login/SearchLodgingWeeklyPrices',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        success: function (data) {
            $("#spinnerModal").fadeOut('fast', function () {
                buildDatesTable(data);
            });
        },
        error: function (xhr) {
            alert("Error");
        }
    });
}

$('#lodging-prices-modal').on('hidden.bs.modal', function () {
    debugger;
    var table = $("#PricesTable");
    table.empty();
})