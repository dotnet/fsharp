module FSharp.Compiler.ComponentTests.Signatures.NestedTypeTests

open Xunit
open FsUnit
open FSharp.Test.Compiler
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

[<Fact>]
let ``Nested type with generics`` () =
    let CSLib =
        CSharp """
namespace Lib
{
    public class Upper<T>
    {
        public class Lower<U>
        {
            public void Meh()
            {
                
            }
        }
    }
}
"""

    FSharp
        """
module Sample

open Lib

let f (g: Upper<int>.Lower<string>) = g.Meh()
"""
    |> withReferences [ CSLib ]
    |> printSignatures
    |> should equal
        """
module Sample

val f: g: Lib.Upper<int>.Lower<string> -> unit"""

[<Fact>]
let ``Multiple generics in nested type`` () =
    let CSLib =
        CSharp """
namespace Lib
{
    public class Root<A, B, C, D, E>
    {
        public class Foo<T, U, V, W>
        {
            public class Bar<X, Y, Z>
            {
                public void Meh()
                {

                }
            }
        }
    }
}
"""

    FSharp
        """
module Sample

open System
open Lib

let f (g: Root<TimeSpan,TimeSpan,TimeSpan,TimeSpan,TimeSpan>.Foo<int, float, string, System.DateTime>.Bar<char, int, string>) = g.Meh()
"""
    |> withReferences [ CSLib ]
    |> printSignatures
    |> should equal
        """
module Sample

val f: g: Lib.Root<System.TimeSpan,System.TimeSpan,System.TimeSpan,System.TimeSpan,System.TimeSpan>.Foo<int,float,string,System.DateTime>.Bar<char,int,string> -> unit"""

#if NETCOREAPP
[<Fact>]
let ``ImmutableArray<'T>.Builder roundtrip`` () =
    let impl =
        """
module Library

open System.Collections.Immutable

type ImmutableArrayViaBuilder<'T>(builder: ImmutableArray<'T>.Builder) = class end
"""

    let signature = printSignatures (Fs impl)

    Fsi signature    
    |> withAdditionalSourceFile (FsSource impl)
    |> compile
    |> shouldSucceed
#endif