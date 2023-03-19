namespace PathFind
{
    public class PathNode
    {
        public bool walkable;
        public int gridX;
        public int gridY;
        public float penalty;

        public int gCost;
        public int hCost;
        public PathNode parent;

        public PathNode(float _price, int _gridX, int _gridY)
        {
            walkable = _price != 0.0f;
            penalty = _price;
            gridX = _gridX;
            gridY = _gridY;
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
    }
}