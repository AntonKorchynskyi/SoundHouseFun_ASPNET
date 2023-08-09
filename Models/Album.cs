using System.ComponentModel.DataAnnotations;

namespace SoundHouseFun.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must provide an Album Name"), MaxLength(50)]
        public string Name { get; set; }

        public string? Photo { get; set; }

        public List<Song>? Songs { get; set; }
    }
}
