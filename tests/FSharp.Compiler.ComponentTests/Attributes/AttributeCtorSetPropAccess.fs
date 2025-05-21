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

    [<Literal>]
    let private setPrivatePropFsCode = """
    namespace TestAttribute
    open System
    open TestAttribute
    
    module TestAttributeModule =

        [<FooAttribute(X = 100)>]
        let myFunction() = ()
    """
    
    [<Literal>]
    let private setInternalPropFsCode = """
    namespace Other
    open System
    open TestAttribute
    
    module TestAttributeModule =

        [<FooAttribute(X = 100)>]
        let myFunction() = ()
    """

    [<Theory>]
    [<InlineData("private", setPrivatePropFsCode)>]
    [<InlineData("internal", setInternalPropFsCode)>]
    let ``Cannot set property outside its accessibility scope``(modifier: string, fsCode: string): unit =
        FSharp fsCode 
        |> withReferences [(CSharp <| csLib.Replace("%s", modifier))] 
        |> compile 
        |> shouldFail
        |> withDiagnostics [ (ErrorType.Error 3248, Line 8, Col 28, Line 8, Col 31, "Property 'X' cannot be set because the setter is private") ] 
        |> ignore

