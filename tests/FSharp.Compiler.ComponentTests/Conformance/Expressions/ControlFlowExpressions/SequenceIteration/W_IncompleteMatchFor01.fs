// #Regression #Conformance #ControlFlow #Sequences 
// Regression for FSHARP1.0:5733
// For expressions should warn when elements will be skipped just like computation expressions do
//<Expects id="FS0025" span="(10,5-10,11)" status="warning">Incomplete pattern matches on this expression\. For example, the value 'None' may indicate a case not covered by the pattern\(s\)\. Unmatched elements will be ignored\.$</Expects>
//<Expects id="FS0025" span="(13,5-13,11)" status="warning">Incomplete pattern matches on this expression\. For example, the value 'None' may indicate a case not covered by the pattern\(s\)\. Unmatched elements will be ignored\.$</Expects>
//<Expects id="FS0025" span="(15,5-15,11)" status="warning">Incomplete pattern matches on this expression\. For example, the value 'Some \(0\)' may indicate a case not covered by the pattern\(s\)\. Unmatched elements will be ignored\.$</Expects>
//<Expects id="FS0025" span="(17,5-17,6)" status="warning">Incomplete pattern matches on this expression\. For example, the value '0' may indicate a case not covered by the pattern\(s\)\. Unmatched elements will be ignored\.$</Expects>
//<Expects id="FS0025" span="(22,21-22,58)" status="warning">Incomplete pattern matches on this expression\. For example, the value 'None' may indicate a case not covered by the pattern\(s\)\.$</Expects>
//<Expects id="FS0025" span="(27,20-27,28)" status="warning">Incomplete pattern matches on this expression\. For example, the value 'None' may indicate a case not covered by the pattern\(s\)\.$</Expects>
for Some x in [Some 3; None] do
    ()
let s = [Some 3; None] :> seq<_>
for Some x in s do
    ()
for Some 1 in s do
    ()
for 1 in [1;2;3] do
    ()

// These warned prior to the fix and throw runtime exceptions
async {
    for Some(nm) in [ Some("James"); None; Some("John") ] do 
        printfn "%d" nm.Length 
} |> Async.RunSynchronously         
 
 
let s2 = seq { for Some(nm) in [ Some("James"); None; Some("John") ] do
                  yield nm.Length }
s2 |> Seq.iter (printfn "%A")
