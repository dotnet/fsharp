// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:4365
// Mistake in generation of code for disposal in "match" sequence expressions
//<Expects status="success"></Expects>
let r = ref 0
let f () = [ if (incr r; true) then yield! failwith "" ]
let x = (try f () with Failure _ -> [!r])

// Correct code yields x = [1]
// Buggy code would yield [2] as incr was called twice
if x.Head <> 1 then exit 1

exit 0

