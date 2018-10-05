function getLotteryRule() {
    let rule = getLastParameter();
    return /^\d+/.test(rule) ? rule : 1;
}