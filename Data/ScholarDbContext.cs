using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Scholar.Models;

namespace Scholar.Data
{
    public class ScholarDbContext : IdentityDbContext<ApplicationUser>
    {
        public ScholarDbContext(DbContextOptions<ScholarDbContext> options)
            : base(options)
        {
        }
    }
}
