function searchHotels() {
    if ($("#destinationName").val() == "" && $("#hotelName").val() == "") {
        alert("Debe ingresar algun destino o algun hotel.");
    }
    else {
        debugger;
        var dateParts = $("#checkinDate").val().split("-");
        var checkinDate = new Date(dateParts[0], (dateParts[1] - 1), dateParts[2]);
        dateParts = $("#checkoutDate").val().split("-");
        var checkoutDate = new Date(dateParts[0], (dateParts[1] - 1), dateParts[2]);
        var destinationId = null;
        if ($("#destinationName").val() != "") {
            destinationId = $("#destinationIdSearch").val();
        }
        
        if ($("#roomTypeFilter").val() != "") {
            var room = {
                "RoomTypeCode": $("#roomTypeFilter").val()
            };

            var rooms = [room];

            var values = {
                "LodgingName": $("#hotelName").val(),
                "DestinationId": destinationId,
                "Checkin": checkinDate,
                "Checkout": checkoutDate,
                "Order": $("#orderFilter").val(),
                "Nationality": $("#nationalityFilter").val(),
                "DisplayType": $("#displaytypeFilter").val(),
                "Tarifa": $("#tarifaFilter").val(),
                "Breakfast": $("#breakfastFilter").val(),
                "Rooms": rooms
            };
        } else {
            var values = {
                "LodgingName": $("#hotelName").val(),
                "DestinationId": destinationId,
                "Checkin": checkinDate,
                "Checkout": checkoutDate,
                "Order": $("#orderFilter").val(),
                "Nationality": $("#nationalityFilter").val(),
                "Tarifa": $("#tarifaFilter").val(),
                "Breakfast": $("#breakfastFilter").val(),
                "DisplayType": $("#displaytypeFilter").val()
            };
        }
        var userKey = $("#currentUserKey").val();
        $.ajax({
            url: "../Login/SearchHotels",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            data: JSON.stringify(values),
            beforeSend: function () {
                startLoadingGif();
                shrinkSearchBox();
            },
            success: function (data) {
                buildLodgingsView(data, userKey)
            },
            error: function () {
            }
        });
    }
}

function searchMoreHotels() {
    if ($("#destinationName").val() == "") {
        alert("Debe ingresar algun destino");
    }
    else{
        var values = {
            "DestinationId": $("#destinationIdSearch").val(),
            "Order": $("#orderFilter").val(),
            "Nationality": $("#nationalityFilter").val(),
            "DisplayType": $("#displaytypeFilter").val()
        };
        $.ajax({
            url: "../Login/SearchHotels",
            dataType: "json",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            cache: false,
            data: JSON.stringify(values),
            beforeSend: function () {
                startLoadingGif();
                shrinkSearchBox();
            },
            success: function (data) {
                buildLodgingsView(data)
            },
            error: function () {
            }
        });
    }
}

function shrinkSearchBox() {
    $("#searchDiv").removeClass("col-md-4").addClass("col-md-3");
    $("#searchDiv").children().removeClass("col-md-4").addClass("col-md-3");
    $("#resultsDiv").removeClass("col-md-8").addClass("col-md-9");
}

function startLoadingGif() {
    $("#panel-results").hide();
    $("#lodgingList").empty();
    $("#panel-ads").hide();
    $("#spinnerHome").show();
}

function buildLodgingsView(data, userkey) {
    var body = "";
    if (data.Lodgings.length == 0) {
        body = body + "<h3>No se encontraron hoteles que cumplan con la busqueda especificada, pruebe una busqueda diferente</h3>";
    } else {
        for (var key in data.Lodgings) {        
            var lodging = data.Lodgings[key];
            var desayuno = "No";
            var tarifaReembolsable = "No";
            if (lodging["LodgingBreakfast"] == 1) {
                desayuno = "Si";
            }
            if (lodging["LodgingTarifa"] == 1) {
                tarifaReembolsable = "Si";
            }
            var stringifiedLodging = JSON.stringify(lodging);
            var categoryText = ""
            for (var i = 0; i < lodging["LodgingCategory"]; i++) {
                categoryText = categoryText + "<span class='glyphicon glyphicon-star'></span>";
            }
            if (userkey.toUpperCase() == "7C918834-8C21-4B4C-B72B-1B8498E24304") {
                body = body + "<div class='panel panel-default lodging-panel'>" +
                                "<div class='panel-heading main-background-color'>" +
                                    "<div class='row'>" +
                                        "<div class='col-md-8'>" +
                                            lodging["LodgingName"] +
                                        "</div>" +
                                        "<div class='col-md-4 text-right'>" +
                                            categoryText +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                                "<div class='panel-body'>" +
                                    "<div class='row'>" +
                                        "<div class='col-md-3'>" +
                                            "<img class='img-responsive' src='../Content/Images/Hotels/" + lodging["LodgingId"].toUpperCase() + ".jpg'>" +
                                        "</div>" +
                                        "<div class='col-md-6 lodging-description-text'>" +
                                            "<p>" + lodging["LodgingDescription"] + "</p>" +
                                        "</div>" +
                                        "<div class='col-md-3 text-center'>" +
                                            "<h3>" + lodging["LodgingCurrency"] + lodging["LodgingPrice"].toString() + "</h3>" +
                                            "<p>Precio X Noche</p>" +
                                            "<p>Impuestos Incluidos</p>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                                "<div class='panel-footer'>" +
                                    "<div class='row'>" +        
                                        "<div class='col-xs-6'>" +
                                            "<div id='promocion-div-" + lodging["LodgingId"] + "'></div>" +
                                        "</div>" +
                                        "<div class='col-xs-6'>" +
                                            "<div class='row'>" +  
                                                '<button class="btn btn-info" style="margin: 5px;" onclick="openDetailsModal(' + "'" + lodging["LodgingId"] + "'" + ')">Mas Informacion</button>' +
                                                '<button class="btn btn-main" style="margin: 5px;" onclick="verTarifasHotel(' + "'" + lodging["LodgingId"] + "'," + "'" + lodging["LodgingName"] + "'" + ')">Ver Tarifas</button>' +
                                                "<button class='btn btn-success' style='margin: 5px;' onclick='openReservationModal(" + stringifiedLodging + ");'>Reservar</button>" +
                                            "</div>" +
                                            "<div class='row'>" +
                                                "Tarifa Reembolsable: " + tarifaReembolsable + " - Desayuno: " + desayuno + 
                                            "</div>" +
                                        "</div>" +   
                                    "</div>" +
                                "</div>" +
                            "</div>";
            }
            else {
                if (lodging["LodgingUnderPetition"]) {
                    body = body + "<div class='panel panel-default lodging-panel'>" +
                                "<div class='panel-heading main-background-color'>" +
                                    "<div class='row'>" +
                                        "<div class='col-md-8'>" +
                                            lodging["LodgingName"] +
                                        "</div>" +
                                        "<div class='col-md-4 text-right'>" +
                                            categoryText +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                                "<div class='panel-body'>" +
                                    "<div class='row'>" +
                                        "<div class='col-md-3'>" +
                                            "<img class='img-responsive' src='../Content/Images/Hotels/" + lodging["LodgingId"].toUpperCase() + ".jpg'>" +
                                        "</div>" +
                                        "<div class='col-md-6 lodging-description-text'>" +
                                            "<p>" + lodging["LodgingDescription"] + "</p>" +
                                        "</div>" +
                                        "<div class='col-md-3 text-center'>" +
                                            "<h3>" + lodging["LodgingCurrency"] + lodging["LodgingPrice"].toString() + "</h3>" +
                                            "<p>Precio X Noche</p>" +
                                            "<p>Impuestos Incluidos</p>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                                "<div class='panel-footer'>" +
                                    "<div class='row'>" +
                                        "<div class='col-xs-6'>" +
                                            "<div id='promocion-div-" + lodging["LodgingId"] + "'></div>" +
                                        "</div>" +
                                        "<div class='col-xs-6'>" +
                                            "<div class='row'>" +
                                                '<button class="btn btn-info" style="margin: 5px;" onclick="openDetailsModal(' + "'" + lodging["LodgingId"] + "'" + ')">Mas Informacion</button>' +
                                                '<button class="btn btn-main" style="margin: 5px;" onclick="verTarifasHotel(' + "'" + lodging["LodgingId"] + "'," + "'" + lodging["LodgingName"] + "'" + ')">Ver Tarifas</button>' +
                                                "<button class='btn btn-default' style='margin: 5px' onclick='openReservationModalSimple(" + stringifiedLodging + ");'>Reservar</button>" +
                                            "</div>" +
                                            "<div class='row'>" +
                                                "Tarifa Reembolsable: " + tarifaReembolsable + " - Desayuno: " + desayuno +
                                            "</div>" +
                                        "</div>" + 
                                    "</div>" +
                                "</div>" +
                            "</div>";
                }
                else {
                    body = body + "<div class='panel panel-default lodging-panel'>" +
                                        "<div class='panel-heading main-background-color'>" +
                                            "<div class='row'>" +
                                                "<div class='col-md-8'>" +
                                                    lodging["LodgingName"] +
                                                "</div>" +
                                                "<div class='col-md-4 text-right'>" +
                                                    categoryText +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                        "<div class='panel-body'>" +
                                            "<div class='row'>" +
                                                "<div class='col-md-3'>" +
                                                    "<img class='img-responsive' src='../Content/Images/Hotels/" + lodging["LodgingId"].toUpperCase() + ".jpg'>" +
                                                "</div>" +
                                                "<div class='col-md-6 lodging-description-text'>" +
                                                    "<p>" + lodging["LodgingDescription"] + "</p>" +
                                                "</div>" +
                                                "<div class='col-md-3 text-center'>" +
                                                    "<h3>" + lodging["LodgingCurrency"] + lodging["LodgingPrice"].toString() + "</h3>" +
                                                    "<p>Precio X Noche</p>" +
                                                    "<p>Impuestos Incluidos</p>" +
                                                "</div>" +
                                            "</div>" +
                                        "</div>" +
                                        "<div class='panel-footer'>" +
                                            "<div class='row'>" +
                                                "<div class='col-xs-6'>" +
                                                    "<div id='promocion-div-" + lodging["LodgingId"] + "'></div>" +                                                 
                                                "</div>" +
                                                "<div class='col-xs-6'>" +
                                                    "<div class='row'>" +
                                                        '<button class="btn btn-info" style="margin: 5px;" onclick="openDetailsModal(' + "'" + lodging["LodgingId"] + "'" + ')">Mas Informacion</button>' +
                                                        '<button class="btn btn-main" style="margin: 5px;" onclick="verTarifasHotel(' + "'" + lodging["LodgingId"] + "'," + "'" + lodging["LodgingName"] + "'" + ')">Ver Tarifas</button>' +
                                                        "<button class='btn btn-success' style='margin: 5px;' onclick='openReservationModal(" + stringifiedLodging + ");'>Reservar</button>" +
                                                    "</div>" +
                                                    "<div class='row'>" +
                                                        "Tarifa Reembolsable: " + tarifaReembolsable + " - Desayuno: " + desayuno +
                                                    "</div>" +
                                                "</div>" +                
                                            "</div>" +
                                        "</div>" +
                                    "</div>";
                }
            }
        }
    }
    $("#lodgingList").append(body);
    for (var    key in data.Lodgings) {
        debugger;
        var lodging = data.Lodgings[key];
        if (lodging["TienePromocion"] == true) {
            $("#promocion-div-" + lodging["LodgingId"] + "").append("<div class='alert alert-info' role='alert'>Este Alojamiento cuenta con <strong>promociones</strong>, click en <strong>Reservar</strong> para conocer mas!</div>");
        }
    }
    $("#spinnerHome").hide();
    $("#panel-results").fadeIn('slow');
}

function showSearchFilters() {
    $("#resultsContainer").removeClass("col-md-12").addClass("col-md-8");
    $("#filtersDiv").show()
    $("#showFiltersButton").text("Ocultar Filtros");
    $("#showFiltersButton").attr("onclick", "hideSearchFilters()");
}

function hideSearchFilters() {
    $("#resultsContainer").removeClass("col-md-8").addClass("col-md-12");
    $("#filtersDiv").hide();
    $("#showFiltersButton").text("Mostrar Filtros");
    $("#showFiltersButton").attr("onclick", "showSearchFilters()");
}