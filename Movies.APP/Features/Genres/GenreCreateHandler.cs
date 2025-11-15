using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace Movies.APP.Features.Genres
{
    public class GenreCreateRequest : Request, IRequest<CommandResponse>
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
    public class GenreCreateHandler : Service<Genre>, IRequestHandler<GenreCreateRequest, CommandResponse>
    {
        public GenreCreateHandler(DbContext db) : base(db)
        {
        }

        public async Task<CommandResponse> Handle(GenreCreateRequest request, CancellationToken cancellationToken)
        {
            if (await Query().AnyAsync(r => r.Name == request.Name.Trim(), cancellationToken))
                return Error("Genre with the same name exists!");
            var entity = new Genre
            {
                Name = request.Name?.Trim()
            };
            await Create(entity, cancellationToken);
            return Success("Role Genre successfully.", entity.Id);
        }
    }

   
}
