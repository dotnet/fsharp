#nowarn "9"
   
namespace Microsoft.FSharp.Compatibility

open System.Collections.Generic

[<RequireQualifiedAccess>]
module Seq = 

    let generate openf compute closef = 
        seq { let r = openf() 
              try 
                let x = ref None
                while (x := compute r; (!x).IsSome) do
                    yield (!x).Value
              finally
                 closef r }
    
    let generate_using (openf : unit -> ('b :> System.IDisposable)) compute = 
        generate openf compute (fun (s:'b) -> s.Dispose())

