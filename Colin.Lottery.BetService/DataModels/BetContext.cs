using Colin.Lottery.Models;
using Colin.Lottery.Models.BetService;
using Colin.Lottery.Utils;
using Microsoft.EntityFrameworkCore;

namespace Colin.Lottery.BetService.DataModels
{
    public class BetContext : DbContext
    {
        public DbSet<BetRecord> BetRecord { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigUtil.Configuration["ConnectionString"]);
        }
    }
}