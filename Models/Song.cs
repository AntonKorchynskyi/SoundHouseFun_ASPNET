using System.ComponentModel.DataAnnotations;

namespace SoundHouseFun.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Display(Name = "Album")]
        public int AlbumId { get; set; }

        [Required(), MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Genre { get; set; }

        [Required(), MaxLength(50)]
        public string Singer { get; set; }

        [Required()]
        public DateTime ReleaseDate { get; set; }

        public string? Audio { get; set; }

        [Required()]
        public decimal Price { get; set; }

        public Album? Album { get; set; }

    }
}
