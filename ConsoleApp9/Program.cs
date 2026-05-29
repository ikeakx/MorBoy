using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    using System;

    class Program
    {
        static void Main()
        {
            Game g = new Game();
            g.Play();
        }
    }

    class Game
    {
        Board p = new Board();
        Board c = new Board();
        Random r = new Random();
        bool turn = true;

        public void Play()
        {
            Console.Clear();
            Console.WriteLine("MORBOY");
            p.SetManual();
            c.SetAuto();

            while (p.HasShips() && c.HasShips())
            {
                Draw();
                if (turn)
                {
                    bool ok = false;
                    while (!ok)
                    {
                        Console.Write("ty: ");
                        string s = Console.ReadLine().ToUpper();
                        if (s.Length < 2) continue;
                        int x = s[0] - 'A';
                        int y = int.Parse(s[1].ToString()) - 1;
                        if (x < 0 || x > 9 || y < 0 || y > 9) continue;

                        if (c.already[x, y])
                        {
                            Console.WriteLine("bilo");
                            continue;
                        }

                        if (c.Shoot(x, y))
                        {
                            Console.WriteLine("popal");
                            turn = true;
                            ok = true;
                            if (c.IsDead(x, y))
                            {
                                Console.WriteLine("ubil");
                                c.MarkDead(x, y);
                                Draw();
                            }
                        }
                        else
                        {
                            Console.WriteLine("mimo");
                            turn = false;
                            ok = true;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("comp hodit...");
                    System.Threading.Thread.Sleep(800);
                    bool ok = false;
                    while (!ok)
                    {
                        int xx = r.Next(10);
                        int yy = r.Next(10);
                        if (p.already[xx, yy]) continue;

                        if (p.Shoot(xx, yy))
                        {
                            Console.WriteLine("comp popal v " + (char)('A' + xx) + (yy + 1));
                            turn = false;
                            ok = true;
                            if (p.IsDead(xx, yy))
                            {
                                Console.WriteLine("comp ubil");
                                p.MarkDead(xx, yy);
                                Draw();
                            }
                        }
                        else
                        {
                            Console.WriteLine("comp mimo v " + (char)('A' + xx) + (yy + 1));
                            turn = true;
                            ok = true;
                        }
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }

            Draw();
            if (!p.HasShips()) Console.WriteLine("loh");
            else Console.WriteLine("win");
            Console.ReadKey();
        }

        void Draw()
        {
            Console.Clear();
            Console.WriteLine("tvoi comp");
            for (int i = 0; i < 10; i++)
            {
                string a = p.GetLine(i, true);
                string b = c.GetLine(i, false);
                Console.WriteLine(a + "        " + b);
            }
        }
    }

    class Board
    {
        int[,] map = new int[10, 10];
        int[,] hits = new int[10, 10];
        public bool[,] already = new bool[10, 10];
        int[,] shipId = new int[10, 10];
        int nextId = 1;
        Random r = new Random();

        public void SetManual()
        {
            int[] len = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            foreach (int l in len)
            {
                DrawSelf(true);
                Console.WriteLine("stav " + l);
                string s = Console.ReadLine().ToUpper();
                string[] parts = s.Split(' ');
                int x = parts[0][0] - 'A';
                int y = int.Parse(parts[0][1].ToString()) - 1;
                bool hor = parts[1] == "H";
                int id = nextId++;
                for (int i = 0; i < l; i++)
                {
                    if (hor)
                    {
                        map[x + i, y] = 1;
                        shipId[x + i, y] = id;
                    }
                else
                    {
                        map[x, y + i] = 1;
                        shipId[x, y + i] = id;
                    }
                }
            }
        }

        public void SetAuto()
        {
            int[] len = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            foreach (int l in len)
            {
                bool ok = false;
                while (!ok)
                {
                    int x = r.Next(10);
                    int y = r.Next(10);
                    bool h = r.Next(2) == 0;
                    if (h && x + l <= 10)
                    {
                        bool free = true;
                        for (int i = 0; i < l; i++) if (map[x + i, y] != 0) free = false;
                        if (free)
                        {
                            int id = nextId++;
                            for (int i = 0; i < l; i++)
                            {
                                map[x + i, y] = 1;
                                shipId[x + i, y] = id;
                            }
                            ok = true;
                        }
                    }
                    else if (!h && y + l <= 10)
                    {
                        bool free = true;
                        for (int i = 0; i < l; i++) if (map[x, y + i] != 0) free = false;
                        if (free)
                        {
                            int id = nextId++;
                            for (int i = 0; i < l; i++)
                            {
                                map[x, y + i] = 1;
                                shipId[x, y + i] = id;
                            }
                            ok = true;
                        }
                    }
                }
            }
        }

        public bool Shoot(int x, int y)
        {
            already[x, y] = true;
            if (map[x, y] == 1)
            {
                hits[x, y] = 1;
                return true;
            }
            hits[x, y] = 2;
            return false;
        }

        public bool IsDead(int x, int y)
        {
            if (map[x, y] != 1) return false;
            int id = shipId[x, y];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (shipId[i, j] == id && hits[i, j] != 1)
                        return false;
            return true;
        }

        public void MarkDead(int x, int y)
        {
            int id = shipId[x, y];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (shipId[i, j] == id)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                int nx = i + dx, ny = j + dy;
                                if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10 && map[nx, ny] == 0 && hits[nx, ny] == 0)
                                {
                                    already[nx, ny] = true;
                                    hits[nx, ny] = 2;
                                }
                            }
                    }
        }

        public bool HasShips()
        {
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                    if (map[i, j] == 1 && hits[i, j] != 1) return true;
            return false;
        }

        public string GetLine(int row, bool showShips)
        {
            string s = (row + 1).ToString().PadLeft(2) + " ";
            for (int i = 0; i < 10; i++)
            {
                if (hits[i, row] == 1) s += "X ";
                else if (hits[i, row] == 2) s += "O ";
                else if (showShips && map[i, row] == 1) s += "# ";
                else s += "~ ";
            }
            return s;
        }

        void DrawSelf(bool show)
        {
            Console.Clear();
            Console.WriteLine("  A B C D E F G H I J");
            for (int i = 0; i < 10; i++)
            {
                Console.Write((i + 1).ToString().PadLeft(2) + " ");
                for (int j = 0; j < 10; j++)
                {
                    if (show && map[j, i] == 1) Console.Write("# ");
                    else Console.Write("~ ");
                }
                Console.WriteLine();
            }
        }
    }
}
