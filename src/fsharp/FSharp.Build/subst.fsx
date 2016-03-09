// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

open System.IO
 
module Seq =
    let evens s = s |> Seq.mapi (fun i x -> (i, x)) |> Seq.choose (function (i, x) when i % 2 = 0 -> Some(x) | _ -> None)
    let odds  s = s |> Seq.mapi (fun i x -> (i, x)) |> Seq.choose (function (i, x) when i % 2 = 1 -> Some(x) | _ -> None)
    
let rawArgs = System.Environment.GetCommandLineArgs()

if rawArgs.Length < 4 || rawArgs.Length % 2 <> 0 then
    eprintfn "Invalid command line args. usage 'subst.exe file origtext1 replacetext1 ... origtextN replacetextN'"
    exit 1
else

(rawArgs |> Seq.evens, rawArgs |> Seq.odds)
||> Seq.zip
|> Seq.skip 1
|> Seq.fold (fun (content:string) (orig, replace) -> content.Replace(orig, replace)) (File.ReadAllText(rawArgs.[1]))
|> printfn "%s"
  
exit 0