using UnityEngine;
using System.Collections.Generic;

namespace PathFind
{
    public class Pathfinding
    {
        public static List<Point> FindPath(Grid grid, Point startPos, Point targetPos)
        {
            List<PathNode> nodes_path = _ImpFindPath(grid, startPos, targetPos);

            List<Point> ret = new List<Point>();
            if (nodes_path != null)
            {
                foreach (PathNode node in nodes_path)
                {
                    ret.Add(new Point(node.gridX, node.gridY));
                }
            }
            return ret;
        }

        private static List<PathNode> _ImpFindPath(Grid grid, Point startPos, Point targetPos)
        {
            PathNode startNode = grid.nodes[startPos.x, startPos.y];
            PathNode targetNode = grid.nodes[targetPos.x, targetPos.y];

            List<PathNode> openSet = new List<PathNode>();
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    return RetracePath(grid, startNode, targetNode);
                }

                foreach (PathNode neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) * (int)(10.0f * neighbour.penalty);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null;
        }

        private static List<PathNode> RetracePath(Grid grid, PathNode startNode, PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }

        private static int GetDistance(PathNode nodeA, PathNode nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

}