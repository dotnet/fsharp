// #Regression #NoMT #Import 
module M

// Regression test for FSharp1.0:4131 - Iteration over StringDictionary seems broken
// test each element of StringDictionary is treated as a DictionaryEntry

open System.Collections.Specialized
open System.Text.RegularExpressions

let strDict = new StringDictionary()

{1..10} |> Seq.iter (fun i -> strDict.Add("Key" + i.ToString(), "Val" + i.ToString()))

for de in strDict do
    let de = de :?> System.Collections.DictionaryEntry
    de.Key.Equals(de.Value) |> ignore


// Returns the list of matches for a given regular expression.
let (|RegExMatch|_|) (pat:string) (inp:string) = 
    let m = Regex.Match(inp, pat) in
    // Note the List.tl, since the first group is always the entirety of the matched string.
    if m.Success then 
        Some (List.tail [ for g in m.Groups -> g.Value ]) 
    else 
        None
