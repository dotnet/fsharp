// #Regression #Libraries #Collections 
// 
// Regression test for FSHARP1.0:4726
// Makes sure that the .Dispose() method, if available, in invoked on IEnumerable
// 
// This test should probably go under the SystematicUnitTests suite, but 
// I could not decide how to make it fit... so I'm leaving it here.
//
//<Expect status="success"></Expect>

let mutable dispose_called_in_E = 0    // we expect this to be incremented 3 times
let mutable dispose_called_in_C = 0    // we expect this to be incremented once (=this is what the bug was about, i.e. .Dispose() was never invoked)

type E(c:int) = class
                 interface System.IDisposable with
                  member x.Dispose () = dispose_called_in_E <- dispose_called_in_E + 1
                end

type C() = class
                    let mutable i = 0
                    interface System.Collections.IEnumerator with
                        member x.Current with get () = new E(i) :> obj
                        member x.MoveNext () = i <- i+1
                                               i<4
                        member x.Reset () = i <- 0
                    interface System.Collections.IEnumerable with
                        member x.GetEnumerator () = x :> System.Collections.IEnumerator
                        
                    interface System.IDisposable with
                      member x.Dispose () = dispose_called_in_C <- dispose_called_in_C + 1
                    end
    
           end

let _ = Seq.cast (new C()) |> Seq.map (fun x -> use o = x; 
                                                o) |> Seq.length
               
(if (dispose_called_in_E<>3 && dispose_called_in_C<>1) then 1 else 0) |> exit 
