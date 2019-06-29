/* jshint browser: true */
/* jshint node: true */
"use strict";
var connection;
document.addEventListener('DOMContentLoaded', function () {
    connection = new signalR.HubConnectionBuilder().withUrl("/myhub")
    .configureLogging(signalR.LogLevel.Information)
    .build();


    connection.on("receivemessage", function (message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        msg=msg.replace(/(\n)+/g, '<BR />');
        document.getElementById("CurrentlyProcessedUrl").innerHTML=msg;
    });

    connection.on("failedurlmessage", function (message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");

        var li=document.createElement("li");
        li.className="list-group-item";
        li.textContent=msg;
        document.getElementById("FailedUrls").appendChild(li);
    });

    connection.start().then(function(){
        console.log("connected");
    }).catch(function (err) {
        return console.error(err.toString());
    });

});
function sendStartMsg(msg) {
    connection.invoke("ClientTriggerToCrawlUrl", msg).catch(err => console.error(err.toString()));
    return false;
}
function sendStopMsg() {
    connection.invoke("ClientStopCrawl").catch(err => console.error(err.toString()));
    return false;
}