// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations #ReqNOMT #RequiresPowerPack 
// Regression test for FSHARP1.0:4687
// There is no specific reason to put this test under 'Conformance\ObjectOrientedTypeDefinitions\InterfaceTypes'
// It is a typical case of feature interaction (interfaces, etc...)
module M
open System
open System.Collections.Generic

[<CustomComparison>]
[<StructuralEquality>]
type myMAP<'k,'v,'c> when 'c :> IComparer<'k> (map: Tagged.Map<'k,'v,'c>) = 
    struct
      
      member __.map' = map

      static member compare (x: myMAP<_,_,_>) (y: myMAP<_,_,_>) =
        (x.map' :> IComparable).CompareTo y.map'

      interface IComparable with
        member x.CompareTo y = myMAP<_,_,_>.compare x (y :?> myMAP<_,_,_>)

    end
