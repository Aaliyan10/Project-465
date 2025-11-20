using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Features.Genres;
using Movies.APP.Domain;
using Movies.APP.Features.Directors;
namespace Movies.APP.Features.Movies

{
    public class MovieQueryRequest : Request, IRequest<IQueryable<MovieQueryResponse>>
    {
        public string Name { get; set; }
        public DateTime? ReleaseDateStart { get; set; }
        public DateTime? ReleaseDateEnd { get; set; }
        public decimal? TotalRevenueStart { get; set; }
        public decimal? TotalRevenueEnd { get; set; }
        public int? DirectorId { get; set; }
    }
    public class MovieQueryResponse : Response
    {
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public decimal TotalRevenue { get; set; }

        public string ReleaseDateF { get; set; }
        public string TotalRevenueF { get; set; }

        public int DirectorId { get; set; }

        public string Director { get; set; }

        public DirectorQueryResponse DirectorResponse { get; set; }
        public string Genre { get; set; }


        public List<GenreQueryResponse> GenreResponses { get; set; }

    }

    public class MovieQueryHandler : Service<Movie>, IRequestHandler<MovieQueryRequest, IQueryable<MovieQueryResponse>>
    {
        public MovieQueryHandler(DbContext db) : base(db)
        {
        }

        protected override IQueryable<Movie> Query(bool isNoTracking = true)
        {
            return base.Query(isNoTracking) 
                .Include(u => u.Director)
                .Include(u => u.MovieGenres).ThenInclude(ur => ur.Genre)
                .OrderByDescending(u => u.ReleaseDate) 
                .ThenBy(u => u.Name); 

        }

        public Task<IQueryable<MovieQueryResponse>> Handle(MovieQueryRequest request, CancellationToken cancellationToken)
        {
            var entityQuery = Query();

            if (!string.IsNullOrWhiteSpace(request.Name)){
                entityQuery = entityQuery.Where(u => u.Name == request.Name);
            }

            if (request.ReleaseDateStart.HasValue)
                entityQuery = entityQuery.Where(u => u.ReleaseDate.HasValue && u.ReleaseDate.Value.Date >= request.ReleaseDateStart.Value.Date);

            if (request.ReleaseDateEnd.HasValue)
                entityQuery = entityQuery.Where(u => u.ReleaseDate.HasValue && u.ReleaseDate.Value.Date <= request.ReleaseDateEnd.Value.Date);

            if (request.TotalRevenueStart.HasValue)
                entityQuery = entityQuery.Where(u => u.TotalRevenue >= request.TotalRevenueStart.Value);

            if (request.TotalRevenueEnd.HasValue)
                entityQuery = entityQuery.Where(u => u.TotalRevenue <= request.TotalRevenueEnd.Value);

            if (request.DirectorId.HasValue)
                entityQuery = entityQuery.Where(u => u.DirectorId == request.DirectorId.Value);



            var query = entityQuery.Select(u => new MovieQueryResponse 
            {
                Id = u.Id,
                Guid = u.Guid,
                Name = u.Name,
                ReleaseDate = u.ReleaseDate,
                TotalRevenue= u.TotalRevenue,
                DirectorId=u.DirectorId,


                ReleaseDateF = u.ReleaseDate.HasValue ? u.ReleaseDate.Value.ToString("MM/dd/yyyy") : string.Empty,
                TotalRevenueF = u.TotalRevenue.ToString("C1"), 

                Director = u.Director.FirstName+""+ u.Director.LastName,

                DirectorResponse = new DirectorQueryResponse
                {
                    Id = u.Director.Id,
                    Guid = u.Director.Guid,
                    FirstName = u.Director.FirstName,
                    LastName = u.Director.LastName,
                    IsRetired=u.Director.IsRetired
                },

                Genre = string.Join(", ", u.MovieGenres.Select(ur => ur.Genre.Name)),

                GenreResponses = u.MovieGenres.Select(ur => new GenreQueryResponse
                {
                    Id = ur.Genre.Id,
                    Guid = ur.Genre.Guid,
                    Name = ur.Genre.Name
                }).ToList()
            });



            return Task.FromResult(query);
        }
    }
}
