// #Regression #Diagnostics 
// Negative test for signature errors

// This test used to be part of the FSHARP suite (fsharp\typecheck\sigs\neg06.ml)
// It has been moved where since we can easily deal with expected errors in FSHARPQA
// (the main issue with dumb "diff against a baseline" is that we cannot easily deal
// with hardcoded paths, like in this case.

// The warning currently looks like this:
// warning FS0064: This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near 'C:\fsharp2\staging\src\tests\fsharp\typecheck\sigs\neg06.ml(35,34)-(35,45)' has been constrained to be type 'SealedType'.

//<Expects id="FS0064" span="(18,35-18,46)" status="warning">SealedType</Expects>
module M

[<Sealed>]
type SealedType() =  member x.P = 1                      // a sealed class
    
let WarningOnHashOfSealedType (x: #SealedType) = x
