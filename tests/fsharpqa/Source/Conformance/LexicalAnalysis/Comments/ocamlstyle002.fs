// #Regression #Conformance #LexicalAnalysis 
#light

open System.Text.RegularExpressions

// FSB 2008, Parse error on comment "(**"

(**
This is a normal comment.
**)
let (|RegExMatch|_|) (pat:string) (inp:string) = 
    let m = Regex.Match(inp, pat) in
    // Note the List.tl, since the first group is always the entirety of the matched string.
    if m.Success
    then Some (List.tail [ for g in m.Groups -> g.Value ]) 
    else None


(**This is a normal comment.**)
let (|RegExMatch2|_|) (pat:string) (inp:string) = 
    let m = Regex.Match(inp, pat) in
    // Note the List.tl, since the first group is always the entirety of the matched string.
    if m.Success
    then Some (List.tail [ for g in m.Groups -> g.Value ]) 
    else None

(** This is a normal comment. **)
let (|RegExMatch3|_|) (pat:string) (inp:string) = 
    let m = Regex.Match(inp, pat) in
    // Note the List.tl, since the first group is always the entirety of the matched string.
    if m.Success
    then Some (List.tail [ for g in m.Groups -> g.Value ]) 
    else None

exit 0
