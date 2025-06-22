using Microsoft.EntityFrameworkCore;

public class EFCore101DbContext : DbContext
{
    public EFCore101DbContext(DbContextOptions<EFCore101DbContext> options)
        : base(options)
    {
    }
}
