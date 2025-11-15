using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Movies.APP.Features.Directors
{
    public class DirectorCreateRequest : Request, IRequest<CommandResponse>
    {

        [StringLength(30, MinimumLength = 4)]
        public string FirstName { get; set; }

        [StringLength(30, MinimumLength = 4)]
        public string LastName { get; set; }

        public bool IsRetired { get; set; }

    }
    public class DirectorCreateHandler : Service<Director>, IRequestHandler<DirectorCreateRequest, CommandResponse>
    {
        public DirectorCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(DirectorCreateRequest request, CancellationToken cancellationToken)
        {
            if (await Query().AnyAsync(d => d.FirstName == request.FirstName && d.LastName==request.LastName, cancellationToken))
                return Error("Director with the same name exists!");

            var entity = new Director
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsRetired = request.IsRetired
            };

            Create(entity);
            return Success("Director created successfully.", entity.Id);
        }
    }
}
