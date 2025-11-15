using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Movies
{
    public class MovieDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class MovieDeleteHandler : Service<Movie>, IRequestHandler<MovieDeleteRequest, CommandResponse>
    {
        public MovieDeleteHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(MovieDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await Query(false).SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Movie not found!");

            Delete(entity.MovieGenres);

            Delete(entity); 

            return Success("Movie deleted successfully.", entity.Id);
        }
    }
}
