using System;

namespace Atomic.Pathfinding.Core
{
    public class AgentAlreadyExistsException : Exception
    {
        public AgentAlreadyExistsException() : base("The agent already exists") { }
    }

    public class AgentNotFoundException : Exception
    {
        public AgentNotFoundException() : base("Agent not found") { }
    }

    public class EmptyGridException : Exception
    {
        public EmptyGridException() : base("Grid cannot be empty or null") { }
    }

    public class AgentBusyException : Exception
    {
        public AgentBusyException() : base("Agent is busy") { }
    }

    public class AgentTooSmallException : Exception
    {
        public AgentTooSmallException() : base("Agent cannot be smaller than 1") { }
    }

    public class AgentTooBigException : Exception
    {
        public AgentTooBigException() : base("Agent cannot be bigger than the field") { }
    }
    
}
