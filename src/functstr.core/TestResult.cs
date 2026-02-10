
namespace functstr.core
{
    public class TestResult<TResult>
    {
        public TResult? Result { get; set; }
        public TimeSpan ElapsedTime { get; internal set; }
    }
}