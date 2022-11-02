using System;

public class NoNeedToUseThisItemException : Exception
{
    public new string Message
    {
        get
        {
            return "Non c'è bisogno di usare questo strumento ora!";
        }
    }

}
