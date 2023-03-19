using UnityEngine;
using System.Collections.Generic;

namespace PathFind
{
    public class Grid
    {
        public PathNode[,] nodes;
        int gridSizeX, gridSizeY;

        public Grid(int width, int height, float[,] tiles_costs)
        {
            gridSizeX = width;
            gridSizeY = height;
            nodes = new PathNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new PathNode(tiles_costs[x, y], x, y);

                }
            }
        }

        public Grid(int width, int height, bool[,] walkable_tiles)
        {
            gridSizeX = width;
            gridSizeY = height;
            nodes = new PathNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    nodes[x, y] = new PathNode(walkable_tiles[x, y] ? 1.0f : 0.0f, x, y);
                }
            }
        }

        public List<PathNode> GetNeighbours(PathNode node)
        {
            List<PathNode> neighbours = new List<PathNode>();

            for (int x = -1; x <= 1; x++)
			{
				for (int y = -1; y <= 1; y++)
				{
					if (x == 0 && y == 0)
						continue;

					if (x == -1 && y == -1)
						continue;

					if (x == -1 && y == 1)
						continue;

					if (x == 1 && y == -1)
						continue;

					if (x == 1 && y == 1)
						continue;

					int checkX = node.gridX + x;
					int checkY = node.gridY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(nodes[checkX, checkY]);
                    }
                }
            }

            return neighbours;
        }
    }
}