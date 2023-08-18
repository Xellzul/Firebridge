namespace Firebridge.Common.Models;

/// <summary>
/// How high priority should Agent process have
/// </summary>
public enum IIntegrityLevel
{
    System, // System
    High,   // Administrator
    Medium, // Normal User
    Low     // below logged in user
}