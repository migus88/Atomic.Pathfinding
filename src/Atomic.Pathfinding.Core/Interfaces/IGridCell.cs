namespace Atomic.Pathfinding.Core.Interfaces
{
    public interface IGridCell
    {
        /// <summary>
        /// Is the agent can move on this cell.
        /// </summary>
        bool IsWalkable { get; }
        /// <summary>
        /// Is the cell occupied by an agent.
        /// </summary>
        bool IsOccupied { get; }
        /// <summary>
        /// <para>Additional complexity for reaching this tile.</para>
        /// For example when calculating a path on an elevating terrain,<br/>
        /// we can decide that the climb is more difficult and we can<br/>
        /// assign a higher value to this cell.
        /// </summary>
        float Weight { get; }
    }
}
