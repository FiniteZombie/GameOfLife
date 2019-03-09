namespace Assets.Scripts
{
    public struct Cell
    {
        public readonly long X;
        public readonly long Y;

        public Cell(long x, long y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cell))
            {
                return false;
            }

            var other = (Cell) obj;
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Hash solution taken from https://stackoverflow.com/questions/892618/create-a-hashcode-of-two-numbers
        /// </summary>
        public override int GetHashCode()
        {
            long hashCode = 23;
            hashCode = (hashCode * 37) + X;
            hashCode = (hashCode * 37) + Y;

            return hashCode.GetHashCode();
        }
    }
}
