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
                    message = "You have already reached the max number of equipped abilities!";
                    break;
                case UnequippableAbilityExceptionType.DuplicateType:
                    message = "An ability of this type is already equipped!";
                    break;
                default:
                    message = "You can't equip this ability now!";
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
