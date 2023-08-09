using System.ComponentModel.DataAnnotations;

namespace SoundHouseFun.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Display(Name = "Album")]
        public int AlbumId { get; set; }

        [Required()]
        public string Name { get; set; }

        public string Genre { get; set; }

        [Required()]
        public string Singer { get; set; }

        public DateTime ReleaseDate { get; set; }

        public Album? Album { get; set; }

    }
}
