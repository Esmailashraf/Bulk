using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {

        private ApplicationContextDB _db;

        public OrderDetailRepository(ApplicationContextDB db):base(db)
        {
            _db = db;
        }


        public void Update(OrderDetail obj)
        {
            _db.orderDetails.Update(obj);
        }
    }
}
