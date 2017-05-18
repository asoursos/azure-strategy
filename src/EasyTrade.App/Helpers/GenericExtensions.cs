using Microsoft.Azure.Documents;

namespace EasyTradeCosmos
{
    public static class GenericExtensions
    {
        public static T Cast<T>(this Document document) where T : class
        {
            return (T)(dynamic)document;
        }
    }
}