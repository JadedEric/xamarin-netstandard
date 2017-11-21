using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Blog.Data
{
    public class BlogContext: DbContext
    {
        public DbSet<Models.Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var @string = new SqliteConnectionStringBuilder
            {
                DataSource = Core.Settings.DbPath
            }.ToString();

            var conn = new SqliteConnection(@string);

            optionsBuilder.UseSqlite(conn);
        }
    }
}
