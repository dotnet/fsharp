// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
//=========================================================================================

#load "scriptlib.fsx"

open System.IO
open System.Text.RegularExpressions

try 
    let input = getCmdLineArgReqd    "--in:"  
    let output = getCmdLineArgReqd    "--out:"  
    let pattern1 = getCmdLineArgReqd    "--pattern1:"  
    let replacement1 = getCmdLineArgReqd    "--replacement1:"  
    let pattern2 = getCmdLineArgOptional    "--pattern2:"  
    let replacement2 = getCmdLineArgOptional    "--replacement2:"  

    let inp0 = File.ReadAllText(input)
    let inp1 = Regex.Replace(inp0, pattern1, replacement1)
    let inp2 = match pattern2, replacement2 with Some p2, Some r2 -> Regex.Replace(inp1, p2, r2) | None, None -> inp1 | _ -> failwith "if pattern2 is given, replacement2 must also be given"
    File.WriteAllText(output,inp2)
    exit 0
with e -> 
    eprintfn "%A" e
    exit 1
