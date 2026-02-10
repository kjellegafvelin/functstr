using System.Reflection;

namespace functstr.core
{
    public interface ITriggerBindingResolver
    {
        bool CanResolve(ParameterInfo parameterInfo);

        object? Resolve(ParameterInfo parameterInfo);
    }
}
