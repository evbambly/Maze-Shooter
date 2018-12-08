using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadedMaze
{
    abstract class Asset
    {
        public int xAsset;
        public int yAsset;
        public string name;

        abstract public Maze AssetCollision(Maze maze, int i);
    }
}
