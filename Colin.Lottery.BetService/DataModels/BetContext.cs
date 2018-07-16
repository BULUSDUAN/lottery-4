using Colin.Lottery.Models;
using Colin.Lottery.Models.BetService;
using Colin.Lottery.Utils;
using Microsoft.EntityFrameworkCore;

namespace Colin.Lottery.BetService.DataModels
{
    public class BetContext : DbContext
    {
        public DbSet<BetGuaRecord> BetGua { get; set; }
        public DbSet<BetAll3Record> BetAll3 { get; set; }
        public DbSet<BetAllRecord> BetAll { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConfigUtil.Configuration["ConnectionString"]);
        }
    }

    public class BetGuaRecord : BetRecordModel
    {
        public BetGuaRecord(LotteryType lottery,int rule,Plan plan,long periodNo,string betNo,int chaseTimes,decimal betMoney,float odds,decimal balance):base(lottery,rule,plan,periodNo,betNo,chaseTimes,betMoney,odds,balance){}
    }
    public class BetAll3Record : BetRecordModel
    {
        public BetAll3Record(LotteryType lottery,int rule,Plan plan,long periodNo,string betNo,int chaseTimes,decimal betMoney,float odds,decimal balance):base(lottery,rule,plan,periodNo,betNo,chaseTimes,betMoney,odds,balance){}
    }
    public class BetAllRecord : BetRecordModel
    {
        public BetAllRecord(LotteryType lottery,int rule,Plan plan,long periodNo,string betNo,int chaseTimes,decimal betMoney,float odds,decimal balance):base(lottery,rule,plan,periodNo,betNo,chaseTimes,betMoney,odds,balance){}
    }
}