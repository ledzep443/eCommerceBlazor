using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eCommerceBlazor_Business.Repository.IRepository;
using eCommerceBlazor_Common;
using eCommerceBlazor_DataAccess;
using eCommerceBlazor_DataAccess.Data;
using eCommerceBlazor_DataAccess.ViewModel;
using eCommerceBlazor_Models;

namespace eCommerceBlazor_Business.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<OrderHeaderDTO> CancelOrder(int id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            if(orderHeader == null)
            {
                return new OrderHeaderDTO();
            }

            if(orderHeader.Status == SD.Status_Pending)
            {
                orderHeader.Status = SD.Status_Cancelled;
                await _context.SaveChangesAsync();
            }

            if(orderHeader.Status == SD.Status_Confirmed)
            {
                //process refund
                var options = new Stripe.RefundCreateOptions
                {
                    Reason = Stripe.RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeader.PaymentIntentId
                };

                var service = new Stripe.RefundService();
                Stripe.Refund refund = service.Create(options);

                orderHeader.Status = SD.Status_Refunded;
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<OrderHeader, OrderHeaderDTO>(orderHeader);
        }

        public async Task<OrderDTO> Create(OrderDTO orderDTO)
        {
            try
            {
                var orderObj = _mapper.Map<OrderDTO, Order>(orderDTO);
                _context.OrderHeaders.Add(orderObj.OrderHeader);
                await _context.SaveChangesAsync();

                foreach(var details in orderObj.OrderDetails)
                {
                    details.OrderHeaderId = orderObj.OrderHeader.Id;
                }
                _context.OrderDetails.AddRange(orderObj.OrderDetails);
                await _context.SaveChangesAsync();

                return new OrderDTO()
                {
                    OrderHeader = _mapper.Map<OrderHeader, OrderHeaderDTO>(orderObj.OrderHeader),
                    OrderDetails = _mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailDTO>>(orderObj.OrderDetails).ToList()
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //return orderDTO;
        }

        public async Task<int> Delete(int id)
        {
            var orderHeader = await _context.OrderHeaders.FirstOrDefaultAsync(header => header.Id == id);
            if (orderHeader != null)
            {
                IEnumerable<OrderDetail> orderDetail = _context.OrderDetails.Where(detail => detail.OrderHeaderId == id);

                _context.OrderDetails.RemoveRange(orderDetail);
                _context.OrderHeaders.Remove(orderHeader);
                return _context.SaveChanges();
            }
            return 0;
        }

        public async Task<OrderDTO> Get(int id)
        {
            Order order = new()
            {
                OrderHeader = _context.OrderHeaders.FirstOrDefault(header => header.Id == id),
                OrderDetails = _context.OrderDetails.Where(detail => detail.OrderHeaderId == id),
            };
            if(order != null)
            {
                return _mapper.Map<Order, OrderDTO>(order);
            }
            return new OrderDTO();
        }

        public async Task<IEnumerable<OrderDTO>> GetAll(string? userId = null, string? status = null)
        {
            List<Order> OrderFromDb = new List<Order>();
            IEnumerable<OrderHeader> orderHeaderList = _context.OrderHeaders;
            IEnumerable<OrderDetail> orderDetailList = _context.OrderDetails;

            foreach (OrderHeader header in orderHeaderList)
            {
                Order order = new()
                {
                    OrderHeader = header,
                    OrderDetails = orderDetailList.Where(detail => detail.OrderHeaderId == header.Id),
                };
                OrderFromDb.Add(order);
            }
            //Add filtering #TODO
            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(OrderFromDb);
        }

        public async Task<OrderHeaderDTO> MarkPaymentSuccessful(int id)
        {
            var data = await _context.OrderHeaders.FindAsync(id);
            if (data == null)
            {
                return new OrderHeaderDTO();
            }
            if (data.Status==SD.Status_Pending)
            {
                data.Status = SD.Status_Confirmed;
                await _context.SaveChangesAsync();
                return _mapper.Map<OrderHeader, OrderHeaderDTO>(data);
            }
            return new OrderHeaderDTO();
        }

        public async Task<OrderHeaderDTO> UpdateHeader(OrderHeaderDTO orderHeaderDTO)
        {
            if (orderHeaderDTO != null)
            {
                var orderHeaderFromDb = _context.OrderHeaders.FirstOrDefault(header => header.Id == orderHeaderDTO.Id);
                orderHeaderFromDb.Name = orderHeaderDTO.Name;
                orderHeaderFromDb.PhoneNumber = orderHeaderDTO.PhoneNumber;
                orderHeaderFromDb.Carrier = orderHeaderDTO.Carrier;
                orderHeaderFromDb.Tracking = orderHeaderDTO.Tracking;
                orderHeaderFromDb.StreetAddress = orderHeaderDTO.StreetAddress;
                orderHeaderFromDb.City = orderHeaderDTO.City;
                orderHeaderFromDb.State = orderHeaderDTO.State;
                orderHeaderFromDb.PostalCode = orderHeaderDTO.PostalCode;
                orderHeaderFromDb.Status = orderHeaderDTO.Status;

                await _context.SaveChangesAsync();
                return _mapper.Map<OrderHeader, OrderHeaderDTO>(orderHeaderFromDb);
            }
            return new OrderHeaderDTO();
        }

        public async Task<bool> UpdateOrderStatus(int orderId, string status)
        {
            var data = await _context.OrderHeaders.FindAsync(orderId);
            if (data == null)
            {
                return false;
            }
            data.State = status;
            if (status == SD.Status_Shipped)
            {
                data.ShippingDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
