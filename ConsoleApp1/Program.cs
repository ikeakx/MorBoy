using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    using System;
    using System.Collections.Generic;

    namespace SeaBattle
    {
        /// <summary>
        /// Класс Ship - представляет корабль
        /// </summary>
        class Ship
        {
            private string type;
            private int length;
            private int startX, startY;
            private bool isHorizontal;
            private int hits;

            // Конструктор
            public Ship(string shipType, int shipLength, int x, int y, bool horizontal)
            {
                type = shipType;
                length = shipLength;
                startX = x;
                startY = y;
                isHorizontal = horizontal;
                hits = 0;
            }

            // Попадание в корабль
            public bool Hit()
            {
                hits++;
                return hits == length;
            }

            // Получить тип корабля
            public string GetShipType() => type;

            // Получить текущее количество попаданий
            public int GetHits() => hits;

            // Получить длину корабля
            public int GetLength() => length;

            // Получить координату X начала
            public int GetStartX() => startX;

            // Получить координату Y начала
            public int GetStartY() => startY;

            // Проверить ориентацию
            public bool GetIsHorizontal() => isHorizontal;

            // Проверить, уничтожен ли корабль
            public bool IsDestroyed() => hits == length;

            // Вывести статус корабля
            public void DisplayStatus()
            {
                Console.Write($"{type}: {hits} из {length} палуб повреждено");
                if (IsDestroyed())
                {
                    Console.Write(" (УНИЧТОЖЕН)");
                }
                Console.WriteLine();
            }

            // Проверить, принадлежит ли клетка этому кораблю
            public bool OwnsCell(int x, int y)
            {
                if (isHorizontal)
                {
                    return y == startY && x >= startX && x < startX + length;
                }
                else
                {
                    return x == startX && y >= startY && y < startY + length;
                }
            }
        }

        
        class GameField
        {
            private const int SIZE = 10;
            private Ship[,] grid;           // Двумерный массив ссылок на корабли
            private List<Ship> ships;       // Список всех кораблей
            private bool[,] shotGrid;       // Отметки о выстрелах (true - был выстрел)

            // Конструктор
            public GameField()
            {
                grid = new Ship[SIZE, SIZE];
                shotGrid = new bool[SIZE, SIZE];
                ships = new List<Ship>();

                // Инициализация массивов
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = 0; j < SIZE; j++)
                    {
                        grid[i, j] = null;
                        shotGrid[i, j] = false;
                    }
                }
            }

            // Добавление корабля на поле
            public bool AddShip(Ship ship)
            {
                int startX = ship.GetStartX();
                int startY = ship.GetStartY();
                int length = ship.GetLength();
                bool isHorizontal = ship.GetIsHorizontal();

                // Проверка выхода за границы
                if (isHorizontal)
                {
                    if (startX < 0 || startX + length > SIZE || startY < 0 || startY >= SIZE)
                    {
                        Console.WriteLine("Ошибка: корабль выходит за границы поля!");
                        return false;
                    }
                    // Проверка, что все клетки свободны
                    for (int i = 0; i < length; i++)
                    {
                        if (grid[startY, startX + i] != null)
                        {
                            Console.WriteLine($"Ошибка: клетка ({startX + i + 1},{startY + 1}) уже занята!");
                            return false;
                        }
                    }
                    // Установка корабля
                    for (int i = 0; i < length; i++)
                    { 
                        grid[startY, startX + i] = ship;
                    }
                }
                else
                {
                    if (startY < 0 || startY + length > SIZE || startX < 0 || startX >= SIZE)
                    {
                        Console.WriteLine("Ошибка: корабль выходит за границы поля!");
                        return false;
                    }
                    // Проверка, что все клетки свободны
                    for (int i = 0; i < length; i++)
                    {
                        if (grid[startY + i, startX] != null)
                        {
                            Console.WriteLine($"Ошибка: клетка ({startX + 1},{startY + i + 1}) уже занята!");
                            return false;
                        }
                    }
                    // Установка корабля
                    for (int i = 0; i < length; i++)
                    {
                        grid[startY + i, startX] = ship;
                    }
                }

                ships.Add(ship);
                return true;
            }

            // Обработка выстрела
            public bool ReceiveShot(int x, int y)
            {
                // Приводим к индексам массива (0-9)
                int gridX = x - 1;
                int gridY = y - 1;

                // Проверка границ
                if (gridX < 0 || gridX >= SIZE || gridY < 0 || gridY >= SIZE)
                {
                    Console.WriteLine("Координаты вне поля! Введите числа от 1 до 10.");
                    return false;
                }

                // Проверка, не стреляли ли уже сюда
                if (shotGrid[gridY, gridX])
                {
                    Console.WriteLine("Вы уже стреляли в эту клетку!");
                    return false;
                }

                // Отмечаем, что выстрел был
                shotGrid[gridY, gridX] = true;

                // Проверка попадания
                if (grid[gridY, gridX] != null)
                {
                    Ship ship = grid[gridY, gridX];
                    bool destroyed = ship.Hit();
                    Console.WriteLine("ПОПАДАНИЕ!");

                    if (destroyed)
                    {
                        Console.WriteLine($"Корабль \"{ship.GetShipType()}\" уничтожен!");
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("ПРОМАХ!");
                    return false;
                }
            }

            // Проверка, все ли корабли уничтожены
            public bool AllShipsDestroyed()
            {
                foreach (Ship ship in ships)
                {
                    if (!ship.IsDestroyed())
                    {
                        return false;
                    }
                }
                return true;
            }

            // Вывод поля
            public void PrintField(bool hideShips)
            {
                Console.WriteLine("\n    ");
                for (int i = 1; i <= SIZE; i++)
                {
                    Console.Write($"{i,2} ");
                }
                Console.WriteLine();
                Console.WriteLine("    ");
                for (int i = 1; i <= SIZE; i++)
                {
                    Console.Write("---");
                }
                Console.WriteLine();

                for (int y = 0; y < SIZE; y++)
                {
                    Console.Write($"{y + 1,2} | ");
                    for (int x = 0; x < SIZE; x++)
                    {
                        char symbol;

                        if (shotGrid[y, x])
                        {
                            // В эту клетку уже стреляли
                            if (grid[y, x] != null && grid[y, x].IsDestroyed())
                            {
                                symbol = 'X';  // Уничтоженный корабль
                            }
                            else if (grid[y, x] != null)
                            {
                                symbol = '#';  // Подбитый корабль
                            }
                            else
                            {
                                symbol = 'o';  // Промах
                            }
                        }
                        else
                        {
                            if (hideShips)
                            {
                                symbol = '~';  // Вода (скрываем корабли)
                            }
                            else
                            {
                                symbol = grid[y, x] != null ? 'S' : '~';  // Корабль виден или вода
                            }
                        }
                        Console.Write($"{symbol}  ");
                    }
                    Console.WriteLine("|");
                }
                Console.WriteLine();
            }

            // Вывод статуса всех кораблей
            public void PrintShipsStatus()
            {
                Console.WriteLine("\n=== Состояние кораблей ===");
                foreach (Ship ship in ships)
                {
                    Console.Write("  ");
                    ship.DisplayStatus();
                }
                Console.WriteLine();
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                Console.Title = "Морской бой";

                Console.WriteLine("=================================");
                Console.WriteLine("     ДОБРО ПОЖАЛОВАТЬ В ИГРУ     ");
                Console.WriteLine("         МОРСКОЙ БОЙ            ");
                Console.WriteLine("=================================\n");

                // Создание игрового поля
                GameField field = new GameField();

                // Создание и добавление кораблей
                Console.WriteLine("Расстановка кораблей...");

                // Линкор (длина 4) - горизонтально
                Ship battleship = new Ship("Линкор", 4, 0, 0, true);
                if (field.AddShip(battleship))
                {
                    Console.WriteLine("✓ Линкор (4 палубы) добавлен на позицию (1,1)-(4,1)");
                }

                // Крейсер (длина 3) - вертикально
                Ship cruiser = new Ship("Крейсер", 3, 2, 2, false);
                if (field.AddShip(cruiser))
                {
                    Console.WriteLine("✓ Крейсер (3 палубы) добавлен на позицию (3,3)-(3,5)");
                }

                // Эсминец 1 (длина 2) - горизонтально
                Ship destroyer1 = new Ship("Эсминец", 2, 5, 7, true);
                if (field.AddShip(destroyer1))
                {
                    Console.WriteLine("✓ Эсминец (2 палубы) добавлен на позицию (6,8)-(7,8)");
                }

                // Эсминец 2 (длина 2) - вертикально
                Ship destroyer2 = new Ship("Эсминец", 2, 8, 4, false);
                if (field.AddShip(destroyer2))
                {
                    Console.WriteLine("✓ Эсминец (2 палубы) добавлен на позицию (9,5)-(9,6)");
                }

                Console.WriteLine("\nНажмите Enter для продолжения...");
                Console.ReadLine();

                // Показываем поле с кораблями
                ClearScreen();
                Console.WriteLine("=== РАССТАНОВКА КОРАБЛЕЙ (вид для игрока) ===");
                field.PrintField(false);

                Console.WriteLine("Нажмите Enter чтобы начать игру...");
                Console.ReadLine();

                // Игровой цикл
                int shotCount = 0;
                bool gameRunning = true;

                while (gameRunning)
                {
                    ClearScreen();

                    // Показываем поле (скрываем корабли)
                    Console.WriteLine("=== ВАШИ ВЫСТРЕЛЫ ===");
                    field.PrintField(true);

                    // Показываем статус кораблей
                    field.PrintShipsStatus();

                    Console.WriteLine($"Выстрелов сделано: {shotCount}");
                    Console.WriteLine("\nВведите координаты для выстрела (X Y от 1 до 10): ");

                    string input = Console.ReadLine();
                    string[] coordinates = input?.Split(' ');

                    if (coordinates == null || coordinates.Length != 2 ||
                        !int.TryParse(coordinates[0], out int x) ||
                        !int.TryParse(coordinates[1], out int y))
                    {
                        
                        Console.WriteLine("Ошибка: введите два числа через пробел!");
                        Console.WriteLine("Нажмите Enter для продолжения...");
                        Console.ReadLine();
                        continue;
                    }

                    shotCount++;
                    Console.WriteLine($"\nРезультат выстрела по координатам ({x},{y}): ");

                    // Обрабатываем выстрел
                    bool hit = field.ReceiveShot(x, y);

                    // Проверяем окончание игры
                    if (field.AllShipsDestroyed())
                    {
                        ClearScreen();
                        Console.WriteLine("=== ПОСЛЕДНИЙ ВЫСТРЕЛ ===");
                        field.PrintField(true);

                        Console.WriteLine("\n=========================================");
                        Console.WriteLine("          ПОЗДРАВЛЯЕМ! ПОБЕДА!          ");
                        Console.WriteLine("=========================================");
                        Console.WriteLine("Вы уничтожили все корабли противника!");
                        Console.WriteLine($"Количество сделанных выстрелов: {shotCount}");
                        Console.WriteLine("=========================================");
                        gameRunning = false;
                    }
                    else
                    {
                        Console.WriteLine("\nНажмите Enter для следующего выстрела...");
                        Console.ReadLine();
                    }
                }

                Console.WriteLine("\nСпасибо за игру!");
                Console.WriteLine("\nНажмите любую клавишу для выхода...");
                Console.ReadKey();
            }

            // Функция для очистки экрана
            static void ClearScreen()
            {
                Console.Clear();
            }
        }
    }
}
