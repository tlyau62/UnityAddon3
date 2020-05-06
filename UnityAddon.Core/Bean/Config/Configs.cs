namespace UnityAddon.Core.Bean.Config
{
    public class Configs<TConfigs> : IConfigs<TConfigs> where TConfigs : class, new()
    {
        public TConfigs Value { get; } = new TConfigs();
    }
}
