namespace Provider
#load "TypeMagic.fs"

open System
open System.Linq.Expressions
open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Linq.NullableOperators
open System.Collections.Generic
open System.IO
open FSharp.TypeMagic

[<assembly: TypeProviderAssembly>]
do ()

[<AutoOpen>]
module Utils = 
    let mkNamespace (name,typ:System.Type) = 
        { new IProvidedNamespace with
          member this.NamespaceName = name
          member this.GetTypes() = [|typ|]
          member this.GetNestedNamespaces() = [| |]
          member this.ResolveTypeName typeName = if typ.Name = typeName then typ else null }

    let mkParamArrayAttributeData() = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<ParamArrayAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| |]
                member __.NamedArguments = upcast [| |] }

    let mkObsoleteAttributeData(msg:string) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<ObsoleteAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<string>, msg)  |]
                member __.NamedArguments = upcast [| |] }

    let mkConditionalAttributeData(msg:string) = 
        { new CustomAttributeData() with 
                member __.Constructor =  typeof<System.Diagnostics.ConditionalAttribute>.GetConstructors().[0]
                member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<string>, msg)  |]
                member __.NamedArguments = upcast [| |] }


type public Runtime() =
    static member Id x = x

type MemoizationTable<'T,'U when 'T : equality>(createType) =
    let createdDB = new Dictionary<'T,'U>()
    member x.Contents = (createdDB :> IDictionary<_,_>)
    member x.Apply typeName =
        let found,result = createdDB.TryGetValue typeName
        if found then
            result 
        else
            let ty = createType typeName
            createdDB.[typeName] <- ty
            ty 

[<TypeProvider>]
type public Provider() =
    let thisAssembly = typeof<Provider>.Assembly
    let modul = thisAssembly.GetModules().[0]
    let rootNamespace = "FSharp.HelloWorld"
    // Test provision of an erased class 
    let mkHelloWorldType namespaceName className baseType =
        let rec allMembers = 
            lazy 
                [| yield TypeBuilder.CreateMethod(theType ,"CallInstrinsics",typeof<obj list>, isStatic=true, parameters=[|  |]) :> MemberInfo 
                |]  
        and theType = TypeBuilder.CreateSimpleType(TypeContainer.Namespace(modul, namespaceName),className,members=allMembers,baseType=baseType)

        theType

    let helloWorldType = mkHelloWorldType rootNamespace "HelloWorldType" (typeof<obj>)

    let types = 
        [| helloWorldType |]

    let invalidation = new Event<System.EventHandler,_>()

    interface IProvidedNamespace with
        member this.NamespaceName = rootNamespace
        member this.GetTypes() = types
        member __.ResolveTypeName typeName : System.Type = 
            match typeName with
            | "HelloWorldType" -> helloWorldType
            | _ -> null
       
        member this.GetNestedNamespaces() = [|  |]

    interface IDisposable with
        member __.Dispose() = ()
    interface ITypeProvider with
        member this.GetNamespaces() = [| this |]

        member this.GetStaticParameters(typeWithoutArguments) = 
         [| 
         |]
        member this.ApplyStaticArguments(typeWithoutArguments, typeNameWithArguments, staticArguments) =
            let typeNameWithArguments = typeNameWithArguments.[typeNameWithArguments.Length-1]
            match typeWithoutArguments.Name with
            | nm -> failwith (sprintf "ApplyStaticArguments %s" nm)

        member __.GetInvokerExpression(syntheticMethodBase:MethodBase, parameterExpressions:Quotations.Expr[]) = 
            let expr = 
                if syntheticMethodBase.IsConstructor then 
                     Quotations.Expr.NewObject(typeof<obj>.GetConstructor [| |], [])
                elif syntheticMethodBase.Name = "CallInstrinsics" then 
                     <@@ [ ((true & false) |> box);
                           ((true && false) |> box);
                           ((true || false) |> box);
                           ((true or false) |> box);
                           ((compare true false) |> box);
                           ((true = false) |> box);
                           ((3 +? System.Nullable 4) |> box);  // should be inlined by the compiler
                           ((System.Nullable 3 ?+ 4) |> box); // should be inlined by the compiler
                           ((3 *? System.Nullable 4) |> box); // should be inlined by the compiler
                           ((System.Nullable 3 ?* 4) |> box); // should be inlined by the compiler
                           ((true =? System.Nullable false) |> box);
                           ((System.Nullable true ?= false) |> box);
                           ((System.Nullable true ?=? System.Nullable false) |> box);
                           ((true <> false) |> box);
                           ((1 < 2) |> box);
                           ((1 <= 2) |> box);
                           (1 > 2) |> box;
                           (1 >= 2) |> box;
                           LanguagePrimitives.GenericComparisonWithComparer LanguagePrimitives.GenericComparer 3 4 |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple2 LanguagePrimitives.GenericEqualityComparer (1,2) |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple3 LanguagePrimitives.GenericEqualityComparer (1,2,3) |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple4 LanguagePrimitives.GenericEqualityComparer (1,2,3,4) |> box;
                           LanguagePrimitives.HashCompare.FastHashTuple5 LanguagePrimitives.GenericEqualityComparer (1,2,3,4,5) |> box;

                           LanguagePrimitives.HashCompare.FastEqualsTuple2 LanguagePrimitives.GenericEqualityComparer (1,2)  (1,2) |> box;
                           LanguagePrimitives.HashCompare.FastEqualsTuple3 LanguagePrimitives.GenericEqualityComparer (1,2,3)  (1,2,3) |> box;
                           LanguagePrimitives.HashCompare.FastEqualsTuple4 LanguagePrimitives.GenericEqualityComparer (1,2,3,4)  (1,2,3,4) |> box;
                           LanguagePrimitives.HashCompare.FastEqualsTuple5 LanguagePrimitives.GenericEqualityComparer (1,2,3,4,5)  (1,2,3,4,5) |> box;

                           LanguagePrimitives.HashCompare.FastCompareTuple2 LanguagePrimitives.GenericComparer (1,2)  (1,2) |> box;
                           LanguagePrimitives.HashCompare.FastCompareTuple3 LanguagePrimitives.GenericComparer (1,2,3)  (1,2,3) |> box;
                           LanguagePrimitives.HashCompare.FastCompareTuple4 LanguagePrimitives.GenericComparer (1,2,3,4)  (1,2,3,4) |> box;
                           LanguagePrimitives.HashCompare.FastCompareTuple5 LanguagePrimitives.GenericComparer (1,2,3,4,5)  (1,2,3,4,5) |> box;
                           (3 ||| 4 |> box);
                           (3 &&& 4 |> box);
                           (3 ^^^ 4 |> box); 
                           (~~~3 |> box); 
                           ((3 <<< 1) |> box); 
                           ((3 >>> 1) |> box); 
                           ((3 + 1) |> box); 
                           ((3 - 1) |> box); 
                           ((3 * 1) |> box); 
                           ((+(3)) |> box); 
                           (List.map id [1;2;3] |> box); 
                           (List.sum [1;2;3] |> box);  // should be inlined by the compiler
                           (List.sumBy id [1;2;3] |> box);  // should be inlined by the compiler
                           (List.average [1.0;2.0;3.0] |> box);  // should be inlined by the compiler
                           (List.averageBy id [1.0;2.0;3.0] |> box);  // should be inlined by the compiler
                           (Array.map id [|1;2;3|] |> box); 
                           (Array.sum [|1;2;3|] |> box);      // should be inlined by the compiler
                           (Array.sumBy id [|1;2;3|] |> box);   // should be inlined by the compiler
                           (Array.average [|1.0;2.0;3.0|] |> box);  // should be inlined by the compiler
                           (Array.averageBy id [|1.0;2.0;3.0|] |> box);  // should be inlined by the compiler
                           (Seq.map id [1;2;3] |> box); 
                           (Seq.sum [1;2;3] |> box); // should be inlined by the compiler
                           (Seq.sumBy id [1;2;3] |> box); // should be inlined by the compiler
                           (Seq.average [1.0;2.0;3.0] |> box); // should be inlined by the compiler
                           (Seq.averageBy id [1.0;2.0;3.0] |> box); // should be inlined by the compiler
                           ((-(3)) |> box); 
                           ((3.0 + 1.0) |> box); 
                           ((3.0 - 1.0) |> box); 
                           ((3.0 * 1.0) |> box); 
                           ((+(3.0)) |> box); 
                           ((-(3.0)) |> box); 
                           ((3.0f + 1.0f) |> box); 
                           ((3.0f - 1.0f) |> box); 
                           ((3.0f * 1.0f) |> box); 
                           ((+(3.0f)) |> box); 
                           ((-(3.0f)) |> box); 
                           (Operators.Checked.(+) 3 4  |> box); 
                           Operators.Checked.(-) 3 4  |> box; 
                           Operators.Checked.(*) 3 4  |> box; 
                           (Operators.Checked.(~-) 3  |> box); 
                           (not true  |> box); 
                           (typeof<int> |> box); 
                           (sizeof<int> |> box); 
                           (Unchecked.defaultof<int> |> box); 
                           (typedefof<int> |> box); 
                           (enum<System.DayOfWeek>(3)  |> box); 
                           ((..) 3 4   |> box); 
                           (OperatorIntrinsics.RangeInt32 3 4   |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray [|1;2|] 0   |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray2D (array2D [[1;2]]) 0 0   |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray3D (Array3D.init 1 1 1 (fun _ _ _ -> 0) ) 0 0 0  |> box); 
                           (LanguagePrimitives.IntrinsicFunctions.GetArray4D (Array4D.init 1 1 1 1 (fun _ _ _ _ -> 0) )  0 0 0 0 |> box); 
                           (Seq.collect (fun x -> [x]) [1;2]   |> box); 
                           (Seq.delay (fun _ -> Seq.empty) |> box); 
                           (Seq.append [1] [2] |> box); 
                           (seq { for i in 0 .. 10 -> (i,i*i) } |> box); 
                           ([| for i in 0 .. 10 -> (i,i*i) |] |> box); 
                           ([ for i in 0 .. 10 -> (i,i*i) ] |> box); 
                           (sprintf "%d" 1 |> box); 
                           ((lazy (3 + 4)) |> box);
                           (1.0M |> box);
                           // These are not supported as provided expressions
                           //(<@ 1+2 @> |> box);
                           //(<@@ 1+2 @@> |> box);
                           ]
                         @@>

                else
                    // trim off the "get_"
                    if not (syntheticMethodBase.Name.StartsWith "get_") then failwith "expected syntheticMethodBase to be a property getter, with name starting with \"get_\""
                    let propertyName = syntheticMethodBase.Name.Substring(4)
                    let syntheticMethodBase = syntheticMethodBase :?> MethodInfo
                    let getClassInstancesByName = 
                        typeof<Provider>
                            .GetMethod("GetPropertyByName", BindingFlags.Static ||| BindingFlags.Public)
                            .MakeGenericMethod([|syntheticMethodBase.ReturnType|])
                    Quotations.Expr.Call(getClassInstancesByName, [ Quotations.Expr.Value(propertyName) ])
            
            let rec trans q = 
                match q with 
                // Eliminate F# property gets to method calls
                | Quotations.Patterns.PropertyGet(obj,propInfo,args) -> 
                    match obj with 
                    | None -> trans (Quotations.Expr.Call(propInfo.GetGetMethod(),args))
                    | Some o -> 
                       trans (Quotations.Expr.Call(trans o,propInfo.GetGetMethod(),args))
                // Eliminate F# property sets to method calls
                | Quotations.Patterns.PropertySet(obj,propInfo,args,v) -> 
                     match obj with 
                     | None -> trans (Quotations.Expr.Call(propInfo.GetSetMethod(),args@[v]))
                     | Some o -> trans (Quotations.Expr.Call(trans o,propInfo.GetSetMethod(),args@[v]))
                // Eliminate F# function applications to FSharpFunc<_,_>.Invoke calls
                | Quotations.Patterns.Application(f,e) -> 
                    Quotations.Expr.Call(trans f, f.Type.GetMethod "Invoke", [ trans e ]) 

                | Quotations.Patterns.NewUnionCase(ci, es) ->
                    trans (Quotations.Expr.Call(Reflection.FSharpValue.PreComputeUnionConstructorInfo ci, es) )
                | Quotations.Patterns.NewRecord(ci, es) ->
                    trans (Quotations.Expr.NewObject(Reflection.FSharpValue.PreComputeRecordConstructorInfo ci, es) )
                | Quotations.Patterns.UnionCaseTest(e,uc) ->
                    let tagInfo = Reflection.FSharpValue.PreComputeUnionTagMemberInfo uc.DeclaringType
                    let tagExpr = 
                        match tagInfo with 
                        | :? PropertyInfo as tagProp ->
                             Quotations.Expr.PropertyGet(e,tagProp) 
                        | :? MethodInfo as tagMeth -> 
                             if tagMeth.IsStatic then Quotations.Expr.Call(tagMeth, [e])
                             else Quotations.Expr.Call(e,tagMeth,[])
                        | _ -> failwith "unreachable: unexpected result from PreComputeUnionTagMemberInfo"
                    let tagNumber = uc.Tag
                    // Translate to a call the the F# library equality routine
                    trans <@@ (%%(tagExpr) : int) = tagNumber @@>

                // Handle the generic cases
                | Quotations.ExprShape.ShapeLambda(v,body) -> 
                    Quotations.Expr.Lambda(v, trans body)
                | Quotations.ExprShape.ShapeCombination(comb,args) -> 
                    Quotations.ExprShape.RebuildShapeCombination(comb,List.map trans args)
                | Quotations.ExprShape.ShapeVar _ -> q
            trans expr

        // This event is never triggered in this sample, because the schema never changes
        [<CLIEvent>]
        member this.Invalidate = invalidation.Publish
       
        member this.GetGeneratedAssemblyContents(assembly) = failwith "GetGeneratedAssemblyContents"

