function openReservationModal(lodging) {
    $("#AvailableRoomsDiv").empty();
    $("#ReservationModalLabel").text("Buscar disponibilidad en " + lodging["LodgingName"]);
    $("#hotelName2").val(lodging["LodgingName"]);
    if (lodging.Vacancies.length != 0) {
        $("#changeReservationDatesForm").hide();
        fillHiddenFields(lodging);
        buildAvailableRooms(lodging.Vacancies);
    } else {
        $("#changeReservationDatesForm").show();
        $("#AvailableRoomsRow").hide();
    }
    $("#ReservationModal").modal('show');
}

function openReservationModalSimple(lodging) {
    $("#AvailableRoomsDiv").empty();
    $("#ReservationModalLabel").text("Buscar disponibilidad en " + lodging["LodgingName"]);
    $("#hotelName2").val(lodging["LodgingName"]);
    $("#changeReservationDatesForm").show();
    $("#AvailableRoomsRow").hide();
}

function agregarHabitacion(lodgingName, destinationId, checkinDate, checkoutDate) {
    checkHotelAvailabilityForReservation(lodgingName, destinationId, checkinDate, checkoutDate);
}

function buildAvailableRooms(vacancies) {
    var body = "";
    for (var key in vacancies) {
        var vacancy = vacancies[key];
        var roomsBody = "";
        for (var key2 in vacancy.Rooms) {
            var room = vacancy.Rooms[key2];
            roomsBody = roomsBody +
                "<input type='hidden' value='" + room["RoomId"] + "' id='vacancy_" + key + "_Room_" + key2 + "_RoomId' />" +
                "<input type='hidden' value='" + room["RoomName"] + "' id='vacancy_" + key + "_Room_" + key2 + "_RoomName' />" +
                "<input type='hidden' value='" + room["RoomType"] + "' id='vacancy_" + key + "_Room_" + key2 + "_RoomType' />";
        }
        var body = body + "<h4 id='vacancy-name-" + vacancy["VacancyId"] + "'>" + vacancy["VacancyName"] + "</h4>" +
            "<input type='hidden' value='" + vacancy["LodgingId"] + "' id='vacancy_" + key + "_LodgingId' />" +
            "<input type='hidden' value='" + vacancy["LodgingName"] + "' id='vacancy_" + key + "_LodgingName' />" +
            "<input type='hidden' value='" + vacancy["LodgingCurrency"] + "' id='vacancy_" + key + "_LodgingCurrency' />" +
            "<input type='hidden' value='" + vacancy["Breakfast"] + "' id='vacancy_" + key + "_Breakfast' />" +
            "<input type='hidden' value='" + vacancy["Tarifa"] + "' id='vacancy_" + key + "_Tarifa' />" +

            "<input type='hidden' value='" + vacancy["VacancyId"] + "' id='vacancy_" + key + "_VacancyId' />" +
            "<input type='hidden' value='" + vacancy["VacancyName"] + "' id='vacancy_" + key + "_VacancyName' />" +
            "<input type='hidden' value='" + vacancy["VacancyCheckin"] + "' id='vacancy_" + key + "_VacancyCheckin' />" +
            "<input type='hidden' value='" + vacancy["VacancyCheckout"] + "' id='vacancy_" + key + "_VacancyCheckout' />" +
            "<input type='hidden' value='" + vacancy["VacancyPrice"] + "' id='vacancy_" + key + "_VacancyPrice' />" +

            roomsBody +

            "<table class='table table-bordered'>" +
                "<thead>" +
                    "<tr>" +
                        "<th class='text-center'>Camas</th>" +
                        "<th class='text-center'>Personas</th>" +
                        "<th class='text-center'>Precio por Noche</th>" +
                        "<th class='text-center'>Acciones</th>" +
                    "</tr>" +
                "</thead>" +
                "<tbody>" +
                    "<tr>" +
                        "<td>" + vacancy["VacancyBeds"] + "</td>" +
                        "<td>" + vacancy["VacancyAdults"] + "</td>" +
                        "<td>$" + vacancy["VacancyPrice"] + "</td>" +
                        '<td>' +
                            '<button id="vacancy_' + key + '_reservarBtn" class="btn btn-main empezarReservaHabitacionButton" onclick="empezarReservaHabitacion(' + "'" + key + "'" + ')">Reservar</button>' +
                            '<button style="display: none;" class="btn btn-main agregarReservaHabitacionButton" onclick="agregarReservaHabitacion(' + "'" + key + "'" + ')">Agregar</button>' +
                        '</td>' +
                    "</tr>" +
                "</tbody>" +
            "</table><br />";
    }
    $("#AvailableRoomsDiv").append(body);
    for (var key in vacancies) {
        var vacancy = vacancies[key];
        var dateParts = $("#checkinDate").val().split("-");
        var checkinDate = new Date(dateParts[0], (dateParts[1] - 1), dateParts[2]);
        dateParts = $("#checkoutDate").val().split("-");
        var checkoutDate = new Date(dateParts[0], (dateParts[1] - 1), dateParts[2]);
        var oneDay = 24 * 60 * 60 * 1000;
        var diffDays = Math.round(Math.abs((checkoutDate.getTime() - checkinDate.getTime()) / (oneDay)));
        debugger;
        if (vacancy["TienePromocionNxM"] == true) {
            $("#vacancy-name-" + vacancy["VacancyId"] + "").append(" - <strong>Noches Free</strong><input type='hidden' value='true' id='TienePromocionNxM-" + key + "' />").addClass("alert alert-info");
        } else if (vacancy["TienePromocionMinimoMaximo"]) {
            if (diffDays < vacancy["MinimoNoches"]) {
                $("#vacancy-name-" + vacancy["VacancyId"] + "").append(" - <strong>Minimo de noches " + vacancy["MinimoNoches"] + "</strong><input type='hidden' value='true' id='TienePromocionMinimoMaximo-" + key + "' />").addClass("alert alert-danger");
                $("#vacancy_" + key + "_reservarBtn").hide();
            }
            if (diffDays > vacancy["MaximoNoches"]) {
                $("#vacancy-name-" + vacancy["VacancyId"] + "").append(" - <strong>Maximo de noches " + vacancy["MaximoNoches"] + "</strong><input type='hidden' value='true' id='TienePromocionMinimoMaximo-" + key + "' />").addClass("alert alert-danger");
                $("#vacancy_" + key + "_reservarBtn").hide();
            }
        }
    }
    $("#spinner2").fadeOut('slow', function () {
        $("#AvailableRoomsRow").fadeIn('slow');
    });
   
}

function checkHotelAvailability() {
    var checkinDate = new Date($("#checkinDate2").val());
    var checkoutDate = new Date($("#checkoutDate2").val());
    var values = {
        "LodgingName": $("#hotelName2").val(),
        "DestinationId": $("#destinationId").val(),
        "Checkin": checkinDate,
        "Checkout": checkoutDate,
        "Order": $("#orderFilter").val(),
        "Nationality": $("#nationalityFilter").val(),
        "DisplayType": $("#displaytypeFilter").val()
    };
    $.ajax({
        url: "../Login/SearchHotels",
        datatype: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        beforeSend: function () {
            $("#spinner2").fadeIn('slow');
        },
        success: function (data) {
            getVacancies(data);
        },
        error: function () {

        }
    });
}

function checkHotelAvailabilityForReservation(lodgingName, destinationId, vacancycheckinDate, vacancycheckoutDate) {
    var from1 = vacancycheckinDate.split("/");
    var from1Year = from1[2].split(" ");
    var from2 = vacancycheckoutDate.split("/");
    var from2Year = from2[2].split(" ");
    var checkinDate = new Date(from1Year[0], from1[1] - 1, from1[0]);
    var checkoutDate = new Date(from2Year[0], from2[1] - 1, from2[0]);
    $("#hotelName2").val(lodgingName);
    var oldVacanciesCount = $("[id^=vacancyDiv]").length;
    var singleRoom = {
        "RoomTypeCode": "single"
    };
    var dobleRoom = {
        "RoomTypeCode": "doble"
    };
    var tripleRoom = {
        "RoomTypeCode": "triple"
    };
    var cuadrupleRoom = {
        "RoomTypeCode": "cuadruple"
    };
    var rooms = [singleRoom, dobleRoom, tripleRoom, cuadrupleRoom]
    for (var i = 0; i < oldVacanciesCount; i++) {
        var vacancyTypeCount = $("#reserved_vacancy_vacancyReserved_" + i).val();
        for (var j = 0; j < vacancyTypeCount; j++) {
            switch ($("#reserved_vacancy_roomType_" + i).val()) {
                case "Single":
                    var room = {
                        "RoomTypeCode": "single"
                    };
                    rooms.push(room);
                    break;
                case "Double":
                    var room = {
                        "RoomTypeCode": "doble"
                    };
                    rooms.push(room);
                    break;
                case "Triple":
                    var room = {
                        "RoomTypeCode": "triple"
                    };
                    rooms.push(room);
                    break;
                case "Quad":
                    var room = {
                        "RoomTypeCode": "cuadruple"
                    };
                    rooms.push(room);
                    break;
            }
        }
    }
    var values = {
        "LodgingName": lodgingName,
        "DestinationId": destinationId,
        "Checkin": checkinDate,
        "Checkout": checkoutDate,
        "Order": "PorNombre",
        "Nationality": "arg",
        "DisplayType": "det",
        "Rooms": rooms
    };
    $.ajax({
        url: "../Login/SearchHotels",
        datatype: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        beforeSend: function () {
            $("#changeReservationDatesForm").hide();
            $("#ReservationModal").modal('show');
            $("#spinner2").fadeIn('slow');
        },
        success: function (data) {    
            getVacancies(data);
            $(".empezarReservaHabitacionButton").hide();
            $(".agregarReservaHabitacionButton").show();
        },
        error: function () {

        }
    });
}

function getVacancies(data) {
    if (data.Lodgings.length == 0) {
        noRoomsFound();
    }
    for (var key in data.Lodgings) {
        var lodging = data.Lodgings[key];
        fillHiddenFields(lodging);
        buildAvailableRooms(lodging.Vacancies);
    }
}

function fillHiddenFields(lodging) {
    
    $("#hotelId").val(lodging["LodgingId"]);
    $("#hotelCategory").val(lodging["LodgingCategory"]);
    $("#hotelCurrency").val(lodging["LodgingCurrency"]);
    $("#hotelCurrencyCode").val(lodging["LodgingCurrencyCode"]);
    $("#hotelPrice").val(lodging["LodgingPrice"]);
    $("#destinationId").val(lodging["DestinationId"]);
    $("#hotelSupplierId").val(lodging["LodgingSupplierId"]);
}

function noRoomsFound() {
    var body = "<h4>TEXTO PARA ACLARAR QUE NO HAY CUPOS PERO PUEDE PEDIR RESERVA ONLINE</h4>" + 
                "<button class='btn btn-default'>Reservar Via Mail</button>";
    $("#AvailableRoomsDiv").append(body);
    $("#spinner2").fadeOut('slow', function () {
        $("#AvailableRoomsRow").fadeIn('slow');
    });
    
}

function empezarReservaHabitacion(vacancyNumber) {
    var roomsElements = $("[id^=vacancy_" + vacancyNumber + "_Room_]");
    var checkinDate = new Date(parseInt($("#vacancy_" + vacancyNumber + "_VacancyCheckin").val().substr(6)));
    var checkoutDate = new Date(parseInt($("#vacancy_" + vacancyNumber + "_VacancyCheckout").val().substr(6)));
    var tienePromocionNxM = false;
    var result = $("#TienePromocionNxM-" + vacancyNumber).val();
    var desayuno = $("#vacancy_" + vacancyNumber + "_Breakfast").val();
    var tarifa = $("#vacancy_" + vacancyNumber + "_Tarifa").val();
    var tienePromocion = false;
    if (result) {
        tienePromocionNxM = true;
        tienePromocion = true;
    }

    var room = {
        "RoomId": roomsElements[0].defaultValue,
        "RoomName": roomsElements[1].defaultValue,
        "RoomType": roomsElements[2].defaultValue
    };
    var rooms = [room];
    var vacancy = {
        "LodgingId": $("#vacancy_" + vacancyNumber + "_LodgingId").val(),
        "LodgingName": $("#vacancy_" + vacancyNumber + "_LodgingName").val(),
        "LodgingName": $("#vacancy_" + vacancyNumber + "_LodgingName").val(),
        "LodgingCurrency": $("#vacancy_" + vacancyNumber + "_LodgingCurrency").val(),
        "VacancyId": $("#vacancy_" + vacancyNumber + "_VacancyId").val(),
        "VacancyName": $("#vacancy_" + vacancyNumber + "_VacancyName").val(),
        "VacancyCheckin": checkinDate,
        "VacancyCheckout": checkoutDate,
        "VacancyPrice": $("#vacancy_" + vacancyNumber + "_VacancyPrice").val(),
        "VacancyReserved": 1,
        "Rooms": rooms,
        "TienePromocionNxM": tienePromocionNxM,
        "Breakfast": desayuno,
        "Tarifa": tarifa,
    };
    var vacancies = [vacancy];
    var values = {
        "LodgingId": $("#hotelId").val(),
        "LodgingName": $("#hotelName2").val(),
        "LodgingCurrency": $("#hotelCurrency").val(),
        "LodgingCurrencyCode": $("#hotelCurrencyCode").val(),
        "LodgingSupplierId": $("#hotelSupplierId").val(),
        "DestinationId": $("#destinationIdSearch").val(),
        "Vacancies": vacancies,
        "TienePromocion": tienePromocion,
    }
    $.ajax({
        url: "../Payment/Confirmation",
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        beforeSend: function () {

        },
        success: function (data) {
            window.location.href = "/Payment/OpenConfirmation";
        },
        error: function () {
            alert("Error");
        }
    });
}

function agregarPasajero() {
    $.ajax({
        url: "../Payment/AddPassenger",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: {
            "travelerName": $("#pasajeroName").val(),
            "travelerLastName": $("#pasajeroLastName").val(),
            "travelerCountry": $("#pasajeroPais").val(),
            "travelerIdType": $("#pasajeroTipoIdentificacion").val(),
            "travelerIdNumber": $("#pasajeroNumeroIdentificacion").val(),
            "vacancyId": $("#pasajeroVacancyId").val()
        },
        beforeSend: function () {

        },
        success: function (data) {
            $("#pasajerosTitle").show();
            window.location.href = "/Payment/OpenConfirmation";
        },
        error: function (data) {

        }
    });
}

function eliminarPasajero(nombre, apellido) {
    $.ajax({
        url: "../Payment/EliminarPasajero",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: {
            "nombre": nombre,
            "apellido": apellido
        },
        beforeSend: function () {

        },
        success: function (data) {
            window.location.href = "/Payment/OpenConfirmation";
        },
        error: function (data) {
        }
    });
}

function openAgregarPasajeroModal(vacancyId) {
    $("#agregarPasajeroModal").modal('show');
    $("#pasajeroVacancyId").val(vacancyId)
}

function agregarReservaHabitacion(vacancyNumber) {
    var checkinDate = new Date(parseInt($("#vacancy_" + vacancyNumber + "_VacancyCheckin").val().substr(6)));
    var checkoutDate = new Date(parseInt($("#vacancy_" + vacancyNumber + "_VacancyCheckout").val().substr(6)));
    var oldVacanciesCount = $("[id^=vacancyDiv]").length;
    var vacancies = [];
    
    var found = false;
    for (var i = 0; i < oldVacanciesCount; i++) {
        var vacancyExists = 0;
        if ($("#reserved_vacancy_vacancyId_" + i).val() == $("#vacancy_" + vacancyNumber + "_VacancyId").val()) {
            vacancyExists = 1;
            found = true;
        }
        var travelersList = $("#vacancy_" + i + "_travelerList");
        var travelersCount = travelersList.length;
        var travelers = [];
        
        for (var j = 0; j < travelersCount; j++) {
            var traveler = {
                "TravelerFirstName": $("#vacancy_" + i + "_travelerName_" + j).val(),
                "TravelerLastName": $("#vacancy_" + i + "_travelerLastName_" + j).val()
            }
            travelers.push(traveler);
        }       
        var room = {
            "RoomId": $("#reserved_vacancy_roomId_" + i).val(),
            "RoomName": $("#reserved_vacancy_roomName_" + i).val(),
            "RoomType": $("#reserved_vacancy_roomType_" + i).val(),
            "Travelers" : travelers
        };
        var rooms = [room];
        var vacancy = {
            "LodgingId": $("#vacancy_" + vacancyNumber + "_LodgingId").val(),
            "LodgingName": $("#vacancy_" + vacancyNumber + "_LodgingName").val(),
            "LodgingCurrency": $("#vacancy_" + vacancyNumber + "_LodgingCurrency").val(),
            "VacancyId": $("#reserved_vacancy_vacancyId_" + i).val(),
            "VacancyName": $("#reserved_vacancy_vacancyName_" + i).val(),
            "VacancyCheckin": checkinDate,
            "VacancyCheckout": checkoutDate,
            "VacancyPrice": $("#reserved_vacancy_vacancyPrice_" + i).val(),
            "VacancyReserved": parseInt($("#reserved_vacancy_vacancyReserved_" + i).val()) + vacancyExists,
            "Rooms": rooms
        };
        vacancies.push(vacancy);
    }
    if (!found) {
        var newRoomsElements = $("[id^=vacancy_" + vacancyNumber + "_Room_]");
        var newRoom = {
            "RoomId": newRoomsElements[0].defaultValue,
            "RoomName": newRoomsElements[1].defaultValue,
            "RoomType": newRoomsElements[2].defaultValue
        };
        var newRooms = [newRoom];
        var newVacancy = {
            "LodgingId": $("#vacancy_" + vacancyNumber + "_LodgingId").val(),
            "LodgingName": $("#vacancy_" + vacancyNumber + "_LodgingName").val(),
            "LodgingName": $("#vacancy_" + vacancyNumber + "_LodgingName").val(),
            "LodgingCurrency": $("#vacancy_" + vacancyNumber + "_LodgingCurrency").val(),
            "VacancyId": $("#vacancy_" + vacancyNumber + "_VacancyId").val(),
            "VacancyName": $("#vacancy_" + vacancyNumber + "_VacancyName").val(),
            "VacancyCheckin": checkinDate,
            "VacancyCheckout": checkoutDate,
            "VacancyPrice": $("#vacancy_" + vacancyNumber + "_VacancyPrice").val(),
            "VacancyReserved": 1,
            "Rooms": newRooms
        };
        vacancies.push(newVacancy)
    }    
    var values = {
        "LodgingId": $("#hotelId").val(),
        "LodgingName": $("#hotelName2").val(),
        "LodgingCurrency": $("#hotelCurrency").val(),
        "LodgingCurrencyCode": $("#hotelCurrencyCode").val(),
        "LodgingSupplierId": $("#hotelSupplierId").val(),
        "DestinationId": $("#destinationIdSearch").val(),
        "Vacancies": vacancies,
    }
    $.ajax({
        url: "../Payment/Confirmation",
        dataType: "json",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        beforeSend: function () {

        },
        success: function (data) {
            window.location.href = "/Payment/OpenConfirmation";
        },
        error: function () {
            alert("Error");
        }
    });
}

function borrarHabitacion(vacancyId) {
    var values = {
        vacancyId: vacancyId
    };
    $.ajax({
        url: "../Payment/BorrarHabitacion",
        dataType: "json",
        type: "POST",
        contentType: "application/json: charset=utf-8",
        cache: false,
        data: JSON.stringify(values),
        success: function (data) {
            window.location.href = "/Payment/OpenConfirmation";
        },
        error: function () {
            alert("Error");
        }
    });
}

function cambiarOperador() {
    var username = $("#SecondaryUserName").val();
    var password = $("#SecondaryUserPass").val();
    $.ajax({
        url: "../Payment/AddSecondaryUser",
        type: "POST",
        cache: false,
        data: { "SecondaryUserName": username, "SecondaryUserPass": password },
        success: function () {
            $('#secondaryUserSelector').hide();
            $('#reservationHeader').append(" Operando como " + username + "<button class='btn btn-default' onclick='$('#secondaryUserSelector').show()'>Cambiar</button>");
        }
    })
}