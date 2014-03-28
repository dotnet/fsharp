// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for DEV11:11983
// Title: Calling Dispose with use/using on a Struct results in boxing because of explicit interfaces

let f1 (x:System.Collections.Generic.List<'T>) =
  for a in x do 
  ()
