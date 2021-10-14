// #Regression #Conformance #TypeInference 
// FSHARP1.0:1445. See also FSHARP1.0:4721
// Failure when generating code for generic interface with generic method
//<Expects span="(16,5-16,11)" status="error" id="FS0030">Value restriction\. The value 'result' has been inferred to have generic type.    val result: '_a array when '_a: equality and '_a: null    .Either define 'result' as a simple data term, make it a function with explicit arguments or, if you do not intend for it to be generic, add a type annotation\.$</Expects>
type 'a IFoo = interface
    abstract DoStuff<'b> : 'a -> 'b array
end

type Foo<'t, 'u>() = class
    interface IFoo<'t> with
        member this.DoStuff (x : 't) = Array.zeroCreate 1
    end
end

let test = new Foo<int, string>()
let result = (test :> IFoo<int>).DoStuff 42    // trouble - we can't tell what DoStuff we want
if result <> [| null |] then exit 1
