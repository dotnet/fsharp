
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
    else (stderr.WriteLine (sprintf "fail, expected %A, got %A" x2 x1); report_failure (s))

let inline getX (x: ^TX) : ^X = 
        (^TX : (member get_X : unit -> ^X) (x))


let inline getY (x: ^TX) : ^X = 
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

    check "coijoiwcnkwle1"  {| a = 1 |} {| a = 1 |}
    check "coijoiwcnkwle2"  {| a = 2 |}  {| a = 2 |}

    check "coijoiwcnkwle3"  (sprintf "%A"  {| X = 10 |}) "{X = 10;}"
    check "coijoiwcnkwle4"  (sprintf "%A"  {| X = 10; Y = 1 |} |> fun s -> s.Replace("\n","").Replace("\r","")) ("{X = 10; Y = 1;}".Replace("\n","").Replace("\r",""))
    check "clekoiew09" (f2 {| X = {| X = 10 |} |}) 10
    check "cewkew0oijew" (f2 {| X = {| X = 20 |} |})  20

    // TODO: field reordering....
    //let test3b() = {| a = 1+1; b = 2 |} = {| b = 1; a = 2 |} 

    // Equality is possible
    check "ceijoewwekcj" {| a = 1-1 |} {| a = Unchecked.defaultof<_> |}

    // Comparison is possible if structural elements are comparable
    check "ceijowere9er" ({| a = 1+1 |} > {| a = 0 |}) true

#if NEGTEST
    // Check we get compile-time errors
    let negTypeTest1() = {| a = 1+1; b = 2 |} = {| a = 2 |} 
    let negTyoeTest2() = {| b = 2 |} = {| a = 2 |} 

    // Check we get parsing error and decent recovery
    let negParsingTest2() = {| b = 2 }

    // Comparison is not possible if structural elements are comparable
    let negTypeTest3() = {| a = id |} > {| a = id |}
#endif

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

#if TODO

    // Both kinds of objects support 'with' to add or rebind properties. 
    let data5 = {| r with X = 1 |}

    // Both kinds of objects support 'with -X' to remove properties.  Syntax is TBD.
    let data6 = {| r without X |}
#endif



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

    // Copy-and-update may not be used, since C# doesn't allow this on anonymous objects

    // Types _can_ be used outside their assembly, but can _not_ be named in the syntax of types, nor created



//let data1 = {| x = 1; y = 2 |}
//let data2 = { x = 1; y = 2 }

module SyntaxCornerCaseTests = 

    let _ = id<{| X: int |}> {| X = 3 |}
    // Check use as type argument
    let _ = id<{| X: int |}> {| X = 3 |}
    let _ = id<{| X: int; Y: int |}> {| X = 3; Y = 4 |}
    let _ = id<{| X: int; Y: int |}> ({| X = 3; Y = 4 |})
    let _ = id<struct {| X: int; Y: int |}> (struct {| X = 3; Y = 44 |})
    let _ = id<struct {| X: int; Y: int |}> (struct {| X = 3; Y = 4 |})