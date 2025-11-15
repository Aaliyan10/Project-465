using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Movies.APP.Features.Movies
{
    public class MovieCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(30, MinimumLength = 4)]
        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public decimal TotalRevenue { get; set; }

        public int DirectorId { get; set; }
        public List<int> GenereIds { get; set; } = new List<int>();

    }
    public class MovieCreateHandler : Service<Movie>, IRequestHandler<MovieCreateRequest, CommandResponse>
    {
        public MovieCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(MovieCreateRequest request, CancellationToken cancellationToken)
        {
            if (await Query().AnyAsync(m=> m.Name == request.Name, cancellationToken))
                return Error("Movie with the same name exists!");

            var entity = new Movie
            {
                Name = request.Name,
                ReleaseDate = request.ReleaseDate,
                TotalRevenue = request.TotalRevenue,
                DirectorId = request.DirectorId,
                GenreIds=request.GenereIds
            };

            Create(entity); 
            return Success("Movie created successfully.", entity.Id);
        }
    }

}
