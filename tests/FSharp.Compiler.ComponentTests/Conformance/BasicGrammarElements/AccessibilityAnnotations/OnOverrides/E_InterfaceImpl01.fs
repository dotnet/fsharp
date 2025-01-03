// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on interface implementation




type IFoo = interface
    abstract M1 : int -> unit
end

type Foo1() = class
               interface IFoo with
                  member this.M1(x) = ()
             end
             
type Foo2() = class
               interface IFoo with
                  member public this.M1(x) = ()         // err
             end

type Foo3() = class
               interface IFoo with
                  member private this.M1(x) = ()        // err
             end

type Foo4() = class
               interface IFoo with
                  member internal this.M1(x) = ()       // err
             end

