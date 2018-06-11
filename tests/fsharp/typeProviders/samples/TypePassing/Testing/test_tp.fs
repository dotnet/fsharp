namespace Test

open System
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open UncheckedQuotations
open System.Collections
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Reflection


[<TypeProvider>]
type TypePassingTp(config: TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces(config)

    let ns = "Testing"
    let runtimeAssembly = Assembly.LoadFrom(config.RuntimeAssembly)

    let createRecordOfArrays (name : string) (t : Type) : ProvidedTypeDefinition =
        let fields = t.GetFields()
        let fieldCount = field.Length

        let underlyingType =
            fields
            |> Array.map (fun f -> f.FieldType.MakeArrayType())
            |> FSharpType.MakeTupleType

        let baseType =
            ProvidedTypeDefinition(
                assembly = runtimeAssembly,
                namespaceName = ns,
                className = name,
                baseType = Some underlyingType,
                hideObjectMethods = true,
                isErased = true
            )

        let makeArrayProperty (name : string) (t : Type) (i : int) =
            let arrayTy = t.MakeArrayType()
            let getterF (e : Expr list) = Expr.TupleGet(e.[0], i)
            ProvidedProperty(name, arrayTy, getterCode = getterF, isStatic = false)

        let replaceTypeOnCall (t : Type) = function
            | Call(o,mi,args) ->
                let newMI = mi.GetGenericMethodDefinition().MakeGenericMethod([|t|])
                match o with
                | Some o -> Expr.Call(o,newMI,args)
                | None -> Expr.Call(newMI, args)
            | _ -> failwith "Called on a non-Call Expr"

        let replaceTypeOnPropertyGet (t : Type) = function
            | PropertyGet(o,mi,args) ->
                let newMI = mi.GetGenericMethodDefinition().MakeGenericMethod([|t|])
                match o with
                | Some o -> Expr.Call(o,newMI,args)
                | None -> Expr.Call(newMI, args)
            | _ -> failwith "Called on a non-Call Expr"

        let construct (e : Expr list) =
            let makeArrExpr (t : Type) =
                replaceTypeOnCall t <@@ Array.zeroCreate %%(e.[0]) @@>
            fields
            |> Array.map (fun f -> makeArrExpr f.FieldType)
            |> Array.toList
            |> Expr.NewTuple
            
        let constructor =

            ProvidedConstructor(
                [ ProvidedParameter("length", typeof<int>)],
                invokeCode = construct)

//        let getItem (e : Expr list) =
//            let arrayGet x i = <@@ (%%x : obj[]).[%%i] @@> 
//            let rfields =
//                [0..fieldCount-1]
//                |> List.map (fun i -> Expr.PropertyGet(Expr.TupleGet(e.[0], i))
//            Expr.NewRecordUnchecked(t, rfields)
//
//        let setItem (e : Expr list) =
//            failwith ""
//
//        let itemProperty =
//
//            ProvidedProperty(
//                [ ProvidedParameter("index", typeof<int>)],
//                getterCode = getItem,
//                setterCode = setItem,
//                isStatic = false
//                )

        fields
        |> Seq.mapi (fun i p -> makeArrayProperty p.Name (p.FieldType) i :> MemberInfo)
        |> Seq.toList
        |> fun l -> (constructor :> MemberInfo) :: l
        |> baseType.AddMembers

        baseType

    let arrayify = ProvidedTypeDefinition(runtimeAssembly, ns, "Arrayify", baseType = Some typeof<obj>, hideObjectMethods=true)
    do arrayify.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> (createRecordOfArrays typeName (unbox args.[0]) :> Type))
    
    // ====== Generic Types ========

    // Type Provider language
    // 
    // This is an encoding of F# computation in type providers.
    //
    // In this encoding, a value V is a type with a static member called X, of type: t
    // t is the type of V, the implementation of the static member is the value.
    //
    // Eg. myInt = 5 : int
    //
    // type MyInt =
    //      static member X : int = 5
    //
    // Values cannot be brought into the Type Provider from F#, but they may be
    // composed based on their type.
    //
    // The idea is that this language is capable of expressing more types than F# itself
    // can.

    let decode (t : Type) =
        let m = t.GetMethods().[0]
        Expr.CallUnchecked(m, []), m.ReturnType

    let encode (name : string) (expr : Expr) (t : Type) =
        let newType = ProvidedTypeDefinition(runtimeAssembly, ns, name, baseType = Some typeof<obj>, hideObjectMethods = true)
//        let m = ProvidedProperty("X", t, getterCode = (fun _ -> expr), isStatic = true)
        let m = ProvidedMethod("X", [ProvidedParameter("dummy", typeof<unit>)], t, invokeCode = (fun _ -> expr), isStatic = true)
        newType.AddMember(m)
        newType

    let createIdType (t : Type) (name : string) : ProvidedTypeDefinition =
        // t is a type, and a value
        let expr, actualT = decode t
        encode name expr actualT

    let idFunc = ProvidedTypeDefinition(runtimeAssembly, ns, "Id", baseType = Some typeof<obj>, hideObjectMethods=true)
    do idFunc.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> (createIdType (unbox args.[0]) typeName :> Type))

    let idType = ProvidedTypeDefinition(runtimeAssembly, ns, "IdType", baseType = Some typeof<obj>, hideObjectMethods=true)
    do idType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> unbox args.[0])


    let createLensType (t : Type) (name : string) : ProvidedTypeDefinition =
        let lensType = ProvidedTypeDefinition(runtimeAssembly, ns, name, baseType = Some typeof<obj>, hideObjectMethods=true)
        let fields = t.GetFields() |> Array.toList
        let makeCons es = Expr.NewRecord(t, es)
        let funTy a b = ProvidedTypeBuilder.MakeGenericType(typedefof<FSharpFunc<_, _>>, [a; b])
        let getExprs (r : Expr) = fields |> List.map (fun f -> Expr.FieldGet(r, f))

        let makeFieldLens (index : int) (f : FieldInfo) =
            let fieldType = f.FieldType
            let getterType = funTy t fieldType
            let setterType = funTy t (funTy fieldType t)
            let lensType = ProvidedTypeBuilder.MakeTupleType([getterType; setterType])

            let getterCode (_ : Expr list) =
                let getterExpr =
                    let aVar = Var("a", t)
                    Expr.Lambda(aVar, Expr.FieldGet(Expr.Var aVar, f))
                let setterExpr =
                    let aVar = Var("a", t)
                    let bVar = Var("b", fieldType)
                    let setBInA =
                        let newFields =
                            getExprs (Expr.Var aVar)
                            |> List.mapi (fun i fe -> if i = index then Expr.Var bVar else fe)
                        Expr.NewRecordUnchecked(t, newFields)
                    Expr.Lambda(aVar, Expr.Lambda(bVar, setBInA))
                Expr.NewTupleUnchecked([getterExpr; setterExpr])
            ProvidedProperty(f.Name, lensType, getterCode = getterCode, isStatic = true)

        fields
        |> List.mapi makeFieldLens
        |> lensType.AddMembers

        lensType

    let lensType = ProvidedTypeDefinition(runtimeAssembly, ns, "Lens", baseType = Some typeof<obj>, hideObjectMethods=true)
    do lensType.DefineStaticParameters(
        [
            ProvidedStaticParameter("Type",typeof<Type>, null)
        ], fun typeName args -> createLensType (unbox args.[0]) typeName :> Type)

    // ===========================

    do this.AddNamespace(ns, [idFunc; arrayify; idType; lensType])

[<assembly:TypeProviderAssembly>]
do()

