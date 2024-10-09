// #Conformance #ObjectConstructors 
#if TESTS_AS_APP
module Core_longnames
#endif
let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)

(* Some test expressions *)

(* Can we access an F# constructor via a long path *)
let v0 = Microsoft.FSharp.Core.Some("")
let v0b = Microsoft.FSharp.Core.Option.Some("")
let v0c = Microsoft.FSharp.Core.option.Some("")

(* Can we access an F# nullary constructor via a long path *)
let v1 = (Microsoft.FSharp.Core.None : int option)
let v1b = (Microsoft.FSharp.Core.Option.None : int option)
let v1c = (Microsoft.FSharp.Core.option.None : int option)

(* Can we access an F# type name via a long path *)
let v2 = (None : int Microsoft.FSharp.Core.Option)

(* Can we access an F# field name via a long path *)
let v3 = { Microsoft.FSharp.Core.contents = 1 }
let v3b = { Microsoft.FSharp.Core.Ref.contents = 1 }
let v3c = { Microsoft.FSharp.Core.ref.contents = 1 }
let v3d = { contents = 1 }
let v3e = { Ref.contents = 1 }
let v3f = { ref.contents = 1 }
let v3g = { Core.contents = 1 }
let v3h = { Core.Ref.contents = 1 }
let v3i = { Core.ref.contents = 1 }

(* Can we construct a "ref" value *)
let v4 = ref 1

(* Can we access an F# exception constructor via a long path *)
let v5 = (Microsoft.FSharp.Core.MatchFailureException("1",2,2) : exn)

(* Can we construct a "lazy" value *)
let v6 = lazy 1

(* Can we pattern match against a constructor specified via a long path *)
let v7 = 
  match v0 with 
  | Microsoft.FSharp.Core.Some(x) -> x
  | _ -> failwith ""
let v7b2 = 
  match v0 with 
  | option.Some(x) -> x
  | _ -> failwith ""
let v7b3 = 
  match v0 with 
  | Option.Some(x) -> x
  | _ -> failwith ""
let v7b = 
  match v0 with 
  | Microsoft.FSharp.Core.option.Some(x) -> x
  | _ -> failwith ""
let v7c = 
  match v0 with 
  | Microsoft.FSharp.Core.Option.Some(x) -> x
  | _ -> failwith ""


(* Can we pattern match against a nullary constructor specified via a long path *)
let v8 = 
  match v1 with 
  | Microsoft.FSharp.Core.None -> 1
  | _ -> failwith ""
let v8b = 
  match v1 with 
  | Microsoft.FSharp.Core.Option.None -> 1
  | _ -> failwith ""
let v8c = 
  match v1 with 
  | Microsoft.FSharp.Core.option.None -> 1
  | _ -> failwith ""

(* Can we pattern match against an exception constructor specified via a long path *)
let v9 = 
  match v5 with 
  | Microsoft.FSharp.Core.MatchFailureException _ -> 1
  | _ -> 2


(* Can we access an F# constructor via a long path *)
let v10 = Microsoft.FSharp.Core.Some(1)


(* Can we pattern match against a constructor specified via a long path *)
let v11 = 
  match v10 with 
  | Microsoft.FSharp.Core.Some(x) -> x
  | _ -> failwith ""

let v12 = 
  match v10 with 
  | Microsoft.FSharp.Core.Some(x) -> x
  | _ -> failwith ""

let v13 = Microsoft.FSharp.Core.Some(1)

#if !NETCOREAPP
(* check lid setting bug *)

open System.Diagnostics
let doubleLidSetter () =
  let p : Process = new Process() in
  p.StartInfo.set_FileName("child.exe"); // OK
  p.StartInfo.FileName <- "child.exe";   // was not OK, now fixed
  ()
#endif

module NameResolutionExample1Bug1218 = begin
    type S = 
        | A
        | B 
        with 
          static member C = "ONE"
        end

    type U = class 
        [<DefaultValue>]
        static val mutable private d : int
        static member D with get() = U.d and set v = U.d <- v
    end

    type s = 
        | S 
        | U
        with 
          member x.A = 1
          member x.C = 1
          member x.D = "1"
        end

    let _ = (S.A : S)  // the type constraint proves that this currently resolves to type S, constructor A
    let _ = (S.C : string)  // the type constraint proves that this currently resolves to type S, property C
    let _ = (U.D : int)  // the type constraint proves that this currently resolves to type S, static value D
end

module NameResolutionExample2Bug1218 = begin
    type S = 
        | A
        | B 

    type s = class 
        new () = { } 
        member x.A = 1
    end
    
    let S : s = new s()
    
    let _ = (S.A : int)  // the type constraint proves that this currently resolves to value S, member A, i.e. value lookups take precedence over types
    let _ = (fun (S : s) -> S.A : int)  // the type constraint proves that this currently resolves to value S, member A, i.e. value lookups take precedence over types
end

module LookupStaticFieldInType = begin

    type TypeInfoResult = 
       | Unknown = 0
       // .NET reference types
       | Null_CanArise_Allowed_NotTrueValue = 1
       // F# types with [<PermitNull>]
       | Null_CanArise_Allowed_TrueValue = 2
       // F# types
       | Null_CanArise_NotAllowed = 3
       // structs
       | Null_Never = 4
       

    type TypeInfo<'a>() = class
       [<DefaultValue>]
       static val mutable private info : TypeInfoResult
       static member TypeInfo 
                with get() = 
                 if TypeInfo<'a>.info = TypeInfoResult.Unknown then (
                     let nullness = 
                         let ty = typeof<'a> in
                         if ty.IsValueType 
                         then TypeInfoResult.Null_Never else
                         let mappingAttrs = ty.GetCustomAttributes(typeof<CompilationMappingAttribute>,false) in
                         if mappingAttrs.Length = 0 
                         then TypeInfoResult.Null_CanArise_Allowed_NotTrueValue
                         else                      
                             let reprAttrs = ty.GetCustomAttributes(typeof<CompilationRepresentationAttribute>,false) in
                             if reprAttrs.Length = 0 
                             then TypeInfoResult.Null_CanArise_NotAllowed 
                             else
                                 let reprAttr = reprAttrs.[0]  in
                                 let reprAttr = (failwith "" : CompilationRepresentationAttribute ) in
                                 if true 
                                 then TypeInfoResult.Null_CanArise_NotAllowed
                                 else TypeInfoResult.Null_CanArise_Allowed_TrueValue in
                     // The lookup on this line was failing to resolve
                     TypeInfo<'a>.info <- nullness
                 );
                 TypeInfo<'a>.info
    end
end


module TestsForUsingTypeNamesAsValuesWhenTheTypeHasAConstructor = begin
    // All the tests in this file relate to FSharp 1.0 bug 4379:	Testing fix	name resolution is weird when T constructor shadows method of same name
    module Test0 = begin
        let x = obj()

        let foo x = x + 1

        type foo() = class end

        let y = foo()  // still does not compile, and this is not shadowing!

        let x2 = ref 1
    end
    module Test1 =  begin
        let _ = Set<int> [3;4;5]
        let _ = Set [3;4;5]
    end
    module Test2 =  begin
        type Set() = class
            let x = 1
            static member Foo = 1
        end
            
        type Set<'T,'Tag>() =  class
            let x = 1
            static member Foo = 1
        end

        let _ = Set()
        //Set<>()
        let _ = Set<_> [3;4;5]
        let _ = Set<int,int>()
        let _ = Set<int,int>.Foo

    //let x : obj list = [ 1;2;3;4]
    end
    module Test3a =   begin
        let f()  = 
            float 1.0 |> ignore;
            decimal 1.0 |> ignore;
            float32 1.0 |> ignore;
            sbyte 1.0 |> ignore;
            byte 1.0 |> ignore;
            int16 1.0 |> ignore;
            uint16 1.0 |> ignore;
            int32 1.0 |> ignore;
            int64 1.0 |> ignore;
            uint32 1.0 |> ignore;
            uint64 1.0 |> ignore;
            string 1.0 |> ignore;
            unativeint 1.0 |> ignore;
            nativeint 1.0 |> ignore

    end
    module Test3b =  begin
        open Microsoft.FSharp.Core
        let f()  = 
            float 1.0 |> ignore;
            decimal 1.0 |> ignore;
            float32 1.0 |> ignore;
            sbyte 1.0 |> ignore;
            byte 1.0 |> ignore;
            int16 1.0 |> ignore;
            uint16 1.0 |> ignore;
            int32 1.0 |> ignore;
            int64 1.0 |> ignore;
            uint32 1.0 |> ignore;
            uint64 1.0 |> ignore;
            string 1.0 |> ignore;
            unativeint 1.0 |> ignore;
            nativeint 1.0 |> ignore;
    end 
    module Test3c =  begin
        open Microsoft.FSharp.Core.Operators
        open Microsoft.FSharp.Core
        let x3 = float 1.0
    end

    module Test3d =  begin
        open System
            // This is somewhat perversely using the type name 'decimal' as a constructor, but it is legal
        let x3 = Decimal 1.0
        let x4 = decimal 1.0
    end
    module Test3e =  begin
        let x3 = System.Decimal 1.0
        let x4 = decimal 1.0
    end
    module Test3f =  begin
            // This is somewhat perversely using the type name 'decimal' as a constructor, but it is legal
            Microsoft.FSharp.Core.decimal 1.0 |> ignore;
            // This is somewhat perversely using the type name 'string' as a constructor
            Microsoft.FSharp.Core.string ('3',100) |> ignore
    end
    module Test7 =  begin
        open System
        let x3 = Decimal 1.0
        let x4 = decimal 1.0
    end

    module TestValuesGetAddedAfterTypes =  begin

        module M = begin
            type string = System.String
      
            let string (x:int,y:int) = "1"
        end
        
        open M
        
        let x = string (1,1)

    end
    module TestValuesGetAddedAfterTypes2 =  begin

        module M = begin
            let string (x:int,y:int) = "1"

            type string = System.String
        end
        open M
        
        let x = string (1,1)

    end
    module TestAUtoOpenNestedModulesGetAddedAfterTypes2 =  begin

        module M = begin
            type string = System.String
            [<AutoOpen>]
            module M2 = begin
                let string (v:int,y:int) = 3
            end
        end
        open M
        
        let x = string (1,1)

    end
    module TestAUtoOpenNestedModulesGetAddedAfterVals =  begin

        module M = begin
            let string(x:string,y:string,z:string) = 23
            [<AutoOpen>]
            module M2 = begin
                let string (v:int,y:int) = 3
            end
        end
        open M
        
        // The module M2 gets auto-opened after the values for "M" get added , hence this typechecks OK
        let x = string (1,1)

    end
    module TestAUtoOpenNestedModulesGetAddedInOrder =  begin

        module M = begin
            let string(x:string,y:string,z:string) = 23
            [<AutoOpen>]
            module M2 = begin
                let string (v:int,y:int) = 3
            end
            [<AutoOpen>]
            module M3 =  begin
                let string (v:int,y:int,z:int) = 3
            end
        end
        open M
        
        // The auto-open modules get added in declaration order 
        let x = string (1,1,3)

    end 
    // AutoOpen modules get auto-opened in the order they appear in the referenced signature
    module TestAUtoOpenNestedModulesGetAddedInOrder_ReverseAlphabetical = begin

        module M = begin
            let string(x:string,y:string,z:string) = 23
            [<AutoOpen>]
            module M3 = begin
                let string (v:int,y:int) = 3
            end
            [<AutoOpen>]
            module M2 =  begin
                let string (v:int,y:int,z:int) = 3
            end
        end
        open M
        
        // The auto-open modules get added in declaration order 
        let x = string (1,1,3)
    end

end

module Ok1 = 

    module A =
        let create() = 1
        type Dummy = A | B


    type A() = 
        member x.P = 1

    test "lkneecec09iew1" (typeof<A.Dummy>.FullName.Contains("AModule") )


module Ok2 = 

    type A() = 
        member x.P = 1


    module A =
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew2" (typeof<A.Dummy>.FullName.Contains("AModule") )


module Ok3 = 

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module A = 
        let create() = 1
        type Dummy = A | B

    type A() = 
        member x.P = 1

    test "lkneecec09iew3" (typeof<A.Dummy>.FullName.Contains("AModule") )


module Ok4 = 

    type A() = 
        member x.P = 1

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module A = 
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew4" (typeof<A.Dummy>.FullName.Contains("AModule") )



module rec Ok5 = 

    module A =
        let create() = 1
        type Dummy = A | B


    type A() = 
        member x.P = 1

    test "lkneecec09iew5" (typeof<A.Dummy>.FullName.Contains("AModule") )


module rec Ok6 = 

    type A() = 
        member x.P = 1


    module A =
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew6" (typeof<A.Dummy>.FullName.Contains("AModule") )


module rec Ok7 = 

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module A = 
        let create() = 1
        type Dummy = A | B

    type A() = 
        member x.P = 1

    test "lkneecec09iew7" (typeof<A.Dummy>.FullName.Contains("AModule") )


module rec Ok8 = 

    type A() = 
        member x.P = 1

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module A = 
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew8" (typeof<A.Dummy>.FullName.Contains("AModule") )


module Ok9 = 

    type A() = 
        member x.P = 1

    type A<'T>() = 
        member x.P = 1

    module A = 
        let create() = 1
        type Dummy = A | B


    test "lkneecec09iew9" (typeof<A.Dummy>.FullName.Contains("AModule") )


module Ok9b = 

    type A<'T>() = 
        member x.P = 1

    module A = 
        let create() = 1
        type Dummy = A | B

    //A<'T> has a type parameter, so appending Module is not necessary.
    test "lkneecec09iew9" (not (typeof<A.Dummy>.FullName.Contains("AModule") ) )

module rec Ok10 = 

    type A() = 
        member x.P = 1

    type A<'T>() = 
        member x.P = 1

    module A = 
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew10" (typeof<A.Dummy>.FullName.Contains("AModule") )

module Ok11 = 

    type A = int

    module A = 
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew11" (typeof<A.Dummy>.FullName.Contains("AModule") )

module Ok12 = 

    type A = A

    module A = 
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew12" (typeof<A.Dummy>.FullName.Contains("AModule") )

module Ok13 = 

    type A = A of string

    module A = 
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew13" (typeof<A.Dummy>.FullName.Contains("AModule") )


module Ok14 = 

    module X = 
        type A = A of string

    type X.A with 
        member x.P = 1

    module A =  // the type definition is an augmentation so doesn't get the suffix
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew14" (not (typeof<A.Dummy>.FullName.Contains("AModule") )) 

module rec Ok15 = 

    open X
    
    module X = 
        type A = A of string

    type A with 
        member x.P = 1

    module A =  // the type definition is an augmentation so doesn't get the suffix
        let create() = 1
        type Dummy = A | B

    test "lkneecec09iew15" (not (typeof<A.Dummy>.FullName.Contains("AModule") )) 

module rec Ok16 =

    type A<'a> = A of 'a

    module A =
        type Dummy = A | B

    test "lkneecec09iew16" (not (typeof<A.Dummy>.FullName.Contains("AModule") ))

module rec Ok17 =

    type A<'a> = A of 'a
    type A = A of int

    module A =
        type Dummy = A | B

    test "lkneecec09iew17" (typeof<A.Dummy>.FullName.Contains("AModule") )

module rec Ok18 =

    type A<[<Measure>]'u> = A of int<'u>

    module A =
        type Dummy = A | B

    test "lkneecec09iew18" (typeof<A.Dummy>.FullName.Contains("AModule") )

module rec Ok19 =

    type A<[<Measure>]'u, 'a> = | A of int<'u> | B of 'a

    module A =
        type Dummy = A | B

    test "lkneecec09iew19" (not (typeof<A.Dummy>.FullName.Contains("AModule") ))

module rec Ok20 =

    type A<'a, [<Measure>]'u> = A of int<'u> | B of 'a

    module A =
        type Dummy = A | B

    test "lkneecec09iew20" (not (typeof<A.Dummy>.FullName.Contains("AModule") ))

module rec Ok21 =

    type A<'a, 'b> = A of 'a | B of 'b

    module A =
        type Dummy = A | B

    test "lkneecec09iew21" (not (typeof<A.Dummy>.FullName.Contains("AModule") ))

module rec Ok22 =

    module A =
        type Dummy = A | B

    type A<'a> = A of 'a

    test "lkneecec09iew22" (not (typeof<A.Dummy>.FullName.Contains("AModule") ))

module rec Ok23 =

    module A =
        type Dummy = A | B

    type A = A of int

    test "lkneecec09iew23" (typeof<A.Dummy>.FullName.Contains("AModule") )

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif
