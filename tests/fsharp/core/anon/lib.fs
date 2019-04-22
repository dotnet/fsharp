
module AnonLib 


let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check (s:string) x1 x2 = 
    stderr.Write(s)
    if (x1 = x2) then stderr.WriteLine " OK"
    else (stderr.WriteLine (sprintf " failed, expected %A, got %A" x2 x1); report_failure (s))

let inline getX (x: ^TX) : ^X = 
        (^TX : (member get_X : unit -> ^X) (x))


let inline Y (x: ^TX) : ^X = 
        (^TX : (member get_Y : unit -> ^X) (x))


module KindB1 = 

    let data1 = {| X = 1 |}

    // Types can be written with the same syntax
    let data2 : {| X : int |} = data1

    // Access is as expected
    let f1 (v : {| X : int |}) = v.X

    // Access can be nested
    let f2 (v : {| X: {| X : int |} |}) = v.X.X

    // Access can be nested
    let f3 (v : {| Y: {| X : int |} |}) = v.Y.X

    // Access can be nested
    let f4 (v : {| Y: {| X : 'T |} |}) = v.Y.X

    check "coijoiwcnkc42c2"  {| Y = 1; X = "1" |} {| X = "1"; Y = 1 |}
    check "coijoiwcnkc42c3"  {| Y = 1; X = "1"; Z = 2 |} {| Z = 2; X = "1"; Y = 1 |}

    check "coijoiwcnkwle1"  {| a = 1 |} {| a = 1 |}
    check "coijoiwcnkwle2"  {| a = 2 |}  {| a = 2 |}

    check "coijoiwcnkwle3"  (sprintf "%A"  {| X = 10 |}) "{X = 10;}"
    check "coijoiwcnkwle4"  (sprintf "%A"  {| X = 10; Y = 1 |} |> fun s -> s.Replace("\n","").Replace("\r","")) ("{X = 10; Y = 1;}".Replace("\n","").Replace("\r",""))
    check "clekoiew09" (f2 {| X = {| X = 10 |} |}) 10
    check "cewkew0oijew" (f2 {| X = {| X = 20 |} |})  20

    check "ceoijew90ewcw1"  (FSharp.Reflection.FSharpType.IsRecord(typeof<{| X : int; Y: string |}>)) true
    check "ceoijew90ewcw2"  (FSharp.Reflection.FSharpType.GetRecordFields(typeof<{| X : int; Y: string |}>).Length) 2
    check "ceoijew90ewcw3"  (FSharp.Reflection.FSharpValue.GetRecordFields({| X = 1; Y = "a" |}).Length) 2
    check "ceoijew90ewcw4"  (FSharp.Reflection.FSharpType.IsRecord(typeof<{| X : int |}>)) true
    check "ceoijew90ewcw5"  (FSharp.Reflection.FSharpType.GetRecordFields(typeof<{| X : int |}>).Length) 1
    check "ceoijew90ewcw6"  (FSharp.Reflection.FSharpValue.GetRecordFields({| X = 1 |}).Length) 1
    check "ceoijew90ewcw7"  (FSharp.Reflection.FSharpValue.GetRecordFields({| X = 1 |}).[0]) (box 1)

    // Equality is possible
    check "ceijoewwekcj" {| a = 1-1 |} {| a = Unchecked.defaultof<_> |}

    // Comparison is possible if structural elements are comparable
    check "ceijowere9er" ({| a = 1+1 |} > {| a = 0 |}) true

    // Check we can alias these types
    type recd1 = {| a : int |}

    // test a generic function
    let test7<'T>(x:'T) = {| a = x  |}

    // test a generic function
    let test8<'T>(x:'T) = {| a = x; b = x  |}

    // Properties may satisfy member constraints
    // Access code may not be generic except through existing member constraints


    // To speciy a struct representation use this:
    let data3 = struct {| X = 1 |}

    // Types can be written with the same syntax
    let data4 : struct {| X : int |} = data3

    let testConstrainedAccess = getX {| X = 0 |},  getX data1,  getX {| X = 2; Y = "2" |}

    check "testConstrainedAccess1" (sprintf "%A"  testConstrainedAccess) "(0, 1, 2)" 

    let testConstrainedAccess2 = getX (struct {| X = 0 |}),  getX data3,  getX (struct {| X = 2; Y = "2" |})

    check "testConstrainedAccess2" (sprintf "%A"  testConstrainedAccess2) "(0, 1, 2)" 

module TestInAttributes = 
    type FooAttribute(ty: System.Type) =
        inherit System.Attribute()
        member x.Type = ty

    [<Foo(typeof<{| Field1: int; Field2 : string |}>)>]
    type C() = 
       member x.P = 1
    check "clkwweclk" ((typeof<C>.GetCustomAttributes(typeof<FooAttribute>,true).[0] :?> FooAttribute).Type) (typeof<{| Field1: int; Field2 : string |}>)

module KindB2 = 

    // Gives object that has full C#-compatibe anonymous metadata. Compiles to an instantiation of a generic type in the declaring assembly with appropriate .NET 
    // metadata (property names). The types are CLIMutable to be C#-compatible. The identity of the types are implicitly assembly-qualified.
    let data1 =   {| X = 1 |}

    let data1b =  {| Y = 1 |}

    let data1c =  {| X = 1; Y = 2 |}

    let data1d =  {| X = 1; Y = 3 |}

    // Types can be written with the same syntax
    let data2 : {| X : int |} = data1

    // Struct representations may be specified, though C# doesn't allow them
    let data3 =  struct {| X = 1; Y = 2 |}

    // Types can be written with the same syntax
    let data4 : struct {| X : int; Y : int |} = data3

    let testAccess = (data4.X, data4.Y, data1.X, data2.X, data3.X, data3.Y)

    printfn "{| X = 10 |} = %A" ({| X = 10 |} )
    printfn "{| X = 10 ; Y = \"abc\" |} = %A" ({| X = 10 ; Y = "abc"|} )
    
    let testConstrainedAccess = getX ({| X = 0 |}),  getX data1,  getX ({| X = 2; Y = "2" |})

    check "cew9cwoi" testConstrainedAccess (0, 1, 2)

    let testConstrainedAccess2 = getX (struct {| X = 0 |}),  getX data3,  getX (struct {| X = 2; Y = "2" |})

    check "cew9cwo3" testConstrainedAccess2 (0, 1, 2)

module CopyAndUpdateOfAnonRecord = 
    let data = {| X = 1 |}
    let data2 = {| data with Y = "1" |}
    let data3 = {| data with X = "3" |}
    let data4 = {| data2 with X = "3" |}
    check "fewjkvwno31" data.X 1
    check "fewjkvwno32" data2.X 1
    check "fewjkvwno33" data2.Y "1"
    check "fewjkvwno34" data3.X "3"
    check "fewjkvwno3443" data4.X "3"
    check "fewjkvwno3443" data4.Y "1"

module CopyAndUpdateOfAnonRecordStruct = 
    let data = struct {| X = 1 |}
    let data2 = struct {| data with Y = "1" |}
    let data3 = struct {| data with X = "3" |}
    let data4 = struct {| data2 with X = "3" |}
    check "fewjkvwno311" data.X 1
    check "fewjkvwno322" data2.X 1
    check "fewjkvwno333" data2.Y "1"
    check "fewjkvwno344" data3.X "3"
    check "fewjkvwno3444" data4.X "3"
    check "fewjkvwno3442" data4.Y "1"

module CopyAndUpdateOfAnonRecordFromRecord = 
    type Base = { X : int }
    let data = { X = 1 }
    let data2 = {| data with Y = "1" |}
    let data3 = {| data with X = "3" |}
    let data4 = {| data2 with X = "3" |}
    check "fewjkvwno315" data.X 1
    check "fewjkvwno326" data2.X 1
    check "fewjkvwno337" data2.Y "1"
    check "fewjkvwno348" data3.X "3"
    check "fewjkvwno344y" data4.X "3"
    check "fewjkvwno344b" data4.Y "1"

module CopyAndUpdateOfAnonRecordFromStructRecord = 
    [<Struct>]
    type Base = { X : int }
    let data = { X = 1 }
    let data2 = {| data with Y = "1" |}
    let data3 = {| data with X = "3" |}
    check "fewjkvwno31q" data.X 1
    check "fewjkvwno32w" data2.X 1
    check "fewjkvwno33ej" data2.Y "1"
    check "fewjkvwno34rs" data3.X "3"

module QuotesNewRecord = 

    open FSharp.Quotations
    open FSharp.Quotations.Patterns
    let ty, args = match <@ {| X = 1; Y = "two" |} @> with NewRecord(a,b) -> a,b

    check "gceoijew90ewcw1"  (FSharp.Reflection.FSharpType.IsRecord(ty)) true
    check "gceoijew90ewcw2"  (FSharp.Reflection.FSharpType.GetRecordFields(ty).Length) 2
    check "gceoijew90ewcw2"  ([ for p in FSharp.Reflection.FSharpType.GetRecordFields(ty) -> p.Name ]) [ "X"; "Y" ]
    check "gceoijew90ewcw3"  args [ <@@ 1 @@>; <@@ "two" @@> ] 

module QuotesNewRecord2 = 

    open FSharp.Quotations
    open FSharp.Quotations.Patterns
    let yarg,ty, args = match <@ {| Y = "two"; X = 1 |} @> with Let(_,yarg,NewRecord(a,b)) -> yarg,a,b

    check "qgceoijew90ewcw1"  (FSharp.Reflection.FSharpType.IsRecord(ty)) true
    check "qgceoijew90ewcw2"  (FSharp.Reflection.FSharpType.GetRecordFields(ty).Length) 2
    // Fields are sorted
    check "qgceoijew90ewcw2"  ([ for p in FSharp.Reflection.FSharpType.GetRecordFields(ty) -> p.Name ]) [ "X"; "Y" ]
    check "qgceoijew90ewcw3"  args.[0] <@@ 1 @@> 
    check "qgceoijew90ewcw4"  yarg <@@ "two" @@> 

module QuotesFieldInitOrder = 

    let mutable x = 1
    let test() = 
        x <- 1
        {| X = (check "clwknckl1" x 1; x <- x + 1; 3)
           Y = (check "cwkencelwe2" x 2; x <- x + 1; 2) 
        |} |> check "ceweoiwe1" {| Y=2; X=3 |}
        x <- 1
        {| X = (check "clwknckl3" x 1; x <- x + 1; 2)
           W = (check "cwkencelwe4" x 2; x <- x + 1; 3) 
        |} |> check "ceweoiwe2" {| W=3; X=2 |}
        x <- 1
        {| X = (check "clwknckl5" x 1; x <- x + 1; 2)
           Y = (check "clwknckl6" x 2; x <- x + 1; 3)
           W = (check "cwkencelwe7" x 3; x <- x + 1; 4) |} 
          |> check "ceweoiwe" {| Y=3; X=2; W=4 |}
        x <- 1
        let a = 
            {| Y = (check "clwknckl8" x 1; x <- x + 1; 2)
               X = (check "clwknckl9" x 2; x <- x + 1; 3)
               W = (check "cwkencel10" x 3; x <- x + 1; 4) 
            |} 
        a |> check "ceweoiwe" {| Y=2; X=3; W=4 |}
        x <- 1
        let b = 
            {| a with 
                 X = (check "clwknckl9" x 1; x <- x + 1; 6)
                 W = (check "cwkencel10" x 2; x <- x + 1; 7) 
            |} 
        b |> check "ceweoiwe87" {| Y=2; X=6; W=7 |}
        x <- 1
        let c = 
            {| a with 
                 X = (check "clwknckl9" x 1; x <- x + 1; 6)
                 A = (check "cwkencel11" x 2; x <- x + 1; 8) 
                 W = (check "cwkencel10" x 3; x <- x + 1; 7) 
            |} 
        c |> check "ceweoiwe87" {| Y=2; X=6; W=7; A=8 |}
    test()


module QuotesPropertyGet = 

    open FSharp.Quotations
    open FSharp.Quotations.Patterns
    let obj, prop = match <@ {| X = 1; Y = "two" |}.X @> with Patterns.PropertyGet(a,b,[]) -> a,b

    check "wgceoijew90ewcw1"  prop.Name "X"


module SampleAPI = 

    let SampleFunction (arg : {| A: int; B: string |}) = arg.A + arg.B.Length
    let SampleFunctionAcceptingList (args : {| A: int; B: string |} list) = args |> List.map (fun arg -> arg.A + arg.B.Length)
    let SampleFunctionReturningAnonRecd () =  {| A=1; B = "abc" |}

module SampleAPIStruct = 
    let SampleFunction (arg : (struct {| A: int; B: string |})) = arg.A + arg.B.Length
    let SampleFunctionAcceptingList (args : (struct {| A: int; B: string |}) list) = args |> List.map (fun arg -> arg.A + arg.B.Length)
    let SampleFunctionReturningAnonRecd () =  struct {| A=1; B = "abc" |}

module SampleAPITupleStruct = 
    let SampleFunction ((a,b) : (struct (int * string))) = a + b.Length
    let SampleFunctionAcceptingList (args : (struct (int * string)) list) = args |> List.map (fun (struct (a,b)) -> a + b.Length)
    let SampleFunctionReturningStructTuple () =  struct (1, "abc")

module SyntaxCornerCaseTests = 

    let _ = id<{| X: int |}> {| X = 3 |}
    // Check use as type argument
    let _ = id<{| X: int |}> {| X = 3 |}
    let _ = id<{| X: int; Y: int |}> {| X = 3; Y = 4 |}
    let _ = id<{| X: int; Y: int |}> ({| X = 3; Y = 4 |})
    let _ = id<struct {| X: int; Y: int |}> (struct {| X = 3; Y = 44 |})
    let _ = id<struct {| X: int; Y: int |}> (struct {| X = 3; Y = 4 |})