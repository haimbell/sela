using Core;
using System.Configuration;

namespace Sela_Reprice
{
    public class FileSystemFactory : IContextFactory
    {
        private FileSystemTimedContext _fileSystemTimedContext;
        public IContext CreateContext()
        {
            var productDirectory = ConfigurationManager.AppSettings["ProductDir"];
            if (productDirectory == null)
            {
                throw new ConfigurationErrorsException("Product directory dosn't configured on application config file");
            }
            string root = System.Web.HttpContext.Current.Server.MapPath(productDirectory);
            if (_fileSystemTimedContext == null)
                _fileSystemTimedContext = new FileSystemTimedContext(root);
            return _fileSystemTimedContext;
        }
    }
}