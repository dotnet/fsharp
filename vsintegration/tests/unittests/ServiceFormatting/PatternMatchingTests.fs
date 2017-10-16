// Copied from https://github.com/dungpa/fantomas and modified by Vasily Kirichenko

module FSharp.Compiler.Service.Tests.ServiceFormatting.PatternMatchingTests

open NUnit.Framework
open FsUnit
open TestHelper

[<Test>]
let ``match expressions``() =
    formatSourceString false """
    let filter123 x =
        match x with
        | 1 | 2 | 3 -> printfn "Found 1, 2, or 3!"
        | a -> printfn "%d" a""" config
    |> prepend newline
    |> should equal """
let filter123 x =
    match x with
    | 1 | 2 | 3 -> printfn "Found 1, 2, or 3!"
    | a -> printfn "%d" a
"""

[<Test>]
let ``function keyword``() =
    formatSourceString false """
    let filterNumbers =
        function | 1 | 2 | 3 -> printfn "Found 1, 2, or 3!"
                 | a -> printfn "%d" a""" config
    |> prepend newline
    |> should equal """
let filterNumbers =
    function 
    | 1 | 2 | 3 -> printfn "Found 1, 2, or 3!"
    | a -> printfn "%d" a
"""

[<Test>]
let ``when clauses and as patterns``() =
    formatSourceString false """
let rangeTest testValue mid size =
    match testValue with
    | var1 when var1 >= mid - size/2 && var1 <= mid + size/2 -> printfn "The test value is in range."
    | _ -> printfn "The test value is out of range."

let (var1, var2) as tuple1 = (1, 2)
printfn "%d %d %A" var1 var2 tuple1""" config
    |> prepend newline
    |> should equal """
let rangeTest testValue mid size =
    match testValue with
    | var1 when var1 >= mid - size / 2 && var1 <= mid + size / 2 -> 
        printfn "The test value is in range."
    | _ -> printfn "The test value is out of range."

let (var1, var2) as tuple1 = (1, 2)

printfn "%d %d %A" var1 var2 tuple1
"""

[<Test>]
let ``and & or patterns``() =
    formatSourceString false """
let detectZeroOR point =
    match point with
    | (0, 0) | (0, _) | (_, 0) -> printfn "Zero found."
    | _ -> printfn "Both nonzero."

let detectZeroAND point =
    match point with
    | (0, 0) -> printfn "Both values zero."
    | (var1, var2) & (0, _) -> printfn "First value is 0 in (%d, %d)" var1 var2
    | (var1, var2)  & (_, 0) -> printfn "Second value is 0 in (%d, %d)" var1 var2
    | _ -> printfn "Both nonzero."
"""  config
    |> prepend newline
    |> should equal """
let detectZeroOR point =
    match point with
    | (0, 0) | (0, _) | (_, 0) -> printfn "Zero found."
    | _ -> printfn "Both nonzero."

let detectZeroAND point =
    match point with
    | (0, 0) -> printfn "Both values zero."
    | (var1, var2) & (0, _) -> printfn "First value is 0 in (%d, %d)" var1 var2
    | (var1, var2) & (_, 0) -> printfn "Second value is 0 in (%d, %d)" var1 var2
    | _ -> printfn "Both nonzero."
"""

[<Test>]
let ``paren and tuple patterns``() =
    formatSourceString false """
let countValues list value =
    let rec checkList list acc =
       match list with
       | (elem1 & head) :: tail when elem1 = value -> checkList tail (acc + 1)
       | head :: tail -> checkList tail acc
       | [] -> acc
    checkList list 0

let detectZeroTuple point =
    match point with
    | (0, 0) -> printfn "Both values zero."
    | (0, var2) -> printfn "First value is 0 in (0, %d)" var2
    | (var1, 0) -> printfn "Second value is 0 in (%d, 0)" var1
    | _ -> printfn "Both nonzero."
"""  config
    |> prepend newline
    |> should equal """
let countValues list value =
    let rec checkList list acc =
        match list with
        | (elem1 & head) :: tail when elem1 = value -> checkList tail (acc + 1)
        | head :: tail -> checkList tail acc
        | [] -> acc
    checkList list 0

let detectZeroTuple point =
    match point with
    | (0, 0) -> printfn "Both values zero."
    | (0, var2) -> printfn "First value is 0 in (0, %d)" var2
    | (var1, 0) -> printfn "Second value is 0 in (%d, 0)" var1
    | _ -> printfn "Both nonzero."
"""

[<Test>]
let ``type test and null patterns``() =
    formatSourceString false """
let detect1 x =
    match x with
    | 1 -> printfn "Found a 1!"
    | (var1 : int) -> printfn "%d" var1

let RegisterControl(control:Control) =
    match control with
    | :? Button as button -> button.Text <- "Registered."
    | :? CheckBox as checkbox -> checkbox.Text <- "Registered."
    | _ -> ()

let ReadFromFile (reader : System.IO.StreamReader) =
    match reader.ReadLine() with
    | null -> printfn "\n"; false
    | line -> printfn "%s" line; true""" config
    |> prepend newline
    |> should equal """
let detect1 x =
    match x with
    | 1 -> printfn "Found a 1!"
    | (var1 : int) -> printfn "%d" var1

let RegisterControl(control : Control) =
    match control with
    | :? Button as button -> button.Text <- "Registered."
    | :? CheckBox as checkbox -> checkbox.Text <- "Registered."
    | _ -> ()

let ReadFromFile(reader : System.IO.StreamReader) =
    match reader.ReadLine() with
    | null -> 
        printfn "\n"
        false
    | line -> 
        printfn "%s" line
        true
"""

[<Test>]
let ``record patterns``() =
    formatSourceString false """
type MyRecord = { Name: string; ID: int }

let IsMatchByName record1 (name: string) =
    match record1 with
    | { MyRecord.Name = nameFound; ID = _; } when nameFound = name -> true
    | _ -> false """ config
    |> prepend newline
    |> should equal """
type MyRecord =
    { Name : string
      ID : int }

let IsMatchByName record1 (name : string) =
    match record1 with
    | { MyRecord.Name = nameFound; ID = _ } when nameFound = name -> true
    | _ -> false
"""

[<Test>]
let ``desugared lambdas``() =
    formatSourceString false """
try 
    fst(find (fun (s, (s', ty): int * int) -> 
                s' = s0 && can (type_match ty ty0) []) (!the_interface))
with
| Failure _ -> s0""" config
    |> prepend newline
    |> should equal """
try 
    fst 
        (find 
             (fun (s, (s', ty) : int * int) -> 
             s' = s0 && can (type_match ty ty0) []) (!the_interface))
with Failure _ -> s0
"""

[<Test>]
let ``another case of desugared lambdas``() =
    formatSourceString false """
find (fun (Ident op) x y -> Combp(Combp(Varp(op,dpty),x),y)) "term after binary operator" inp
"""  config
    |> prepend newline
    |> should equal """
find (fun (Ident op) x y -> Combp(Combp(Varp(op, dpty), x), y)) 
    "term after binary operator" inp
"""

[<Test>]
let ``yet another case of desugared lambdas``() =
    formatSourceString false """
let UNIFY_ACCEPT_TAC mvs th (asl, w) = 
    let insts = term_unify mvs (concl th) w
    ([], insts), [], 
    let th' = INSTANTIATE insts th
    fun i [] -> INSTANTIATE i th'""" config
    |> prepend newline
    |> should equal """
let UNIFY_ACCEPT_TAC mvs th (asl, w) =
    let insts = term_unify mvs (concl th) w
    ([], insts), [], 
    let th' = INSTANTIATE insts th
    fun i [] -> INSTANTIATE i th'
"""

[<Test>]
let ``desugared lambdas again``() =
    formatSourceString false """
fun P -> T""" config
    |> prepend newline
    |> should equal """
fun P -> T
"""

[<Test>]
let ``should consume spaces before inserting comments``() =
    formatSourceString false """
let f x = 
  a || // other case
        match n with
        | 17 -> false
        | _ -> true""" config
    |> prepend newline
    |> should equal """
let f x =
    a || // other case
         match n with
         | 17 -> false
         | _ -> true
"""

[<Test>]
let ``should not remove parentheses in patterns``() =
    formatSourceString false """
let x =
    match y with
    | Start(-1) -> true
    | _ -> false""" config
    |> prepend newline
    |> should equal """
let x =
    match y with
    | Start(-1) -> true
    | _ -> false
"""

[<Test>]
let ``should indent function keyword in function application``() =
    formatSourceString false """
let v =
    List.tryPick (function 1 -> Some 1 | _ -> None) [1; 2; 3]""" config
    |> prepend newline
    |> should equal """
let v =
    List.tryPick (function 
        | 1 -> Some 1
        | _ -> None) [ 1; 2; 3 ]
"""

[<Test>]
let ``should put brackets around tuples in type tests``() =
    formatSourceString false """
match item.Item with
| :? FSharpToolTipText as titem -> ()
| :? (string * XmlDoc) as tip -> ()
| _ -> ()""" config
    |> prepend newline
    |> should equal """
match item.Item with
| :? FSharpToolTipText as titem -> ()
| :? (string * XmlDoc) as tip -> ()
| _ -> ()
"""

[<Test>]
let ``should support rational powers on units of measures``() =
    formatSourceString false """
[<Measure>] type X = cm^(1/2)/W""" config
    |> prepend newline
    |> should equal """
[<Measure>]
type X = cm^(1/2) / W
"""