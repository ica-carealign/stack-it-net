/// <reference path="~/Scripts/jquery.signalR-2.2.0.js" />
$(function() {
    "use strict";

    var eventBus = $.connection.eventBus;

    $.extend(eventBus.client, {
        // Add client-side hub methods that the server will call
        // Corresponds to IEventSubscriber
        onEvent: function(e) {
            console.log(e);
        }
    });

    // Lets the server know that there's a connected user, so it should try to keep the AWS data fresh.
    window.setInterval(eventBus.server.heartbeat, 5000);

    $.connection.hub.error(function(error) {
        console.log('SignalR error: ' + error);
    });

    // Start the connection
    $.connection.hub.start();
});