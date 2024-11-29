﻿using Migs.Pathfinding.Core.Data;

namespace Migs.Pathfinding.Core.Interfaces
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
    }
}
