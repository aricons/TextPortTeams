$(function () {
    var inHub = $.connection.inboundHub;
    // Create a function that the hub can call back to display messages.
    inHub.client.messageNotification = function (notifyJson) {
        var notification = $.parseJSON(notifyJson);
        var activeDestinationNumber = '';
        //debugger;
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

    // Start the hub
    $.connection.hub.start();
});

function numberToE164(number) {
    return number.replace("\D", "");
}