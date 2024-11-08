// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test


module ``IEnumerable Tests`` =

    // Regression test for FSHARP1.0:4726
    // Makes sure that the .Dispose() method, if available, in invoked on IEnumerable

    let mutable dispose_called_in_E = 0    // we expect this to be incremented 3 times
    let mutable dispose_called_in_C = 0    // we expect this to be incremented once (=this is what the bug was about, i.e. .Dispose() was never invoked)
    
    type E(_c:int) = class
        interface System.IDisposable with
            member _.Dispose () = dispose_called_in_E <- dispose_called_in_E + 1
    end
    
    type C() = class
        let mutable i = 0
        interface System.Collections.IEnumerator with
            member _.Current with get () = new E(i) :> obj
            member _.MoveNext () = 
                i <- i+1 
                i<4
            member _.Reset () = i <- 0
        interface System.Collections.IEnumerable with
            member x.GetEnumerator () = x :> System.Collections.IEnumerator
                            
        interface System.IDisposable with
            member _.Dispose () = dispose_called_in_C <- dispose_called_in_C + 1
        end
    end

    [<Fact>]
    let ``Dispose``() =
        let _ = Seq.cast (new C()) |> Seq.map (fun x -> use o = x; 
                                                        o) |> Seq.length

        Assert.areEqual 3 dispose_called_in_E
        Assert.areEqual 1 dispose_called_in_C