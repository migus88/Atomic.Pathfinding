namespace Atomic.Pathfinding.Core.Data
{
    public class PathfinderSettings
    {
        /// <summary>
        /// Determines is the agent can move diagonally.
        /// </summary>
        public bool IsDiagonalMovementEnabled { get; set; } = true;
        /// <summary>
        /// Determines is the occupied cells (by other agents) considered as blocked or ignored.<br/>
        /// When multiple agents can occupy one cell, this should be set to 'false'.
        /// </summary>
        public bool IsCalculatingOccupiedCells { get; set; } = true;
        /// <summary>
        /// Determines is the agent can move between two corners.<br/>
        /// For example, if set to 'false', this movement is illegal:<br/>
        /// ◻◎◼◻<br/>
        /// ◻◼◎◻<br/>
        /// *Cirle is the movement path, blank squares - walkable cells and black squares - walls.
        /// </summary>
        public bool IsMovementBetweenCornersEnabled { get; set; } = false;
        /// <summary>
        /// Is additional cell weight calculation enabled.
        /// </summary>
        public bool IsCellWeightEnabled { get; set; } = true;
        /// <summary>
        /// The cost of the movement in a horizontal or vertical line.
        /// </summary>
        public double StraightMovementCost { get; set; } = 1;
        /// <summary>
        /// The cost of the movement in a diagonal line.
        /// </summary>
        public double DiagonalMovementCost { get; set; } = 1.5f;
    }
}
