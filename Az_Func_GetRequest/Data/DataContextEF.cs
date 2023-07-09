
using Az_Func_GetRequest.Models;
using Microsoft.EntityFrameworkCore;


namespace Az_Func_GetRequest.Data
{
    public class DataContextEF : DbContext
    {
        public DbSet<Todo>? Todo { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer("Server=localhost;Database=DotNetCourseDatabase;Trusted_connection=false;TrustServerCertificate=True;User Id=sa;Password=SQLConnect1;",
                //options.UseSqlServer("Server=localhost;Database=DotNetCourseDatabase;Trusted_Connection=true;TrustServerCertificate=true;",
                    options => options.EnableRetryOnFailure());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Todo>()
                // .HasNoKey()
                .HasKey(c => c.id);
            // .ToTable("Todo", "TutorialAppSchema");
            // .ToTable("TableName", "SchemaName");
        }


    }
}