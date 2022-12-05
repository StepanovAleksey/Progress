## Проект по созданию игры "Морской бой" на ASP.Net Core от команды Progress
### Правила игры

#### Правила размещения кораблей (флота)

Игровое поле — квадрат 10×10 у каждого игрока, на котором размещается флот кораблей. 
Горизонтали нумеруются сверху вниз, а вертикали помечаются буквами слева направо.
При этом используются буквы русского алфавита от «а» до «к» (буквы «ё» и «й» пропускаются).

Размещаются:
* 1 корабль — ряд из 4 клеток («четырёхпалубный»; линкор)
* 2 корабля — ряд из 3 клеток («трёхпалубные»; крейсера)
* 3 корабля — ряд из 2 клеток («двухпалубные»; эсминцы)
* 4 корабля — 1 клетка («однопалубные»; торпедные катера)

При размещении корабли не могут касаться друг друга сторонами и углами.

Рядом со «своим» полем размещается «чужое» такого же размера, только пустое. Это участок моря, где плавают корабли противника.

При попадании в корабль противника — на чужом поле ставится крестик, при холостом выстреле — точка. Попавший стреляет ещё раз.

Самыми уязвимыми являются линкор и торпедный катер: первый из-за крупных размеров, в связи с чем его сравнительно легко найти, а второй из-за того, что топится с одного удара, хотя его найти достаточно сложно.

#### Потопление кораблей противника
1. Ожидающий подключения игрок ходит первым.

2. Игрок, выполняющий ход, совершает выстрел — нажимает на поле противника на клетку, в которой, по его мнению, находится корабль противника, например, «В1».

3. * Если выстрел пришёлся в клетку, не занятую ни одним кораблём противника, то следует сообщение «Мимо!» и на чужом квадрате в этом месте появляется точка. Право хода переходит к сопернику.
   * Если выстрел пришёлся в клетку, где находится многопалубный корабль (размером больше чем 1 клетка), то следует сообщение «Ранил!» или «Попал!», кроме одного случая (см. далее).  В этом месте на чужом поле появляется крестик, а у противника появляется крестик на своём поле также в эту клетку. Стрелявший игрок получает право на ещё один выстрел.
    * Если выстрел пришёлся в клетку, где находится катер, или последнюю непоражённую клетку многопалубного корабля, то следует ответ «Убил!» или «Потопил!». У двоих игроков отмечается потопленный корабль на соответтствующем поле. Стрелявший игрок получает право на ещё один выстрел.

4. Победителем считается тот, кто первым потопит все 10 кораблей противника. 

#### Реализация

При открытии стартовой страницы мы имеем: 
* 2 игровых поля размером 10 на 10 в каждая ячейка имеет уникальный атрибут по оси X и оси Y, одно из полей активно (поле «Игрока») и в стартовых позициях размещены корабли, второе – не активное (поле «Противника»); 
*Форму чата;
*Кнопку начала игры;
*Игровое поле представляем как двухмерный массив nullable булевых значений;
Данные между игроками и сервером пересылаем в виде координат цели и ID «Противника» в этом поле.
Размещение кораблей проводим по правилам описанным выше.

После размещения кораблей при нажатии на кнопку «Начать игру»:
* Поле «Игрока» становиться неактивным для перетаскивания (меняем состояние атрибута, на который завязан jquery);
* Генерируем ID для «Игрока» играющего без авторизации;
* Передаем данные о «Игроке» и расположении кораблей на игровом поле на сервер, меняем его статус на готового к игре и ожидающего подключения.

Сервер реализован в качестве веб-сервиса (включает слои BL и DAL), который:
* Ожидает подключения «Игроков» и заносит их в таблицу GAMERS/USERS при регистрации как постоянных игроков или генерирует одноразовые при игре без регистрации  и аутентификации;
* При начале игры сохраняет в базе в таблице BATTELFIELD расстановку кораблей на поле;
* Случайным образом выбирает игрока из подключенных к серверу и находящихся в режиме ожидания и переводит их в статус – В ИГРЕ/ ИГРАЮЩИЙ; 
* Создает запись в таблице GAMES содержащую ID игроков и время начала игры;
* Получив от «Игрока» запрос на игру отправляет ему ID «Противника», на основании которого клиент RabbitMQ будет отправлять сообщения в очередь, на определенный сервер;
* Реализует отслеживание попаданий и промахов путем проверки соответствующей координаты на игровом поле противника, т.е. содержание соответствующей ячейки в массиве в таблице BATTELFIELD;
* Реализует переход хода от «Игрока» к «Противнику» и обратно;
* В случае длительного простоя или разрыва соединения уведомляет игрока о конце игры.

Чат реализован с помощью клиента RabitMQ у каждого пользователя, при начале игры «Игрок» получает ID «Противника» и может отправлять ему сообщения в чат через реализованную форму. По окончании игры, форма чата очищается.
