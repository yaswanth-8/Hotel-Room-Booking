using System.ComponentModel.DataAnnotations;

namespace Hotel_Room_Booking.Models
{
    public class HotelModel
    {
        [Key]
        public int HotelId { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string HotelName { get; set; }

        [Required]
        public int Rooms { get; set; }

        public string Image { get; set; }

        public string Description { get; set; }

        [Required]
        public int Price { get; set; }
    }
}
