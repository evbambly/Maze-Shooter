using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadedMaze
{
    class Monster : Asset
    {
        public Monster(Maze maze)
        {
            name = "monster";
            bool noPlace = true;
            while (noPlace)
            {
                Random rnd = new Random();
                xAsset = rnd.Next(1, maze.x);
                yAsset = rnd.Next(1, maze.y);
                if (xAsset < 3 || xAsset > maze.x - 3)
                {
                    if (yAsset < 3 || yAsset > maze.y - 3)
                    {
                        if ((xAsset - maze.xPlayer) + (yAsset - maze.yPlayer) < -4 || (xAsset - maze.xPlayer) + (yAsset - maze.yPlayer) > 4)
                        {
                            if (maze.map[xAsset, yAsset] != maze.wall)
                            {
                                noPlace = false;
                                foreach (Monster other in maze.monsters)
                                    if (other.xAsset == xAsset && other.yAsset == yAsset)
                                        noPlace = true;
                                foreach (Asset asset in maze.boosts)
                                    if (asset.xAsset == xAsset && asset.yAsset == yAsset)
                                        noPlace = true;
                            }
                        }
                    }
                }
            }
            maze.map[xAsset, yAsset] = maze.enemy;
        }
        public override Maze AssetCollision(Maze maze, int i)
        {
            return maze;
        }
    }

}

