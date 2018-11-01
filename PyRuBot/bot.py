#!/usr/bin/env python3

import sys
import json

while True:
    state = input()
    gameState = json.loads(state)
    sys.stderr.write(state)
    print('[{"Type":1,"Direction":3}]')

