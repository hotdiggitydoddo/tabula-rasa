namespace TabulaRasa.Core.Contracts
{
    public interface IScriptManager
    {
        string GetComponentScript(string name);
        void RegisterObjectType<T>();
    }
}