using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;

namespace Movies.APP.Features.Genres
{
    public class GenreDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class GenreDeleteHandler : Service<Genre>, IRequestHandler<GenreDeleteRequest, CommandResponse>
    {
        public GenreDeleteHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(GenreDeleteRequest request, CancellationToken cancellationToken)
        {
            var entity = await Query(false).SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Genre not found!");
            await Delete(entity, cancellationToken);
            return Success("Genre deleted successfully.", entity.Id);
        }
    }
}
