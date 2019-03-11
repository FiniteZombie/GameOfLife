namespace Assets.Scripts
{
    /// <summary>
    /// A representation of one cell in the grid.
    /// </summary>
    public struct Cell
    {
        public readonly long X;
        public readonly long Y;

        /// <summary>
        /// Constructor
        /// </summary>
        public Cell(long x, long y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Needed for Hashset
        /// Compare x and y values
        /// </summary>
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
        /// Needed for Hashset
        /// Hash solution taken from https://stackoverflow.com/questions/892618/create-a-hashcode-of-two-numbers
        /// Combine X and Y values into one number, return hashchode from that number
        /// </summary>
        public override int GetHashCode()
        {
            long hashCode = 23;

            // We want to allow overflow here because:
            // (A) the end result doesn't actually matter as long as it is as evenly distributed as we can get
            // (B) if clamped, larger numbers would pile on the last "bucket" of the hashset reducing access time
            // (C) we would absolutely get overflow errors for any values of sufficient size
            unchecked 
            {
                hashCode = (hashCode * 37) + X;
                hashCode = (hashCode * 37) + Y;
            }

            return hashCode.GetHashCode();
        }
    }
}
