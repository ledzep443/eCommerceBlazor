using Newtonsoft.Json;
using System.Text;
using eCommerceBlazor_Models;
using eCommerceBlazor_Client.Service.IService;

namespace eCommerceBlazor_Client.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SuccessModelDTO> Checkout(StripePaymentDTO stripePayment)
        {
            try
            {
                var content = JsonConvert.SerializeObject(stripePayment);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/stripepayment/create", bodyContent);
                string responseResult = response.Content.ReadAsStringAsync().Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<SuccessModelDTO>(responseResult);
                    return result;
                }
                else
                {
                    var errorModel = JsonConvert.DeserializeObject<ErrorModelDTO>(responseResult);
                    throw new Exception(errorModel.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
