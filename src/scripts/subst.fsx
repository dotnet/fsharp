// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information,
//
//=========================================================================================

#load "scriptlib.fsx"

open System.IO
open System.Text.RegularExpressions

try 
    let input = getCmdLineArgReqd    "--in:"  
    let output = getCmdLineArgReqd    "--out:"  
    let pattern = getCmdLineArgReqd    "--pattern:"  
    let replacement = getCmdLineArgReqd    "--replacement:"  

    File.WriteAllText(output,Regex.Replace(File.ReadAllText(input), pattern, replacement))
    exit 0
with e -> 
    eprintfn "%A" e
    exit 1
