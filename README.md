# TankFight

## Элементы и механика игры
Цель игры - выжить, управляя танком. Победителем считается последний умерший танк.

Вся игровая деятельность происходит на одной из нескольких карт. Карта поделена на координаты, отсчет координат идет с левого нижнего угла карты, начинается с 0. Карта всегда по периметру ограничена не разрушаемыми барьерами.

![Map image](map_example.png)

На карте расположены разрушаемые и не разрушаемые объекты, в том числе танки игроков.

Объекты бывают следующих типов [Подробнее](#CellContentType):

- Танк
- Барьер
- Не разрушаемый барьер
- Вода

Каждый объект занимает одну ячейку, два объекта не могут находится на одинаковых координатах.

Игроки могут передвигаться и стрелять. В результате стрельбы на карте создается пуля, которая движется с заданной скоростью по направлению стрельбы. У одного игрока не может одновременно быть более одной пули.

При попадании пули в объект, пуля исчезает, а у разрушаемого объекта отнимается жизнь. Не разрушаемые объекты игнорируют пулю.

Разрушаемые объекты имеют счетчик жизней и считаются живыми, пока их счетчик жизней превышает значение 0. После того, как их счетчик жизни опускается до 0, объекты удаляются с карты.

Так же танки получают урон от зоны. Зона представляет область, ограниченную радиусом зоны. Все танки, находящиеся за пределами радиуса зоны получают урон размером в одну жизнь за ход. Радиус зоны уменьшается на один каждый ход.

## Игра

Игра происходит по ходам. Между ходами возможно получать состояние игры:

1. Информацию о расположении и жизнях всех объектов.
2. Информацию расположении и направлении движения пуль.

Ход игрока представляет из себя набор действий (на текущий момент набор имеет размер 1). Каждое действие — это либо выстрел, либо движение. У обоих типов действий требуется передавать направление действия: вверх, вниз, влево либо вправо.

Действия всех игроков применяются в случайном порядке в рамках хода.

Обработка событий происходит в цикле из действий игроков и обработки пуль: проверяются попадания, затем производится передвижение пуль, затем опять проверяются попадания. На каждое действие пользователей пуля движется на определенное количество клеток, устанавливаемое в настройках (2 на текущий момент).

В случае некорректного действия игрока (передвижение на занятую клетку, попытка создать вторую пулю) это действие игнорируется.

## Запуск игры

Для работы игры необходим установленный docker и docker-compose.

Игра распространяется в виде docker-compose файла, содержащего сервисы, необходимые для игры и пример пары ботов.

Для запуска необходимо любым способом скачать файл [docker-compose.yml](https://github.com/vblz/TankFight/blob/master/docker-compose.yml) в консоли перейти в папку, содержащую этот файл и ввести команду `docker-compose up`. Команда произведёт автоматическое скачивание необходимых для игры образов и запустит их с настроенными параметрами.

**Для пользователей Windows**

Существует [баг на текущей стабильной версии docker for win](https://github.com/docker/for-win/issues/1829), в связи с чем, им так же надо скачать файл [.env](https://github.com/vblz/TankFight/blob/master/.env) и разместить его в одной папке с `docker-compose.yml`.

После старта всех необходимых сервисов, для запуска игры необходимо открыть страничку http://localhost:5006/. На этой странице располагается окно игры. Для запуска игры необходимо ввести имена образов ботов (образы, введение по умолчанию уже скачаны при запуске команды `docker-compose up`) и нажать кнопку Start. Битва запустится.
Для остановки сервисов игры можно закрыть консоль.

### Сборка собственного бота

Для примера произведем сборку бота со своим именем. Для примера сборку произведем над исходниками [RandomBot](https://github.com/vblz/TankFight/tree/master/RandomBot).

Для этого необходимо скачать любым способом папку с ботом и сделать в ней необходимые изменения для игры (например, реализовать игровую логику). После того, как появилась уверенность, что c# проект собирается, можно приступить к сборке docker образа.

Для сборки образа docker необходимо открыть консоль и перейти в ней в папку с ботом.

Для примера, дадим боту тег `my_name/mega_destructor`. Для этого выполним команду

`docker build -t my_name/mega_destructor .`

которая запустит сборку образа в контексте текущей папке, используя Dockerfile, располагающийся в текущей папке.
После успешной сборки, можно запустить битву (перезапускать сервисы нет необходимости, достаточно обновить страницу с игрой), ввести имя только что собранного контейнера и следить за ходом боя.

### Управление игрой

Во время проведения боя действуют следующие горячие клавиши:

`a` - включить/отключить автоматическое проигрывание

`space` - отключить автоматическое проигрывание и перейти на следующий ход

`n` - переключить состояние игры на ход вперед

`b` - переключить состояние игры на ход назад

`lshift+n` - переключить состояние игры на 10 ходов вперед

`lshift+b` - переключить состояние игры на 10 ходов назад

## Написание бота

Бот представляет из себя docker-образ, содержащий программу, взаимодействую с игрой посредством консоли (stdin, stdout). В stdin пишется текущее состояние игры, и ожидается в ответ набор действий пользователя в течении ограниченного времени. Команды и состояние игры разделяются символами новой строки (`\n`).

В случае нарушения формата ответа, превышения допустимого времени ответа, отсутствия ответа, выполнение приложения завершается, танк игрока перестает выполнять какие-либо действия.

Перед начало игры всем ботам дается некоторое время на "прогрев" - бот запускается, но ему не поступают команды.

Взаимодействие происходит с помощью объектов, сериализованных в формате JSON.

### Состояние игры

В stdin пишется состояние игры, сериализованный объект типа:

```json
{
    "ContentsInfo": [CellContentInfo,CellContentInfo],
    "BulletsInfo": [BulletInfo, BulletInfo],
    "ZoneRadius": 119
}
```

Он содержит информацию обо всех объектах на карте, пулях и радиусе зоны.

Поле `ZoneRadius` указывает текущий размер зоны.

Поле `BulletsInfo` содержит информацию обо всех пулях на карте. Информация представлена в формате

```json
{
    "Coordinates":{"X":17,"Y":3},
    "Direction":2,
    "OwnerId":"vblz/RandomBot"
}
```

где
`Coordinates` - координаты пули на текущий ход,

`Direction` - направление движения пули,

`OwnerId` - идентификатор владельца пули.

Поле `ContentsInfo` содержит информацию о объектах на карте в формате

```json
{
    "Coordinates":{"X":0,"Y":0},
    "HealthCount":255,
    "Type":2,
    "UserId":"r2"
}
```

где

`Coordinates` - координаты объекта на текущий ход,

`HealthCount` - количество жизней объекта
`Type` - [тип объекта](#CellContentType),

`UserId` - идентификатор танка, присутствует только у танков, у остальных объектов это поле следует игнорировать. Так же это поле может отсутствовать в JSON.

[Пример полного состояния игры](#Пример-JSON-состояния-игры)

### Ход игрока

Для совершения хода игрок отправляет **ASCII** строку с массивом своих действий. На текущий момент ход игрока состоит из одного действия фомата

```json
[UserAction]
```

где
`UserAction` действие игрока.

Формат `UserAction` действия игрока

```json
{
    "Type": 0,
    "Direction": 0
}
```

где
`Type` - [тип хода игрока](#UserActionType),

`Direction` - [направление хода игрока](#Direction).

[Примеры хода игрока](#Примеры-JSON-хода-игрока)

### Ограничения, накладываемые на бота

Время прогрева (от старта контейнера до первого хода): 20 секунд

Память, доступная контейнеру: 256mb

Время на ответ ходом игрока: 100 ms

#### Требования к образу Docker

В образе бота должен быть указан способ связи с автором для награждения. Этот раздел будет дополнен.

### Используемые перечисления

#### Direction

```cson
enum Direction
{
    Up = 0,
    Down = 1,
    Left = 2,
    Right = 3
}
```

#### UserActionType

```c#
enum UserActionType
{
	Move = 0,
	Shoot = 1
}
```

### CellContentType

```c#
enum CellContentType
	{
		Tank = 0,
		Barrier = 1,
		NotDestroyable = 2,
		Water = 3,
		Spawn = 4
}
```

### Примеры сообщений
#### Пример JSON состояния игры
```json
{"ContentsInfo":[{"Coordinates":{"X":0,"Y":0},"HealthCount":255,"Type":2},{"Coordinates":{"X":1,"Y":0},"HealthCount":255,"Type":2},{"Coordinates":{"X":2,"Y":0},{"X":3,"Y":2},"HealthCount":1,"Type":1},{"Coordinates":{"X":11,"Y":2},"HealthCount":255,"Type":2},{"Coordinates":{"X":16,"Y":2},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":2},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":3},"HealthCount":255,"Type":2},{"Coordinates":{"X":7,"Y":3},"HealthCount":2,"Type":1},{"Coordinates":{"X":11,"Y":3},"HealthCount":255,"Type":2},{"Coordinates":{"X":20,"Y":3},"HealthCount":3,"Type":0,"UserId":"r1"},{"Coordinates":{"X":21,"Y":3},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":4},"HealthCount":255,"Type":2},{"Coordinates":{"X":1,"Y":4},"HealthCount":255,"Type":2},{"Coordinates":{"X":5,"Y":4},"HealthCount":1,"Type":1},{"Coordinates":{"X":10,"Y":4},"HealthCount":255,"Type":2},{"Coordinates":{"X":11,"Y":4},"HealthCount":255,"Type":2},{"Coordinates":{"X":14,"Y":4},"HealthCount":1,"Type":1},{"Coordinates":{"X":21,"Y":4},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":5},"HealthCount":255,"Type":2},{"Coordinates":{"X":1,"Y":5},"HealthCount":255,"Type":2},{"Coordinates":{"X":2,"Y":5},"HealthCount":255,"Type":2},{"Coordinates":{"X":11,"Y":5},"HealthCount":255,"Type":2},{"Coordinates":{"X":20,"Y":5},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":5},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":6},"HealthCount":255,"Type":2},{"Coordinates":{"X":1,"Y":6},"HealthCount":255,"Type":2},{"Coordinates":{"X":2,"Y":6},"HealthCount":255,"Type":2},{"Coordinates":{"X":7,"Y":6},"HealthCount":255,"Type":3},{"Coordinates":{"X":8,"Y":6},"HealthCount":255,"Type":3},{"Coordinates":{"X":9,"Y":6},"HealthCount":255,"Type":3},{"Coordinates":{"X":19,"Y":6},"HealthCount":255,"Type":2},{"Coordinates":{"X":20,"Y":6},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":6},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":7},"HealthCount":255,"Type":2},{"Coordinates":{"X":1,"Y":7},"HealthCount":255,"Type":2},{"Coordinates":{"X":8,"Y":7},"HealthCount":255,"Type":3},{"Coordinates":{"X":9,"Y":7},"HealthCount":255,"Type":3},{"Coordinates":{"X":16,"Y":7},"HealthCount":1,"Type":1},{"Coordinates":{"X":19,"Y":7},"HealthCount":255,"Type":2},{"Coordinates":{"X":20,"Y":7},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":7},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":8},"HealthCount":255,"Type":2},{"Coordinates":{"X":3,"Y":8},"HealthCount":1,"Type":1},{"Coordinates":{"X":9,"Y":8},"HealthCount":255,"Type":3},{"Coordinates":{"X":17,"Y":8},"HealthCount":2,"Type":1},{"Coordinates":{"X":20,"Y":8},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":8},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":9},"HealthCount":255,"Type":2},{"Coordinates":{"X":13,"Y":9},"HealthCount":255,"Type":2},{"Coordinates":{"X":14,"Y":9},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":9},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":10},"HealthCount":255,"Type":2},{"Coordinates":{"X":5,"Y":10},"HealthCount":255,"Type":2},{"Coordinates":{"X":7,"Y":10},"HealthCount":2,"Type":1},{"Coordinates":{"X":13,"Y":10},"HealthCount":255,"Type":2},{"Coordinates":{"X":14,"Y":10},"HealthCount":255,"Type":2},{"Coordinates":{"X":15,"Y":10},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":10},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":11},"HealthCount":255,"Type":2},{"Coordinates":{"X":5,"Y":11},"HealthCount":255,"Type":2},{"Coordinates":{"X":6,"Y":11},"HealthCount":255,"Type":2},{"Coordinates":{"X":15,"Y":11},"HealthCount":255,"Type":2},{"Coordinates":{"X":16,"Y":11},"HealthCount":255,"Type":2},{"Coordinates":{"X":18,"Y":11},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":11},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":1,"Y":12},"HealthCount":3,"Type":0,"UserId":"r2"},{"Coordinates":{"X":4,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":5,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":6,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":7,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":16,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":17,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":18,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":19,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":20,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":12},"HealthCount":255,"Type":2},{"Coordinates":{"X":0,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":1,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":2,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":3,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":4,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":5,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":6,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":7,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":8,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":9,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":10,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":11,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":12,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":13,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":14,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":15,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":16,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":17,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":18,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":19,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":20,"Y":13},"HealthCount":255,"Type":2},{"Coordinates":{"X":21,"Y":13},"HealthCount":255,"Type":2}],"BulletsInfo":[{"Coordinates":{"X":17,"Y":3},"Direction":2,"OwnerId":"r1"}],"ZoneRadius":94}
```

Примеры JSON хода игрока

```json
[{"Type":1,"Direction":3}]
```
```json
[{"Type":0,"Direction":0}]
```