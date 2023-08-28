// #Regression #Conformance #TypeInference 
// FSHARP1.0:1445. See also FSHARP1.0:4721
// Failure when generating code for generic interface with generic method
//<Expects id="FS0030" span="(16,5-16,11)" status="error">Value restriction: The value 'result' has an inferred generic type\n    val result: '_a array when '_a: equality and '_a: null\nHowever, values cannot have generic type variables like '_a in "let x: '_a"\. You can do one of the following:\n- Define it as a simple data term (e\.g\. a string literal or a union case)\n- Add an explicit type annotation\n- Use the value as a non-generic type in later code for type inference,\nor if you still want type-dependent results, you can define 'result' as a function instead by doing either:\n- Add a unit parameter\n- Write explicit type parameters like "let x<'a>"\.\nThis error is because a let binding without parameters defines a value, not a function\. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results\.$</Expects>
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
