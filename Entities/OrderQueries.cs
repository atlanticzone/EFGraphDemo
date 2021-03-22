using GraphDemo.Model;
using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;


using NUnit.Framework;

using System.Linq.Expressions;

namespace GraphDemo.Entities
{
    #region Queries
    public class OrderQueries
    {
        public async Task<List<Order>> GetOrders([Service] OMSOrdersContext context, long customerId)
        {
            try
            {
                return Convert(await GetOrderHeaders(context, customerId));
            }
            catch (System.Exception ex)
            {
                //ExceptionHandler(ex);
                return null;
            }
        }
        public async Task<List<OrderHeader>> GetOrderHeaders([Service] OMSOrdersContext context, long customerId) =>
        //await Expression.TryCatch(Expression.Block(typeof(void), context .OrderHeaders.ToListAsync()), Expression.Catch(typeof(System.Exception), null));
                await context
                .OrderHeaders
                .Where(w => w.CustomerId == customerId)
                .OrderBy(o => o.OrderedDate)
                .ToListAsync();

        public async Task<List<OrderDetail>> GetOrderDetails([Service] OMSOrdersContext context, long orderId) =>
        await context
        .OrderDetails
        .Where(o => o.OrderId == orderId)
        .OrderBy(i => i.OrderDetailId)
        .ToListAsync();

        public async Task<List<OrderDetailLine>> GetOrderDetailLines([Service] OMSOrdersContext context, long orderDetailId) =>
        await context
        .OrderDetailLines
        .Where(o => o.OrderDetailId == orderDetailId)
        .OrderBy(i => i.OrderDetailLineId)
        .ToListAsync();


        public List<Order> Convert(List<OrderHeader> l)
        {
            List<Order> lo = new List<Order>();
            foreach (var item in l)
            {
                Order o = new Order
                {
                    OrderId = item.OrderId
                };
                lo.Add(o);
            }

            return lo;
        }
#endregion

    #region Tests
        public class OrdersRepository
        {
            private readonly IOMSOrdersContext _context;

            public OrdersRepository(IOMSOrdersContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public int Count()
            {
                return _context.Orders.Count();
            }

            public Order Find(long id)
            {
                return _context.Orders.Find(id);
            }
        }

        [TestFixture]
        public class CustomersRepositoryTests
        {
            private OrdersRepository _ordersRepository;
            private IOMSOrdersContext _context;

            [SetUp]
            public void Setup()
            {
                // Arrange
                _context = new FakeOMSOrdersContext();

                var list = new List<Order>
                    {
                        new Order
                        {
                            OrderId = 1
                        },
                        new Order
                        {
                            OrderId = 2
                        }
                    };

                _context.Orders.AddRange(list);
                _ordersRepository = new OrdersRepository(_context);
            }

            [Test]
            public void GetCount()
            {
                // Act
                int count = _ordersRepository.Count();

                // Assert
                Assert.AreEqual(2, count);
            }

            [Test]
            public void Find()
            {
                // Act
                var abc = _ordersRepository.Find(1);
                var def = _ordersRepository.Find(2);
                var ghi = _ordersRepository.Find(3);

                // Assert
                Assert.IsNotNull(abc);
                Assert.IsNotNull(def);
                Assert.IsNull(ghi);

                Assert.AreEqual("abc", abc.OrderId);
                Assert.AreEqual("def", def.OrderId);
            }
        }
        #endregion

    }
}