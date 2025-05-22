namespace FSharp.Compiler.ComponentTests.Attributes

open Xunit
open FSharp.Test.Compiler

module AttributeCtorSetPropAccess =
    let private csLib = """
    using System;

    namespace TestAttribute
    {
        [AttributeUsage(AttributeTargets.All, Inherited = false)]
        public class FooAttribute : Attribute
        {
            public int X { get; %s set; }
        }
    }
    """

    let private fsCode = """
    namespace Other
    open System
    open TestAttribute
    
    module TestAttributeModule =

        [<FooAttribute(X = 100)>]
        let myFunction() = ()
    """

    [<Theory>]
    [<InlineData("private")>]
    [<InlineData("internal")>]
    [<InlineData("protected")>]
    [<InlineData("private protected")>]
    [<InlineData("protected internal")>]
    let ``Cannot set property outside its accessibility scope``(modifier: string): unit =
        FSharp fsCode 
        |> withReferences [(CSharp <| csLib.Replace("%s", modifier))] 
        |> compile 
        |> shouldFail
        |> withDiagnostics [ (ErrorType.Error 3248, Line 8, Col 28, Line 8, Col 31, "Property 'X' cannot be set because the setter is private") ] 
        |> ignore

    [<Fact>]
    let ``Can set property inside its accessibility scope``(): unit =
        FSharp fsCode 
        |> withReferences [(CSharp <| csLib.Replace("%s", ""))] 
        |> compile 
        |> shouldSucceed
        |> ignore