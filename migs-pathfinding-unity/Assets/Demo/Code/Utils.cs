namespace Demo
{
    public static class Utils
    {
        public static int FieldCellComparison(FieldCell a, FieldCell b)
        {
            var result = a.Cell.Coordinate.Y.CompareTo(b.Cell.Coordinate.Y);
            return result == 0 ? a.Cell.Coordinate.X.CompareTo(b.Cell.Coordinate.X) : result;
        }
    }
}