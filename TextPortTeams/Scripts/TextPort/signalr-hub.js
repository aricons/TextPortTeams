$(function () {
    var inHub = $.connection.inboundHub;
    inHub.client.messageNotification = function (notifyJson) {
        var notification = $.parseJSON(notifyJson);
        var activeDestinationNumber = '';
        // Look for hidActiveDestinationNumber to check whether on messages page. Only display the notification
        // if the destination virtual number matches the ID of the virtual number currintly being viewed.
        var activeDestNum = $("#hidActiveDestinationNumber");
        if (activeDestNum.length && $("#hidActiveVirtualNumberId").val() == notification.VirtualNumberId) {
            activeDestinationNumber = activeDestNum.val();

            // Delete the existing recents item for this number, if it exists, and add a new one. This places the new one at the top.
            var recentMessageItem = $('#recent_' + notification.MobileNumber);
            if (recentMessageItem.length) {
                recentMessageItem.remove();
            }
            var newRecent = "<div id='recent_" + notification.MobileNumber + "' class='chl chat_list new_message'>" + notification.Htmls.RecentHtml + "</div>";
            var recentsList = $('#recents');
            if (recentsList.length) {
                recentsList.prepend(newRecent);
                setChangeDetect();
            }

            if (activeDestinationNumber == '') {
                activeDestinationNumber = notification.MobileNumber;
                $("#hidActiveDestinationNumber").val(activeDestinationNumber);
            }

            // Add notification item (only if this is the active destination number)
            if (notification.MobileNumber == activeDestinationNumber) {
                var msgList = document.getElementById("msg_list");
                var lastMessageItem = $('.msg_item:last').attr('id');

                if (typeof lastMessageItem != 'undefined') {
                    $("#" + lastMessageItem).after(notification.Htmls.MessageHtml);
                }
                else {
                    $("#msg_list").html('');
                    //var msgList = document.getElementById("msg_list");
                    msgList.insertAdjacentHTML("beforeend", notification.Htmls.MessageHtml);
                }

                var ml = $("#msg_list");
                ml.animate({ scrollTop: ml.prop("scrollHeight") - ml.height() }, 50);

                ga('send', 'pageview', '/messages/receive');
            }
        }
        else {
            // Not on messages page, or not the active virtual number. Pop up a note.
            //alert("Not on messages page. New message received: " + notification.MessageText);
            $.jnoty(notification.MessageText, {
                sticky: false,
                header: 'Message received from ' + numberToE164(notification.MobileNumber) + ' to ' + numberToE164(notification.VirtualNumber),
                theme: 'jnoty-info',
                life: 6000
            });
        }
    };

    inHub.client.deliveryReceipt = function (messageId, messageHtml) {
        var msgItem = document.getElementById("td_" + messageId);
        msgItem.insertAdjacentHTML("beforeend", messageHtml);
    };

    inHub.client.balanceUpdate = function (balTxt) {
        $("#balance").html(balTxt);
    };

    // Start the hub.
    $.connection.hub.start().done(function () {
        if ($('#SessionId').length) {
            if ($('#SessionId').val() === "") {
                $("#SessionId").val($.connection.hub.id);
            }
        }
    });

});

function numberToE164(countryCode, number) {
    return number.replace(/\D/g, '');
}

function numberToDisplay(number, countryCode) {
    if (countryCode === "") {
        countryCode = getCountryCodeFromNumber(number);
    }

    if (number.length >= 1) {
        if (countryCode === "1") {
            return '+1 ' + number.substr(1, 3) + ' ' + number.substr(4, 3) + '-' + number.substr(7, 4);
        }
        else if (countryCode === "44") {
            return '+44 ' + number.substr(2, 4) + '-' + number.substr(6);
        }
        else if (countryCode.length && countryCode.length === 2) {
            return "+" + countryCode + " " + number.substr(2);
        }
        else if (countryCode.length && countryCode.length === 3) {
            return "+" + countryCode + " " + number.substr(3);
        }
        else {
            return "+" + number;
        }
    }
    return "";
}

function getCountryCodeFromNumber(number) {
    if (number.length >= 1) {
        number = number.replace(/\D/g, '');
        if (number.substr(0, 1) === "1") {
            return "1";
        }
        else {
            return number.substr(0, 2); // Assumes 2-digit country codes only.
        }
    }
    return "";
}