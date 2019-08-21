using System;
using System.Collections.Generic;

namespace Core
{
    public class ProductRepository : IRepository<Product>
    {
        #region Data Memebers

        private readonly IContextFactory _contextFactory;
        private readonly ICacheService _iCacheService;
        private IContext _context;
        private object syncObject = new object(); 
      
        #endregion

        /// <summary>
        /// We only call the create context when we need. when we deal with web the context many not be available before   
        /// </summary>
        private IContext Context
        {
            get
            {
                if (_context == null)
                {
                    //thread safe instance
                    lock (syncObject)
                    {
                        if (_context == null)
                        {
                            _context = _contextFactory.CreateContext();
                        }
                    }
                }
                return _context;
            }
        }

        public ProductRepository(IContextFactory contextFactory, ICacheService iCacheService)
        {
            _contextFactory = contextFactory;
            _iCacheService = iCacheService;
        }

        public void Set(Product item)
        {
            Context.Add(item);
            _iCacheService.Set($"product:{item.Id}", item, 60);
        }

        public Product Get(object id)
        {
            var product = _iCacheService.Get($"product:{id}", 60, () =>
            {
                var products = Context.All<Product>();
                foreach (var product1 in products)
                {
                    if (product1.Id == (Guid) id)
                        return product1;
                }

                return null;
            });
            return product;
        }

        public IEnumerable<Product> All()
        {
            return  Context.All<Product>();
        }
    }
}