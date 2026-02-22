// #Regression #Diagnostics 
// Negative test for signature errors

// This test used to be part of the FSHARP suite (fsharp\typecheck\sigs\neg04.ml)
// It has been moved where since we can easily deal with expected errors in FSHARPQA
// (the main issue with dumb "diff against a baseline" is that we cannot easily deal
// with hardcoded paths, like in this case.

// The error currently looks like this:
// The type 'unit' is not compatible with the type 'GrowingArray<int>'. See also C:\Users\t\AppData\Local\Temp\1\ConsoleApplication4\ConsoleApplication4\Program.fs(9,41)-(9,46).

//<Expects status="error" span="(19,33-19,45)" id="FS0193">.+'unit'.+'GrowingArray<int>'.+\(18,41\)-\(18,46\)</Expects>
module M
type GrowingArray<'a> = System.Collections.Generic.List<'a>
let nextPrime (sofar : GrowingArray<int>) n = failwith "nyi"
let primes = Seq.unfold
                 (fun (soFar,n) -> 
                    let next = nextPrime soFar n in 
                    Some(next, (soFar.Add(n), next+1)))
