// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// --------------------------------------------------------------------	
// Outcomes.  These are used to describe steps of a machine that
// may raise errors.  The errors can be trapped.
// --------------------------------------------------------------------	

module internal Microsoft.FSharp.Compiler.Outcome

open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library

let success a = Result a
let raze (b:exn) = Exception b

// map
let (|?>) res f = 
  match res with 
  | Result x -> Result(f x )
  | Exception err -> Exception err
  
let ForceRaise = function
  | Result x -> x
  | Exception err -> raise err

let otherwise f x =
  match x with 
  | Result x -> success x
  | Exception _err -> f()

    
