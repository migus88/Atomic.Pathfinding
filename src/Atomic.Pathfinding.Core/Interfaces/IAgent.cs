using Atomic.Pathfinding.Core.Data;

namespace Atomic.Pathfinding.Core.Interfaces
{
    public interface IAgent
    {
        /// <summary>
        /// The square size of the agent, measured in occupied cells from the top left corner<br/>
        /// Example for size 3:<br/>
        /// ◼◻◻<br/>
        /// ◻◻◻<br/>
        /// ◻◻◻<br/>
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Triggered when a path is requested and found in a new thread.</br>
        /// </summary>
        /// <param name="result"></param>
        void OnPathResult(PathResult result);
    }
}
