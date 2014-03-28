// #Conformance #ObjectOrientedTypes #TypeExtensions 
#light

// Verify type extensions extend the full hierarchy of a type
// Verify that if you extend an interface a class implements, the class is extended

type System.Collections.Generic.IEnumerable<'a> with 
    member x.SeqLength = Seq.length x
    
let aString = "abc"
if aString.SeqLength <> aString.Length then 
    exit 1

let aList = [1; 2; 3]
if aList.SeqLength <> 3 then
    exit 1

let anArray = [|1;2;3|]
if anArray.SeqLength <> 3 then
    exit 1

exit 0
