// verify we properly import and resolve C# extension methods with generic, array, tuple, and FSharpFunc "this" parameters

let ob = obj()
let dateTime1 = System.DateTime.Now // make sure we try a struct
let intList1 = [10]
let intList2 = [20]
let stringList1 = ["ten"]
let arr1D = [| 0 .. 10 |]
let arr2D = array2D [ [| 0 .. 10 |]; [| 0 .. 10 |] ]
let tup2 = (10,20)
let tup3 = (10,20,30)
let tup4 = (10,20,30,40)
let tup5 = (10,20,30,40,50)
let tup6 = (10,20,30,40,50,60)
let tup7 = (10,20,30,40,50,60,70)
let tup8 = (10,20,30,40,50,60,70,80)
let tup9 = (10,20,30,40,50,60,70,80,90)
let func1 = (fun x -> x + 1)
let func2 = (fun (x : string) -> x.Length)
let gclist1 = System.Collections.Generic.List<int>( [ 0 .. 10 ])

module UseCSharpExtensionsMethodsWithVariableThisTy = 

    open System
    open System.Collections.Generic
    open CSExtensionMethods

    let base0 = gclist1.ExtendCollListIgnore(3)
    let base0b = gclist1.ExtendCollListIgnore<int,int>(3)

    let w0 = ob.ExtendGenericIgnore(3)
    let w0b = ob.ExtendGenericIgnore<obj,int>(3)
    let w0c : int = ob.ExtendGenericIgnoreBoth<obj, string>("test")

    let w1 = ob.ExtendGenericUnconstrainedNoArg()
    let w2 = intList1.ExtendGenericUnconstrainedNoArg()
    let w3 = dateTime1.ExtendGenericUnconstrainedNoArg()
    let w4 = intList1.ExtendFSharpListNoArg()
    let w5 = intList1.ExtendFSharpIntListNoArg()
    let w7 : (int -> int) = func1.ExtendGenericUnconstrainedNoArg()
    let w8 : (int * int) = tup2.ExtendGenericUnconstrainedNoArg()
    let w9 : int = tup2.ExtendTupleItem1()
    let w10 : int = tup2.ExtendTupleItem2()

    let x1 : obj = ob.ExtendGenericUnconstrainedOneArg("3")
    let x2 : int list = intList1.ExtendGenericUnconstrainedOneArg(intList2)
    let x3 : DateTime = dateTime1.ExtendGenericUnconstrainedOneArg(dateTime1)
    let x4 : (int -> int) = func1.ExtendGenericUnconstrainedOneArg(func1)
    let x5 : (int * int) = tup2.ExtendGenericUnconstrainedOneArg(tup2)
    let x6 : int = tup3.GItem1()
    let x7 : int = tup4.GItem1()
    let x8 : int = tup5.GItem1()
    let x9 : int = tup6.GItem1()
    let x10 : int = tup7.GItem1()
    let x11 : int = tup8.GItem1()
    let x12 : int = tup9.GItem1()

    let y2 : int list = intList1.ExtendGenericConstrainedNoArg()
    let y3 : int list = intList1.ExtendGenericConstrainedOneArg(intList2)
    let y4 : DateTime = dateTime1.ExtendGenericConstrainedNoArg()

    let z2 : int list = intList1.ExtendGenericConstrainedTightNoArg()
    let z3 : int list = intList1.ExtendGenericConstrainedTightOneArg(intList2)
    let z4 : DateTime = dateTime1.ExtendGenericConstrainedTightNoArg()

    let a1 : int[] = arr1D.ExtendArrayNoArg()
    let a2 : int[] = arr1D.ExtendArrayOneArg(intList1)
    let a3 : int[,] = arr2D.ExtendArray2DNoArg()
    let a4 : int[,] = arr2D.ExtendArray2DOneArg(intList1)
    let a5 : int = func1.ExtendFSharpFuncIntIntNoArg()    
    let a6 : (int -> int) = func1.ExtendFSharpFuncGenericNoArg()
    let a7 : (string -> int) = func2.ExtendFSharpFuncGenericNoArg()
    let a8 : int = func1.ExtendFSharpFuncGenericOneArg(5)
    let a9 : int = func2.ExtendFSharpFuncGenericOneArg("test")

module UseFSharpDefinedILExtensionsMethods = 

    open System
    open System.Runtime.CompilerServices
    [<assembly: Extension>]
    do()

    [<Extension>]
    type CSharpStyleExtensionMethodsInFSharp () = 
        [<Extension>] static member ExtendGenericUnconstrainedNoArg(s1: 'T) = s1
        [<Extension>] static member ExtendGenericUnconstrainedOneArg(s1: 'T, s2: 'T) = s1
        [<Extension>] static member GItem1((a,b)) = a
        [<Extension>] static member GItem1((a,b,c)) = a
        [<Extension>] static member GItem1((a,b,c,d)) = a
        [<Extension>] static member GItem1((a,b,c,d,e)) = a
        [<Extension>] static member GItem1((a,b,c,d,e,f)) = a
        [<Extension>] static member GItem1((a,b,c,d,e,f,g)) = a
        [<Extension>] static member GItem1((a,b,c,d,e,f,g,h)) = a
        [<Extension>] static member GItem1((a,b,c,d,e,f,g,h,i)) = a
        [<Extension>] static member ExtendGenericConstrainedNoArg(s1: 'T when 'T :> System.IComparable) = s1
        [<Extension>] static member ExtendGenericConstrainedOneArg(s1 : 'T, s2 : 'T when 'T :> System.IComparable) = s1
        [<Extension>] static member ExtendGenericConstrainedTightNoArg(s1 : 'T when  'T :> System.IComparable) = s1
        [<Extension>] static member ExtendGenericConstrainedTightOneArg(s1 : 'T, s2: 'T when  'T :> System.IComparable) = s1
        [<Extension>] static member ExtendFSharpListNoArg(s1: list<'T>) = s1
        [<Extension>] static member ExtendFSharpListOneArg(s1: list<'T>, s2: list<'T>) = s1
        [<Extension>] static member ExtendFSharpIntListNoArg(s1 : list<int>) = s1
        [<Extension>] static member ExtendFSharpIntListOneArg(s1: list<int>, s2: list<int>) = s1

        [<Extension>] static member ExtendArrayNoArg(s1: 'T[]) = s1
        [<Extension>] static member ExtendArrayOneArg(s1: 'T[], s2: list<'T>) = s1
        [<Extension>] static member ExtendIntArrayNoArg(s1: int[]) = s1
        [<Extension>] static member ExtendIntArrayOneArg(s1: int[], s2: int[]) = s1

        [<Extension>] static member ExtendArray2DNoArg(s1: 'T[,] ) = s1
        [<Extension>] static member ExtendArray2DOneArg(s1: 'T[,] , s2: list<'T>) = s1
        [<Extension>] static member ExtendIntArray2DNoArg(s1: int[,]) = s1
        [<Extension>] static member ExtendIntArray2DOneArg(s1: int[,], s2: int[,]) = s1

        [<Extension>] static member ExtendFSharpFuncIntIntNoArg(s1: (int -> int)) = s1(3)
        [<Extension>] static member ExtendFSharpFuncGenericNoArg(s1: ('T -> 'U)) = s1
        [<Extension>] static member ExtendFSharpFuncGenericOneArg(s1: ('T -> 'U), arg) = s1(arg)

        [<Extension>] static member ExtendGenericIgnore<'T, 'U>(s1 : 'T, s2: 'U) = s2
        [<Extension>] static member ExtendGenericIgnoreBoth<'T, 'U>(s1 : 'T, s2: 'U) = 3
        [<Extension>] static member ExtendCollListIgnore<'T, 'U>(s1: ResizeArray<'T>, s2: 'U) = s2

    let ob = obj()
    let s1 = "three"
    let w1 = ob.ExtendGenericUnconstrainedNoArg()
    let w2 = s1.ExtendGenericUnconstrainedNoArg()
    let w3 = ob.ExtendGenericUnconstrainedOneArg(ob)
    let w4 = s1.ExtendGenericUnconstrainedOneArg(s1)
    let w5 = tup2.GItem1()
    let w6 = tup3.GItem1()
    let w7 = tup4.GItem1()
    let w8 = tup5.GItem1()
    let w9 = tup6.GItem1()
    let w10 = tup7.GItem1()
    let w11 = tup8.GItem1()

    let base0 = gclist1.ExtendCollListIgnore(3)
    let base0b = gclist1.ExtendCollListIgnore<int,int>(3)

    let w0 = ob.ExtendGenericIgnore(3)
    let w0b = ob.ExtendGenericIgnore<obj,int>(3)
    let w0c : int = ob.ExtendGenericIgnoreBoth<obj, string>("test")

    let qw1 = ob.ExtendGenericUnconstrainedNoArg()
    let qw2 = intList1.ExtendGenericUnconstrainedNoArg()
    let qw3 = dateTime1.ExtendGenericUnconstrainedNoArg()
    let qw4 = intList1.ExtendFSharpListNoArg()
    let qw5 = intList1.ExtendFSharpIntListNoArg()
    let qw7 : (int -> int) = func1.ExtendGenericUnconstrainedNoArg()
    let qw8 : (int * int) = tup2.ExtendGenericUnconstrainedNoArg()

    let x1 : obj = ob.ExtendGenericUnconstrainedOneArg("3")
    let x2 : int list = intList1.ExtendGenericUnconstrainedOneArg(intList2)
    let x3 : DateTime = dateTime1.ExtendGenericUnconstrainedOneArg(dateTime1)
    let x4 : (int -> int) = func1.ExtendGenericUnconstrainedOneArg(func1)
    let x5 : (int * int) = tup2.ExtendGenericUnconstrainedOneArg(tup2)
    let x6 : int = tup3.GItem1()
    let x7 : int = tup4.GItem1()
    let x8 : int = tup5.GItem1()
    let x9 : int = tup6.GItem1()
    let x10 : int = tup7.GItem1()
    let x11 : int = tup8.GItem1()
    let x12 : int = tup9.GItem1()

    let y2 : int list = intList1.ExtendGenericConstrainedNoArg()
    let y3 : int list = intList1.ExtendGenericConstrainedOneArg(intList2)
    let y4 : DateTime = dateTime1.ExtendGenericConstrainedNoArg()

    let z2 : int list = intList1.ExtendGenericConstrainedTightNoArg()
    let z3 : int list = intList1.ExtendGenericConstrainedTightOneArg(intList2)
    let z4 : DateTime = dateTime1.ExtendGenericConstrainedTightNoArg()

    let a1 : int[] = arr1D.ExtendArrayNoArg()
    let a2 : int[] = arr1D.ExtendArrayOneArg(intList1)
    let a3 : int[,] = arr2D.ExtendArray2DNoArg()
    let a4 : int[,] = arr2D.ExtendArray2DOneArg(intList1)
    let a5 : int = func1.ExtendFSharpFuncIntIntNoArg()
    let a6 : (int -> int) = func1.ExtendFSharpFuncGenericNoArg()
    let a7 : (string -> int) = func2.ExtendFSharpFuncGenericNoArg()
    let a8 : int = func1.ExtendFSharpFuncGenericOneArg(5)
    let a9 : int = func2.ExtendFSharpFuncGenericOneArg("test")