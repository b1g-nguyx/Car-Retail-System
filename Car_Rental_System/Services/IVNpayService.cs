using Car_Rental_System.Models;
using Car_Rental_System.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Web;
namespace Car_Rental_System.Services
{
    public interface IVNPayService
    {
       string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }

}
