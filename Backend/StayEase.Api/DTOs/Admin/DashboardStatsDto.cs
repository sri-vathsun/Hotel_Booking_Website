namespace StayEase.Api.DTOs.Admin
{
    public class DashboardStatsDto
    {
        public int TotalCustomers { get; set; }

        public int TotalHotelManagers { get; set; }

        public int TotalHotels { get; set; }

        public int PendingApprovals { get; set; }

        public int TotalRooms { get; set; }

        public int TotalBookings { get; set; }

        public int PendingBookings { get; set; }
    }
}