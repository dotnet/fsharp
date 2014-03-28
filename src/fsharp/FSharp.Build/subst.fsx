// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

let x1,y1,x2,y2,x3,y3,file = 
   match System.Environment.GetCommandLineArgs() with 
   | [| _; x1;y1;x2;y2;x3;y3;file |] -> x1,y1,x2,y2,x3,y3,file
   | _ -> 
       eprintfn "Invalid command line args. usage 'subst.exe origtext1 replacetext1 origtext2 replacetext2 origtext3 replacetext3 file'"
       exit 1 
   
file 
   |> System.IO.File.ReadAllText 
   |> (fun s -> s.Replace(x1,y1)) 
   |> (fun s -> s.Replace(x2,y2)) 
   |> (fun s -> s.Replace(x3,y3)) 
   |> printfn "%s"
exit 0