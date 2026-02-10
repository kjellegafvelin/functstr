using functstr.core;

namespace SampleFunctions.test
{
    public class UnitTests
    {
        private FunctionsTester<Function1> tester;

        public UnitTests(ITestOutputHelper testOutputHelper)
        {
            var builder = FunctionsTester<Function1>.CreateBuilder();
            builder.UseTestOutputHelper(testOutputHelper);
            
            //builder.ConfigureHttpRequest()
            this.tester = builder.Build();

        }


        [Fact]
        public void Test1()
        {


        }
    }
}
