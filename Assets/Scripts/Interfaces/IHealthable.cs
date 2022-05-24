
using System;

/// <summary>
/// Interface <c>IHealthable</c>
/// Interface for character health management.
/// </summary>
interface IHealthable
{
    public HealthComponent healthComponent
    { get; set; }

    /// <summary>
    /// Method <c>Dead</c>
    /// Procedure that is called every time a character reaches 0 health
    /// </summary>
    protected void Dead();
}
