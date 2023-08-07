namespace Firebridge.Controller.Models;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ControllerPluginAttribute : Attribute
{
    public required ScopeType ScopeType { get; init; }
    public required Type Implements { get; init; }
}