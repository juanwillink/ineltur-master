function openDetailsModal(lodgingId) {
    $("#lodgingDetailsId").val(lodgingId);
    var values = {
        "LodgingId": lodgingId
    }
    $.ajax({
        url: '../National/SearchLodgingInfoJson',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        beforeSend: function () {
            $("#UbicacionDiv").empty();
            $("#ServiciosDiv").empty();
            for (var i = 0; i < 5; i++) {
                $("#carousel-foto" + i).empty();
            }
            $("#lodging-details-modal").modal('show');
        },
        success: function (data) {
            debugger;
            buildDetailsTabs(data);
            if (data["Latitud"] == null || data["Longitud"] == null) {
                $("#supermap").hide();
            } else {
                initMap(data["Latitud"], data["Longitud"]);
            }

        },
        error: function (xhr) {
            alert("Error");
        }
    });

}

function buildDetailsTabs(data) {
    $("#lodging-details-modalLabel").text(data["LodgingName"]);
    $("#lodging-details-modal-location").text(data["LodgingLocation"] + " - " + data["LodgingCity"] + " - " + data["LodgingPhone"]);
    $("#UbicacionDiv").text(data["LodgingDescription"]);
    var serviciosBody = "";
    for (var key in data.LodgingServices) {
        var servicio = data.LodgingServices[key];
        if (key == "0") {
            serviciosBody = serviciosBody + servicio
        } else {
            serviciosBody = serviciosBody + " - " + servicio;
        }
    }
    $("#ServiciosDiv").append("<ul>" + serviciosBody + "</ul>");
    for (var i = 0; i < 5; i++) {
        var lodgingId = data["LodgingId"].toUpperCase()
        $("#carousel-foto" + i).append("<img src='../Content/Images/Hotels/" + lodgingId + "_" + (i + 1) + ".jpg' alt='...' onerror=this.style.display='none'>");
    }
    $("#PoliticasDiv").append(data["LodgingCancelationPolitic"]);
    $('a[href="#Servicios"]').trigger('click');
    $('a[href="#Ubicacion"]').trigger('click');
}

function GetHotelRoomsInfo() {
    var lodgingId = $("#lodgingDetailsId").val();
    var values = {
        "LodgingId": lodgingId
    };
    $.ajax({
        url: '../Login/SearchLodgingWeeklyPrices',
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        beforeSend: function () {
            $("#LoadingModal").show();
        },
        success: function (data) {
            $("#LoadingModal").fadeOut('fast', function () {
                BuildRoomsInfo(data);
            });
        },
        error: function (xhr) {
            alert("Error");
        }
    });
}

function BuildRoomsInfo(data) {
    debugger;
    var container = $("#UnitsInfoDiv");
    container.empty();
    var body = ""
    for (var key in data.Units) {
        var unit = data.Units[key];
        var description = replaceEnters(unit["Description"], "<br>");
        body = body +
                        "<h2>" + unit["NombreUnidad"] + "</h2>" +
                        "<p>" + description + "</p>";                     
    }
    container.append(body);
}

function replaceEnters(str, strEnter) {
    var replacedString = str.replace(/(?:\r\n|\r|\n)/g, strEnter);
    return replacedString;
}