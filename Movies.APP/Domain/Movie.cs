using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Movies.APP.Domain
{
    public class Movie : Entity
    {
        [StringLength(100)]
        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public decimal TotalRevenue { get; set; }  

        public int DirectorId { get; set; }

        public Director Director { get; set; } // navigational property

        public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

        [NotMapped] 
        public List<int> GenreIds 
        {
            get => MovieGenres.Select(movieGenreEntity => movieGenreEntity.GenreId).ToList();
            set => MovieGenres = value.Select(genreId => new MovieGenre() { GenreId = genreId }).ToList();
        }
    }
}
