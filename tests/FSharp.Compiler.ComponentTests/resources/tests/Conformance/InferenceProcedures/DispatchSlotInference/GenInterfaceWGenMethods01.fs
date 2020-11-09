// #Regression #Conformance #TypeInference 
// FSHARP1.0:1445. See also FSHARP1.0:4721
// Failure when generating code for generic interface with generic method

type 'a IFoo = interface
    abstract DoStuff<'b> : 'a -> 'b array
end

type Foo<'t, 'u>() = class
    interface IFoo<'t> with
        member this.DoStuff (x : 't) = Array.zeroCreate 1
    end
end

let test = new Foo<int, string>()
let result = (test :> IFoo<int>).DoStuff<string> 42
if result <> [| null |] then exit 1

exit 0
