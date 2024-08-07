// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open Xunit
open FSharp.Test.Compiler

module ParamArrayMigrated =

    let csharp =
        CSharp """
using System;

namespace CSharpAssembly
{
    [AttributeUsage(AttributeTargets.All)]
    public class AttributeWithParamArray : Attribute
    {
        public object[] Parameters;

        public AttributeWithParamArray(params object[] x)
        {

            Parameters = x;
        }
    }

    public class CSParamArray
    {
        public static int Method(params int[] allArgs)
        {
            int total = 0;
            foreach (int i in allArgs)
                total += i;

            return total;
        }

        public static int Method<T>(params T[] args)
        {
            return args.Length;
        }
    }
}"""

    [<Fact>]
    let ``Valid params call`` () =
        FSharp """
open System
open CSharpAssembly

// Apply the attribute
[<AttributeWithParamArray([| (0 :> obj) |])>]
type Foo() =
    [<AttributeWithParamArray([| ("foo" :> obj); ("bar" :> obj) |])>]
    override this.ToString() = "Stuff"

let callCSGenericMethod (a: 't[]) = CSParamArray.Method(a)

[<assembly:AttributeWithParamArray ([| |])>]
do
    let getTestAttribute (t : Type) =
        let tyAttributes = t.GetCustomAttributes(false)
        let attrib = tyAttributes |> Array.find (fun attrib -> match attrib with :? AttributeWithParamArray -> true | _ -> false)
        (attrib :?> AttributeWithParamArray)
    
    let tyFoo = typeof<Foo>
    let testAtt = getTestAttribute tyFoo
    if testAtt.Parameters <> [| (0 :> obj) |] then
        failwith "Attribute parameters not as expected"
    
    let directCallWorks =
        CSParamArray.Method(9, 8, 7) + CSParamArray.Method(1, 2) + CSParamArray.Method() = (9 + 8 + 7) + (1 + 2)
    if not directCallWorks then
        failwith "Calling C# param array method gave unexpected result"

    let callParamArray (x : int array) = CSParamArray.Method(x)
    let asArrayCallWorks = (callParamArray [| 9; 8; 7 |]) = (9 + 8 + 7)
    if not asArrayCallWorks then
        failwith "Calling C# param array method, passing args as an array, gave unexpected result"

    if callCSGenericMethod [|"1";"2";"3"|] <> 3 then
        failwith "Calling C# generic param array method gave unexpected result"

    if CSParamArray.Method("1", "2", "3") <> CSParamArray.Method([|"1"; "2"; "3"|]) then
        failwith "Calling C# generic param array in normal and expanded method gave unexpected result"
"""
        |> withReferences [csharp]
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Invalid params call`` () =
        FSharp """
open CSharpAssembly

[<AttributeWithParamArray([|upcast 0|])>]
type Foo() =
    override this.ToString() = "Stuff"
"""
        |> withReferences [csharp]
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 13, Line 4, Col 29, Line 4, Col 37,
            "The static coercion from type\n    int    \nto \n    'a    \n involves an indeterminate type based on information prior to this program point. Static coercions are not allowed on some types. Further type annotations are needed.")
            (Error 267, Line 4, Col 29, Line 4, Col 37,
            "This is not a valid constant expression or custom attribute value")
        ]
