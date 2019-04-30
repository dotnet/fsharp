// #Regression #TypeInference 
// Regression for FSHARP1.0:5749
// Better error message for overload resolution to help ease pain associated with mismatch of intellisense information


let array = [| "Ted"; "Katie"; |]
Array.iter (fun it -> System.Console.WriteLine(it))

Array.iter (fun it -> System.Console.WriteLine(it)) array

array |> Array.iter (fun it -> System.Console.WriteLine(it))

//<Expects status="error" span="(7,23-7,51)" id="FS0041">A unique overload for method 'WriteLine' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: System\.Console\.WriteLine\(buffer: char \[\]\) : unit, System\.Console\.WriteLine\(format: string, \[<System\.ParamArray>\] arg: obj \[\]\) : unit, System\.Console\.WriteLine\(value: bool\) : unit, System\.Console\.WriteLine\(value: char\) : unit, System\.Console\.WriteLine\(value: decimal\) : unit, System\.Console\.WriteLine\(value: float\) : unit, System\.Console\.WriteLine\(value: float32\) : unit, System\.Console\.WriteLine\(value: int\) : unit, System\.Console\.WriteLine\(value: int64\) : unit, System\.Console\.WriteLine\(value: obj\) : unit, System\.Console\.WriteLine\(value: string\) : unit, System\.Console\.WriteLine\(value: uint32\) : unit, System\.Console\.WriteLine\(value: uint64\) : unit$</Expects>
//<Expects status="error" span="(9,23-9,51)" id="FS0041">A unique overload for method 'WriteLine' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: System\.Console\.WriteLine\(buffer: char \[\]\) : unit, System\.Console\.WriteLine\(format: string, \[<System\.ParamArray>\] arg: obj \[\]\) : unit, System\.Console\.WriteLine\(value: bool\) : unit, System\.Console\.WriteLine\(value: char\) : unit, System\.Console\.WriteLine\(value: decimal\) : unit, System\.Console\.WriteLine\(value: float\) : unit, System\.Console\.WriteLine\(value: float32\) : unit, System\.Console\.WriteLine\(value: int\) : unit, System\.Console\.WriteLine\(value: int64\) : unit, System\.Console\.WriteLine\(value: obj\) : unit, System\.Console\.WriteLine\(value: string\) : unit, System\.Console\.WriteLine\(value: uint32\) : unit, System\.Console\.WriteLine\(value: uint64\) : unit$</Expects>
