using TabulaRasa.Services.Objects;

namespace TabulaRasa.Services.Contracts
{
    public interface IComponentManager
    {
        Component CreateComponent(string componentName, string traitData);
    }
}