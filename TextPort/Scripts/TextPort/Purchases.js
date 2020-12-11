$(function () {
    $.ajaxSetup({
        cache: false
    });

    $('#btnContinue').on("click", function (e) {
        if ($("#form-signup").valid()) {
            showConfirmationModal();
        }
    });

    //$("#NumberCountryId").on("change", function (e) {
    //    $("#DivAreaCode").toggle();
    //    $("#DivTollFreePrefix").toggle();
    //    $("#AreaCodeDescription").text('');
    //    $("#VirtualNumber").html('');

    //    var optionId = $("#NumberCountryId").val();
    //    if (optionId === 23) {
    //        $("#TollFreePrefix").val('');
    //        $("#BaseNumberCost").val(10);
    //    }
    //    else {
    //        $("#AreaCode").val('');
    //        $("#BaseNumberCost").val(6);
    //    }

    //    calculateCost();
    //});

    $('#LeasePeriodCode').on("change", function (e) {
        calculateCost();
    });

    $('#CreditPurchaseAmount').on("change", function (e) {
        calculateCost();
    });

    $('#purchaseModalCont').on('shown.bs.modal', function () {
        //generateProductDescription();
        renderPayPalButton();
    });

    calculateCost();
});

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
        NumberCost: $("#NumberCost").val(),
        PayPalCustom: $('#PayPalCustom').val(),
        VirtualNumberId: $("#VirtualNumberId").val(),
        LeasePeriodCode: $("#LeasePeriodCode option:selected").val(),
        LeasePeriodType: $("#LeasePeriodType").val(),
        LeasePeriod: $("#LeasePeriod").val(),
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
    var leasePeriodValues = null;
    var numberCost = 0;
    var creditCost = 0;
    var totalCost = 0;
    var leasePeriodType = "";
    var leasePeriod = 0;
    var availableCredit = 0;
    var remainingCreditAfterPurchase = 0;
    var creditPurchaseAmount = $("#CreditPurchaseAmount");

    if (creditPurchaseAmount.length) {
        creditCost = parseFloat($("#CreditPurchaseAmount").val());
    }

    leasePeriodValues = $("#LeasePeriodCode").val().split("|");
    if (leasePeriodValues.length > 2) {
        leasePeriodType = leasePeriodValues[0];
        leasePeriod = parseFloat(leasePeriodValues[1]);
        numberCost = parseFloat(leasePeriodValues[2]);
    }

    availableCredit = parseFloat($("#AvailableCredit").text().replace("$", ""));
    totalCost = numberCost + creditCost;
    remainingCreditAfterPurchase = availableCredit - numberCost; //.toFixed(2);

    if (totalCost < 0) { totalCost = 0; }

    if (totalCost > 0) {
        $("#TotalCost").text("$" + totalCost.formatMoney());
    }
    else {
        $("#TotalCost").text("");
    }

    $("#hidTotalCost").val(totalCost.formatMoney());
    $("#NumberCost").val(numberCost);
    $("#RemainingCredit").text("$" + remainingCreditAfterPurchase.formatMoney());
    $("#LeasePeriodType").val(leasePeriodType);
    $("#LeasePeriod").val(leasePeriod);

    if (remainingCreditAfterPurchase <= 0) {
        $("#creditOK").hide();
        $("#creditInsufficient").show();
    } else {
        $("#creditOK").show();
        $("#creditInsufficient").hide();
    }
}

//function generateProductDescription() {
//    var leasePeriodValues = null;
//    var purchaseType = $('#PurchaseType').val();
//    var accountId = $('#AccountId').val();
//    var productDesctiption = '';
//    var numberCost = 0;
//    var creditAmount = 0;
//    var leasePeriodType = "";
//    var leasePeriodWord = "";
//    var leasePeriod = 0;
//    var number = $("#VirtualNumber").val();
//    var countryCode = $("#CountryId option:selected").val();
//    var numberType = 'TextPort number ';

//    leasePeriodValues = $("#LeasePeriodCode").val().split("|");
//    if (leasePeriodValues.length > 2) {
//        leasePeriodType = leasePeriodValues[0];
//        leasePeriod = parseFloat(leasePeriodValues[1]);
//        numberCost = parseFloat(leasePeriodValues[2]);

//        switch (leasePeriodType) {
//            case "D":
//                leasePeriodWord = "day";
//                break;
//            case "W":
//                leasePeriodWord = "week";
//                break;
//            case "Y":
//                leasePeriodWord = "year";
//                break;
//            default:
//                leasePeriodWord = "month";
//                break;
//        }
//    }

//    var leasePer = $("#LeasePeriod");
//    if (leasePer.length) {
//        leasePeriod = parseFloat($("#LeasePeriod").val());
//    }

//    var creditPurchaseAmount = $("#CreditPurchaseAmount");
//    if (creditPurchaseAmount.length) {
//        creditAmount = parseFloat($("#CreditPurchaseAmount").val());
//    }

//    if (purchaseType === "VirtualNumber" || purchaseType === "VirtualNumberSignUp") {
//        productDesctiption = numberType + numberToDisplay(number, countryCode) + " " + leasePeriod + " " + leasePeriodWord + " lease - $" + numberCost.formatMoney();
//        if (creditAmount > 0) {
//            productDesctiption = productDesctiption + ". Plus $" + creditAmount.formatMoney() + " TextPort credit";
//        }
//    }
//    else if (purchaseType === "VirtualNumberRenew") {
//        productDesctiption = numberType + numberToDisplay(number, countryCode) + " " + leasePeriod + " " + leasePeriodWord + " lease renewal - $" + numberCost.formatMoney();
//        if (creditAmount > 0) {
//            productDesctiption = productDesctiption + ". Plus $" + creditAmount.formatMoney() + " TextPort credit";
//        }
//    }
//    else if (purchaseType === "Credit") {
//        productDesctiption = "Add " + $("#TotalCost").text() + " TextPort credit";
//    }

//    var payPalCustom = "";
//    if (purchaseType == "Credit") {
//        payPalCustom = purchaseType + "|" + accountId + "|" + creditAmount;
//    }
//    else {
//        payPalCustom = "VMN|" + accountId + "|" + number + "|" + countryCode + "|" + leasePeriod + "|" + creditAmount;
//    }

//    $('#ProductDescription').val(productDesctiption);
//    $('#PayPalCustom').val(payPalCustom);
//}

function registration_complete(purchaseType) {
    var url = '/account/profile';

    if (purchaseType === "VirtualNumber" || purchaseType === "VirtualNumberRenew" || purchaseType === "ComplimentaryNumber") {
        url = '/numbers/';
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
                }
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
                        LeasePeriodCode: $("#LeasePeriodCode option:selected").val(),
                        LeasePeriodType: $("#LeasePeriodType").val(),
                        LeasePeriod: $("#LeasePeriod").val(),
                        NumberCost: $("#NumberCost").val(),
                        TotalCost: $('#hidTotalCost').val(),
                        PayPalCustom: $('#PayPalCustom').val(),
                        PurchaseTitle: $('#PurchaseTitle').val(),
                        ProductDescription: $('#ProductDescription').val(),
                        CreditPurchaseAmount: $('#CreditPurchaseAmount').val(),
                        NumberType: $('#NumberType').val(),
                        FreeTrial: $('#FreeTrial').val()
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
        LeasePeriodCode: $("#LeasePeriodCode option:selected").val(),
        LeasePeriodType: $("#LeasePeriodType").val(),
        LeasePeriod: $("#LeasePeriod").val(),
        NumberCost: $("#NumberCost").val(),
        TotalCost: $('#hidTotalCost').val(),
        PayPalCustom: $('#PayPalCustom').val(),
        PurchaseTitle: $('#PurchaseTitle').val(),
        ProductDescription: $('#ProductDescription').val(),
        CreditPurchaseAmount: $('#CreditPurchaseAmount').val(),
        NumberType: $('#NumberType').val(),
        FreeTrial: $('#FreeTrial').val()
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
    return s + (j ? i.substr(0, j) + t : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + t) + (c ? d + Math.abs(n - i).toFixed(c).slice(2) : "");
};