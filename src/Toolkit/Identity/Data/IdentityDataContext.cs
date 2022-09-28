using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Toolkit.Identity.Data;

public class IdentityDataContext : IdentityDbContext
{
    public IdentityDataContext(DbContextOptions<IdentityDataContext> options)
        : base(options)
    {

    }
}