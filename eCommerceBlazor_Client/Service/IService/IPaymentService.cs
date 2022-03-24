using eCommerceBlazor_Models;
using eCommerceBlazor_Client.ViewModels;

namespace eCommerceBlazor_Client.Service.IService
{
    public interface IPaymentService
    {
        public Task<SuccessModelDTO> Checkout(StripePaymentDTO paymentDTO);
    }
}
