// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Toggle chatbox visibility
    $('#chatbox-icon, #chatbox-close').click(function () {
        $('#chatbox').toggleClass('open');
        $('#chatbox-icon').toggle(!$('#chatbox').hasClass('open'));

        // Add introductory message if chatbox is empty
        if ($('#chatbox-messages').children().length === 0) {
            appendMessage('Xin chào! Tôi là AI hỗ trợ luyện thi bằng lái xe. Bạn có thể hỏi tôi bất kỳ điều gì về giao thông. Tôi sẽ sẵn lòng giải đáp thắt mắt của bạn', 'ai');
        }
    });

    // Send message on button click
    $('#chatbox-send').click(function () {
        sendMessage();
    });

    // Send message on Enter key press
    $('#chatbox-input').keypress(function (e) {
        if (e.which === 13) {
            sendMessage();
        }
    });

    function sendMessage() {
        const input = $('#chatbox-input');
        const message = input.val().trim();

        if (message) {
            // Append user message
            appendMessage(message, 'user');
            input.val('');

            // Call API to get AI response
            $.get('/api/ChatBox/get-response', { prompt: message })
                .done(function (response) {
                    appendMessage(response, 'ai');
                })
                .fail(function () {
                    appendMessage('Lỗi khi kết nối với AI. Vui lòng thử lại.', 'ai');
                });
        }
    }

    function appendMessage(message, sender) {
        const messageElement = $('<div>').addClass('chatbox-message').addClass(sender);
        const contentElement = $('<div>').addClass('message-content').text(message);
        messageElement.append(contentElement);
        $('#chatbox-messages').append(messageElement);
        $('#chatbox-messages').scrollTop($('#chatbox-messages')[0].scrollHeight);
    }
});