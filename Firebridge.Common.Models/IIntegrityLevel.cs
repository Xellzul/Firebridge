namespace Firebridge.Common.Models;

public enum IIntegrityLevel
{
    System, // System
    High,   // Administrator
    Medium, // Normal User
    Low     // below logged in user
}