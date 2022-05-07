// #Regression #Conformance #DeclarationElements #InterfacesAndImplementations 
#light 

// FSB 1147, implementing interfaces with generic members gives internal error - undefined type variable

type IFoo = interface
    abstract Ignore<'b> : 'b -> unit
end

type Derived() = class
    interface IFoo with 
        member x.Ignore<'b> (y:'b) = ()
    end
end

let test = (new Derived()) :> IFoo

if test.Ignore<(int -> int)> (fun x -> x * x) <> () then failwith "Failed: 1"
if test.Ignore<int>           4               <> () then failwith "Failed: 2"
if test.Ignore<string>        ""              <> () then failwith "Failed: 3"

// Note the double () which is parens about value 'unit' to disambiguate a zero-arg function call
if test.Ignore<unit>          (())            <> () then failwith "Failed: 4"
