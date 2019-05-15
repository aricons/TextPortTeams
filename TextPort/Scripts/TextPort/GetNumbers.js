$(function () {
    $.ajaxSetup({
        cache: false
    });

    $('#AreaCode').on("keyup", function (e) {
        var areaCode = $(this).val();
        if (areaCode.length === 3) {
            getAreaCodeName(areaCode);
        } else {
            $("#VirtualNumber").html('');
            $("#AreaCodeDescription").text('');
        }
    });

    $('#btnContinue').on("click", function (e) {
        if ($("#form-signup").valid()) {
            var purchaseType = $('#PurchaseType').val();
            if (purchaseType === "ComplimentaryNumber") {
                submitPurchase();
            }
            else {
                showConfirmationModal("purchaseModalCont", "/Account/Purchase");
            }
        }
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

function getAreaCodeName(areaCode) {
    var url = '/numbers/getareacodename';
    $.getJSON(url, {
        areaCode: areaCode
    }, function (response) {
        if (response !== "") {
            $("#AreaCodeDescription").text(response);
            getAvailableNumbers(areaCode);
        } else {
            $("#AreaCodeDescription").text("Invalid area code");
        }
    });
}

function getAvailableNumbers(areaCode) {
    var url = '/Numbers/GetAvailableNumbers';
    var numbers = "";
    $("#VirtualNumber").html("");
    $("#spintiller1").show();
    $.getJSON(url, {
        areaCode: areaCode
    }, function (response) {
        $.each(response, function (index, item) {
            numbers += "<option value='" + item.Value + "'>" + item.Text + "</option>";
        });
        $("#VirtualNumber").html(numbers);
        $("#spintiller1").hide();
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
    var cost = 0;
    var creditCost = 0;
    var countryRate = 6;
    var leasePeriod = 0;
    var purchaseType = $("#PurchaseType").val();


    if (purchaseType === "Credits") {
        creditCost = parseFloat($("#CreditPurchaseAmount").val());

        $("#TotalCost").text('$' + $("#CreditPurchaseAmount").val() + ".00");
        $("#hidTotalCost").val(creditCost);
    }
    else {
        leasePeriod = parseFloat($("#LeasePeriod").val());
        if (purchaseType !== "ComplimentaryNumber") {
            cost = leasePeriod * countryRate;
        }

        $("#TotalCost").text("$" + cost + ".00");
        $("#hidTotalCost").val(cost);
    }
}

function registration_complete(purchaseType) {
    var url = '/account/profile';

    if (purchaseType === "VirtualNumber" || purchaseType === "VirtualNumberRenew" || purchaseType === "ComplimentaryNumber") {
        url = '/numbers/manage';
    }
    else if (purchaseType === "Credits") {
        url = '/account/balance';
    }

    window.location.href = url;
}

function renderPayPalButton() {

    var purchaseType = $('#PurchaseType').val();
    var productDesctiption = $('#ProductDescription').val();

    if (purchaseType === "VirtualNumber") {
        productDesctiption = "TextPort virtual number " + $("#VirtualNumber").val() + " " + $("#LeasePeriod option:selected").val() + " month lease."; // Cost: " + $("#TotalCost").text();
    }
    else if (purchaseType === "VirtualNumberRenew") {
        productDesctiption = "TextPort virtual number " + $("#VirtualNumber").val() + " " + $("#LeasePeriod option:selected").val() + " month lease renewal."; // Cost: " + $("#TotalCost").text();
    }
    else if (purchaseType === "Credits") {
        productDesctiption = "Add " + $("#TotalCost").text() + " TextPort credit.";
    }

    paypal.Button.render({
        env: 'sandbox', // sandbox | production
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
            sandbox: 'Ac6TTAGnmNme-wJJSdgU6rm8SSyW5nSc757nHhsqNWDz3X7lOa8Yx3eE-96JK-Z2YbN3N54PE_oRRGbO',
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
                            description: productDesctiption,
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
                        PurchaseType: purchaseType,
                        AccountId: $('#AccountId').val(),
                        UserName: $("#UserName").val(),
                        EmailAddress: $("#EmailAddress").val(),
                        Password: $("#Password").val(),
                        NumberCountryId: $("#NumberCountryId").val(),
                        AreaCode: $("#AreaCode").val(),
                        VirtualNumber: $("#VirtualNumber").val(),
                        VirtualNumberId: $("#VirtualNumberId").val(),
                        LeasePeriod: $("#LeasePeriod option:selected").val(),
                        NumberCost: $("#NumberCost").val(),
                        TotalCost: $('#hidTotalCost').val(),
                        PayPalCustom: $('#PayPalCustom').val(),
                        PurchaseTitle: $('#PurchaseTitle').val(),
                        ProductDescription: $('#ProductDescription').val(),
                        CreditPurchaseAmount: $('#CreditPurchaseAmount').val()
                    };

                    var url = '/account/postpurchase';
                    //if (purchaseType === "VirtualNumberSignUp") {
                    //    url = '/account/postsignuppurchase?id=' + $('#AccountId').val();
                    //} else if (purchaseType === "VirtualNumber") {
                    //    url = '/account/postnumberpurchase';
                    //}
                    //var url = '@Url.Action("PostPurchase", "Account")' + '?id=' + $('#AccountId').val();
                    //var url = '/account/postsignuppurchase?id=' + $('#AccountId').val();
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

    var purchaseType = $('#PurchaseType').val();
    var regData = {
        PurchaseType: purchaseType,
        AccountId: $('#AccountId').val(),
        UserName: $("#UserName").val(),
        EmailAddress: $("#EmailAddress").val(),
        Password: $("#Password").val(),
        NumberCountryId: $("#NumberCountryId").val(),
        AreaCode: $("#AreaCode").val(),
        VirtualNumber: $("#VirtualNumber").val(),
        VirtualNumberId: $("#VirtualNumberId").val(),
        LeasePeriod: $("#LeasePeriod option:selected").val(),
        NumberCost: $("#NumberCost").val(),
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
            //$('#purchaseModalCont').modal('toggle');
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