const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.start().then(() => {
    const ticketId = window.location.pathname.split('/').pop();
    connection.invoke("JoinTicket", ticketId);
});

document.getElementById("message-form").addEventListener("submit", function(e) {
    e.preventDefault();

    const input = document.getElementById("msgText");
    const message = input.value.trim();

    if (!message) return;

    const ticketId = window.location.pathname.split('/').pop();

    // Send message via SignalR (optional: with file upload you'd need to handle that differently)
    connection.invoke("SendMessage", ticketId, message)
        .catch(err => console.error(err.toString()));

    input.value = '';
});

connection.on("ReceiveMessage", (sender, text, fileName, fileUrl, timestamp) => {
    const chatBox = document.getElementById("chat-box");

    const currentUser = '@User.Identity?.Name';
    const isCurrentUser = sender === currentUser;

    const wrapperDiv = document.createElement('div');
    wrapperDiv.classList.add('chat-message-wrapper', isCurrentUser ? 'justify-end' : 'justify-start');

    const messageDiv = document.createElement('div');
    messageDiv.classList.add('chat-message');

    if (!isCurrentUser) {
        const senderDiv = document.createElement('div');
        senderDiv.classList.add('chat-sender', 'text-primary', 'fw-bold');
        senderDiv.textContent = sender;
        messageDiv.appendChild(senderDiv);
    }

    const bubbleDiv = document.createElement('div');
    bubbleDiv.classList.add('chat-bubble');
    bubbleDiv.textContent = text;

    if (fileName && fileUrl) {
        const fileDiv = document.createElement('div');
        fileDiv.classList.add('mt-1');
        const link = document.createElement('a');
        link.href = fileUrl;
        link.target = '_blank';
        link.rel = 'noopener noreferrer';
        link.classList.add('text-decoration-none', 'chat-attachment');
        link.innerHTML = '📎 <strong>' + fileName + '</strong>';
        fileDiv.appendChild(link);
        bubbleDiv.appendChild(fileDiv);
    }

    messageDiv.appendChild(bubbleDiv);

    const timestampDiv = document.createElement('div');
    timestampDiv.classList.add('chat-timestamp', 'text-muted', 'small');
    timestampDiv.textContent = new Date(timestamp).toLocaleString();
    messageDiv.appendChild(timestampDiv);

    wrapperDiv.appendChild(messageDiv);
    chatBox.appendChild(wrapperDiv);

    chatBox.scrollTop = chatBox.scrollHeight;
});
