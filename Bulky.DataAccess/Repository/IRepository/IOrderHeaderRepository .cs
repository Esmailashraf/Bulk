using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository :IRespository<OrderHeader>
    {
        void Update(OrderHeader obj);

        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void updateStripePaymentID(int id, string sessionId, string paymentIntentId);
     
    }
}
