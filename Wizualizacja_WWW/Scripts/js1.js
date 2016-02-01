

function filter(element) {
    var nazwa = $("#input_filter_nazwa").val();

    alert(nazwa);



    $.ajax({
        type: 'POST',
        url: 'Default.aspx/filter_items',
        data: nazwa
    });

    //return false;
}

function filter_orders() {

    var kod = $('#input_filter_kod_order').val().toLowerCase();
    var nazwa = $('#input_filter_nazwa_order').val().toLowerCase();

    $('#GridView_order tr').each(function (index, value) {
        if (index == 0) {
            return;
        }

        if (!kod.isNullOrWhiteSpace()) {
            if ($(value).find('.entry_id').html().toLowerCase() != kod) {
                $(value).hide(0);
            }
            else {
                $(value).show(0);
            }
        }
        if (!nazwa.isNullOrWhiteSpace()) {
            if ($(value).find('.entry_name').html().toLowerCase().indexOf(nazwa) == -1) {
                $(value).hide(0);
            }
            else {
                $(value).show(0);
            }
        }

        if (!nazwa.isNullOrWhiteSpace() && !kod.isNullOrWhiteSpace()) {
            if ($(value).find('.entry_name').html().toLowerCase().indexOf(nazwa) == -1 || $(value).find('.entry_id').html().toLowerCase() != kod) {
                $(value).hide(0);
            }
            else {
                $(value).show(0);
            }
        }
        if (nazwa.isNullOrWhiteSpace() && kod.isNullOrWhiteSpace()) {
            $(value).show(0);
        }

    });
}

var Orders = {};
var Items = {};
var Items_inc = [];

function loadData() {
    $.ajax({
        type: "POST",
        url: "Order.aspx/loadDataAsJSON",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            Items_inc = JSON.parse(data["d"]);

            for (var i = 0; i < Items_inc.length; i++) {
                Orders[Items_inc[i]["kod_towaru"]] = 0;
            }

            for (var i = 0; i < Items_inc.length; i++) {
                Items[Items_inc[i]["kod_towaru"]] = Items_inc[i];
            }

            console.log("Items");
            console.dir(Items);
            //console.log("Items_inc");
            //console.dir(Items_inc);
            console.log("Orders");
            console.dir(Orders);
        }
    });
}

function loadDataToOrderForm() {
    // select_delivery

    $.ajax({
        type: "POST",
        url: "Order.aspx/getDeliveryMethodsAsJSON",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            var json_data = JSON.parse(data["d"]);

            //console.dir(json_data);

            $.each(json_data, function (i, v) {
                var o = document.createElement('option');
                o.value = v["id"];
                o.innerHTML = v["nazwa"];
                $('#select_delivery').append(o);
            });


        }
    });
}

function addOrder() {
    var dane = {
        osoba: $('#input_osoba').val(),
        adres: $('#input_adres').val(),
        dostawa: $('#select_delivery').val(),
        towar: Orders
    };
    console.dir(dane);

    if (dane.osoba.isNullOrWhiteSpace() || dane.adres.isNullOrWhiteSpace()) {
        alertError("Wypełnij wszystkie pola");
        return;
    }


    $.ajax({
        type: "POST",
        url: "Order.aspx/addOrder",
        data: JSON.stringify({
            data: dane
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response["d"] == "0") {
                var str = "Dodano zamówienie";
                alertSuccess(str);

                setTimeout(function () {
                    window.location.reload();
                }, 1500);
            }
            else {
                alertError(response["d"]);
            }
        },
        error: function (xhr, status, errorThrown) {
            try {
                var err = JSON.parse(xhr.responseText);
            }
            catch (e) {
                var err = xhr.responseText;
            }

        },
    });
}


function btn_order_minus(id, sender) {
    var currentAmount = Orders[id];
    var availableAmount = Items[id]["ilosc"];

    if (currentAmount - 1 >= 0) {
        Orders[id]--;
        $(sender).siblings('.order_amount').html(Orders[id]);
    }

    if (Orders[id] < availableAmount) {
        $(sender).siblings('button').attr("disabled", false);;
    }

    if (Orders[id] <= 0) {
        $(sender).attr("disabled", true);
    }

    if (Orders[id] > 0) {
        $(sender).siblings('button').attr("disabled", false);
    }
}


function btn_order_plus(id, sender) {
    var currentAmount = Orders[id];
    var availableAmount = Items[id]["ilosc"];

    if (currentAmount + 1 <= availableAmount) {
        Orders[id]++;
        $(sender).siblings('.order_amount').html(Orders[id]);
    }

    if (Orders[id] >= availableAmount) {
        $(sender).attr("disabled", true);
    }

    if (Orders[id] > 0) {
        $(sender).siblings('button').attr("disabled", false);
    }
}




function alertSuccess(msg) {
    $('#alert_box').html('<div role="alert" class="alert alert-success alert-dismissible fade in"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><span id="alert_box_text">' + msg + '</span></div>');

    $("html, body").animate({ scrollTop: 0 }, 400);
}

function alertError(msg) {
    $('#alert_box').html('<div role="alert" class="alert alert-danger alert-dismissible fade in"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><span id="alert_box_text">' + msg + '</span></div>');

    $("html, body").animate({ scrollTop: 0 }, 400);
}





String.prototype.isNullOrWhiteSpace = function () {
    if (this && this.length > 0 && this !== ' ') {
        return false;
    }
    else {
        return true;
    }
}



//todo: order, filrty wyłączyć uzupełnianie
//todo: order, filtry przycisk reset


///////////////////////////////



//function filter(element) {
//    var nazwa = $("#input_filter_nazwa").val();

//    alert(nazwa);



//    $.ajax({
//        type: 'POST',
//        url: 'Default.aspx/filter_items',
//        data: nazwa
//    });

//    //return false;
//}

//function filter_orders() {

//    var kod = $('#input_filter_kod_order').val().toLowerCase();
//    var nazwa = $('#input_filter_nazwa_order').val().toLowerCase();
//    console.log(kod);
//    console.log(nazwa);

//    $('#GridView_order tr').each(function (index, value) {
//        if (index == 0) {
//            return;
//        }

//        if (!kod.isNullOrWhiteSpace()) {
//            if ($(value).find('#entry_id').html().toLowerCase() != kod) {
//                $(value).hide(0);
//            }
//            else {
//                $(value).show(0);
//            }
//        }
//        else {
//            $(value).show(0);
//        }

//        if (!nazwa.isNullOrWhiteSpace()) {
//            if ($(value).find('#entry_name').html().toLowerCase().indexOf(nazwa) == -1) {
//                $(value).hide(0);
//            }
//            else {
//                $(value).show(0);
//            }
//        }
//        //else {
//        //    $(value).show(0);
//        //}

//    });
//}



//String.prototype.isNullOrWhiteSpace = function () {
//    if (this && this.length > 0 && this !== ' ') {
//        return false;
//    }
//    else {
//        return true;
//    }
//}

