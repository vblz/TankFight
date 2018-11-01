const linq = require('linq'),
    readline = require('readline'),
    log = console.log,
    rl = readline.createInterface(process.stdin, process.stdout),
    botName = 'jsrubot';

rl.on('line', (state) => {
    let userAction = [];
    try {
    let gameState = JSON.parse(state);
    let myTank = linq.from(gameState).firstOrDefault((c) => c.Type === "Tank" && c.UserId === botName).Coordinates;
    let enemyTank = linq.from(gameState).firstOrDefault((c) => c.Type === "Tank" && c.UserId !== botName).Coordinates;
    }
    catch (e) {
        log(state);
    }
    userAction.push({Type: 1, Direction: 3});
    log(userAction);
});
