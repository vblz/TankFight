const linq = require('linq'),
    readline = require('readline'),
    log = console.log,
    rl = readline.createInterface(process.stdin, process.stdout),
    botName = 'vblz/tanks:jsrubot';

const UserActionType = { Move: 0, Shoot: 1},
    Direction = {Left: 0, Up: 1, Right: 2, Down: 3},
    CellType = {Tank: 0, Barrier: 1, NotDestroyable: 2, Water: 3};

rl.on('line', (state) => {
    let userAction = [];
    let gameState = JSON.parse(state);
    let myTank = linq.from(gameState.ContentsInfo).firstOrDefault((c) => c.Type === CellType.Tank && c.UserId === botName).Coordinates;
    let enemyTank = linq.from(gameState.ContentsInfo).firstOrDefault((c) => c.Type === CellType.Tank && c.UserId !== botName).Coordinates;
    let dX = myTank.X - enemyTank.X;
    let dY = myTank.Y - enemyTank.Y;

    let direction = (Math.random() > 0.7) ? (Math.floor(Math.random() * 4)) : (Math.abs(dX) > Math.abs(dY)
        ? (dX < 0 ? Direction.Right : Direction.Left)
        : (dY < 0 ? Direction.Up : Direction.Down));

    let action = (Math.random() > 0.5) ? (UserActionType.Move) : (UserActionType.Shoot);

    userAction.push({Type: action, Direction: direction});
    log(userAction);
});
