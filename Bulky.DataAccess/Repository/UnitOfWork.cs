using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; set; }
        public ApplicationContextDB _db;
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository shoppingCart { get; set; }
        public IApplicationUserRepository ApplicationUser { get; set; }

        public IOrderHeaderRepository OrderHeader { get; set; }
        public IOrderDetailRepository OrderDetail { get; set; }
        public UnitOfWork(ApplicationContextDB db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            Company = new CompanyRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            ApplicationUser=new ApplicationUserRepository(_db);
            OrderHeader=new OrderHeaderRepository(_db);
            OrderDetail=new OrderDetailRepository(_db);


        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
