using Amazon.S3.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Unator.App.Data;

public class DbRole : IdentityRole
{ }

public class DbUser : IdentityUser<string>
{
    public static DbUser New(string email)
    {
        return new DbUser()
        {
            Email = email,
            UserName = email
        };
    }

    //public IReadOnlyList<DbEmail> Emails { get; set; } = default!;

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<DbUser>();
        //user.HasMany(x => x.Emails);
    }
}

public class DbContact
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string Email { get; set; } // can be used as Id?
    public bool Subscribed { get; set; } = true;
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public class DbEmail
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string FromName { get; set; }
    public required string FromEmail { get; set; }
    public required string Subject { get; set; }
    public required string Content { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public class DbReceiver
{
    public required string EmailId { get; set; }

    /// <summary>
    /// We might need to change it to Contact Email.
    /// I didn't decide yet what I want in this situation.
    /// </summary>
    public required string ContactId { get; set; }

    /// <summary>
    /// Before using it you should load it inside of the query.
    /// Field will be null if you don't Include it from database.
    /// </summary>
    public DbContact Contact { get; } = default!;
}

public class UnatorDb : IdentityDbContext<DbUser, DbRole, string>
{
    public UnatorDb(DbContextOptions options) : base(options)
    {
    }

    protected UnatorDb()
    {
    }

    public DbSet<DbContact> Contacts { get; set; } = default!;
    public DbSet<DbEmail> Emails { get; set; } = default!;
    public DbSet<DbReceiver> Receivers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var contact = modelBuilder.Entity<DbContact>();
        contact.HasKey(x => x.Id);

        modelBuilder
            .Entity<DbEmail>()
            .HasKey(x => x.Id);

        var receiver = modelBuilder.Entity<DbReceiver>();
        receiver.HasKey(x => new { x.EmailId, x.ContactId });
        receiver.HasOne<DbEmail>().WithMany().HasForeignKey(x => x.EmailId).OnDelete(DeleteBehavior.SetNull);
        receiver.HasOne<DbContact>(x => x.Contact).WithMany().HasForeignKey(x => x.ContactId).OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}