module FSharp.Test.ReflectionHelper

open System
open System.Reflection

/// Gets the given type from the assembly (otherwise throws)
let getType typeName (asm: Assembly) =
    match asm.GetType(typeName, false) with
    | null ->
        let allTypes =
            asm.GetTypes()
            |> Array.map (fun ty -> ty.Name)
            |> Array.reduce (fun x y -> $"%s{x}\r%s{y}")

        failwith $"Error: Assembly did not contain type %s{typeName}.\nAll types in asm:\n%s{allTypes}"
    | ty -> ty

/// Gets all anonymous types from the assembly
let getAnonymousTypes (asm: Assembly) =
    [ for ty in asm.GetTypes() do
          if ty.FullName.StartsWith "<>f__AnonymousType" then ty ]

/// Gets the first anonymous type from the assembly
let getFirstAnonymousType asm =
    match getAnonymousTypes asm with
    | ty :: _ -> ty
    | [] -> failwith "Error: No anonymous types found in the assembly"

/// Gets a type's method
let getMethod methodName (ty: Type) =
    match ty.GetMethod(methodName) with
    | null -> failwith $"Error: Type did not contain member %s{methodName}"
    | methodInfo -> methodInfo

/// Assert that function f returns Ok for given input
let should f x y =
    match f x y with
    | Ok _ -> y
    | Error message -> failwith $"%s{message} but it should"

/// Assert that function f doesn't return Ok for given input
let shouldn't f x y =
    match f x y with
    | Ok message -> failwith $"%s{message} but it shouldn't"
    | Error _ -> y

/// Verify the object contains a custom attribute with the given name. E.g. "ObsoleteAttribute"
let haveAttribute attrName thingy =
    let attrs =
        match box thingy with
        | :? Type as ty -> ty.GetCustomAttributes(false)
        | :? MethodInfo as mi -> mi.GetCustomAttributes(false)
        | :? PropertyInfo as pi -> pi.GetCustomAttributes(false)
        | :? EventInfo as ei -> ei.GetCustomAttributes(false)
        | _ -> failwith "Error: Unsupported primitive type, unable to get custom attributes."

    let hasAttribute =
        attrs |> Array.exists (fun att -> att.GetType().Name = attrName)

    if hasAttribute then
        Ok $"'{thingy}' has attribute '{attrName}'"
    else
        Error $"'{thingy}' doesn't have attribute '{attrName}'"
