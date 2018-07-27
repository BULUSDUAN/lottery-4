function getLotteryRule() {
    let rule = getLastParameter();
    return /^\d+/.test(rule) ? rule : 1;
}

function notify(title, message, level) {
    $.notify({
        title: "<strong>" + title + "</strong>",
        message: message
    }, {
            type: level
        });
}