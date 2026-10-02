using ClassLibrary;

namespace ConsoleApp
{
    /// <summary>
    /// Sits between Main and the F# library so every stack has C# frames above the F# ones -
    /// that mix is what "Show Call Stack on Code Map" has to render.
    /// </summary>
    internal static class Driver
    {
        internal static void Run(string name, Func<int> scenario) => Dispatch(name, scenario);

        private static void Dispatch(string name, Func<int> scenario) => scenario();

        internal static int CallbackIntoCSharp(int value) => Demo.sink("C# callback from F#");

        internal static int CallbackScenario() => Scenarios.callbackIntoCSharp(CallbackIntoCSharp);
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Every scenario is passed as a method group: a C# lambda would show up on the Code Map
            // as an `AnonymousMethod__N` node and hide the F# frames this repro is about.
            Driver.Run("module functions", Demo.moduleFunctions);
            Driver.Run("pipeline lambdas", Demo.pipelineLambdas);
            Driver.Run("nested closures", Demo.nestedClosures);
            Driver.Run("local functions", Scenarios.localFunctions);

            Driver.Run("generic function", Scenarios.genericFunction);
            Driver.Run("custom operator", Demo.customOperator);
            Driver.Run("CompiledName rename", Demo.RenamedInMetadata);
            Driver.Run("active pattern", Demo.activePattern);
            Driver.Run("partial active pattern", Demo.partialActivePattern);
            Driver.Run("recursion", Demo.recursion);
            Driver.Run("mutual recursion", Demo.mutualRecursion);
            Driver.Run("higher order", Demo.higherOrder);

            Driver.Run("async body", Demo.asyncBody);
            Driver.Run("task body", Demo.taskBody);
            Driver.Run("seq body", Demo.seqBody);
            Driver.Run("nested modules", Demo.nestedModules);

            Driver.Run("class members", Scenarios.instanceMember);
            Driver.Run("static member", Worker.StaticEntry);
            Driver.Run("property getter", Scenarios.propertyGetter);
            Driver.Run("property setter", Scenarios.propertySetter);
            Driver.Run("interface implementation", Scenarios.interfaceImplementation);
            Driver.Run("generic type member", Scenarios.genericTypeMember);
            Driver.Run("union member", Scenarios.unionMember);
            Driver.Run("record member", Scenarios.recordMember);

            Driver.Run("ctor + static ctor", Scenarios.constructors);
            Driver.Run("module initialization", Scenarios.moduleInitialization);

            Driver.Run("delegate call", Scenarios.delegateCall);
            Driver.Run("event handler", Scenarios.eventHandler);

            Driver.Run("C# callback from F#", Driver.CallbackScenario);
            Driver.Run("long mixed chain", MixedChain.run);

            Console.WriteLine("done");
        }
    }
}
