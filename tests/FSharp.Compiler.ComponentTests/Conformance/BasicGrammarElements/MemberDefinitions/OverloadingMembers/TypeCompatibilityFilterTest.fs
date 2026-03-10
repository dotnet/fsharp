// #Conformance #DeclarationElements #MemberDefinitions #Overloading

open System
open System.Collections.Generic

type TypeCompatTest() =
    static member Process(x: int) = "int"
    static member Process(x: string) = "string"
    static member Process(x: float) = "float"
    static member Process(x: bool) = "bool"
    static member Process(x: byte) = "byte"
    
    static member Generic<'T>(x: 'T) = sprintf "generic-%s" (typeof<'T>.Name)
    
    static member WithInterface(x: IComparable) = "IComparable"
    static member WithInterface(x: IEnumerable<int>) = "IEnumerable<int>"
    
    static member WithObject(x: obj) = "obj"
    
    static member WithTuple(x: int * int) = "tuple2"
    static member WithTuple(x: int * int * int) = "tuple3"
    
    static member WithArray(x: int[]) = "array1d"
    static member WithArray(x: int[,]) = "array2d"
    
    static member Multi(x: int, y: int) = "int-int"
    static member Multi(x: string, y: string) = "string-string"
    static member Multi(x: int, y: string) = "int-string"
    static member Multi(x: string, y: int) = "string-int"
    
    static member WithNullable(x: Nullable<int>) = "nullable-int"
    static member WithNullable(x: Nullable<float>) = "nullable-float"
    
    static member NumericConversions(x: int64) = "int64"
    static member NumericConversions(x: nativeint) = "nativeint"

if TypeCompatTest.Process(42) <> "int" then failwith "Failed: Process int"
if TypeCompatTest.Process("hello") <> "string" then failwith "Failed: Process string"
if TypeCompatTest.Process(3.14) <> "float" then failwith "Failed: Process float"
if TypeCompatTest.Process(true) <> "bool" then failwith "Failed: Process bool"
if TypeCompatTest.Process(42uy) <> "byte" then failwith "Failed: Process byte"

if TypeCompatTest.Generic(42) <> "generic-Int32" then failwith "Failed: Generic int"
if TypeCompatTest.Generic("test") <> "generic-String" then failwith "Failed: Generic string"

if TypeCompatTest.WithInterface(42 :> IComparable) <> "IComparable" then failwith "Failed: WithInterface IComparable"
if TypeCompatTest.WithInterface([1; 2; 3] :> IEnumerable<int>) <> "IEnumerable<int>" then failwith "Failed: WithInterface IEnumerable"

if TypeCompatTest.WithObject(42) <> "obj" then failwith "Failed: WithObject int"
if TypeCompatTest.WithObject("test") <> "obj" then failwith "Failed: WithObject string"

if TypeCompatTest.WithTuple((1, 2)) <> "tuple2" then failwith "Failed: WithTuple 2"
if TypeCompatTest.WithTuple((1, 2, 3)) <> "tuple3" then failwith "Failed: WithTuple 3"

if TypeCompatTest.WithArray([| 1; 2; 3 |]) <> "array1d" then failwith "Failed: WithArray 1d"
if TypeCompatTest.WithArray(Array2D.init 2 2 (fun i j -> i + j)) <> "array2d" then failwith "Failed: WithArray 2d"

if TypeCompatTest.Multi(1, 2) <> "int-int" then failwith "Failed: Multi int-int"
if TypeCompatTest.Multi("a", "b") <> "string-string" then failwith "Failed: Multi string-string"
if TypeCompatTest.Multi(1, "b") <> "int-string" then failwith "Failed: Multi int-string"
if TypeCompatTest.Multi("a", 2) <> "string-int" then failwith "Failed: Multi string-int"

if TypeCompatTest.WithNullable(Nullable<int>(42)) <> "nullable-int" then failwith "Failed: WithNullable int"
if TypeCompatTest.WithNullable(Nullable<float>(3.14)) <> "nullable-float" then failwith "Failed: WithNullable float"

if TypeCompatTest.NumericConversions(42L) <> "int64" then failwith "Failed: NumericConversions int64"
if TypeCompatTest.NumericConversions(42n) <> "nativeint" then failwith "Failed: NumericConversions nativeint"

type ParamArrayTypeTest() =
    static member Process([<ParamArray>] args: int[]) = sprintf "ints-%d" args.Length
    static member Process([<ParamArray>] args: string[]) = sprintf "strings-%d" args.Length
    static member Process([<ParamArray>] args: obj[]) = sprintf "objs-%d" args.Length
    
    static member Mixed(prefix: string, [<ParamArray>] values: int[]) = sprintf "%s-%d" prefix values.Length
    static member Mixed(prefix: string, [<ParamArray>] values: string[]) = sprintf "%s-strs-%d" prefix values.Length

if ParamArrayTypeTest.Process(1, 2, 3) <> "ints-3" then failwith "Failed: ParamArray int"
if ParamArrayTypeTest.Process("a", "b") <> "strings-2" then failwith "Failed: ParamArray string"

if ParamArrayTypeTest.Mixed("test", 1, 2) <> "test-2" then failwith "Failed: Mixed ParamArray int"
if ParamArrayTypeTest.Mixed("test", "a", "b", "c") <> "test-strs-3" then failwith "Failed: Mixed ParamArray string"

type OptionalArgsTypeTest() =
    static member Complex(x: int, y: int, ?comparer: IComparable) =
        match comparer with
        | Some _ -> "with-comparer"
        | None -> "no-comparer"
    static member Complex(x: int, y: string, ?list: IEnumerable<int>) =
        match list with
        | Some _ -> "with-list"
        | None -> "no-list"

if OptionalArgsTypeTest.Complex(42, 10) <> "no-comparer" then failwith "Failed: Optional Complex int-int no-opt"
if OptionalArgsTypeTest.Complex(42, 10, comparer = (42 :> IComparable)) <> "with-comparer" then failwith "Failed: Optional Complex with-comparer"
if OptionalArgsTypeTest.Complex(42, "test") <> "no-list" then failwith "Failed: Optional Complex int-string no-opt"
if OptionalArgsTypeTest.Complex(42, "test", list = [1; 2; 3]) <> "with-list" then failwith "Failed: Optional Complex with-list"

printfn "All type compatibility filtering tests passed!"
