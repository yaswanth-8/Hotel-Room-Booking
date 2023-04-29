using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Hotel_Room_Booking.Models;

namespace Hotel_Room_Booking.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Hotel_Room_Booking.Models.HotelModel>? HotelModel { get; set; }
        public DbSet<Hotel_Room_Booking.Models.CustomerModel>? CustomerModel { get; set; }
    }
}