module Neg04

type myRecord1 = { field1: int; field2: string }

type myRecord1 with abstract member AbstractMemberNotAllowedInAugmentation : string -> string end

type myRecord1 with val x : string end


type myRecord2 = { field1: int; field2: string }
  with abstract member AbstractMemberNotAllowedInAugmentation : string -> string end

type myRecord3 = { field1: int; field2: string }
  with val x : string end

type myRecord4 = { field1: int; field2: string }
  with override x.ToString() = x.field2 end

type myRecord5 = 
    { field1: int; field2: string }
    with 
       let x = 1
    end

type System.Int32 with 
       let x = 1
    end


// random test of what was once a poor error message
open System;;
Double.Nan;;

//// random test of what was once a poor error message
// This test is now under the FSHARPQA suite.
//type GrowingArray<'a> = System.Collections.Generic.List<'a>
//let nextPrime (sofar : GrowingArray<int>) n = failwith "nyi"
//let primes = Seq.unfold
//                 (fun (soFar,n) -> 
//                    let next = nextPrime soFar n in 
//                    Some(next, (soFar.Add(n), next+1)))
 
 

let f2 x = 
  let approxs = Seq.unfold (fun (i,y,z,sum) -> Some (sum + (y / z),(i + 1N,x * y,(i + 1N) * z))) (0N,1N,1N) in  
  approxs |> Seq.take 100 |> List.fold      (+) 0N;;


type ClassType1 =
  class
     inherit System.Object 
     new(s: string) = { inherit System.Object() }
  end

type ClassType2 =
  class
     inherit ClassType1 
     val rf : int
     // Check that a wrong type for 'inherits' gets reported in a decent location
     new(s) = { inherit System.Object(); rf = 1 }
  end

let Walk (x: obj) = ()


do Walk ["a";"b";"b"]

module ConstraintExample1 = begin
    type c<'a> when 'a :> c<string> = A | B
end

module ConstraintExample2 = begin

  type d = A | B
  type c<'a> when 'a :> d = A | B
end

module BracketNotation1 = begin

  let stringArrayDoesntSupport2DLookup (x : string) = x.[0,0]

  let unconstrainedTypeGivesError x = x.[0,0]

  let intArrayDoesntSupportLookup (x : int) = x.[0]

  let resolutionOfStringOperatorAfterTheFactGivesOcamlCompatWarning x = x.[0]
  let c : char =  resolutionOfStringOperatorAfterTheFactGivesOcamlCompatWarning "1"
  let _ =  resolutionOfStringOperatorAfterTheFactGivesOcamlCompatWarning "1"

  let resolutionOfArrayOperatorAfterTheFactGivesOcamlCompatWarning x = x.[0]
  let _ : int = resolutionOfArrayOperatorAfterTheFactGivesOcamlCompatWarning [| 0 |]


end


module InterfaceCastTests = begin
    type IBar = 
        interface
        end

    type IFoo = 
        interface
        end

    type Struct = 
        struct
           val x : int
        end
        
    type R = 
        { c : int }
        
    type U = 
        A | B
        
        
    let staticallyIllegalPatternTestInterfaceToSealedRecord(l:IBar list) =
        match l with
        | [:? R] -> None
        | _ -> None
        
    let staticallyIllegalPatternTestInterfaceToSealedUnion(l:IBar  list) =
        match l with
        | [:? U] -> None
        | _ -> None
        
    let staticallyIllegalPatternTestInterfaceToStruct(l:IBar list) =
        match l with
        | [:? Struct] -> None
        | _ -> None

    let staticallyIllegalCoercionInterfaceToSealedRecord(l:IBar ) =
        (l :? R)
        
    let staticallyIllegalCoercionInterfaceToSealedUnion(l:IBar) =
        (l :? U)
        
    let staticallyIllegalCoercionInterfaceToStruct(l:IBar) =
        (l :? Struct)
        
    let staticallyIllegalCoercionInterfaceToTuple(l:IBar) =
        (l :? (int * int))
        
    let staticallyIllegalCoercionInterfaceToArray(l:IBar) =
        (l :? int[])
        
    let staticallyIllegalCoercionInterfaceToFunction(l:IBar) =
        (l :? (int -> int))
        
end


module NegativeDelegateByrefCreationTests = begin
    type D = delegate of int byref -> int

    let byrefFun (b : int byref) = b
    let negateTest_createDelegateToFun = new D(byrefFun)


    let byrefFun2 (b1 : int) (b2 : int byref)  = b1 + b2
    let negateTest_createDelegateToFunPartial = new D(byrefFun2 3)
end

module AmbiguousImplicitInstantiationWarning = begin

    type IDuplex<'a> = 
      interface 
      end

    type IServer<'a> = 
      interface 
      end

    let Delay(v: 'a when 'a :> IDuplex<'a> and 'a :> IServer<'a>)  = ()

    do hash Delay   // An instantiation must be implicitly determined for Delay. But there is no single valid choice. Hence an error should be raised here.
end
