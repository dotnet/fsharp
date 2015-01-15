namespace User

open System

module Main =

  [<EntryPoint>]
  let start args =
     let _ = PCL.Lib.year DateTime.Now
     printfn "OK"
     0
