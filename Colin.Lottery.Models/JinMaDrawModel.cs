using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colin.Lottery.Models
{
    public class JinMaDrawModel : DrawModel
    {
        public long QiHao { set { this.PeriodNo = value; } }

        public string Code { set { this.DrawNo = value; } }
    }

    public class JinMaLotteryModelCollection : DrawCollectionModel
    {
        public List<JinMaDrawModel> CodeData
        {
            set
            {
                this.DrawData = new List<IDrawModel>();
                this.DrawData.AddRange(value);
            }
        }
    }
}
