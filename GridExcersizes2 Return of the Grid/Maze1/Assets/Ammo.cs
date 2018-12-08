using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadedMaze
{
    class Ammo : Asset
    {
       
        public Ammo(Maze maze)
        {
            name = "ammo";
            bool noPlace = true;
            while (noPlace)
            {
                Random rnd = new Random();
                xAsset = rnd.Next(1, maze.x);
                yAsset = rnd.Next(1, maze.y);

                if ((xAsset - maze.xPlayer) + (yAsset - maze.yPlayer) < -4 || (xAsset - maze.xPlayer) + (yAsset - maze.yPlayer) > 4)
                {
                    if (maze.map[xAsset, yAsset] != maze.wall)
                    {
                        noPlace = false;
                        foreach (Asset boost in maze.boosts)
                            if (boost.xAsset == xAsset && boost.yAsset == yAsset)
                                noPlace = true;
                        foreach (Asset asset in maze.boosts)
                            if (asset.xAsset == xAsset && asset.yAsset == yAsset)
                                noPlace = true;
                    }
                }
            }
            maze.map[xAsset, yAsset] = maze.ammo;
        }
        public override Maze AssetCollision(Maze maze, int i)
        {
            if (maze.xPlayer == xAsset && maze.yPlayer == yAsset)
                maze.bullets += 3;
            maze.boosts.RemoveAt(i);
            return maze;
        }
    }
}
