using System.Collections.Generic;

namespace Colin.Lottery.Models
{
    /// <summary>
    /// 开奖数据模型
    /// </summary>
    public abstract class DrawModel : IDrawModel
    {
        public long PeriodNo { get; set; }

        public string DrawNo { get; set; }
    }

    /// <summary>
    /// 开奖数据集模型
    /// </summary>
    public abstract class DrawCollectionModel : IDrawCollectionModel
    {
        public List<IDrawModel> DrawData { get; set; }
    }
}
