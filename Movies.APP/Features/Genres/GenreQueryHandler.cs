using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;
using System.Data;

namespace Movies.APP.Features.Genres
{
    public class GenreQueryRequest : Request, IRequest<IQueryable<GenreQueryResponse>>
    {
    }

    public class GenreQueryResponse : Response
    {
        public string Name { get; set; }
    }

    public class RoleQueryHandler : Service<Genre>, IRequestHandler<GenreQueryRequest, IQueryable<GenreQueryResponse>>
    {
        public RoleQueryHandler(DbContext db) : base(db)
        {
        }

        public Task<IQueryable<GenreQueryResponse>> Handle(GenreQueryRequest request, CancellationToken cancellationToken)
        {
            var query = Query().Select(r => new GenreQueryResponse
            {
                Guid = r.Guid,
                Id = r.Id,
                Name = r.Name
            });
            return Task.FromResult(query);
        }
    }
}
