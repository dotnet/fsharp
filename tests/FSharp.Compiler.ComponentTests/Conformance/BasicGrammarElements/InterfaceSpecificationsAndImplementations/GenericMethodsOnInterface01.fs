// #Conformance #DeclarationElements #InterfacesAndImplementations 
#light

// FS1 997, Unable to implement interfaces with generic methods

type ITest = interface
    abstract Foo<'t> : 't -> 't list
    abstract Bar<'t, 'u> : 't -> 'u option
end


type Test() = class
    interface ITest with
        member x.Foo<'t> (v:'t) = [v; v; v]
        member x.Bar<'t, 'u> (y:'t) : 'u option = None
    end
end

// Instantiate type
let x = new Test()
// Need to cast as ITest in order to call Foo
let itx = x :> ITest

let result = itx.Foo(5)
if result <> [5; 5; 5] then failwith "Failed: 1"

let result2 : unit option = itx.Bar("string")
if result2 <> None then failwith "Failed: 2"

let result2b : int option = itx.Bar( () )
if result2b <> None then failwith "Failed: 3"
