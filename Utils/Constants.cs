using System.Runtime.CompilerServices;

namespace Car_Rental_System.Utils;
public static class Constant
{
    public const string car = "car";
    public const string user = "user";
    public const string brand = "brand";
    public const string category = "category";
    public const string contract = "contract";
    public const string carRental = "carRetail";

}

public static class StatusCarRental
{
    public const string PendingProcessing = "Chờ xử lý";
    public const string PendingConfirmation = "Chờ xác nhận";
    public const string Completed = "Hoàn thành";
    public const string Canceled = "Hủy";

}

public static class Contracts
{
    public const string error = "Sai thông tin";
    public const string Pending = "Chờ Ký";
    public const string Signature = "Đã Ký";
}
