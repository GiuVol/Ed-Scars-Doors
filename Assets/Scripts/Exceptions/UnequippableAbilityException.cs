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
                    message = "Hai già raggiunto il massimo numero di abilità equipaggiabili!";
                    break;
                case UnequippableAbilityExceptionType.DuplicateType:
                    message = "Un'abilità di questo tipo è già equipaggiata!";
                    break;
                default:
                    message = "Non puoi equipaggiare questa abilità ora!";
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
