using System;
namespace Colin.Lottery.Models
{
    /// <summary>
    /// JavaScript脚本
    /// </summary>
    public class Script
    {
        public string Code { get; set; }

        public object[] Args { get; set; }

        public Script(string code, params object[] args)
        {
            this.Code = code;
            this.Args = args;
        }
    }
}
