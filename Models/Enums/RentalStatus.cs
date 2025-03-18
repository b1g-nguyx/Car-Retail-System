namespace Car_Rental_System.Models.Enums
{
    public enum RentalStatus
    {
        Available,       // Xe có sẵn để thuê
        PendingPayment,  // Đang chờ thanh toán
        Rented,          // Đã được thuê
        PendingApproval, // Đang chờ duyệt đơn thuê
        Completed,       // Đơn thuê đã hoàn tất
        Cancelled,      // Đơn thuê bị hủy
        PendingConfirmation //Chờ xác nhận từ người dùng
    }
}
