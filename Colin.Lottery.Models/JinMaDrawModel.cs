using System.Collections.Generic;

namespace Colin.Lottery.Models
{
    public class JinMaDrawModel : DrawModel
    {
        public long QiHao { set => PeriodNo = value; }

        public string Code { set => DrawNo = value; }
    }

    public class JinMaLotteryModelCollection : DrawCollectionModel
    {
        public List<JinMaDrawModel> CodeData
        {
            set
            {
                DrawData = new List<IDrawModel>();
                DrawData.AddRange(value);
            }
        }
    }
}
