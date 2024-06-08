using Microsoft.CodeAnalysis;
using NetPad.Events;

namespace NetPad.Scripts.Events;

public class ScriptOptimizationLevelUpdatedEvent : IEvent
{
    public ScriptOptimizationLevelUpdatedEvent(Script script, OptimizationLevel oldValue, OptimizationLevel newValue)
    {
        Script = script;
        OldValue = oldValue;
        NewValue = newValue;
    }

    public Script Script { get; }
    public OptimizationLevel OldValue { get; }
    public OptimizationLevel NewValue { get; }
}
