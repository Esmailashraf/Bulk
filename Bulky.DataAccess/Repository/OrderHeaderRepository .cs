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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {

        private ApplicationContextDB _db;

        public OrderHeaderRepository(ApplicationContextDB db) : base(db)
        {
            _db = db;
        }


        public void Update(OrderHeader obj)
        {
            _db.orderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var OrdFrmDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if (OrdFrmDb != null)
            {
                OrdFrmDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                    OrdFrmDb.PaymentStatus = paymentStatus;
            }
        }

        public void updateStripePaymentID(int id, string sessionId, string paymentIntentId)
        {
            var OrdFrmDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                OrdFrmDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                OrdFrmDb.PaymentIntentId = paymentIntentId;
                OrdFrmDb.PaymentTime = DateTime.Now;
            }
        }
    }
}
