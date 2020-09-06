// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Verify error when trying to implement the same generic interface twice.
// Regression for FSB 3574, PE Verification failure when implementing multiple generic interfaces (one generic, one specifc)
//<Expects status="error" id="FS3350" span="(10,6-10,8)">Feature 'interfaces with multiple generic instantiation' is not available in F# 4.7. Please use language version 'preview' or greater.</Expects>

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
