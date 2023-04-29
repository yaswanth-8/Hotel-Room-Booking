using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Room_Booking.Models
{
    public class CustomerModel
    {
        [Key]
        public int CustomerId { get; set; }

        public IdentityUser User { get; set; }

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }
        public HotelModel Hotel { get; set; }

        [Required]
        public int Rooms { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckIn { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOut { get; set; }

        public int Rating { get; set; }

        public string Comments { get; set; }
    }
}
