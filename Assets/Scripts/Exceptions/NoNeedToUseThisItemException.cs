using System;

public class NoNeedToUseThisItemException : Exception
{
    public new string Message
    {
        get
        {
            return "There is no need to use this item now!";
        }
    }

}
