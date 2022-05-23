
using System;

/// <summary>
/// Interface <c>IStatsable</c>
/// Interface for character stats management.
/// </summary>
interface IHealthable
{
    /// <summary>
    /// Const <c>PointOfNoMovement</c>
    /// Value for which the character can be considered stopped
    /// </summary>
    public const int StopThreshold = 0;

    /// <summary>
    /// Const <c>PointOfNoDamage</c>
    /// Value for which the character can't deal any damage
    /// </summary>
    public const int NoDamageThreshold = 0;

    /// <summary>
    /// Const <c>PointOfNoDefence</c>
    /// Value for which the character can't reduce any damage recieved
    /// </summary>
    public const int NoDefenceThreshold = 0;

    /// <summary>
    /// Property <c>MaxSpeed</c>
    /// properties for viewing and editing the maximum speed of the character
    /// </summary>
    public int MaxSpeed
    { get; private protected set; }

    /// <summary>
    /// Property <c>MaxSpeed</c>
    /// properties for viewing and editing the maximum attack of the character
    /// </summary>
    public int MaxAttack
    { get; private protected set; }

    /// <summary>
    /// Property <c>MaxSpeed</c>
    /// properties for viewing and editing the maximum defence of the character
    /// </summary>
    public int MaxDefence
    { get; private protected set; }

    /// <summary>
    /// Property <c>CourrentSpeed</c>
    /// properties for viewing and editing the current speed of the character
    /// </summary>
    public int CurrentSpeed
    { get; private protected set; }

    /// <summary>
    /// Property <c>CourrentAttack</c>
    /// properties for viewing and editing the current attack of the character
    /// </summary>
    public int CurrentAttack
    { get; private protected set; }

    /// <summary>
    /// Property <c>CourrentDefence</c>
    /// properties for viewing and editing the current defence of the character
    /// </summary>
    public int CurrentDefence
    { get; private protected set; }

    /// <summary>
    /// Method <c>DecrementSpeed</c>
    /// Decreases the speed of the character
    /// </summary>
    /// <param name="decrement">The integer value of the speed malus the character received</param>
    public void DecrementSpeed(int decrement)
    {
        decrement = Math.Max(decrement, 0); // if decrement has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentSpeed = Math.Max(CurrentSpeed - decrement, StopThreshold); // if the decrement is greater than CurrentSpeed, the CurrentSpeed will be 0, otherwise it will be equal to the difference
    }

    /// <summary>
    /// Method <c>IncrementSpeed</c>
    /// Increases the speed of the character
    /// </summary>
    /// <param name="increment">The integer value of the speed bonus the character received </param>
    public void IncrementSpeed(int increment)
    {
        increment = Math.Max(increment, 0); // if increment has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentSpeed = Math.Min(increment + CurrentSpeed, MaxSpeed); // if the sum between the increase and the CurrentSpeed is greater than the MaxSpeed, then the CurrentSpeed will be equal to the MaxSpeed, otherwise it will be equal to the sum
    }

    /// <summary>
    /// Method <c>DecrementAttack</c>
    /// Decreases the attack of the character
    /// </summary>
    /// <param name="decrement">The integer value of the attack malus the character received</param>
    public void DecrementAttack(int decrement)
    {
        decrement = Math.Max(decrement, 0); // if decrement has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentAttack = Math.Max(CurrentAttack - decrement, NoDamageThreshold); // if the decrement is greater than CurrentAttack, the CurrentAttack will be 0, otherwise it will be equal to the difference
    }

    /// <summary>
    /// Method <c>IncrementAttack</c>
    /// Increases the attack of the character
    /// </summary>
    /// <param name="increment">The integer value of the attack bonus the character received </param>
    public void IncrementAttack(int increment)
    {
        increment = Math.Max(increment, 0); // if increment has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentAttack = Math.Min(increment + CurrentAttack, MaxAttack); // if the sum between the increase and the CurrentAttack is greater than the MaxAttack, then the CurrentAttack will be equal to the MaxAttack, otherwise it will be equal to the sum
    }

    /// <summary>
    /// Method <c>DecrementDefence</c>
    /// Decreases the defence of the character
    /// </summary>
    /// <param name="decrement">The integer value of the defence malus the character received</param>
    public void DecrementDefence(int decrement)
    {
        decrement = Math.Max(decrement, 0); // if decrement has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentDefence = Math.Max(CurrentDefence - decrement, NoDefenceThreshold); // if the decrement greater is than CurrentDefence, the CurrentDefence will be 0, otherwise it will be equal to the difference
    }

    /// <summary>
    /// Method <c>IncrementDefence</c>
    /// Increases the defence of the character
    /// </summary>
    /// <param name="increment">The integer value of the defence bonus the character received </param>
    public void IncrementDefence(int increment)
    {
        increment = Math.Max(increment, 0); // if increment has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentDefence = Math.Min(increment + CurrentDefence, MaxDefence); // if the sum between the increase and the CurrentDefence is greater than the MaxDefence, then the CurrentDefence will be equal to the MaxDefence, otherwise it will be equal to the sum
    }
}