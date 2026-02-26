// #Conformance #DataExpressions #ObjectConstructors 
// Verify Dispatch Slot Checking when the interface has an overloaded method.

open System

type IHaveOverload =
    abstract DoStuff : int    -> int
    abstract DoStuff : string -> int

type SomeClass<'a> (arg : 'a) =
    member this.Value = arg

let test = { 
            new SomeClass<_>("ConstructorArgument") with
                override this.ToString() = "SomeClass"
                
            interface IHaveOverload with
                  override this.DoStuff (x : int) = x
                  override this.DoStuff (x : string) = Int32.Parse(x)
        }

if test.Value <> "ConstructorArgument" then exit 1

if (box test :?> IHaveOverload).DoStuff 128 <> 128 then exit 1
if (box test :?> IHaveOverload).DoStuff "3" <>   3 then exit 1

exit 0
