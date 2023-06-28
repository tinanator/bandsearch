namespace BandSearch.Web.Exceptions
{
    public class WrongCredentialsException : Exception
    {
        public WrongCredentialsException(string message)
    : base(message)
        {
        }

        public WrongCredentialsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
