using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadedMaze
{

    class Maze
    {
        public static char playerChoice = '-';
        public int x;
        public int y;
        public int xPlayer;
        public int yPlayer;
        public int lives = 3;
        public int bullets = 3;
        public int kills = 0;
        public bool hurt = false;
        public int maxMonsters = 6;
        public int maxBoosts = 2;
        public char[,] map = new char[1, 1];
        public char wall = '/';
        public char marker = '@';
        public char floor = '.';
        public char blank = ' ';
        public char enemy = 'C';
        public char dead = 'c';
        public char zap = '~';
        public char pew;
        public char ammo = '=';
        public char health = '%';
        public bool stuck = false;
        public bool assistingBool = false;
        public string[] hudText = new string[8];
        public List<Monster> monsters = new List<Monster>();
        public List<Asset> boosts = new List<Asset>();


        public Maze()
        {
            x = 20;
            y = 40;
            map = new char[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (i == 0 || j == 0 || i == x - 1 || j == y - 1)
                        map[i, j] = wall;
                    else
                        map[i, j] = floor;
                }
            }
            for (int i = 0; i < 3; i++)
            {
                assistingBool = true;
                Random rnd = new Random();
                int cont;
                int xWall = 0;
                int yWall = 0;
                while (true)
                {
                    xWall = rnd.Next(x);
                    yWall = rnd.Next(y);
                    if (map[xWall, yWall] != wall)
                        break;
                }
                cont = rnd.Next(4);
                switch (cont)
                {
                    case 0:
                    case 1:
                        if (cont == 0) cont = -1;
                        for (int j = 0; j < x / 4; j++)
                        {
                            xWall += cont;
                            if (map[xWall, yWall] == wall) break;
                            else map[xWall, yWall] = wall;
                        }
                        break;
                    case 2:
                    case 3:
                        cont -= 2;
                        if (cont == 0) cont = -1;
                        for (int j = 0; j < y / 4; j++)
                        {
                            yWall += cont;
                            if (map[xWall, yWall] == wall) break;
                            else map[xWall, yWall] = wall;
                        }
                        break;
                }
            }
            initPlayer();
        }
        public void initPlayer()
        {
            while (true)
            {
                Random rnd = new Random();
                yPlayer = rnd.Next(1, y);
                xPlayer = rnd.Next(1, x);
                if (map[xPlayer, yPlayer] != wall)
                {
                    map[xPlayer, yPlayer] = marker;
                    break;
                }
            }
        }
        public void MonsterMove(Monster monster)
        {
            int w = 0;
            int s = 0;
            int a = 0;
            int d = 0;
            int stay = 0;
            if (monster.xAsset < xPlayer)
            {
                s = 33;
                w = 7;
            }
            if (monster.xAsset > xPlayer)
            {
                w = 33;
                s = 7;
            }
            if (monster.yAsset > yPlayer)
            {
                a = 33;
                d = 7;
            }
            if (monster.yAsset < yPlayer)
            {
                d = 33;
                a = 7;
            }
            if (monster.xAsset == xPlayer)
            {
                s = 10;
                w = 10;
                if (d > a)
                    d = 60;
                else a = 60;
            }
            if (monster.yAsset == yPlayer)
            {
                d = 10;
                a = 10;
                if (s > w)
                    s = 60;
                else w = 60;
            }
            stay = 100 - w - a - s - d;

            bool noMove = true;
            while (noMove)
            {
                if ((monster.xAsset - xPlayer > -2 && monster.xAsset - xPlayer < 2) && (monster.yAsset - yPlayer > -2 && monster.yAsset - yPlayer < 2))
                {
                    hurt = true;
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (map[xPlayer + i, yPlayer + j] == floor) map[xPlayer + i, yPlayer + j] = zap;
                        }
                    }
                    break;
                }
                Random rnd = new Random();
                int choice = rnd.Next(w + a + s + d + stay);
                if (choice < w + a + s + d)
                {
                    map[monster.xAsset, monster.yAsset] = floor;
                    if ((choice < w) && (map[monster.xAsset - 1, monster.yAsset] != wall) && (map[monster.xAsset - 1, monster.yAsset] != enemy)) monster.xAsset--;
                    else if ((choice < w + a) && (map[monster.xAsset, monster.yAsset - 1] != wall) && (map[monster.xAsset, monster.yAsset - 1] != enemy)) monster.yAsset--;
                    else if ((choice < w + a + s) && (map[monster.xAsset + 1, monster.yAsset] != wall) && (map[monster.xAsset + 1, monster.yAsset] != enemy)) monster.xAsset++;
                    else if (map[monster.xAsset, monster.yAsset + 1] != wall && map[monster.xAsset, monster.yAsset + 1] != enemy) monster.yAsset++;
                    noMove = false;
                }
                choice = rnd.Next(100);
                if (choice > 80) noMove = true;
                for (int i = 0; i < boosts.Count; i++)
                {
                    if (monster.xAsset == boosts[i].xAsset && monster.yAsset == boosts[i].yAsset)
                        boosts.RemoveAt(i);
                }
            }
            map[monster.xAsset, monster.yAsset] = enemy;
        }
        public void PrintMaze()
        {
            Console.Clear();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Console.Write(map[i, j]);
                }
                if (i < 8)
                    GetHud(i);
                Console.WriteLine();
            }
            
        }
        public void LineOfSight()
        {
            char[][] lineOfSight = new char[5][];
            Console.Clear();
            for (int i = 0; i < 5; i++) lineOfSight[i] = new char[5];
            for (int i = 0; i < 5; i++)
            {
                lineOfSight[i][0] = blank;
                lineOfSight[i][4] = blank;
                lineOfSight[0][i] = blank;
                lineOfSight[4][i] = blank;
            }
            if (map[xPlayer - 1, yPlayer] != wall) lineOfSight[0][2] = map[xPlayer - 2, yPlayer];
            if (map[xPlayer + 1, yPlayer] != wall) lineOfSight[4][2] = map[xPlayer + 2, yPlayer];
            if (map[xPlayer, yPlayer - 1] != wall) lineOfSight[2][0] = map[xPlayer, yPlayer - 2];
            if (map[xPlayer, yPlayer + 1] != wall) lineOfSight[2][4] = map[xPlayer, yPlayer + 2];
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    lineOfSight[i + 2][j + 2] = map[xPlayer + i, yPlayer + j];
                }
            }


            for (int x = 0; x < 8; x++)
            {
                if (x < 5)
                {
                    string temp = new string(lineOfSight[x]);
                    Console.Write(temp);
                }
                GetHud(x);
                Console.WriteLine();

            }
            if (stuck) Console.WriteLine("You can't move there!");

        }
        public void LineOfSight(bool xTreme)
        {
            char[][] lineOfSight = new char[7][];
            Console.Clear();
            for (int i = 0; i < 7; i++) lineOfSight[i] = new char[7];
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    lineOfSight[i][j] = blank;
                }
            }
            lineOfSight[3][3] = marker;
            #region copyingData
            int miny;
            int minx;
            int maxx;
            int maxy;
            if (xPlayer > 3) minx = 3;
            else minx = xPlayer;
            if (yPlayer > 3) miny = 3;
            else miny = yPlayer;
            if (xPlayer < x - 4) maxx = 3;
            else maxx = x - xPlayer - 1;
            if (yPlayer < y - 4) maxy = 3;
            else maxy = y - yPlayer - 1;
            for (int i = 1; i <= minx; i++)
            {
                lineOfSight[3 - i][3] = map[xPlayer - i, yPlayer];
                if (i < minx)
                    if (lineOfSight[3 - i - 1][3] == wall) break;
            }
            for (int i = 1; i <= miny; i++)
            {
                lineOfSight[3][3 - i] = map[xPlayer, yPlayer - i];
                if (i < miny)
                    if (lineOfSight[3][3 - i - 1] == wall) break;
            }
            for (int i = 1; i <= maxx; i++)
            {
                lineOfSight[3 + i][3] = map[xPlayer + i, yPlayer];
                if (i < maxx)
                    if (lineOfSight[3 + i + 1][3] == wall) break;
            }
            for (int i = 1; i <= maxy; i++)
            {
                lineOfSight[3][3 + i] = map[xPlayer, yPlayer + i];
                if (i < maxy)
                    if (lineOfSight[3][3 + i + 1] == wall) break;
            }
            if (minx == 3) minx = 2;
            if (miny == 3) miny = 2;
            if (maxx == 3) maxx = 2;
            if (maxy == 3) maxy = 2;
            for (int i = 1; i <= minx && i <= miny; i++)
            {
                lineOfSight[3 - i][3 - i] = map[xPlayer - i, yPlayer - i];
                if (i < minx && i < miny)
                    if (lineOfSight[3 - i - 1][3 - i - 1] == wall) break;
            }
            for (int i = 1; i <= minx && i <= maxy; i++)
            {
                lineOfSight[3 - i][3 + i] = map[xPlayer - i, yPlayer + i];
                if (i < minx && i < maxy)
                    if (lineOfSight[3 - i - 1][3 + i + 1] == wall) break;
            }
            for (int i = 1; i <= maxx && i <= miny; i++)
            {
                lineOfSight[3 + i][3 - i] = map[xPlayer + i, yPlayer - i];
                if (i < maxx && i < miny)
                    if (lineOfSight[3 + i + 1][3 - i - 1] == wall) break;
            }
            for (int i = 1; i <= maxx && i <= maxy; i++)
            {
                lineOfSight[3 + i][3 + i] = map[xPlayer + i, yPlayer + i];
                if (i < maxx && i < maxy)
                    if (lineOfSight[3 + i + 1][3 + i + 1] == wall) break;
            }
            #endregion
            for (int i = 0; i < 8; i++)
            {
                if (i < 7)
                {
                    string temp = new string(lineOfSight[i]);
                    Console.Write(temp);
                }
                GetHud(i);
                Console.WriteLine();

            }

        }
        public void GetHud(int i)
        {
            hudText[0] = "   Here are some useful things to know";
            hudText[1] = "   ------------------------------------";
            hudText[2] = "   ";
            hudText[3] = $"   There are {monsters.Count} monsters on the board";
            hudText[4] = $"   Your position is {xPlayer}|{yPlayer}";
            string lives = "";
            for (int j = 0; j < this.lives; j++)
            {
                lives += "<3";
            }
            hudText[5] = $"   Lives: {lives}    Bullets left: {bullets}";
            hudText[6] = "";
            assistingBool = false;
            foreach (Asset asset in boosts)
            {
                if (asset.name == "health")
                    assistingBool = true;
            }
            if (assistingBool)
                hudText[6] = $"   The closest health potion is {GetClosest("health")}";
            assistingBool = false;
            foreach (Asset asset in boosts)
            {
                if (asset.name == "ammo")
                    assistingBool = true;
            }
            if (assistingBool)
                hudText[7] = $"   The closest ammo pack is {GetClosest("ammo")}";
            assistingBool = false;

            Console.Write(hudText[i]);
        }
        public void MazeCleaner()
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                    if (map[i, j] == zap || map[i, j] == dead || map[i, j] == pew) map[i, j] = floor;
            }
            foreach (Asset asset in boosts)
            {
                if (asset.name == "health")
                    map[asset.xAsset, asset.yAsset] = health;
                else if (asset.name == "ammo")
                    map[asset.xAsset, asset.yAsset] = ammo;
            }
        }
        public string GetClosest(string type)
        {
            int closest = x * x + y * y;
            string result = "";

            foreach (Asset asset in boosts)
            {
                if (closest > Math.Abs(asset.xAsset - xPlayer + asset.yAsset - yPlayer) && asset.name == type)
                {
                    closest = Math.Abs(asset.xAsset - xPlayer + asset.yAsset - yPlayer);
                    if (asset.xAsset < xPlayer)
                        result = "top";
                    else if (asset.xAsset > xPlayer)
                        result = "bottom";
                    else result = "";
                    if (asset.yAsset < yPlayer)
                        result += " left";
                    if (asset.yAsset > yPlayer)
                        result += " right";
                }
            }
            return result;
        }
    }
}

