package main

import (
	"bufio"
	"encoding/json"
	"math/rand"
	"os"
)

type Coordinates struct {
	X int
	Y int
}

type CellContentType byte

const (
	Tank           CellContentType = 0
	Barrier        CellContentType = 1
	NotDestroyable CellContentType = 2
	Water          CellContentType = 3
	Spawn          CellContentType = 4
)

type CellContentInfo struct {
	Coordinates Coordinates
	HealthCount byte
	Type        CellContentType
	UserId      string
}

type Direction byte

const (
	Up    Direction = 0
	Down  Direction = 1
	Left  Direction = 2
	Right Direction = 3
)

type BulletInfo struct {
	Coordinates Coordinates
	Direction   Direction
	OwnerId     string
}

type GameState struct {
	ContentsInfo []CellContentInfo
	BulletsInfo  []BulletInfo
	ZoneRadius   byte
}

type UserActionType byte

const (
	Move  UserActionType = 0
	Shoot UserActionType = 1
)

type UserAction struct {
	Type      UserActionType
	Direction Direction
}

func main() {
	stdin := bufio.NewScanner(os.Stdin)
	enc := json.NewEncoder(os.Stdout)
	gameState := GameState{}
	for stdin.Scan() {
		json.Unmarshal([]byte(stdin.Text()), &gameState)
		userAction := UserAction{Type: UserActionType(rand.Intn(2)), Direction: Direction(rand.Intn(4))}
		enc.Encode([]UserAction{userAction})
	}
}
