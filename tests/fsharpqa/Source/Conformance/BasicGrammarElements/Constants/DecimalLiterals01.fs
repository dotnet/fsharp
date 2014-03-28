// #Regression #Conformance #BasicGrammarElements #Constants 
#light

// Regressiont test for FSharp1.0: 2543 - Decimal literals do not support exponents

let oneOfOneMiDec = 1.0E-6M
let oneMiDec      = 1.0E+6M

let one = oneOfOneMiDec * oneMiDec

if one <> 1.0M then exit 1

let result = 1.0E0M + 2.0E1M + 3.0E2M + 4.0E3M + 5.0E4M + 6.0E5M + 7.0E6M + 8.0E7M + 9.0E8M + 
             1.0E-1M + 2.0E-2M + 3.0E-3M + 4.0E-4M + 5.0E-5M + 6.0E-6M + 7.0E-7M + 8.0E-8M + 9.0E-9M

if result <> 987654321.123456789M then exit 1

// Test boundary case
let xMax = 1.0E28M
let XMin = 1.0E-28M

// Test with leading zeros in exponential and
let y = 1.0E00M + 2.0E01M + 3.E02M + 1.E-01M + 2.0E-02M
if y <> 321.12M then exit 1

exit 0
