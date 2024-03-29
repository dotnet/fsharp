module Program

open System
open System.Text
open System.Collections.Generic
open EmittedIL.StringEncoding.NormalizationForm

let csharpValues1 = (NormalizationFormDCSharp_0000.Values()).GetEnumerator()
let csharpValues2 = (NormalizationFormDCSharp_0080.Values()).GetEnumerator()
let fsharpValues1 = (NormalizationFormDFSharp_0000.Values()).GetEnumerator()
let fsharpValues2 = (NormalizationFormDFSharp_0080.Values()).GetEnumerator()

// TODO should be 0x123
for i in [0 .. 0x07F] do
    if not(csharpValues1.MoveNext()) then
        raise (new Exception($"Insufficient values for C# #1 failed at'{i}'"))
    if not (fsharpValues1.MoveNext()) then
        raise (new Exception($"Insufficient values for F# #1 failed at'{i}'"))
    if not(csharpValues2.MoveNext()) then
        raise (new Exception($"Insufficient values for C# #2failed at'{i}'"))
    if not (fsharpValues2.MoveNext()) then
        raise (new Exception($"Insufficient values for F# #2 failed at'{i}'"))

    let csv1 = unbox<int> csharpValues1.Current
    let fsv1 = unbox<int> fsharpValues1.Current
    let csv2 = unbox<int> csharpValues2.Current
    let fsv2 = unbox<int> fsharpValues2.Current

    printfn "'%A' - '%A'" csv1 fsv1
    printfn "'%A' - '%A'" csv2 fsv2

    if csv1 <> fsv1 then
        raise (new Exception($"Difference detected NormalizationFormatD at: '{i}': '{csv1} <> '{fsv1}'"))

    if csv2 <> fsv2 then
        raise (new Exception($"Difference detected NormalizationFormatD at: '{i}': '{csv2} <> '{fsv2}'"))
()
