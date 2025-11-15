using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;
using System.ComponentModel.DataAnnotations;


namespace Movies.APP.Features.Movies
{
    public class MovieUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required, StringLength(30, MinimumLength = 4)]
        public string Name { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public decimal TotalRevenue { get; set; }

        public int DirectorId { get; set; }
        public List<int> GenereIds { get; set; } = new List<int>();

    }
    public class MovieUpdateHandler : Service<Movie>, IRequestHandler<MovieUpdateRequest, CommandResponse>
    {
        public MovieUpdateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(MovieUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await Query().AnyAsync(m => m.Id != request.Id && m.Name == request.Name, cancellationToken))
                return Error("Movie with the same name exists!");

            var entity = await Query(false).SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Movie not found!");

            Delete(entity.MovieGenres);


            entity.Name = request.Name;
            entity.ReleaseDate = request.ReleaseDate;
            entity.TotalRevenue = request.TotalRevenue;
            entity.DirectorId = request.DirectorId;
            entity.GenreIds = request.GenereIds;


            Update(entity);
            return Success("Movie updated successfully.", entity.Id);
        }
    }
}
