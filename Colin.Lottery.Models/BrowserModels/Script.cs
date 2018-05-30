namespace Colin.Lottery.Models.BrowserModels
{
    /// <summary>
    /// JavaScript脚本
    /// </summary>
    public class Script
    {
        public string Code { get; private set; }

        public object[] Args { get; private set; }

        public Script(string code, params object[] args)
        {
            Code = code;
            Args = args;
        }
    }
}
