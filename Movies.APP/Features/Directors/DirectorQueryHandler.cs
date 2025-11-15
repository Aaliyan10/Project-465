using CORE.APP.Models;
using CORE.APP.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Movies.APP.Domain;
using Movies.APP.Features.Movies;

namespace Movies.APP.Features.Directors
{
    public class DirectorQueryRequest : Request, IRequest<IQueryable<DirectorQueryResponse>>
    {
    }
    public class DirectorQueryResponse : Response
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsRetired { get; set; }

        public int MovieCount { get; set; }

        public string Movies { get; set; }
        public List<MovieQueryResponse> MoviesList { get; set; }

        public string FullName
        {
            get; set;
        }

        public class DirectorQueryHandler : Service<Director>, IRequestHandler<DirectorQueryRequest, IQueryable<DirectorQueryResponse>>
        {
            public DirectorQueryHandler(DbContext db) : base(db)
            {
            }
            protected override IQueryable<Director> Query(bool isNoTracking = true)
            {
                return base.Query(isNoTracking)
                   .Include(m => m.Movies)
                   .OrderBy(m => m.FirstName)
                   .ThenBy(m => m.LastName)
                   .ThenBy(m => m.IsRetired);
            }

            public Task<IQueryable<DirectorQueryResponse>> Handle(DirectorQueryRequest request, CancellationToken cancellationToken)
            {
                var query = Query().Select(d => new DirectorQueryResponse
                {
                    Guid = d.Guid,
                    Id = d.Id,
                    FirstName = d.FirstName,

                    MovieCount = d.Movies.Count,
                    Movies = string.Join(", ", d.Movies.Select(m => m.Name)),
                    FullName= d.FirstName + "" + d.LastName,

                    MoviesList = d.Movies.Select(m => new MovieQueryResponse
                    {
                        Id = m.Id,
                        Guid = m.Guid,
                        Name = m.Name,
                        ReleaseDate = m.ReleaseDate,
                        TotalRevenue = m.TotalRevenue,
                    }).ToList()
                });

                return Task.FromResult(query);
            }
        }

    }
}
