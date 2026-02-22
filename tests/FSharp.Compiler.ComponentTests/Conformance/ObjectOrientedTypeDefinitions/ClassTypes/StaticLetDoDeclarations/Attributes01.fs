// #Conformance #ObjectOrientedTypes #Classes #LetBindings 
// Attributes
// As of Beta2, static let bindings may have attributes
//<Expects status="success"></Expects>
module M
[<Class>]
type A() = inherit System.Attribute()

type C() = class
             [<A>]                                      // some attribute (does not really matter)
             static let can_have_attributes = [1;2]     // OK!
             
             [<A>]
             let can_have_attributes2 = 1

             [<A>]
             let mutable can_have_attributes3 = 1
             
           end
