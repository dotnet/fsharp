// #Regression #Conformance #UnitsOfMeasure #TypeInference #TypeConstraints 
#light

// Regression test for FSharp1.0:4036 - acos should force its argument to be dimensionless (also asin etc.)
//<Expects id="FS0464" status="warning" span="(9,18)">This code is less generic than indicated by its annotations\. A unit-of-measure specified using '_' has been determined to be '1', i\.e\. dimensionless</Expects>

let sq =
    seq {
    yield sin   (3.14<_>)
    yield cos   (3.14<_>)
    yield tan   (3.14<_>)
    yield sinh  (3.14<_>)
    yield cosh  (3.14<_>)
    yield tanh  (3.14<_>)
    yield asin  (0.86<_>)
    yield acos  (0.89<_>)
    yield atan  (3.14<_>)
    yield ceil  (3.14<_>)
    yield floor (3.14<_>)
    yield log   (3.14<_>)
    yield log10 (3.14<_>)
    yield exp   (3.14<_>)
    yield pown  (3.14<_>) 2
    yield round (3.14<_>)
    yield truncate (3.13<_>)
    }
    |> Seq.sum

ignore 0
