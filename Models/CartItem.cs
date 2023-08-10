namespace SoundHouseFun.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int CartId { get; set; }

        public int SongId { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public Cart? Cart { get; set; }

        public Song? Song { get; set; }
    }
}