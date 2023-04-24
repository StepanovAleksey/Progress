"use strict";

function GetData()
{
    //Получаем текущее дату и время
    var Data = new Date();
    var Year = Data.getFullYear();
    var Month = Data.getMonth();
    var Day = Data.getDate();
    var Hour = Data.getHours();
    var Minutes = Data.getMinutes();
    var Seconds = Data.getSeconds();
    //// Преобразуем месяца
    switch (Month) {
        case 0: var fMonth = "января"; break;
        case 1: var fMonth = "февраля"; break;
        case 2: var fMonth = "марта"; break;
        case 3: var fMonth = "апреля"; break;
        case 4: var fMonth = "мае"; break;
        case 5: var fMonth = "июня"; break;
        case 6: var fMonth = "июля"; break;
        case 7: var fMonth = "августа"; break;
        case 8: var fMonth = "сентября"; break;
        case 9: var fMonth = "октября"; break;
        case 10: var fMonth = "ноября"; break;
        case 11: var fMonth = "декабря"; break;
    }
    return Day + " " + fMonth + " " + Year + " года " + Hour + ":" + Minutes + ":" + Seconds;
}


var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:6001/chatHub", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
}).withAutomaticReconnect().build();
    

var myName = "";

//Отключаем кнопку отправки при старте до установки соединенния
document.getElementById("sendButton").disabled = true;

//Обработка нажатия кнопки на создание группового чата.
document.getElementById("chatOnBtn").addEventListener("click", function (event)
    {
    this.hidden = true;
    this.disabled = true;
    document.getElementById("chatOffBtn").disabled = false;
    document.getElementById("chatOffBtn").hidden = false;
    connection.invoke("AddToGroup").catch(function (err)
        {
            return console.error(err.toString());
        });
        event.preventDefault();
});



//Обработка нажатия кнопки на выхода из группового чата.
document.getElementById("chatOffBtn").addEventListener("click", function (event) {
    document.getElementById("chatOffBtn").hidden = true;
    document.getElementById("chatOffBtn").disabled = true;
    document.getElementById("chatOnBtn").disabled = false;
    document.getElementById("chatOnBtn").hidden = false;
    connection.invoke("RemoveFromGroup").catch(function (err)
    {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("Waiting", function (message) {
    var div1 = document.createElement("div");
    div1.className = "d-flex align-items-center";
    div1.id = "snipperDiv";
    var s = document.createElement("strong");
    var divInDiv = document.createElement("div");
    divInDiv.className = "spinner-border ms-auto";
    divInDiv.role = "status";
    s.innerText = message;
    document.getElementById("messagesList").appendChild(div1);
    div1.appendChild(s);
    div1.appendChild(divInDiv);
});

connection.on("SendToGroup", function (user, connected, message) {
      
        if (user == myName) { user = "Вы"; }
        var div2 = document.createElement("div");
        div2.className = "d-flex flex-row justify-content-center";
        var divInDiv2 = document.createElement("div");
        var p3 = document.createElement("p");
        var p1 = document.createElement("p");
        p1.className = "small mb-1 text-muted";
        if (connected == "true") {
            document.getElementById("messagesList").innerHTML="";
            p3.className = "small p-2 mb-3 text-white rounded-3 bg-success";
            document.getElementById("sendButton").disabled = false;
            document.getElementById("messageInput").disabled = false;
        } else if (connected == "false") {
            document.getElementById("messagesList").innerHTML = "";
            p3.className = "small p-2 mb-3 text-white rounded-3 bg-danger";
            document.getElementById("sendButton").disabled = true;
            document.getElementById("messageInput").disabled = true;
            document.getElementById("messageInput").value = "";
            document.getElementById("chatOffBtn").disabled = true;
            document.getElementById("chatOffBtn").hidden = true;
            document.getElementById("chatOnBtn").disabled = false;
            document.getElementById("chatOnBtn").hidden = false;
        }
        // Вывод        
        p1.innerText = GetData();
        p3.innerText = user + message;
        document.getElementById("messagesList").appendChild(div2);
        div2.appendChild(divInDiv2);
        divInDiv2.appendChild(p1);
        divInDiv2.appendChild(p3);  
});

//обрабатываем нажатие на кнопку
connection.on("ReceiveMessage", function (user, message) {
    if (user == myName) {
        var div1 = document.createElement("div");
        div1.className = "d-flex justify-content-between";
        var div2 = document.createElement("div");
        div2.className = "d-flex flex-row justify-content-end mb-4 pt-1";
        var divInDiv2 = document.createElement("div");
        var p3 = document.createElement("p");
        var p1 = document.createElement("p");
        var p2 = document.createElement("p");
        var img = document.createElement("img");
        img.src = "/image/avatar/boy.png";
        img.style = "width: 45px; height: 100%;";
        img.alt = "imag1";
        p1.className = "small mb-1 text-muted";
        p2.className = "small mb-1";
        p3.className = "small p-2 me-3 mb-3 text-white rounded-3 bg-warning";
        // Вывод
        p1.innerText = GetData();
        p2.innerText = user;
        p3.innerText = message;
        document.getElementById("messagesList").appendChild(div1);
        document.getElementById("messagesList").appendChild(div2);
        div1.appendChild(p1);
        div1.appendChild(p2);
        div2.appendChild(divInDiv2);
        divInDiv2.appendChild(p3);
        div2.appendChild(img);
    }
    else
    {
        var div1 = document.createElement("div");
        div1.className = "d-flex justify-content-between";
        var div2 = document.createElement("div");
        div2.className = "d-flex flex-row justify-content-start";
        var divInDiv2 = document.createElement("div");
        var p3 = document.createElement("p");
        var p1 = document.createElement("p");
        var p2 = document.createElement("p");
        var img = document.createElement("img");
        img.src = "/image/avatar/man-1.png";
        img.style = "width: 45px; height: 100%;";
        img.alt = "imag2";
        p1.className = "small mb-1 text-muted";
        p2.className = "small mb-1";
        p3.className = "small p-2 ms-3 mb-3 rounded-3";
        p3.style = "background-color: #f5f6f7;";
        // Вывод
        p1.innerText = GetData();
        p2.innerText = user;
        p3.innerText = message;
        document.getElementById("messagesList").appendChild(div1);
        document.getElementById("messagesList").appendChild(div2);
        div1.appendChild(p2);
        div1.appendChild(p1);
        div2.appendChild(img);
        div2.appendChild(divInDiv2);
        divInDiv2.appendChild(p3);
       
    }
});

connection.start().then(function () {
     //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {    
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = myName;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("Notify", function (message) {
    var span = document.createElement("p");
    span.className = "small p-1 mb-0 text-white rounded-3 bg-success";
    span.innerText = "Hello " + message;
    document.getElementById("namePill").appendChild(span);
    myName = message;
});



document.getElementById("connectButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("connectionInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});




