using Firebridge.Common.Models;

namespace Firebridge.Controller.Models;

public class Agent
{
    public AgentState State { get; private set; }

    public Agent(Type program)
    {
        State = AgentState.Starting;
    }
}
