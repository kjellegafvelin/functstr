using System.Reflection;

namespace functstr.core
{
    public interface IOutputBindingResolver<TResult>
    {
        bool CanResolve(ParameterInfo parameterInfo);

        TestResult<TResult> Resolve(ParameterInfo parameterInfo);
    }
}
