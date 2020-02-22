#!/usr/bin/env python3
import random
import json

botName = 'vblz/tanks:pyrubot'

while True:
    state = input()
    gameState = json.loads(state)
    myTank = next(c for c in gameState["ContentsInfo"] if c["Type"] == 0 and c["UserId"] == botName)["Coordinates"]
    enemyTank = next(c for c in gameState["ContentsInfo"] if c["Type"] == 0 and c["UserId"] != botName)["Coordinates"]
    dX = myTank["X"] - enemyTank["X"]
    dY = myTank["Y"] - enemyTank["Y"]
    if random.randint(0, 100) < 70:
        direction = ((0 if dX < 0 else 2) if (abs(dX) > abs(dY)) else (1 if dY < 0 else 3))
    else:
        direction = (random.randint(0, 3))

    print([{'Type': 0 if random.randint(0, 100) < 70 else 1, 'Direction': direction}])

