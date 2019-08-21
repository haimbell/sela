using System.Configuration;
using Core;

namespace Sela_Reprice
{
    public class FileSystemTestFactory : IContextFactory
    {
        public IContext CreateContext()
        {
            var productDirectory = ConfigurationManager.AppSettings["ProductDir"];
            if (productDirectory == null)
            {
                throw new ConfigurationErrorsException("Product directory dosn't configured on application config file");
            }
            return new FileSystemTimedContext(productDirectory);
        }
    }
}