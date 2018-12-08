using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ThreadedMaze
{
    class Play
    {
        public Play()
        {
            int kills;
            int turns = 1;
            Maze maze = new Maze();
            maze.PrintMaze();
            Console.WriteLine("Press any key to start");
            Console.ReadKey();
            Thread playerChoice = new Thread(GetMove);
            playerChoice.Start(maze);
            while (maze.lives > 0)
            {
                Maze.playerChoice = '-';
                Thread.Sleep(700);
                maze = Move(maze);
                maze = Spawn(maze);
                for (int i = 0; i < maze.monsters.Count; i++)
                {
                    maze.MonsterMove(maze.monsters[i]);
                }
                maze.PrintMaze();
                maze.MazeCleaner();
                turns++;
                if (maze.hurt) maze.lives--;
                maze.hurt = false;
            }
            kills = maze.kills;
            Console.WriteLine($"you have killed {kills} monsters, and survived {turns} turns");
            Console.ReadLine();
        }
        public void GetMove(object o)
        {
            Maze maze = (Maze)o;
            while (maze.lives > 0)
            {
                ConsoleKeyInfo input = Console.ReadKey(false);
                char output = ' ';
                switch (input.Key)
                {
                    case ConsoleKey.W:
                        output = 'w';
                        break;
                    case ConsoleKey.A:
                        output = 'a';
                        break;
                    case ConsoleKey.S:
                        output = 's';
                        break;
                    case ConsoleKey.D:
                        output = 'd';
                        break;
                    case ConsoleKey.UpArrow:
                        output = 't';
                        break;
                    case ConsoleKey.LeftArrow:
                        output = 'f';
                        break;
                    case ConsoleKey.DownArrow:
                        output = 'g';
                        break;
                    case ConsoleKey.RightArrow:
                        output = 'h';
                        break;
                }


                Maze.playerChoice = output;
            }
        }
        public Maze Move(Maze maze)
        {
            maze.stuck = false;
            switch (Maze.playerChoice)
            {
                case 'w':
                    if (maze.map[maze.xPlayer - 1, maze.yPlayer] == maze.wall || maze.map[maze.xPlayer, maze.yPlayer + 1] == maze.enemy) maze.stuck = true;
                    else
                    {
                        maze.map[maze.xPlayer, maze.yPlayer] = maze.floor;
                        maze.xPlayer--;
                    }
                    break;
                case 's':
                    if (maze.map[maze.xPlayer + 1, maze.yPlayer] == maze.wall || maze.map[maze.xPlayer, maze.yPlayer + 1] == maze.enemy) maze.stuck = true;
                    else
                    {
                        maze.map[maze.xPlayer, maze.yPlayer] = maze.floor;
                        maze.xPlayer++;
                    }
                    break;
                case 'a':
                    if (maze.map[maze.xPlayer, maze.yPlayer - 1] == maze.wall || maze.map[maze.xPlayer, maze.yPlayer + 1] == maze.enemy) maze.stuck = true;
                    else
                    {
                        maze.map[maze.xPlayer, maze.yPlayer] = maze.floor;
                        maze.yPlayer--;
                    }
                    break;
                case 'd':
                    if (maze.map[maze.xPlayer, maze.yPlayer + 1] == maze.wall || maze.map[maze.xPlayer, maze.yPlayer + 1] == maze.enemy) maze.stuck = true;
                    else
                    {
                        maze.map[maze.xPlayer, maze.yPlayer] = maze.floor;
                        maze.yPlayer++;
                    }
                    break;
                case 't':
                case 'f':
                case 'g':
                case 'h':
                    Shoot(maze);
              //      if (maze.stuck) Console.WriteLine("You shot the wall..");
                    maze.stuck = false;
                    break;
            }
            //if (maze.stuck) Console.WriteLine("You can't move there!");
            
                //Console.WriteLine("You don't have anymore bullets left!");
             


            for (int i = 0; i < maze.boosts.Count; i++)
            {
                if (maze.boosts[i].xAsset == maze.xPlayer && maze.boosts[i].yAsset == maze.yPlayer)
                    maze.boosts[i].AssetCollision(maze, i);
            }
            maze.map[maze.xPlayer, maze.yPlayer] = maze.marker;
            return maze;
        }
        public Maze Spawn(Maze maze)
        {
            Random rnd = new Random();
            int spawn = rnd.Next(10);
            if (spawn > 7 && maze.monsters.Count < maze.maxMonsters)
            {
                Monster monster = new Monster(maze);
                maze.monsters.Add(monster);
            }
            spawn = rnd.Next(20);
            if (spawn > 18 && maze.boosts.Count < maze.maxBoosts)
            {
                Health health = new Health(maze);
                maze.boosts.Add(health);
            }
            spawn = rnd.Next(10);
            if (spawn > 8 && maze.boosts.Count < maze.maxBoosts)
            {
                Ammo ammo = new Ammo(maze);
                maze.boosts.Add(ammo);
            }
            return maze;
        }
        public Maze Shoot(Maze maze)
        {
            char move = Maze.playerChoice;

            maze.assistingBool = false;
            maze.stuck = false;
            if (maze.bullets > 0)
            {
                switch (move)
                {

                    case 't':
                        for (int i = 0; i < maze.monsters.Count; i++)
                        {
                            if (maze.monsters[i].xAsset < maze.xPlayer && maze.monsters[i].yAsset == maze.yPlayer)
                            {
                                maze.map[maze.monsters[i].xAsset, maze.monsters[i].yAsset] = maze.dead;
                                maze.monsters.RemoveAt(i);
                                maze.kills++;
                            }
                        }
                        maze.pew = 'v';
                        if (maze.map[maze.xPlayer - 1, maze.yPlayer] != maze.wall)
                            maze.map[maze.xPlayer - 1, maze.yPlayer] = maze.pew;
                        else maze.stuck = true;
                        break;
                    case 'g':
                        for (int i = 0; i < maze.monsters.Count; i++)
                        {
                            if (maze.monsters[i].xAsset > maze.xPlayer && maze.monsters[i].yAsset == maze.yPlayer)
                            {
                                maze.map[maze.monsters[i].xAsset, maze.monsters[i].yAsset] = maze.dead;
                                maze.monsters.RemoveAt(i);
                                maze.kills++;
                            }
                        }
                        maze.pew = '^';
                        if (maze.map[maze.xPlayer + 1, maze.yPlayer] != maze.wall)
                            maze.map[maze.xPlayer + 1, maze.yPlayer] = maze.pew;
                        else maze.stuck = true;
                        break;
                    case 'f':
                        for (int i = 0; i < maze.monsters.Count; i++)
                        {
                            if (maze.monsters[i].xAsset == maze.xPlayer && maze.monsters[i].yAsset < maze.yPlayer)
                            {
                                maze.map[maze.monsters[i].xAsset, maze.monsters[i].yAsset] = maze.dead;
                                maze.monsters.RemoveAt(i);
                                maze.kills++;
                            }
                        }
                        maze.pew = '>';
                        if (maze.map[maze.xPlayer, maze.yPlayer - 1] != maze.wall)
                            maze.map[maze.xPlayer, maze.yPlayer - 1] = maze.pew;
                        else maze.stuck = true;
                        break;
                    case 'h':
                        for (int i = 0; i < maze.monsters.Count; i++)
                        {
                            if (maze.monsters[i].xAsset == maze.xPlayer && maze.monsters[i].yAsset > maze.yPlayer)
                            {
                                maze.map[maze.monsters[i].xAsset, maze.monsters[i].yAsset] = maze.dead;
                                maze.monsters.RemoveAt(i);
                                maze.kills++;
                            }
                        }
                        maze.pew = '<';
                        if (maze.map[maze.xPlayer, maze.yPlayer + 1] != maze.wall)
                            maze.map[maze.xPlayer, maze.yPlayer + 1] = maze.pew;
                        else maze.stuck = true;
                        break;
                    case 'q':
                        maze.assistingBool = true;
                        break;
                }
                maze.bullets--;
            }
            return maze;
        }
    }
}
