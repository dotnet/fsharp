// #Regression #Conformance #LexicalAnalysis 
// Regression test for FSharp1.0:2371 - Compiler should, at least, emit a better error message when trying to define a function whose name is ±, §, or §§
//<Expects id="FS0010" span="(13,6)" status="error">Unexpected character '.+' in pattern. Expected '\)' or other token</Expects>
//<Expects id="FS0583" span="(13,5)" status="error">Unmatched '\('</Expects>
//<Expects id="FS0010" span="(15,6)" status="error">Unexpected character '.+' in expression</Expects>
#light
let (++) x = x + 1
if (++) 4 <> 5 then exit 1

let (+!+) x y = x + y
if 1 +!+ 2 <> 3 then exit 1

let (±) x y = (x + y), (x - y)

if 4 ± 2 <> (6, 2) then exit 1

let rec (§) x =
    if x > 0 then
       (§) -(x - 1)
    elif x = 0 then
       "done"
    else
       (§) -(x + 1)

if (§) 99 <> "done" then exit 1

exit 0
