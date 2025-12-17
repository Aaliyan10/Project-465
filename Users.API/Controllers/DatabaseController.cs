using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Users.APP.Domain;

namespace Users.API.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly UsersDb _db;
        private readonly IWebHostEnvironment _environment;

       
        public DatabaseController(UsersDb db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
        }

       
        /// </returns>
        [HttpGet, Route("~/api/SeedDb")]
        public IActionResult Seed()
        {
            
            var userRoles = _db.UserRoles.ToList();
            _db.UserRoles.RemoveRange(userRoles);

            var roles = _db.Roles.ToList();
            _db.Roles.RemoveRange(roles);

            var users = _db.Users.ToList();
            _db.Users.RemoveRange(users);

            var groups = _db.Groups.ToList();
            _db.Groups.RemoveRange(groups);

            _db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='UserRoles';");
            _db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='Roles';");
            _db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='Users';");
            _db.Database.ExecuteSqlRaw("UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='Groups';");

            // Add default roles
            _db.Roles.Add(new Role()
            {
                Name = "Admin"
            });
            _db.Roles.Add(new Role()
            {
                Name = "User"
            });

            _db.SaveChanges();

            _db.Groups.Add(new Group()
            {
                Title = "General",
                Users = new List<User>()
                {
                    new User()
                    {
                        Address = "Lahore",
                        BirthDate = new DateTime(1980, 8, 21),
                        CityId = 3,
                        CountryId = 2,
                        FirstName = "Aaliyan",
                        Gender = Genders.Man,
                        Guid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        LastName = "Aslam",
                        Password = "admin",
                        RegistrationDate = DateTime.UtcNow,
                        Score = 3.8M,
                        UserName = "admin",
                        UserRoles = new List<UserRole>()
                        {
                            new UserRole() { RoleId = _db.Roles.SingleOrDefault(r => r.Name == "Admin").Id }
                        }
                    },
                    new User()
                    {
                        BirthDate = DateTime.Parse("09/13/2004", new CultureInfo("en-US")),
                        CityId = 42,
                        CountryId = 4,
                        FirstName = "Wolf",
                        Gender = Genders.Man,
                        Guid = Guid.NewGuid().ToString(),
                        IsActive = true,
                        LastName = "Kurt",
                        Password = "user",
                        RegistrationDate = DateTime.UtcNow,
                        Score = 3.2m,
                        UserName = "user",
                        UserRoles = new List<UserRole>()
                        {
                            new UserRole() { RoleId = _db.Roles.SingleOrDefault(r => r.Name == "User").Id }
                        }
                    },
                }
            });

            _db.SaveChanges();

            return Ok("Database seed successful.");
        }
    }
}