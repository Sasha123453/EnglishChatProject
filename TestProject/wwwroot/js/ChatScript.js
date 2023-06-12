$(document).ready(function () {
    $("#chat-container").show();

    $("#send-button").click(function () {
        var userInput = $("#message-input").val();
        if (userInput !== "") {
            disableUserInput();
            sendMessage(userInput);
        }
    });
    $("#reset-button").click(function () {
        $.ajax({
            url: '/Chat/ChatReset',
            type: 'POST',
            success: function () {
                window.location.reload();
            },

            error: function (xhr) {
                console.log(xhr.responseText);
            }
        });
    });

    $("#message-input").keypress(function (e) {
        if (e.which === 13) {
            var userInput = $("#message-input").val();
            if (userInput !== "") {
                disableUserInput();
                sendMessage(userInput);
            }
        }
    });

    function disableUserInput() {
        $("#message-input").prop("disabled", true);
        $("#send-button").prop("disabled", true);
    }

    function enableUserInput() {
        $("#message-input").prop("disabled", false);
        $("#send-button").prop("disabled", false);
        $("#message-input").val("");
    }

    function sendMessage(message) {
        $("#chat-body").append(`
                    <p class="user-message">
                        <span class="message-content">
                            ${message}
                        </span>
                    </p>
                `);
        $("#message-input").val("");

        $("#chat-body").append(`
                    <p class="assistant-message typing-indicator">
                       <span class="message-content" style="background-color: rgba(150, 150, 150, 0);"></span>
                    </p>
                `);
        $("#chat-body").scrollTop($("#chat-body")[0].scrollHeight);

        $.ajax({
            url: '/Chat/Chat',
            type: 'POST',
            data: { userInput: message },
            success: function (data) {
                $(".typing-indicator").remove();
                $("#chat-body").append(`
                <p class="assistant-message">
                    <span class="message-content">
                        ${data.message}
                    </span>
                </p>
            `);
                $("#chat-body").scrollTop($("#chat-body")[0].scrollHeight);
                enableUserInput();
            },

            error: function (xhr) {
                console.log(xhr.responseText);
                $(".typing-indicator").remove();
                enableUserInput();
            }
        });
    }

    function GetChatMessages() {
        $.ajax({
            url: '/Chat/GetChatMessages',
            type: 'POST',
            success: function (data) {
                var message;
                for (const message of data.messages) {
                    if (message.rawRole == "user") {
                        $("#chat-body").append(`
                    <p class="user-message">
                        <span class="message-content">
                            ${message.content}
                        </span>
                    </p>
                `);
                    }
                    else if (message.rawRole == "assistant") {
                        $("#chat-body").append(`
                    <p class="assistant-message">
                        <span class="message-content">
                            ${message.content}
                        </span>
                    </p>
                `);
                    }
                }
            },

            error: function (xhr) {
                console.log(xhr.responseText);
            }
        });
    }
    GetChatMessages();
});