namespace Exceptions;

[Serializable]
public class GameInvalidException : Exception
{
    public GameInvalidException() { }
    public GameInvalidException(string message) : base(message) { }
}
