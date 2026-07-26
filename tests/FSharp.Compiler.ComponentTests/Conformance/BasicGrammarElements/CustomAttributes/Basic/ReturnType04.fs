// #Regression #Conformance #DeclarationElements #Attributes
// Regression test for https://github.com/dotnet/fsharp/issues/462
// Attributes on return type of unparenthesized tuple must not be silently dropped.
//<Expects status="success"></Expects>

open System

[<AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)>]
type ReturnDescriptionAttribute(info: string) =
    inherit Attribute()
    member _.Info = info

type T =
    static member Parenthesized(name: string) :
        [<ReturnDescription("first")>]
        [<ReturnDescription("second")>]
        (string * string) =
            name, name

    static member Bare(name: string) :
        [<ReturnDescription("first")>]
        [<ReturnDescription("second")>]
        string * string =
            name, name

let getAttrs (methodName: string) =
    typeof<T>
        .GetMethod(methodName)
        .ReturnParameter
        .GetCustomAttributes(typeof<ReturnDescriptionAttribute>, false)
    |> Array.map (fun a -> (a :?> ReturnDescriptionAttribute).Info)
    |> Array.sort

let parenthesized = getAttrs "Parenthesized"
if parenthesized.Length <> 2 then failwithf "Parenthesized: expected 2 attributes, got %d" parenthesized.Length

let bare = getAttrs "Bare"
if bare.Length <> 2 then failwithf "Bare: expected 2 attributes, got %d (bug #462 — attributes on unparenthesized tuple return type are silently dropped)" bare.Length
