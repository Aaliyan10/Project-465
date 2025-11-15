using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Movies.APP.Features.Directors
{
    public class DirectorUpdateRequest : Request, IRequest<CommandResponse>
    {
        [StringLength(30, MinimumLength = 4)]
        public string FirstName { get; set; }

        [StringLength(30, MinimumLength = 4)]
        public string LastName { get; set; }

        public bool IsRetired { get; set; }

    }
    public class DirectorUpdateHandler : Service<Director>, IRequestHandler<DirectorUpdateRequest, CommandResponse>
    {
        public DirectorUpdateHandler(DbContext db) : base(db)
        {
        }
        public async Task<CommandResponse> Handle(DirectorUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await Query().AnyAsync(m => m.Id != request.Id && m.FirstName == request.FirstName && m.LastName == request.LastName, cancellationToken))
                return Error("Director with the same name exists!");

            var entity = await Query(false).SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
            if (entity is null)
                return Error("Director not found!");



            entity.FirstName = request.FirstName;
            entity.LastName = request.LastName;
            entity.IsRetired = request.IsRetired;


            Update(entity);
            return Success("Director updated successfully.", entity.Id);
        }

    }
}
