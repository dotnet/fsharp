
let mutable failures = []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures <- failures @ [s]

let test s b =
    stderr.Write(s:string)
    if b then stderr.WriteLine " OK" else report_failure s
    stderr.WriteLine "" 

let check s v1 v2 = 
   stderr.Write(s:string);  
   if (v1 = v2) then 
       stderr.WriteLine " OK" 
       stderr.WriteLine "" 
   else
       eprintf " FAILED: got %A, expected %A" v1 v2 
       stderr.WriteLine "" 
       report_failure s


type T() =
    member this.H<[<Measure>]'u> (x : int<'u>) = x

    member this.F<[<Measure>]'u> (x : int<'u>) =
        let g x =
            this.H x

        g x

[<Measure>] type M

type TheType = { A : int; B : int; D : int; G : seq<int> } with
    static member Create a b _ d _ _ g = { A = a; B = b; D = d; G = g }
    
let CreateBadImageFormatException () =
    let create a b c d (e:int<_>) (f:int) g = TheType.Create a b (int c) d e f g
    seq { yield create 0 0 0 0 0 0 [0] }


// Regression test for https://github.com/Microsoft/visualfsharp/issues/30
// (Compilation error: "Incorrect number of type arguments to local call"

type R<[<Measure>] 'u> (f : float<'u>) =
    member r.Member = f

let get (r : R<_>) = r.Member
let foo =
    let problem _ = List.map get
    problem // Error: Incorrect number of type arguments to local call


// This was causing bad codegen in debug code
module InnerFunctionGenericOnlyByMeasure =
    let nullReferenceError weightedList =
        let rec loop accumululatedWeight (remaining : float<'u> list) =
            match remaining with
            | [] -> accumululatedWeight
            | weight :: tail ->
                loop (accumululatedWeight + weight) tail

        loop 0.0<_> weightedList

    let ``is this null?`` = nullReferenceError [ 0.3; 0.3; 0.4 ]

// Another variation on the above test case, where the recursive thing is a type function generic unly by a unit of measure
module TopLevelTypeFunctionGenericOnlyByMeasure =
    type C< [<Measure>] 'u> =
       abstract Invoke: float<'u> list -> int

    let rec loop<[<Measure>]'u>  =
        { new C<'u> with 
            member _.Invoke(xs) =
                match xs with
                | [] -> 1
                | weight :: tail ->
                    loop<'u>.Invoke(tail) }

    let nullReferenceError2 (weightedList: float<'u> list) =
        loop<'u>.Invoke(weightedList)

    let ``is this null 2?`` = nullReferenceError2 [ 0.3; 0.3; 0.4 ]

module TestLibrary =

    [<Measure>] 
    type unit =
        //converts an integer into a  unit of measureof type int<seconds> 
        static member convertLP x = LanguagePrimitives.Int32WithMeasure x  
        static member convert x = x * 1<unit> 
        static member convertFromInt (x: int) = x * 1<unit>  //not generic 
 

    //add a unit of measure to a number and convert it back again
    let test1 num =  
        let output = unit.convertLP(num)
        int output
    let test2 num =  
        let output = unit.convert(num)
        int output


    //convert the number in a sub function
    let test3 num = 
        let convert i =         
            unit.convertLP(i)
        let output = convert num //BadImageFormatException is thrown here
        int output               //type of output inferred as int ?!

    let test4 num = 
        let convert (i : int) =  //type of i is specified
            unit.convertLP(i)
        let output = convert num //BadImageFormatException is thrown here
        int output               //type of output inferred as int ?!

    let test5 num =  //type of num is inferred as int<u'> but no compile errors are reported 
        let convert i =          
            unit.convert(i)
        let output = convert num //BadImageFormatException is thrown here
        int output 

    let test6 (num : int) =  
        let convert i =          //inference looks incorrect
            unit.convert(i)   
        let output = convert num //BadImageFormatException is thrown here
        int output 


    //two work arounds to the problem
    let test7 num =  
        let convert (i : int) =  //with the type specified here, this doesn't crash
            unit.convert(i)
        let output = convert num 
        int output 

    let test8 num =  
        let convert i  =  
            unit.convertFromInt(i)  //with the type specified in the converter no exception
        let output = convert num  
        int output


    printfn "test 1: %i" (test1 1000)
    printfn "test 2: %i" (test2 1000)
    printfn "test 3: %i" (test3 1000)
    printfn "test 4: %i" (test4 1000)
    printfn "test 5: %i" (test5 1000)
    printfn "test 6: %i" (test6 1000)
    printfn "test 7: %i" (test7 1000)
    printfn "test 8: %i" (test8 1000)

module InterfacesOfMeasureAnnotatedTypes =
    open System
    type IDerivedComparable<'T> =
        inherit IComparable<'T>

    type IRandomOtherInterface<'T> =
        abstract M: 'T -> 'T

    type IDerivedEquatable<'T> =
        inherit IEquatable<'T>

    type Prim() =
        interface IComparable with 
            member x.CompareTo(y) = 0
        interface IDerivedComparable<Prim> with 
            member x.CompareTo(y) = 0
        interface IDerivedEquatable<Prim> with 
            member x.Equals(y) = true
        interface IRandomOtherInterface<Prim> with 
            member x.M(y) = y
        override x.Equals(y) = true
        override x.GetHashCode() = 0

    [<MeasureAnnotatedAbbreviation>]
    type Prim<[<Measure>] 'm> = Prim

    // Check that Prim<'m> supports the unit-annotated IComparable interface
    let f1 (x: Prim<'m>) = (x :> IComparable<Prim<'m>>)
    let f3 (x: Prim<'m>) = (x :> IEquatable<Prim<'m>>)
    let f5 (x: Prim<'m>) = (x :> IComparable)
    // Does not apply to other interfaces
    let f6 (x: Prim<'m>) = (x :> IRandomOtherInterface<Prim>)

module CheckModuleNames =
    type WithMeasure<[<Measure>] 'u> =
        | WithMeasure of float32<'u>

    module WithMeasure = ()
    type A = class end
    
    test "celjcelwj" (typeof<A>.DeclaringType.GetNestedType("WithMeasureModule") <> null)

[<EntryPoint>]
let main argv = 
    // test1
    let _ = T().F (LanguagePrimitives.Int32WithMeasure<M> 0)
    // test2
    for _ in CreateBadImageFormatException () do ()

    System.IO.File.WriteAllText("test.ok","ok"); 

    match failures with 
    | [] -> 
        stdout.WriteLine "Test Passed"
    | _ -> 
        stdout.WriteLine "Test Failed"
    0