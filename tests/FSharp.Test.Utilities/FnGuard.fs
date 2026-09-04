namespace FSharp.Test

open System
open System.Reflection
open Xunit.Sdk
open Xunit.v3

/// Fails an F# [<Fact>]/[<Theory>] whose declared return type is a value xUnit never executes -- an
/// FSharpFunc (accidental partial application), a delegate, or a Lazy<_> -- since the body then runs
/// no assertions yet reports a false green (dotnet/fsharp#18147). Only discovery is customized: the
/// offender becomes xUnit's stock ExecutionErrorTestCase, so the whole execution chain is untouched.
/// (A lazy seq is the same footgun but its IEnumerable<_> is indistinguishable from a legit collection
/// return, so it is out of scope.)
type FnGuardDiscoverer(testAssembly: IXunitTestAssembly) =
    inherit XunitTestFrameworkDiscoverer(testAssembly)

    static let neverExecuted (returnType: Type) =
        typeof<Delegate>.IsAssignableFrom returnType
        || (returnType.IsGenericType
            && (let d = returnType.GetGenericTypeDefinition() in d = typedefof<int -> int> || d = typedefof<Lazy<_>>))

    override _.FindTestsForMethod(testMethod, discoveryOptions, discoveryCallback) =
        match Seq.tryHead testMethod.FactAttributes with
        | Some fact when isNull fact.Skip && neverExecuted testMethod.ReturnType ->
            let struct (displayName, _, _, _, _, _, _, sourceFile, sourceLine, _, uniqueId, resolved) =
                TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, fact, label = null)
            let message =
                $"Test method '{testMethod.TestClass.TestClassName}.{testMethod.MethodName}' returns {testMethod.ReturnType.Name}, "
                + "which xUnit never executes, so none of its assertions run. Fully apply the call, or return unit/Task/ValueTask/Async."
            discoveryCallback.Invoke(ExecutionErrorTestCase(resolved, displayName, uniqueId, sourceFile, sourceLine, message) :> ITestCase)
        | _ -> base.FindTestsForMethod(testMethod, discoveryOptions, discoveryCallback)

/// Registered assembly-wide via [<assembly: TestFramework>] in XunitSetup.fs. CreateExecutor is not
/// overridden, so execution stays stock.
type FnGuardTestFramework(configFileName: string) =
    inherit XunitTestFramework(configFileName)
    new() = FnGuardTestFramework null

    override _.CreateDiscoverer(assembly: Assembly) =
        FnGuardDiscoverer(XunitTestAssembly(assembly, configFileName, assembly.GetName().Version))
        :> ITestFrameworkDiscoverer
