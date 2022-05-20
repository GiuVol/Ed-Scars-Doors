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
    public bool Blindness
    { get; set; }


    /// <summary>
    /// Property <c>CurrentBlindness</c>
    /// Properties to see if the characters/props can be given the state of blindness
    /// when it reaches its maximum value
    /// </summary>
    public int CurrentBlindness
    { get; set; }


    /// <summary>
    /// Property <c>MaxBlindness</c>
    /// Property to see if the characters/propscan be given the state of blindness
    /// when the property CurrentBlindness reaches the value of MaxBlindness 
    /// </summary>
    public int MaxBlindness
    { get; set; }


    /// <summary>
    /// Property <c>CoolDown</c>
    /// Properties to see if the state of blindness can be removed from characters/props that have it
    /// </summary>
    public int CoolDown
    { get; set;}

    /// <summary>
    /// Property <c>MaxCoolDown</c>
    /// Property that represents the max value of CoolDown of a character/prop
    /// </summary>
    public int MaxCoolDown
    { get; set; }

    /// <super>
    /// Constant <c>MinCoolDown</c>
    /// Constant that represent the min value of CoolDown
    /// </super>
    protected const int MinCoolDown=0; 

    /// <summary>
    /// Property <c>Corrosion</c>
    /// Property to see if the characters/props has the state of corrosion 
    /// and change its state
    /// </summary>
    public bool Corrosion
    { get; set; }

    /// <summary>
    /// Procedure <c>GiveBlindness</c>
    /// Gives the player a state of blindness if the current 
    /// blindness counter reaches its maximum value
    /// </summary>
    public void GiveBlindness()
    {
        if(CurrentBlindness<MaxBlindness)
        {
            CurrentBlindness++;
        }
        
        if((CurrentBlindness==MaxBlindness) && (Blindness==false))
        {
            Blindness=true;
        }
    }

    /// <summary>
    /// Procedure <c>RemoveBlindness</c>
    /// Removes from player the state of blindness
    /// </summary>
    protected void RemoveBlindness()
    {
        if(Blindness==true)
        {
            Blindness=false;
            CurrentBlindness=0;
        }
        
    }

    /// <summary>
    /// Procedure <c>DecrementCoolDown</c>
    /// decreases the cool down value
    /// </summary>
    /// <parame name="decrement"> Decreases the CoolDown value to its minimum value </param>
    public void DecrementCoolDown(int decrement)
    {
        if(CoolDown!=MinCoolDown)
        {
            if((CoolDown-decrement)<MinCoolDown)
            {
                CoolDown=0;
                this.RemoveBlindness();
            } else {
                CoolDown-=decrement;
            }
        }
    }

    /// <summary>
    /// Procedure <c>IncrementCoolDown</c>
    /// increase the cool down value
    /// </summary>
    /// <parame name="increment"> Increase the CoolDown value to its maximum value </param>
    public void IncrementCoolDown(int increment)
    {
        increment=Math.Max(increment,0);
        CoolDown+=Math.Min(increment+CoolDown, MaxCoolDown);
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
