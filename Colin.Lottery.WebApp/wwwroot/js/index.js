(function () {
    //建立链接
    var connection = new signalR.HubConnection("/hubs/pk10");






    connection.on("ShowServerTime", function (data) {
        alert(data);
    });

    $("#btn").on("click", function () {
        connection.send("Test");
    })





    //启动链接
    connection.start().then(
            function () {
                //alert("与服务器建立链接成功")
            },
            function () {
                alert("与服务器建立链接失败")
            })
        .catch(error => {
            console.error(error.message);
        });



})()




// var sendForm = document.getElementById("send-form"); 
// var sendButton = document.getElementById("send-button"); 
// var messagesList = document.getElementById("messages-list"); 
// var messageTextBox = document.getElementById("message-textbox"); 




// function appendMessage(content) {
// var li = document.createElement("li"); 
// li.innerText = content; 
// messagesList.appendChild(li); 
// }

// var connection = new signalR.HubConnection("/hubs/chat"); 

// sendForm.addEventListener("submit", function(evt) {
// var message = messageTextBox.value; 
// messageTextBox.value = ""; 
// connection.send("Send", message); 
// evt.preventDefault(); 
// }); 

// connection.on("SendMessage", function(sender, message) {
// appendMessage(sender + ': ' + message); 
// }); 

// connection.on("SendAction", function(sender, action) {
// appendMessage(sender + ' ' + action); 
// }); 

// connection.start().then(function () {
// messageTextBox.disabled = false; 
// sendButton.disabled = false; 
// });