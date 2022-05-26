using System;

/// <summary>
/// This interface contains properties and methods to modify 
/// status of a characters/props in the game. This interface will be 
/// implemented from all the characters/props which can have a status.
/// </summary>
interface IStatusable{

    /// <summary>
    /// Property <c>Blindness</c>
    /// Property to see if the characters/props has the state of blindness 
    /// and change its state
    /// </summary>
    public bool IsBlinded
    { get; set; }


    /// <summary>
    /// Property <c>CurrentBlindness</c>
    /// Properties to see if the characters/props can be given the state of blindness
    /// when it reaches its maximum value
    /// </summary>
    public int CurrentBlindnessLevel
    { get; set; }


    /// <summary>
    /// Property <c>MaxBlindness</c>
    /// Property to see if the characters/propscan be given the state of blindness
    /// when the property CurrentBlindness reaches the value of MaxBlindness 
    /// </summary>
    public int MaxBlindnessLevel
    { get; set; }


    /// <summary>
    /// Property <c>CoolDownTime</c>
    /// Properties to see if the state of blindness can be removed from characters/props that have it
    /// </summary>
    public int CoolDownTime
    { get; set; }

    /// <summary>
    /// Property <c>MaxCoolDownTime</c>
    /// Property that represents the max value of CoolDownTime of a character/prop
    /// </summary>
    public int MaxCoolDownTime
    { get; set; }

    /// <super>
    /// Constant <c>MinCoolDownTime</c>
    /// Constant that represent the min value of CoolDownTime
    /// </super>
    protected const int MinCoolDownTime=0; 

    /// <summary>
    /// Property <c>Corrosion</c>
    /// Property to see if the characters/props has the state of corrosion 
    /// and change its state
    /// </summary>
    public bool Corrosion
    { get; set; }

    /// <summary>
    /// Procedure <c>InflictBlindness</c>
    /// Gives the player a state of blindness if the current 
    /// blindness counter reaches its maximum value
    /// </summary>
    public void InflictBlindness()
    {
        if(CurrentBlindnessLevel < MaxBlindnessLevel)
        {
            CurrentBlindnessLevel++;
        }
        
        if((CurrentBlindnessLevel == MaxBlindnessLevel) && (IsBlinded == false))
        {
            IsBlinded=true;
        }
    }

    /// <summary>
    /// Procedure <c>RemoveBlindness</c>
    /// Removes from player the state of blindness
    /// </summary>
    protected void RemoveBlindness()
    {
        if(IsBlinded==true)
        {
            IsBlinded=false;
            CurrentBlindnessLevel=0;
        }
        
    }

    /// <summary>
    /// Procedure <c>DecrementCoolDownTime</c>
    /// decreases the cool down value
    /// </summary>
    /// <parame name="decrement"> Decreases the CoolDownTime value to its minimum value </param>
    public void DecrementCoolDownTime(int decrement)
    {
        if(CoolDownTime!=MinCoolDownTime)
        {
            if((CoolDownTime-decrement)<MinCoolDownTime)
            {
                CoolDownTime=0;
                this.RemoveBlindness();
            } else {
                CoolDownTime-=decrement;
            }
        }
    }

    /// <summary>
    /// Procedure <c>IncrementCoolDownTime</c>
    /// increase the cool down value
    /// </summary>
    /// <parame name="increment"> Increase the CoolDownTime value to its maximum value </param>
    public void IncrementCoolDownTime(int increment)
    {
        increment=Math.Max(increment,0);
        CoolDownTime+=Math.Min(increment+CoolDownTime, MaxCoolDownTime);
    }

    /// <summary>
    /// Procedure <c>GiveCorrosion</c>
    /// Gives the player a state of corrosion
    /// </summary>
    public void GiveCorrosion()
    {
        if(Corrosion==false)
        {
            Corrosion=true;
        }
    }

    /// <summary>
    /// Procedure <c>RemoveCorrosion</c>
    /// Removes from player the state of corrosion
    /// </summary>
    public void RemoveCorrosion()
    {
        if(Corrosion==true)
        {
            Corrosion=false;
        }
    }
}
