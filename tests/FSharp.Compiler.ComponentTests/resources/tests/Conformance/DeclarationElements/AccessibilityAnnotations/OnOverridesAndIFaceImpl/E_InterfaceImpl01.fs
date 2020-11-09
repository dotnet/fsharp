// #Regression #Conformance #DeclarationElements #Accessibility #Overloading 
// Regression test for FSHARP1.0:4485
// Visibility decl on interface implementation
//<Expects id="FS0941" span="(19,38-19,40)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(24,39-24,41)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>
//<Expects id="FS0941" span="(29,40-29,42)" status="error">Accessibility modifiers are not permitted on overrides or interface implementations</Expects>

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

