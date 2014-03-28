// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 


// Verify error when trying to implement the same generic interface twice.
// Regression for FSB 3574, PE Verification failure when implementing multiple generic interfaces (one generic, one specifc)
//<Expects status="error" id="FS0443" span="(12,6)">This type implements or inherits the same interface at different generic instantiations 'IA<int>' and 'IA<'b>'\. This is not permitted in this version of F#</Expects>

type IA<'b> = 
  interface
    abstract Foo : int -> int
  end
type IB<'b> = 
  interface
    inherit IA<'b>
    inherit IA<int>
  end
 
type A() =
  class
    interface IB<int> with
      member obj.Foo (x : int) = x
    end
  end
 
let x = A()

exit 1
