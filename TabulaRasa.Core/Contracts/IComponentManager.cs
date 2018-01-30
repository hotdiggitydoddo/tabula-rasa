using TabulaRasa.Core.Objects;

namespace TabulaRasa.Core.Contracts
{
    public interface IComponentManager
    {
        Component CreateComponent(string componentName, params string[] traits);
    }
}