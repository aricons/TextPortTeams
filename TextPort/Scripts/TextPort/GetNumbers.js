$(function () {
    $.ajaxSetup({
        cache: false
    });

    $('#AreaCode').on("keyup", function (e) {
        var areaCode = $(this).val();
        if (areaCode.length === 3) {
            getAreaCodeName(areaCode, false);
        } else {
            $("#VirtualNumber").html('');
            $("#AreaCodeDescription").text('');
        }
    });

    $('#TollFreePrefix').on("change", function (e) {
        getAreaCodeName($(this).val(), true);
    });

    $('#btnContinue').on("click", function (e) {
        if ($("#form-signup").valid()) {

            generateProductDescription();

            var purchaseType = $('#PurchaseType').val();
            if (purchaseType === "ComplimentaryNumber") {
                submitPurchase();
            }
            else {
                showConfirmationModal();
            }
        }
    });

    $("#NumberCountryId").on("change", function (e) {
        $("#DivAreaCode").toggle();
        $("#DivTollFreePrefix").toggle();
        $("#AreaCodeDescription").text('');
        $("#VirtualNumber").html('');

        var optionId = $("#NumberCountryId").val();
        if (optionId == 23) {
            $("#TollFreePrefix").val('');
            $("#BaseNumberCost").val(10);
        }
        else {
            $("#AreaCode").val('');
            $("#BaseNumberCost").val(6);
        }

        calculateCost();
    });

    $('#LeasePeriod').on("change", function (e) {
        calculateCost();
    });

    $('#CreditPurchaseAmount').on("change", function (e) {
        calculateCost();
    });

    $('#purchaseModalCont').on('show.bs.modal', function () {
        renderPayPalButton();
    });

    calculateCost();
});

function getAreaCodeName(areaCode, tollFree) {
    var url = '/numbers/getareacodename';
    $.getJSON(url, {
        areaCode: areaCode,
        tollFree: tollFree
    }, function (response) {
        if (response !== "") {
            $("#AreaCodeDescription").text(response);
            getAvailableNumbers(areaCode, tollFree);
        } else {
            $("#AreaCodeDescription").text("Invalid area code");
        }
    });
}

function getAvailableNumbers(areaCode, tollFree) {
    var url = '/Numbers/GetAvailableNumbers';
    var numbers = "";
    var spinId = "spintiller1";
    if (tollFree) {
        spinId = "spintiller2";
    }

    $("#VirtualNumber").html("");
    $("#" + spinId).show();

    $.getJSON(url, {
        areaCode: areaCode,
        tollFree: tollFree
    }, function (response) {
        $.each(response, function (index, item) {
            numbers += "<option value='" + item.Value + "'>" + item.Text + "</option>";
        });
        $("#VirtualNumber").html(numbers);
        $("#" + spinId).hide();
    });
}

function showConfirmationModal() {
    $('#purchaseModalCont').html('');

    var regData = {
        PurchaseType: $('#PurchaseType').val(),
        AccountId: $('#AccountId').val(),
        UserName: $("#UserName").val(),
        EmailAddress: $("#EmailAddress").val(),
        Password: $("#Password").val(),
        NumberCountryId: $("#NumberCountryId").val(),
        AreaCode: $("#AreaCode").val(),
        VirtualNumber: $("#VirtualNumber").val(),
        BaseNumberCost: $("#BaseNumberCost").val(),
        NumberCost: $("#NumberCost").val(),
        PayPalCustom: $('#PayPalCustom').val(),
        VirtualNumberId: $("#VirtualNumberId").val(),
        LeasePeriod: $("#LeasePeriod option:selected").val(),
        TotalCost: $('#hidTotalCost').val(),
        PurchaseTitle: $('#PurchaseTitle').val(),
        ProductDescription: $('#ProductDescription').val(),
        CreditPurchaseAmount: $('#CreditPurchaseAmount').val()
    };

    $.post({
        url: '/account/prepurchase',
        data: JSON.stringify(regData),
        contentType: "application/json",
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ": Couldn't load pre-purchase. " + errorThrown);
        },
        success: function (newInputHTML) {
            var form = document.getElementById("purchaseModalCont");
            form.insertAdjacentHTML("beforeend", newInputHTML);
            $(form).removeData("validator") // Added by jQuery Validate
                .removeData("unobtrusiveValidation"); // Added by jQuery Unobtrusive Validation
            $.validator.unobtrusive.parse(form);

            $('#purchaseModalCont').modal({
                keyboard: true
            }, 'show');
        }
    });
}

function calculateCost() {
    var numberCost = 0;
    var creditCost = 0;
    var totalCost = 0;
    var leasePeriod = 0;
    var baseNumberCost = 0;
    var creditPurchaseAmount = $("#CreditPurchaseAmount");

    if (creditPurchaseAmount.length) {
        creditCost = parseFloat($("#CreditPurchaseAmount").val());
    }

    leasePeriod = parseFloat($("#LeasePeriod").val());
    baseNumberCost = parseFloat($("#BaseNumberCost").val());
    numberCost = leasePeriod * baseNumberCost;
    totalCost = numberCost + creditCost;

    if (totalCost < 0) { totalCost = 0; }

    $("#NumberCostTxt").text("$" + numberCost + ".00");
    $("#TotalCost").text("$" + totalCost + ".00");
    $("#hidTotalCost").val(totalCost);
    $("#NumberCost").val(numberCost);
}

function generateProductDescription() {
    var purchaseType = $('#PurchaseType').val();
    var productDesctiption = '';
    var numberAmount = 0;
    var creditAmount = 0;
    var leasePeriod = 0;
    var baseNumberCost = 0;
    var number = numberToE164($("#VirtualNumber").val());
    var numberType = 'TextPort number ';

    var leasePer = $("#LeasePeriod");
    if (leasePer.length) {
        leasePeriod = parseFloat($("#LeasePeriod").val());
        baseNumberCost = parseFloat($("#BaseNumberCost").val());
        numberAmount = leasePeriod * baseNumberCost;
    }

    var creditPurchaseAmount = $("#CreditPurchaseAmount");
    if (creditPurchaseAmount.length) {
        creditAmount = parseFloat($("#CreditPurchaseAmount").val());
    }

    if (baseNumberCost === 10) {
        numberType = 'TextPort toll-free number ';
    }

    if (purchaseType === "VirtualNumber" || purchaseType === "VirtualNumberSignUp") {
        productDesctiption = numberType + number + " " + $("#LeasePeriod option:selected").val() + " month lease - " + "$" + numberAmount + ".00";
        if (creditAmount > 0) {
            productDesctiption = productDesctiption + ". Plus $" + creditAmount + ".00 TextPort credit";
        }
    }
    else if (purchaseType === "VirtualNumberRenew") {
        productDesctiption = numberType + number + " " + $("#LeasePeriod option:selected").val() + " month lease renewal - " + "$" + numberAmount + ".00";
        if (creditAmount > 0) {
            productDesctiption = productDesctiption + ". Plus $" + creditAmount + ".00 TextPort credit";
        }
    }
    else if (purchaseType === "Credit") {
        productDesctiption = "Add " + $("#TotalCost").text() + " TextPort credit";
    }

    $('#ProductDescription').val(productDesctiption);
}

function registration_complete(purchaseType) {
    var url = '/account/profile';

    if (purchaseType === "VirtualNumber" || purchaseType === "VirtualNumberRenew" || purchaseType === "ComplimentaryNumber") {
        url = '/numbers/manage';
    }
    else if (purchaseType === "Credit") {
        url = '/account/balance';
    }

    window.location.href = url;
}

function renderPayPalButton() {
    paypal.Button.render({
        env: 'production', // sandbox | production
        style: {
            layout: 'vertical',  // horizontal | vertical
            size: 'medium',    // medium | large | responsive
            shape: 'rect',      // pill | rect
            color: 'blue'       // gold | blue | silver | white | black
        },

        // Options:
        // - paypal.FUNDING.CARD
        // - paypal.FUNDING.CREDIT
        // - paypal.FUNDING.ELV
        funding: {
            allowed: [
                paypal.FUNDING.CARD,
                paypal.FUNDING.CREDIT
            ],
            disallowed: []
        },

        // Enable Pay Now checkout flow (optional)
        commit: true,

        client: {
            //sandbox: 'Ac6TTAGnmNme-wJJSdgU6rm8SSyW5nSc757nHhsqNWDz3X7lOa8Yx3eE-96JK-Z2YbN3N54PE_oRRGbO',
            production: 'AUjK3Zugk_dkKu2ScI-f1S-8Ibxu99MeiuS9MSzogMOiEyKUa8Q4kz5L-wHfMxqZhF3p7ZjIm_64Ju_q'
        },

        payment: function (data, actions) {
            return actions.payment.create({
                payment: {
                    transactions: [
                        {
                            amount: {
                                total: $('#hidTotalCost').val(),
                                currency: 'USD'
                            },
                            description: $('#ProductDescription').val(),
                            custom: $('#PayPalCustom').val()
                        }
                    ]
                },
            });
        },

        onAuthorize: function (data, actions) {
            return actions.payment.execute()
                .then(function () {
                    var regData = {
                        PurchaseType: $('#PurchaseType').val(),
                        AccountId: $('#AccountId').val(),
                        UserName: $("#UserName").val(),
                        EmailAddress: $("#EmailAddress").val(),
                        Password: $("#Password").val(),
                        NumberCountryId: $("#NumberCountryId").val(),
                        AreaCode: $("#AreaCode").val(),
                        TollFreePrefix: $("#TollFreePrefix").val(),
                        VirtualNumber: $("#VirtualNumber").val(),
                        VirtualNumberId: $("#VirtualNumberId").val(),
                        LeasePeriod: $("#LeasePeriod option:selected").val(),
                        NumberCost: $("#NumberCost").val(),
                        BaseNumberCost: $("#BaseNumberCost").val(),
                        TotalCost: $('#hidTotalCost').val(),
                        PayPalCustom: $('#PayPalCustom').val(),
                        PurchaseTitle: $('#PurchaseTitle').val(),
                        ProductDescription: $('#ProductDescription').val(),
                        CreditPurchaseAmount: $('#CreditPurchaseAmount').val()
                    };

                    var url = '/account/postpurchase';
                    $.post({
                        url: url,
                        data: JSON.stringify(regData),
                        contentType: "application/json",
                        error: function (jqXHR, textStatus, errorThrown) {
                            alert(textStatus + ": Couldn't load post-purchase. " + errorThrown);
                        },
                        success: function (newInputHTML) {
                            $('#purchaseModalCont').modal('toggle');
                            $('#postPurchaseModalCont').html('');
                            var form = document.getElementById("postPurchaseModalCont");
                            form.insertAdjacentHTML("beforeend", newInputHTML);
                            $('#postPurchaseModalCont').modal({
                                backdrop: 'static',
                                keyboard: false
                            }, 'show');
                        }
                    });
                });
        }
    }, '#paypal-button-container');
}

function submitPurchase() {
    var regData = {
        PurchaseType: $('#PurchaseType').val(),
        AccountId: $('#AccountId').val(),
        UserName: $("#UserName").val(),
        EmailAddress: $("#EmailAddress").val(),
        Password: $("#Password").val(),
        NumberCountryId: $("#NumberCountryId").val(),
        AreaCode: $("#AreaCode").val(),
        TollFreePrefix: $("#TollFreePrefix").val(),
        VirtualNumber: $("#VirtualNumber").val(),
        VirtualNumberId: $("#VirtualNumberId").val(),
        LeasePeriod: $("#LeasePeriod option:selected").val(),
        NumberCost: $("#NumberCost").val(),
        BaseNumberCost: $("#BaseNumberCost").val(),
        TotalCost: $('#hidTotalCost').val(),
        PayPalCustom: $('#PayPalCustom').val(),
        PurchaseTitle: $('#PurchaseTitle').val(),
        ProductDescription: $('#ProductDescription').val(),
        CreditPurchaseAmount: $('#CreditPurchaseAmount').val()
    };

    var url = '/account/postpurchase';

    $.post({
        url: url,
        data: JSON.stringify(regData),
        contentType: "application/json",
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ": Couldn't load post-purchase. " + errorThrown);
        },
        success: function (newInputHTML) {
            $('#postPurchaseModalCont').html('');
            var form = document.getElementById("postPurchaseModalCont");
            form.insertAdjacentHTML("beforeend", newInputHTML);
            $('#postPurchaseModalCont').modal({
                backdrop: 'static',
                keyboard: false
            }, 'show');
        }
    });
}

Number.prototype.formatMoney = function (c, d, t) {
    var n = this,
        c = isNaN(c = Math.abs(c)) ? 2 : c,
        d = d == undefined ? "." : d,
        t = t == undefined ? "," : t,
        s = n < 0 ? "-" : "",
        i = String(parseInt(n = Math.abs(Number(n) || 0).toFixed(c))),
        j = (j = i.length) > 3 ? j % 3 : 0;
    return '$' + s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};