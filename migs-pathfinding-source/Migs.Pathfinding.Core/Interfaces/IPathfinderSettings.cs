namespace Migs.Pathfinding.Core.Interfaces;

public interface IPathfinderSettings
{
    /// <summary>
    /// Determines is the agent can move diagonally.
    /// </summary>
    bool IsDiagonalMovementEnabled { get; set; }

    /// <summary>
    /// Determines is the occupied cells (by other agents) considered as blocked or ignored.<br/>
    /// When multiple agents can occupy one cell, this should be set to 'false'.
    /// </summary>
    bool IsCalculatingOccupiedCells { get; set; }

    /// <summary>
    /// Determines is the agent can move between two corners.<br/>
    /// For example, if set to 'false', this movement is illegal:<br/>
    /// ◻◎◼◻<br/>
    /// ◻◼◎◻<br/>
    /// * Circle is the movement path, blank squares - walkable cells and black squares - walls.
    /// </summary>
    bool IsMovementBetweenCornersEnabled { get; set; }

    /// <summary>
    /// Is additional cell weight calculation enabled.
    /// </summary>
    bool IsCellWeightEnabled { get; set; }

    /// <summary>
    /// The cost of the movement in a horizontal or vertical line
    /// </summary>
    float StraightMovementMultiplier { get; set; }

    /// <summary>
    /// The cost of the movement in a diagonal line
    /// </summary>
    float DiagonalMovementMultiplier { get; set; }

    /// <summary>
    /// The initial size of the Open Set buffer. <br/>
    /// After multiple benchmarks, it looks like even in big mazes, the buffer size is not exceeding 100-200 elements. <br/>
    /// Adjust this value if you're experiencing allocations during the pathfinding process.
    /// </summary>
    int? InitialBufferSize { get; set; }
}