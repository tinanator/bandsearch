
namespace BandSearch.Web.Exceptions
{
    public class DALException : Exception
    {
        public DALException(string message)
    : base(message)
        {
        }

        public DALException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
