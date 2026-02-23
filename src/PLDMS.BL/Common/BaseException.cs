namespace PLDMS.BL.Common;

public class BaseException : Exception
{
    public BaseException(string options) : base(options) { }

    public BaseException() : base("Something went wrong") { }
}