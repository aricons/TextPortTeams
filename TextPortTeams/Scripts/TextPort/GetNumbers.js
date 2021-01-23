var page = 1;
var numberCount = 48;

$(function () {
    $.ajaxSetup({
        cache: false
    });

    $('#area-code').on("keyup", function (e) {
        var areaCode = $(this).val();
        if (areaCode.length === 3) {
            getAreaCodeName(areaCode, false);
        } else {
            $("#number-results").hide();
            $("#div-sel-number").addClass("d-none");
            $("#lnk-show-more").hide();
            $("#area-code-result").html('');
            $("#numbers-list").html('');
            page = 1;
        }
    });

    $("#lnk-show-more").on("click", function (e) {
        var countryCode = $("#CountryId option:selected").val();
        var areaCode = $('#area-code').val();
        if (countryCode === "1" && areaCode !== "") {
            page++;
            getAvailableNumbers(areaCode);
        }
        else {
            page++;
            getAvailableNumbers(areaCode);
        }
        return false;
    });

    $("#CountryId").on("change", function (e) {
        var countryCode = $("#CountryId option:selected").val();
        $("#numbers-list").html("");
        if (countryCode === "1") {
            $("#lbl-sel-number").text("Search by area code");
            $("#lbl-sel-number").show();
            $("#area-code").show();
        }
        else {
            $("#lbl-sel-number").hide();
            $("#area-code").hide();
            $("#number-results").show();
            getAvailableNumbers("");
        }

        page = 1;
        $("#area-code").val('');
        $("#area-code-result").html('');
        $("#selected-number").text('');
        $("#selected-number-wrap").hide();
        $("#lnk-show-more").hide();
        $("#number-chooser").show();
        getLeasePeriodsForCountry();
    });

    $("#number-results").hide();
    $("#area-code").mask("000");
});

function getAreaCodeName(areaCode, tollFree) {
    var url = '/numbers/getareacodename';
    if ($(window).width() <= 600) {
        count = 20;
    }

    $.getJSON(url, {
        areaCode: areaCode,
        tollFree: tollFree
    }, function (response) {
        $("#number-results").show();
        if (response !== "Invalid area code" && response !== "") {
            $("#area-code-result").html("<label class='h5'>Available numbers for " + response + "</label> ");
            getAvailableNumbers(areaCode);
        } else {
            $("#area-code-result").html("<div class='alert alert-danger text-center w-50 mx-auto text-center' role='alert'><h5>Invalid area code</h5></div>");
        }
    });
}

function getAvailableNumbers(areaCode) {
    var url = '/numbers/getavailablenumbers';
    var numbers = "";
    var countryId = $("#CountryId").val();
    var countryName = $("#CountryId option:selected").text();

    $("#spintiller-num").show();
    $.getJSON(url, {
        countryId: countryId,
        areaCode: areaCode,
        tollFree: false,
        count: numberCount,
        page: page
    }, function (response) {
        var resultCount = 0;
        var noAvailableNumbers = false;
        $.each(response, function (index, item) {
            numbers += "<li class='list-inline-item vn-result-sm'>" + item.Text + "</li>";
            resultCount += 1;
            if (resultCount === 1) {
                if (item.Text === "No available numbers") {
                    noAvailableNumbers = true;
                    if (countryId === "1") {
                        numbers = "<li><div class='alert alert-warning text-center' role='alert'>Sorry, no available numbers for area code " + areaCode + "</div></li>";
                    } else {
                        numbers = "<li><div class='alert alert-warning text-center' role='alert'>Sorry, no available numbers for " + countryName + "</div></li>";
                    }
                } else if (item.Text === "No more numbers") {
                    noAvailableNumbers = true;
                    if (countryId === "1") {
                        numbers = "<li><div class='alert alert-warning text-center mt-2 mr-4' role='alert'>No more numbers for area code " + areaCode + "</div></li>";
                    } else {
                        numbers = "<li><div class='alert alert-warning text-center mt-2 mr-4' role='alert'>No more numbers for " + countryName + "</div></li>";
                    }
                }
            }
        });

        if (countryId !== "1") {
            $("#area-code-result").html("<label class='h5'>Available numbers for " + $("#CountryId option:selected").text() + "</label> ");
        }
        $("#numbers-list").append(numbers);
        $("#spintiller-num").hide();
        $("#number-results").show();

        if (!noAvailableNumbers) {
            $("#lnk-show-more").show();
        } else {
            $("#lnk-show-more").hide();
        }

        $('.vn-result-sm').off("click");
        $('.vn-result-sm').on("click", function (e) {
            var number = $(this).text().replace(/\D/g, "");
            getNumber(number);
        });
    });
}

function getLeasePeriodsForCountry() {
    var url = '/numbers/getleaseperiods';
    var listitems = "";
    var countryId = $("#CountryId").val();
    var periodsdd = $('#LeasePeriodCode');
    periodsdd.find("option").remove();

    $("#spintiller-num").show();
    $.getJSON(url, {
        countryId: countryId
    }, function (response) {
        $.each(response, function (index, item) {
            listitems += "<option value='" + item.Value + "'>" + item.Text + "</option>";
        });
        periodsdd.append(listitems);
    });
    $("#spintiller-num").hide();
}