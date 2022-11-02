using System;

public class UnequippableAbilityException : Exception
{
    public enum UnequippableAbilityExceptionType
    {
        NumberExceeded,
        DuplicateType
    }

    private UnequippableAbilityExceptionType _type;
    public new string Message
    {
        get
        {
            string message;

            switch (_type)
            {
                case UnequippableAbilityExceptionType.NumberExceeded:
                    message = "Hai gi� raggiunto il massimo numero di abilit� equipaggiabili!";
                    break;
                case UnequippableAbilityExceptionType.DuplicateType:
                    message = "Un'abilit� di questo tipo � gi� equipaggiata!";
                    break;
                default:
                    message = "Non puoi equipaggiare questa abilit� ora!";
                    break;
            }

            return message;
        }
    }

    public UnequippableAbilityException(UnequippableAbilityExceptionType type)
    {
        _type = type;
    }
}
