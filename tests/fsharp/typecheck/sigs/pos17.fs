module Pos17



open System
open System.Collections.Generic

module BasicTests = 
  let f1() = "a" |> System.Uri
  let f2() = "a" |> Uri
  let f3() = ["a"] |> Set
  let f4() = ["a", 4 ] |> Map

module CheckFunctionNameHasPriorityOverTypeName = 

  let Uri (x:int) =  4
  let f3() = 3 |> Uri

module TestResolutionOfNonGenericTypeNameAsOverloadedConstructor = 

  let f4() = ('a',3) |> String

module TestResolutionOfGenericTypeNameWithTypeArgumentsAsOverloadedConstructor = 
  let w1 = List<int>()
  let w2 = () |> List<int>
  let w3 = List<int>(3)
  let w4 = 3 |> List<int>
  let w5 = List<int>([1;3])
  let w6 = [1;3] |> List<int>
  let w7 = List<int>(seq { yield 1; yield 3 } )
  let w8 = seq { yield 1; yield 3 } |> List<int>

module TestResolutionOfGenericTypeNameAsOverloadedConstructorWithoutTypeAnnotation = 
  let x1 : List<int> = List()
  let x2 : List<int> = () |> List
  let x3 : List<int> = List(3)
  let x4 : List<int> = 3 |> List
  let x5 : List<int> = List([1;3])
  let x6 : List<int> = [1;3] |> List
  let x7 : List<int> = List(seq { yield 1; yield 3 } )
  let x8 : List<int> = seq { yield 1; yield 3 } |> List

module TestUseAsLambdaOfGenericTypeNameAsOverloadedConstructorWithoutTypeAnnotation = 
  let x1 : unit -> List<int> = List
  let x4 : int -> List<int> = List
  let x5 : seq<int> -> List<int> = List

module TestUseAsLambdaOfGenericTypeNameWithLongPathAsOverloadedConstructorWithoutTypeAnnotation = 
  let x1 : unit -> List<int> = System.Collections.Generic.List
  let x4 : int -> List<int> = System.Collections.Generic.List
  let x5 : seq<int> -> List<int> = System.Collections.Generic.List

module TestUseAsLambda2 = 
  let x1 : unit -> List<_> = List<int>
  let x4 : int -> List<_> = List<int>
  let x5 : seq<int> -> List<_> = List<int>






module TestResolutionOfAbbreviatedGenericTypeNameWithTypeArgumentsAsOverloadedConstructor = 
  let w1 = ResizeArray<int>()
  let w2 = () |> ResizeArray<int>
  let w3 = ResizeArray<int>(3)
  let w4 = 3 |> ResizeArray<int>
  let w5 = ResizeArray<int>([1;3])
  let w6 = [1;3] |> ResizeArray<int>
  let w7 = ResizeArray<int>(seq { yield 1; yield 3 } )
  let w8 = seq { yield 1; yield 3 } |> ResizeArray<int>

module TestResolutionOfAbbreviatedGenericTypeNameAsOverloadedConstructorWithoutTypeAnnotation = 
  let x1 : ResizeArray<int> = ResizeArray()
  let x2 : ResizeArray<int> = () |> ResizeArray
  let x3 : ResizeArray<int> = ResizeArray(3)
  let x4 : ResizeArray<int> = 3 |> ResizeArray
  let x5 : ResizeArray<int> = ResizeArray([1;3])
  let x6 : ResizeArray<int> = [1;3] |> ResizeArray
  let x7 : ResizeArray<int> = ResizeArray(seq { yield 1; yield 3 } )
  let x8 : ResizeArray<int> = seq { yield 1; yield 3 } |> ResizeArray

module TestUseOfAbbreviatedGenericTypeNameAsLambdaForOverloadedConstructor = 
  let x1 : unit -> ResizeArray<int> = ResizeArray
  let x4 : int -> ResizeArray<int> = ResizeArray
  let x5 : seq<int> -> ResizeArray<int> = ResizeArray

module TestUseOfAbbreviatedGenericTypeNameWithTypeArgumentsAsLambdaForOverloadedConstructor = 
  let x1 : unit -> ResizeArray<_> = ResizeArray<int>
  let x4 : int -> ResizeArray<_> = ResizeArray<int>
  let x5 : seq<int> -> ResizeArray<_> = ResizeArray<int>


type IntResizeArray = List<int>

module TestResolutionOfAbbreviatedAndInstantiatedGenericTypeNameWithTypeArgumentsAsOverloadedConstructor = 
  let w1 = IntResizeArray()
  let w2 = () |> IntResizeArray
  let w3 = IntResizeArray(3)
  let w4 = 3 |> IntResizeArray
  let w5 = IntResizeArray([1;3])
  let w6 = [1;3] |> IntResizeArray
  let w7 = IntResizeArray(seq { yield 1; yield 3 } )
  let w8 = seq { yield 1; yield 3 } |> IntResizeArray

module TestResolutionOfAbbreviatedAndInstantiatedGenericTypeNameAsOverloadedConstructorWithoutTypeAnnotation = 
  let x1 : IntResizeArray = IntResizeArray()
  let x2 : IntResizeArray = () |> IntResizeArray
  let x3 : IntResizeArray = IntResizeArray(3)
  let x4 : IntResizeArray = 3 |> IntResizeArray
  let x5 : IntResizeArray = IntResizeArray([1;3])
  let x6 : IntResizeArray = [1;3] |> IntResizeArray
  let x7 : IntResizeArray = IntResizeArray(seq { yield 1; yield 3 } )
  let x8 : IntResizeArray = seq { yield 1; yield 3 } |> IntResizeArray

module TestUseOfAbbreviatedAndInstantiatedGenericTypeNameAsLambdaForOverloadedConstructor = 
  let x1 : unit -> IntResizeArray = IntResizeArray
  let x4 : int -> IntResizeArray = IntResizeArray
  let x5 : seq<int> -> IntResizeArray = IntResizeArray

module TestUseOfAbbreviatedAndInstantiatedGenericTypeNameWithTypeArgumentsAsLambdaForOverloadedConstructor = 
  let x1 : unit -> IntResizeArray = IntResizeArray
  let x4 : int -> IntResizeArray = IntResizeArray
  let x5 : seq<int> -> IntResizeArray = IntResizeArray



module OverloadedTypeNamesBothWithConstructors = 
    type OverloadedClassName<'T>(x:int) = 
        new (y:string) = OverloadedClassName<'T>(1)
        member __.P = x
        static member S() = 3


    type OverloadedClassName<'T1,'T2>(x:int) = 
        new (y:string) = OverloadedClassName<'T1,'T2>(1)
        member __.P = x
        static member S() = 3

    let t1 = 3 |> OverloadedClassName<int>
    let t2 = 3 |> OverloadedClassName<int,int>
    //let t3 = 3 |> OverloadedClassName // expected error (see neg20.fs) - multiple types exist
    let t1s = "3" |> OverloadedClassName<int>
    let t2s = "3" |> OverloadedClassName<int,int>
    //let t3s = "3" |> OverloadedClassName // expected error (see neg20.fs) - multiple types exist


module OverloadedTypeNamesSomeConstructors = 
    type OverloadedClassName<'T>(x:int) = 
        new (y:string) = OverloadedClassName<'T>(1)
        member __.P = x
        static member S() = 3


    type OverloadedClassName<'T1,'T2> = 
        member __.P = 1
        static member S() = 3

    let t1 = 3 |> OverloadedClassName<int>
    //let t2 = 3 |> OverloadedClassName<int,int> //  expected error (see neg20.fs) -  "The value or constructor 'OverloadedClassName' is not defined"
    //let t3 = 3 |> OverloadedClassName // expected error (see neg20.fs) - - multiple types exist
    let t1s = "3" |> OverloadedClassName<int>
    //let t2s = "3" |> OverloadedClassName<int,int> //  expected error (see neg20.fs) - "The value or constructor 'OverloadedClassName' is not defined"
    //let t3s = "3" |> OverloadedClassName // expected error (see neg20.fs) - - multiple types exist

module OverloadedTypeNamesNoConstructors = 
    type OverloadedClassName<'T> = 
        static member S(x:int) = 3

    type OverloadedClassName<'T1,'T2> = 
        static member S(x:int) = 3

    let t1 = 3 |> OverloadedClassName<int>.S
    let t2 = 3 |> OverloadedClassName<int,int>.S
    //let t3 = 3 |> OverloadedClassName.S // expected error (see neg20.fs) - multiple types exist
    //let t3b = 3 |> OverloadedClassName<int>.S // expected error (see neg20.fs) - multiple types exist
    //let t3c = 3 |> OverloadedClassName<int,int>.S // expected error (see neg20.fs) - multiple types exist
    //let t4 = 3 |> OverloadedClassName.S2 // expected error (see neg20.fs) -  The field, constructor or member 'S2' is not defined




module OverloadedTypeNamesIncludingNonGenericTypeAllWithConstructors = 

    type OverloadedClassName(x:int) = 
        new (y:string) = OverloadedClassName(1)
        member __.P = x
        static member S() = 3

    type OverloadedClassName<'T>(x:int) = 
        new (y:string) = OverloadedClassName<'T>(1)
        member __.P = x
        static member S() = 3


    type OverloadedClassName<'T1,'T2>(x:int) = 
        new (y:string) = OverloadedClassName<'T1,'T2>(1)
        member __.P = x
        static member S() = 3

    let t1 = 3 |> OverloadedClassName<int>
    let t2 = 3 |> OverloadedClassName<int,int>
    let t3 = 3 |> OverloadedClassName
    let t1s = "3" |> OverloadedClassName<int>
    let t2s = "3" |> OverloadedClassName<int,int>
    let t3s = "3" |> OverloadedClassName


module OverloadedTypeNamesIncludingNonGenericTypeSomeConstructors = 
    type OverloadedClassName(x:int) = 
        new (y:string) = OverloadedClassName(1)
        member __.P = x
        static member S() = 3

    type OverloadedClassName<'T>(x:int) = 
        new (y:string) = OverloadedClassName<'T>(1)
        member __.P = x
        static member S() = 3


    type OverloadedClassName<'T1,'T2> = 
        member __.P = 1
        static member S() = 3

    let t1 = 3 |> OverloadedClassName<int>
    let t1s = "3" |> OverloadedClassName<int>

module OverloadedTypeNamesIncludingNonGenericTypeNoConstructors = 

    type OverloadedClassName = 
        static member S(x:int) = 3

    type OverloadedClassName<'T> = 
        static member S(x:int) = 3

    type OverloadedClassName<'T1,'T2> = 
        static member S(x:int) = 3

    let t1 = 3 |> OverloadedClassName<int>.S
    let t2 = 3 |> OverloadedClassName<int,int>.S
    let t3 = 3 |> OverloadedClassName.S 
    let t3b = 3 |> OverloadedClassName<int>.S 
    let t3c = 3 |> OverloadedClassName<int,int>.S 





