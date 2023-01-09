// #Regression #NoMT #FSI 
// Regression test for FSharp1.0:1626 - Classes without constructors can't be emitted by Reflection.Emit. This causes late error in compilation
// <Expects id="FS0193" status="error">Parent does not have a default constructor\. The default constructor must be explicitly defined</Expects>

type B = class

  new(a:int) = {}

end

type C = class
  inherit B
end

#if INTERACTIVE
;;
exit 1;;
#endif

