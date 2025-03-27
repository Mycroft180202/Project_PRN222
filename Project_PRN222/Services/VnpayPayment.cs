using VNPAY.NET;
using VNPAY.NET.Models;

namespace Project_PRN222.Services
{
    public class VnpayPayment
    {
        private readonly string _tmnCode;
        private readonly string _hashSecret;
        private readonly string _baseUrl;
        private readonly string _callbackUrl;
        private readonly IVnpay _vnpay;

        public VnpayPayment(IConfiguration configuration)
        {
            _tmnCode = configuration["VNPAY:TmnCode"];
            _hashSecret = configuration["VNPAY:HashSecret"];
            _baseUrl = configuration["VNPAY:BaseUrl"];
            _callbackUrl = configuration["VNPAY:CallbackUrl"];
            _vnpay = new Vnpay();
            _vnpay.Initialize(_tmnCode, _hashSecret, _baseUrl, _callbackUrl);
        }

        public string CreatePaymentUrl(PaymentRequest request)
        {
            return _vnpay.GetPaymentUrl(request);
        }

        public PaymentResult GetPaymentResult(IQueryCollection query)
        {
            return _vnpay.GetPaymentResult(query);
        }
    }
}