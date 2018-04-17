using System;
using System.Collections.Generic;
using System.Text;

namespace Colin.Lottery.Models
{
    public abstract class Singleton<T> where T : class, new()
    {
        /// <summary>
        /// 当前类型实例
        /// </summary>
        public static T Instance { get { return new T(); } }
    }
}
