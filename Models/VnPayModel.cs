using System.ComponentModel.DataAnnotations;

namespace Car_Rental_System.Models;

  public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderDescription { get; set; }
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VnPayResponseCode { get; set; }
    }

    public class VnPaymentRequestModel
    {
        public long OrderId { get; set; }
        public string? FullName { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class VNPayConfig
    {
        public static string? vnp_Url { get; set; }
        public static string? vnp_Returnurl { get; set; }
        public static string? vnp_TmnCode { get; set; }
        public static string? vnp_HashSecret { get; set; }
        public static string? querydr { get; set; }
    }
