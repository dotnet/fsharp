module Pos16

// verify backticked active pattern cases

// single case
let (|``A B C``|) (x:int) = x * 2
let (``A B C``(k)) = 5
if k <> 10 then exit 1

// partial case
let (|``Alpha  Beta `` |_|) (x:int) = 
    if x = 0 then Some() else None

match 0,1 with
| (_, ``Alpha  Beta ``) -> exit 1
| (``Alpha  Beta ``,_) -> ()
| _ -> exit 1

// multi-case
let (|``Charlie Delta``|Echo|``Fox Trot``|) (x:int) =
    if x = 0 then ``Charlie Delta``
    elif x = 1 then Echo
    else ``Fox Trot``

match 0,1,2 with
| ``Charlie Delta``,Echo,``Fox Trot`` -> ()
| _ -> exit 1


module UnitOfMeasurePrintfPositiveTests = 

    open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames
    open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

    let _ = sprintf "%f" 1.0<meter>
    let _ = sprintf "%f" 1.0<metre>
    let _ = sprintf "%f" 1.0<m>
    let _ = sprintf "%f" 1.0<m/s>

    let _ = sprintf "%d" 1<meter>
    let _ = sprintf "%d" 1<metre>
    let _ = sprintf "%d" 1<m>
    let _ = sprintf "%d" 1<m/s>

    let _ = sprintf "%g" 1.0M<meter>
    let _ = sprintf "%g" 1.0M<metre>
    let _ = sprintf "%g" 1.0M<m>
    let _ = sprintf "%g" 1.0M<m/s>
