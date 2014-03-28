//==========================================================================
// The interface to the module 
// is similar to that found in versions of other ML implementations, 
// but is not an exact match.
//===========================================================================

module Microsoft.FSharp.Compatibility.OCaml.Printexc


let to_string (e:exn) = 
  match e with 
  | Failure s -> s
  | :? System.ArgumentException as e -> sprintf "invalid argument: %s" e.Message
  | MatchFailureException(s,n,m) -> sprintf "match failure, file '%s', line %d, column %d" s n m
  | _ -> sprintf "%A\n" e

let print f x = try f x with e -> stderr.WriteLine (to_string e) ; raise e 
