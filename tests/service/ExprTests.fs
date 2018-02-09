
#if INTERACTIVE
#r "../../Debug/fcs/net45/FSharp.Compiler.Service.dll" // note, run 'build fcs debug' to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../Debug/net40/bin/FSharp.Compiler.Service.ProjectCracker.dll"
#r "../../packages/NUnit.3.5.0/lib/net45/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module FSharp.Compiler.Service.Tests.ExprTests
#endif


open NUnit.Framework
open FsUnit
open System
open System.IO
open System.Collections.Generic
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Service
open FSharp.Compiler.Service.Tests.Common

let internal exprChecker = FSharpChecker.Create(keepAssemblyContents=true)


[<AutoOpen>]
module internal Utils = 
    let rec printExpr low (e:FSharpExpr) = 
        match e with 
        | BasicPatterns.AddressOf(e1) -> "&"+printExpr 0 e1
        | BasicPatterns.AddressSet(e1,e2) -> printExpr 0 e1 + " <- " + printExpr 0 e2
        | BasicPatterns.Application(f,tyargs,args) -> quote low (printExpr 10 f + printTyargs tyargs + " " + printCurriedArgs args)
        | BasicPatterns.BaseValue(_) -> "base"
        | BasicPatterns.Call(Some obj,v,tyargs1,tyargs2,argsL) -> printObjOpt (Some obj) + v.CompiledName  + printTyargs tyargs2 + printTupledArgs argsL
        | BasicPatterns.Call(None,v,tyargs1,tyargs2,argsL) -> v.DeclaringEntity.Value.CompiledName + printTyargs tyargs1 + "." + v.CompiledName  + printTyargs tyargs2 + " " + printTupledArgs argsL
        | BasicPatterns.Coerce(ty1,e1) -> quote low (printExpr 10 e1 + " :> " + printTy ty1)
        | BasicPatterns.DefaultValue(ty1) -> "dflt"
        | BasicPatterns.FastIntegerForLoop _ -> "for-loop"
        | BasicPatterns.ILAsm(s,tyargs,args) -> s + printTupledArgs args 
        | BasicPatterns.ILFieldGet _ -> "ILFieldGet"
        | BasicPatterns.ILFieldSet _ -> "ILFieldSet"
        | BasicPatterns.IfThenElse (a,b,c) -> "(if " + printExpr 0 a + " then " + printExpr 0 b + " else " + printExpr 0 c + ")"
        | BasicPatterns.Lambda(v,e1) -> "fun " + v.CompiledName + " -> " + printExpr 0 e1
        | BasicPatterns.Let((v,e1),b) -> "let " + (if v.IsMutable then "mutable " else "") + v.CompiledName + ": " + printTy v.FullType + " = " + printExpr 0 e1 + " in " + printExpr 0 b
        | BasicPatterns.LetRec(vse,b) -> "let rec ... in " + printExpr 0 b
        | BasicPatterns.NewArray(ty,es) -> "[|" + (es |> Seq.map (printExpr 0) |> String.concat "; ") +  "|]" 
        | BasicPatterns.NewDelegate(ty,es) -> "new-delegate" 
        | BasicPatterns.NewObject(v,tys,args) -> "new " + v.DeclaringEntity.Value.CompiledName + printTupledArgs args 
        | BasicPatterns.NewRecord(v,args) -> 
            let fields = v.TypeDefinition.FSharpFields
            "{" + ((fields, args) ||> Seq.map2 (fun f a -> f.Name + " = " + printExpr 0 a) |> String.concat "; ") + "}" 
        | BasicPatterns.NewTuple(v,args) -> printTupledArgs args 
        | BasicPatterns.NewUnionCase(ty,uc,args) -> uc.CompiledName + printTupledArgs args 
        | BasicPatterns.Quote(e1) -> "quote" + printTupledArgs [e1]
        | BasicPatterns.FSharpFieldGet(obj, ty,f) -> printObjOpt obj + f.Name 
        | BasicPatterns.FSharpFieldSet(obj, ty,f,arg) -> printObjOpt obj + f.Name + " <- " + printExpr 0 arg
        | BasicPatterns.Sequential(e1,e2) -> "(" + printExpr 0 e1 + "; " + printExpr 0 e2 + ")"
        | BasicPatterns.ThisValue _ -> "this"
        | BasicPatterns.TryFinally(e1,e2) -> "try " + printExpr 0 e1 + " finally " + printExpr 0 e2
        | BasicPatterns.TryWith(e1,_,_,vC,eC) -> "try " + printExpr 0 e1 + " with " + vC.CompiledName + " -> " + printExpr 0 eC
        | BasicPatterns.TupleGet(ty,n,e1) -> printExpr 10 e1 + ".Item" + string n
        | BasicPatterns.DecisionTree(dtree,targets) -> "match " + printExpr 10 dtree + " targets ..."
        | BasicPatterns.DecisionTreeSuccess (tg,es) -> "$" + string tg
        | BasicPatterns.TypeLambda(gp1,e1) -> "FUN ... -> " + printExpr 0 e1 
        | BasicPatterns.TypeTest(ty,e1) -> printExpr 10 e1 + " :? " + printTy ty
        | BasicPatterns.UnionCaseSet(obj,ty,uc,f1,e1) -> printExpr 10 obj + "." + f1.Name + " <- " + printExpr 0 e1
        | BasicPatterns.UnionCaseGet(obj,ty,uc,f1) -> printExpr 10 obj + "." + f1.Name
        | BasicPatterns.UnionCaseTest(obj,ty,f1) -> printExpr 10 obj + ".Is" + f1.Name
        | BasicPatterns.UnionCaseTag(obj,ty) -> printExpr 10 obj + ".Tag" 
        | BasicPatterns.ObjectExpr(ty,basecall,overrides,iimpls) -> "{ " + printExpr 10 basecall + " with " + printOverrides overrides + " " + printIimpls iimpls + " }"
        | BasicPatterns.TraitCall(tys,nm,_,argtys,tinst,args) -> "trait call " + nm + printTupledArgs args
        | BasicPatterns.Const(obj,ty) -> 
            match obj with 
            | :? string  as s -> "\"" + s + "\""
            | null -> "()"
            | _ -> string obj
        | BasicPatterns.Value(v) -> v.CompiledName
        | BasicPatterns.ValueSet(v,e1) -> quote low (v.CompiledName + " <- " + printExpr 0 e1)
        | BasicPatterns.WhileLoop(e1,e2) -> "while " + printExpr 0 e1 + " do " + printExpr 0 e2 + " done"
        | _ -> failwith (sprintf "unrecognized %+A" e)

    and quote low s = if low > 0 then "(" + s + ")" else s
    and printObjOpt e = match e with None -> "" | Some e -> printExpr 10 e + "."
    and printTupledArgs args = "(" + String.concat "," (List.map (printExpr 0) args) + ")"
    and printCurriedArgs args = String.concat " " (List.map (printExpr 10) args)
    and printParams (vs: FSharpMemberOrFunctionOrValue list) = "(" + String.concat "," (vs |> List.map (fun v -> v.CompiledName)) + ")"
    and printCurriedParams (vs: FSharpMemberOrFunctionOrValue list list) = String.concat " " (List.map printParams vs)
    and printTy ty = ty.Format(FSharpDisplayContext.Empty)
    and printTyargs tyargs = match tyargs with [] -> "" | args -> "<" + String.concat "," (List.map printTy tyargs) + ">"
    and printOverrides ors = String.concat ";" (List.map printOverride ors)
    and printOverride o = 
        match o.CurriedParameterGroups with
        | [t] :: a ->
            "member " + t.CompiledName + "." + o.Signature.Name + printCurriedParams a + " = " + printExpr 10 o.Body
        | _ -> failwith "wrong this argument in object expression override"
    and printIimpls iis = String.concat ";" (List.map printImlementation iis)
    and printImlementation (i, ors) = "interface " + printTy i + " with " + printOverrides ors

    let rec printFSharpDecls prefix decls =
        seq {
            let mutable i = 0
            for decl in decls do
                i <- i + 1
                match decl with
                | FSharpImplementationFileDeclaration.Entity (e, sub) ->
                    yield sprintf "%s%i) ENTITY: %s %A" prefix i e.CompiledName (attribsOfSymbol e)
                    if not (Seq.isEmpty e.Attributes) then
                        yield sprintf "%sattributes: %A" prefix (Seq.toList e.Attributes)
                    if not (Seq.isEmpty e.DeclaredInterfaces) then
                        yield sprintf "%sinterfaces: %A" prefix (Seq.toList e.DeclaredInterfaces)
                    yield ""
                    yield! printFSharpDecls (prefix + "\t") sub
                | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue (meth, args, body) ->
                    yield sprintf "%s%i) METHOD: %s %A" prefix i meth.CompiledName (attribsOfSymbol meth)
                    yield sprintf "%stype: %A" prefix meth.FullType
                    yield sprintf "%sargs: %A" prefix args
                    // if not meth.IsCompilerGenerated then
                    yield sprintf "%sbody: %A" prefix body
                    yield ""
                | FSharpImplementationFileDeclaration.InitAction (expr) ->
                    yield sprintf "%s%i) ACTION" prefix i
                    yield sprintf "%s%A" prefix expr
                    yield ""
        }

    let rec printDeclaration (excludes:HashSet<_> option) (d: FSharpImplementationFileDeclaration) = 
        seq {
           match d with 
            | FSharpImplementationFileDeclaration.Entity(e,ds) ->
                yield sprintf "type %s" e.LogicalName
                yield! printDeclarations excludes ds
            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue(v,vs,e) ->
            
               if not v.IsCompilerGenerated && 
                  not (match excludes with None -> false | Some t -> t.Contains v.CompiledName) then
                let text = 
                    //printfn "%s" v.CompiledName
//                 try
                    if v.IsMember then 
                        sprintf "member %s%s = %s @ %s" v.CompiledName (printCurriedParams vs)  (printExpr 0 e) (e.Range.ToShortString())
                    else 
                        sprintf "let %s%s = %s @ %s" v.CompiledName (printCurriedParams vs) (printExpr 0 e) (e.Range.ToShortString())
//                 with e -> 
//                     printfn "FAILURE STACK: %A" e
//                     sprintf "!!!!!!!!!! FAILED on %s @ %s, message: %s" v.CompiledName (v.DeclarationLocation.ToString()) e.Message
                yield text
            | FSharpImplementationFileDeclaration.InitAction(e) ->
                yield sprintf "do %s" (printExpr 0 e) }
    and printDeclarations excludes ds = 
        seq { for d in ds do 
                yield! printDeclaration excludes d }

    let rec exprsOfDecl (d: FSharpImplementationFileDeclaration) = 
        seq {
           match d with 
            | FSharpImplementationFileDeclaration.Entity(e,ds) ->
                yield! exprsOfDecls ds
            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue(v,vs,e) ->
               if not v.IsCompilerGenerated then
                  yield e, e.Range
            | FSharpImplementationFileDeclaration.InitAction(e) ->
                yield e, e.Range }
    and exprsOfDecls ds = 
        seq { for d in ds do 
                yield! exprsOfDecl d }

    let printGenericConstraint name (p: FSharpGenericParameterConstraint) =
        if p.IsCoercesToConstraint then
            Some <| name + " :> " + printTy p.CoercesToTarget 
        elif p.IsComparisonConstraint then 
            Some <| name + " : comparison"
        elif p.IsEqualityConstraint then
            Some <| name + " : equality"
        elif p.IsReferenceTypeConstraint then
            Some <| name + " : class"
        elif p.IsNonNullableValueTypeConstraint then
            Some <| name + " : struct"
        elif p.IsEnumConstraint then
            Some <| name + " : enum"
        elif p.IsSupportsNullConstraint then
            Some <| name + " : null"
        else None

    let printGenericParameter (p: FSharpGenericParameter) =
        let name = 
            if p.Name.StartsWith "?" then "_"
            elif p.IsSolveAtCompileTime then "^" + p.Name 
            else "'" + p.Name
        let constraints =
            p.Constraints |> Seq.choose (printGenericConstraint name) |> List.ofSeq
        name, constraints
    
    let printMemberSignature (v: FSharpMemberOrFunctionOrValue) =
        let genParams =
            let ps = v.GenericParameters |> Seq.map printGenericParameter |> List.ofSeq
            if List.isEmpty ps then "" else
                let constraints = ps |> List.collect snd
                "<" + (ps |> Seq.map fst |> String.concat ", ") + 
                    (if List.isEmpty constraints then "" else " when " + String.concat " and " constraints) + ">"

        v.CompiledName + genParams + ": " + printTy v.FullType

    let rec collectMembers (e:FSharpExpr) = 
        match e with 
        | BasicPatterns.AddressOf(e) -> collectMembers e
        | BasicPatterns.AddressSet(e1,e2) -> Seq.append (collectMembers e1) (collectMembers e2)
        | BasicPatterns.Application(f,_,args) -> Seq.append (collectMembers f) (Seq.collect collectMembers args)
        | BasicPatterns.BaseValue(_) -> Seq.empty
        | BasicPatterns.Call(Some obj,v,_,_,argsL) -> Seq.concat [ collectMembers obj; Seq.singleton v; Seq.collect collectMembers argsL ]
        | BasicPatterns.Call(None,v,_,_,argsL) -> Seq.concat [ Seq.singleton v; Seq.collect collectMembers argsL ]
        | BasicPatterns.Coerce(_,e) -> collectMembers e
        | BasicPatterns.DefaultValue(_) -> Seq.empty
        | BasicPatterns.FastIntegerForLoop (fromArg, toArg, body, _) -> Seq.collect collectMembers [ fromArg; toArg; body ]
        | BasicPatterns.ILAsm(_,_,args) -> Seq.collect collectMembers args 
        | BasicPatterns.ILFieldGet (Some e,_,_) -> collectMembers e
        | BasicPatterns.ILFieldGet _ -> Seq.empty
        | BasicPatterns.ILFieldSet (Some e,_,_,v) -> Seq.append (collectMembers e) (collectMembers v)
        | BasicPatterns.ILFieldSet _ -> Seq.empty
        | BasicPatterns.IfThenElse (a,b,c) -> Seq.collect collectMembers [ a; b; c ]
        | BasicPatterns.Lambda(v,e1) -> collectMembers e1
        | BasicPatterns.Let((v,e1),b) -> Seq.append (collectMembers e1) (collectMembers b)
        | BasicPatterns.LetRec(vse,b) -> Seq.append (vse |> Seq.collect (snd >> collectMembers)) (collectMembers b)
        | BasicPatterns.NewArray(_,es) -> Seq.collect collectMembers es
        | BasicPatterns.NewDelegate(ty,es) -> collectMembers es
        | BasicPatterns.NewObject(v,tys,args) -> Seq.append (Seq.singleton v) (Seq.collect collectMembers args)
        | BasicPatterns.NewRecord(v,args) -> Seq.collect collectMembers args
        | BasicPatterns.NewTuple(v,args) -> Seq.collect collectMembers args
        | BasicPatterns.NewUnionCase(ty,uc,args) -> Seq.collect collectMembers args
        | BasicPatterns.Quote(e1) -> collectMembers e1
        | BasicPatterns.FSharpFieldGet(Some obj, _,_) -> collectMembers obj
        | BasicPatterns.FSharpFieldGet _ -> Seq.empty
        | BasicPatterns.FSharpFieldSet(Some obj,_,_,arg) -> Seq.append (collectMembers obj) (collectMembers arg)
        | BasicPatterns.FSharpFieldSet(None,_,_,arg) -> collectMembers arg
        | BasicPatterns.Sequential(e1,e2) -> Seq.append (collectMembers e1) (collectMembers e2)
        | BasicPatterns.ThisValue _ -> Seq.empty
        | BasicPatterns.TryFinally(e1,e2) -> Seq.append (collectMembers e1) (collectMembers e2)
        | BasicPatterns.TryWith(e1,_,f,_,eC) -> Seq.collect collectMembers [ e1; f; eC ]
        | BasicPatterns.TupleGet(ty,n,e1) -> collectMembers e1
        | BasicPatterns.DecisionTree(dtree,targets) -> Seq.append (collectMembers dtree) (targets |> Seq.collect (snd >> collectMembers))
        | BasicPatterns.DecisionTreeSuccess (tg,es) -> Seq.collect collectMembers es
        | BasicPatterns.TypeLambda(gp1,e1) -> collectMembers e1
        | BasicPatterns.TypeTest(ty,e1) -> collectMembers e1
        | BasicPatterns.UnionCaseSet(obj,ty,uc,f1,e1) -> Seq.append (collectMembers obj) (collectMembers e1)
        | BasicPatterns.UnionCaseGet(obj,ty,uc,f1) -> collectMembers obj
        | BasicPatterns.UnionCaseTest(obj,ty,f1) -> collectMembers obj
        | BasicPatterns.UnionCaseTag(obj,ty) -> collectMembers obj
        | BasicPatterns.ObjectExpr(ty,basecall,overrides,iimpls) -> 
            seq {
                yield! collectMembers basecall
                for o in overrides do
                    yield! collectMembers o.Body
                for _, i in iimpls do
                    for o in i do
                        yield! collectMembers o.Body
            }
        | BasicPatterns.TraitCall(tys,nm,_,argtys,tinst,args) -> Seq.collect collectMembers args
        | BasicPatterns.Const(obj,ty) -> Seq.empty
        | BasicPatterns.Value(v) -> Seq.singleton v
        | BasicPatterns.ValueSet(v,e1) -> Seq.append (Seq.singleton v) (collectMembers e1)
        | BasicPatterns.WhileLoop(e1,e2) -> Seq.append (collectMembers e1) (collectMembers e2) 
        | _ -> failwith (sprintf "unrecognized %+A" e)

    let rec printMembersOfDeclatations ds = 
        seq { 
            for d in ds do 
            match d with 
            | FSharpImplementationFileDeclaration.Entity(_,ds) ->
                yield! printMembersOfDeclatations ds
            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue(v,vs,e) ->
                yield printMemberSignature v
                yield! collectMembers e |> Seq.map printMemberSignature
            | FSharpImplementationFileDeclaration.InitAction(e) ->
                yield! collectMembers e |> Seq.map printMemberSignature
        }


//---------------------------------------------------------------------------------------------------------
// This project is a smoke test for a whole range of standard and obscure expressions

module internal Project1 = 
    open System.IO

    let fileName1 = Path.ChangeExtension(Path.GetTempFileName(), ".fs")
    let base2 = Path.GetTempFileName()
    let fileName2 = Path.ChangeExtension(base2, ".fs")
    let dllName = Path.ChangeExtension(base2, ".dll")
    let projFileName = Path.ChangeExtension(base2, ".fsproj")
    let fileSource1 = """
module M

type IntAbbrev = int

let boolEx1 = true
let intEx1 = 1
let int64Ex1 = 1L
let tupleEx1 = (1, 1L)
let tupleEx2 = (1, 1L, 1u)
let tupleEx3 = (1, 1L, 1u, 1s)

let localExample = 
   let y = 1
   let z = 1
   y, z

let localGenericFunctionExample() = 
   let y = 1
   let compiledAsLocalGenericFunction x = x
   compiledAsLocalGenericFunction y, compiledAsLocalGenericFunction 1.0

let funcEx1 (x:int) =  x
let genericFuncEx1 (x:'T) =  x
let (topPair1a, topPair1b) = (1,2)

let testILCall1 = new obj()
let testILCall2 = System.Console.WriteLine("176")

// Test recursive values in a module
let rec recValNeverUsedAtRuntime = recFuncIgnoresFirstArg (fun _ -> recValNeverUsedAtRuntime) 1
and recFuncIgnoresFirstArg g v = v

let testFun4() = 
    // Test recursive values in expression position
    let rec recValNeverUsedAtRuntime = recFuncIgnoresFirstArg (fun _ -> recValNeverUsedAtRuntime) 1
    and recFuncIgnoresFirstArg g v = v

    recValNeverUsedAtRuntime

type ClassWithImplicitConstructor(compiledAsArg: int) = 
    inherit obj()
    let compiledAsField = 1
    let compiledAsLocal = 1
    let compiledAsLocal2 = compiledAsLocal + compiledAsLocal
    let compiledAsInstanceMethod () = compiledAsField + compiledAsField
    let compiledAsGenericInstanceMethod x = x

    static let compiledAsStaticField = 1
    static let compiledAsStaticLocal = 1
    static let compiledAsStaticLocal2 = compiledAsStaticLocal + compiledAsStaticLocal
    static let compiledAsStaticMethod () = compiledAsStaticField + compiledAsStaticField
    static let compiledAsGenericStaticMethod x = x

    member __.M1() = compiledAsField + compiledAsGenericInstanceMethod compiledAsField + compiledAsArg
    member __.M2() = compiledAsInstanceMethod()
    static member SM1() = compiledAsStaticField + compiledAsGenericStaticMethod compiledAsStaticField 
    static member SM2() = compiledAsStaticMethod()
    override __.ToString() = base.ToString() + string 999
    member this.TestCallinToString() = this.ToString()

exception Error of int * int

let err = Error(3,4)

let matchOnException err = match err with Error(a,b) -> 3  | e -> raise e

let upwardForLoop () = 
    let mutable a = 1
    for i in 0 .. 10 do a <- a + 1
    a

let upwardForLoop2 () = 
    let mutable a = 1
    for i = 0 to 10 do a <- a + 1
    a

let downwardForLoop () = 
    let mutable a = 1
    for i = 10 downto 1 do a <- a + 1
    a

let quotationTest1() =  <@ 1 + 1 @>
let quotationTest2 v =  <@ %v + 1 @>

type RecdType = { Field1: int; Field2: int }
type UnionType = Case1 of int | Case2 | Case3 of int * string 

type ClassWithEventsAndProperties() = 
    let ev = new Event<_>()
    static let sev = new Event<_>()
    member x.InstanceProperty = ev.Trigger(1); 1
    static member StaticProperty = sev.Trigger(1); 1
    member x.InstanceEvent = ev.Publish
    member x.StaticEvent = sev.Publish

let c = ClassWithEventsAndProperties()
let v = c.InstanceProperty

System.Console.WriteLine("777") // do a top-levl action

let functionWithSubmsumption(x:obj)  =  x :?> string
let functionWithCoercion(x:string)  =  (x :> obj) :?> string |> functionWithSubmsumption |> functionWithSubmsumption

type MultiArgMethods(c:int,d:int) = 
   member x.Method(a:int, b : int) = 1
   member x.CurriedMethod(a1:int, b1: int)  (a2:int, b2:int) = 1

let testFunctionThatCallsMultiArgMethods() = 
    let m = MultiArgMethods(3,4)
    (m.Method(7,8) + m.CurriedMethod (9,10) (11,12))

let functionThatUsesObjectExpression() = 
   { new obj() with  member x.ToString() = string 888 } 

let functionThatUsesObjectExpressionWithInterfaceImpl() = 
   { new obj() with  
       member x.ToString() = string 888 
     interface System.IComparable with 
       member x.CompareTo(y:obj) = 0 } 

let testFunctionThatUsesUnitsOfMeasure (x : float<_>) (y: float<_>) = x + y

let testFunctionThatUsesAddressesAndByrefs (x: byref<int>) = 
    let mutable w = 4
    let y1 = &x  // address-of
    let y2 = &w  // address-of
    let arr = [| 3;4 |]  // address-of
    let r = ref 3  // address-of
    let y3 = &arr.[0] // address-of array
    let y4 = &r.contents // address-of field
    let z = x + y1 + y2 + y3 // dereference      
    w <- 3 // assign to pointer
    x <- 4 // assign to byref
    y2 <- 4 // assign to byref
    y3 <- 5 // assign to byref
    z + x + y1 + y2 + y3 + y4 + arr.[0] + r.contents

let testFunctionThatUsesStructs1 (dt:System.DateTime) =  dt.AddDays(3.0)

let testFunctionThatUsesStructs2 () = 
   let dt1 = System.DateTime.Now
   let mutable dt2 = System.DateTime.Now
   let dt3 = dt1 - dt2
   let dt4 = dt1.AddDays(3.0)
   let dt5 = dt1.Millisecond
   let dt6 = &dt2
   let dt7 = dt6 - dt4
   dt7

let testFunctionThatUsesWhileLoop() = 
   let mutable x = 1
   while x  < 100 do
      x <- x + 1
   x

let testFunctionThatUsesTryWith() = 
   try 
     testFunctionThatUsesWhileLoop()
   with :? System.ArgumentException as e -> e.Message.Length


let testFunctionThatUsesTryFinally() = 
   try 
     testFunctionThatUsesWhileLoop()
   finally
     System.Console.WriteLine("8888")

type System.Console with
    static member WriteTwoLines() = System.Console.WriteLine(); System.Console.WriteLine()

type System.DateTime with
    member x.TwoMinute = x.Minute + x.Minute

let testFunctionThatUsesExtensionMembers() = 
   System.Console.WriteTwoLines()
   let v = System.DateTime.Now.TwoMinute
   System.Console.WriteTwoLines()

let testFunctionThatUsesOptionMembers() = 
   let x = Some(3)
   (x.IsSome, x.IsNone)

let testFunctionThatUsesOverAppliedFunction() = 
   id id 3

let testFunctionThatUsesPatternMatchingOnLists(x) = 
    match x with 
    | [] -> 1
    | [h] -> 2
    | [h;h2] -> 3
    | _ -> 4

let testFunctionThatUsesPatternMatchingOnOptions(x) = 
    match x with 
    | None -> 1
    | Some h -> 2 + h

let testFunctionThatUsesPatternMatchingOnOptions2(x) = 
    match x with 
    | None -> 1
    | Some _ -> 2

let testFunctionThatUsesConditionalOnOptions2(x: int option) = 
    if x.IsSome then 1 else 2

let f x y = x+y
let g = f 1
let h = (g 2) + 3

type TestFuncProp() =
    member this.Id = fun x -> x

let wrong = TestFuncProp().Id 0 = 0

let start (name:string) =
    name, name

let last (name:string, values:string ) =
    id (name, values)

let last2 (name:string) =
    id name

let test7(s:string) =
    start s |> last

let test8() =
    last

let test9(s:string) =
    (s,s) |> last

let test10() =
    last2

let test11(s:string) =
    s |> last2

let rec badLoop : (int -> int) =
    () // so that it is a function value
    fun x -> badLoop (x + 1)   

module LetLambda =
    let f =
        () // so that it is a function value
        fun a b -> a + b

let letLambdaRes = [ 1, 2 ] |> List.map (fun (a, b) -> LetLambda.f a b) 


    """
    File.WriteAllText(fileName1, fileSource1)

    let fileSource2 = """
module N

type IntAbbrev = int

let bool2 = false

let testHashChar (x:char) = hash x
let testHashSByte (x:sbyte) = hash x
let testHashInt16 (x:int16) = hash x
let testHashInt64 (x:int64) = hash x
let testHashUInt64 (x:uint64) = hash x
let testHashIntPtr (x:nativeint) = hash x
let testHashUIntPtr (x:unativeint) = hash x

let testHashString (x:string) = hash x
let testTypeOf (x:'T) = typeof<'T>

let testEqualsOperator               e1 e2 = (=) e1 e2
let testNotEqualsOperator            e1 e2 = (<>) e1 e2
let testLessThanOperator             e1 e2 = (<) e1 e2
let testLessThanOrEqualsOperator     e1 e2 = (<=) e1 e2
let testGreaterThanOperator          e1 e2 = (>) e1 e2
let testGreaterThanOrEqualsOperator  e1 e2 = (>=) e1 e2

let testAdditionOperator     e1 e2 = (+) e1 e2
let testSubtractionOperator  e1 e2 = (-) e1 e2
let testMultiplyOperator     e1 e2 = (*) e1 e2
let testDivisionOperator     e1 e2 = (/) e1 e2
let testModulusOperator      e1 e2 = (%) e1 e2
let testBitwiseAndOperator   e1 e2 = (&&&) e1 e2
let testBitwiseOrOperator    e1 e2 = (|||) e1 e2
let testBitwiseXorOperator   e1 e2 = (^^^) e1 e2
let testShiftLeftOperator    e1 e2 = (<<<) e1 e2
let testShiftRightOperator   e1 e2 = (>>>) e1 e2

let testUnaryNegOperator   e1 = (~-) e1
let testUnaryNotOperator   e1 = not e1

let testAdditionChecked    e1 e2 = Checked.(+) e1 e2
let testSubtractionChecked e1 e2 = Checked.(-) e1 e2
let testMultiplyChecked    e1 e2 = Checked.(*) e1 e2
let testUnaryNegChecked    e1 = Checked.(~-) e1

let testToByteChecked      e1 = Checked.byte e1
let testToSByteChecked     e1 = Checked.sbyte e1
let testToInt16Checked     e1 = Checked.int16 e1
let testToUInt16Checked    e1 = Checked.uint16 e1
let testToIntChecked       e1 = Checked.int e1
let testToInt32Checked     e1 = Checked.int32 e1
let testToUInt32Checked    e1 = Checked.uint32 e1
let testToInt64Checked     e1 = Checked.int64 e1
let testToUInt64Checked    e1 = Checked.uint64 e1
let testToIntPtrChecked    e1 = Checked.nativeint e1
let testToUIntPtrChecked   e1 = Checked.unativeint e1

let testToByteOperator     e1 = byte e1
let testToSByteOperator    e1 = sbyte e1
let testToInt16Operator    e1 = int16 e1
let testToUInt16Operator   e1 = uint16 e1
let testToIntOperator      e1 = int e1
let testToInt32Operator    e1 = int32 e1
let testToUInt32Operator   e1 = uint32 e1
let testToInt64Operator    e1 = int64 e1
let testToUInt64Operator   e1 = uint64 e1
let testToIntPtrOperator   e1 = nativeint e1
let testToUIntPtrOperator  e1 = unativeint e1
let testToSingleOperator   e1 = float32 e1
let testToDoubleOperator   e1 = float e1
let testToDecimalOperator  e1 = decimal e1
let testToCharOperator     e1 = char e1
let testToStringOperator   e1 = string e1

let testByteToByte    (x:byte) = byte x
let testByteToSByte   (x:byte) = sbyte x
let testByteToInt16   (x:byte) = int16 x
let testByteToUInt16  (x:byte) = uint16 x
let testByteToInt32   (x:byte) = int x
let testByteToUInt32  (x:byte) = uint32 x
let testByteToInt64   (x:byte) = int64 x
let testByteToUInt64  (x:byte) = uint64 x
let testByteToIntPtr  (x:byte) = nativeint x
let testByteToUIntPtr (x:byte) = unativeint x
let testByteToSingle  (x:byte) = float32 x
let testByteToDouble  (x:byte) = float x
let testByteToDecimal (x:byte) = decimal x
let testByteToChar    (x:byte) = char x
let testByteToString  (x:byte) = string x

let testSByteToByte    (x:sbyte) = byte x
let testSByteToSByte   (x:sbyte) = sbyte x
let testSByteToInt16   (x:sbyte) = int16 x
let testSByteToUInt16  (x:sbyte) = uint16 x
let testSByteToInt32   (x:sbyte) = int x
let testSByteToUInt32  (x:sbyte) = uint32 x
let testSByteToInt64   (x:sbyte) = int64 x
let testSByteToUInt64  (x:sbyte) = uint64 x
let testSByteToIntPtr  (x:sbyte) = nativeint x
let testSByteToUIntPtr (x:sbyte) = unativeint x
let testSByteToSingle  (x:sbyte) = float32 x
let testSByteToDouble  (x:sbyte) = float x
let testSByteToDecimal (x:sbyte) = decimal x
let testSByteToChar    (x:sbyte) = char x
let testSByteToString  (x:sbyte) = string x

let testInt16ToByte    (x:int16) = byte x
let testInt16ToSByte   (x:int16) = sbyte x
let testInt16ToInt16   (x:int16) = int16 x
let testInt16ToUInt16  (x:int16) = uint16 x
let testInt16ToInt32   (x:int16) = int x
let testInt16ToUInt32  (x:int16) = uint32 x
let testInt16ToInt64   (x:int16) = int64 x
let testInt16ToUInt64  (x:int16) = uint64 x
let testInt16ToIntPtr  (x:int16) = nativeint x
let testInt16ToUIntPtr (x:int16) = unativeint x
let testInt16ToSingle  (x:int16) = float32 x
let testInt16ToDouble  (x:int16) = float x
let testInt16ToDecimal (x:int16) = decimal x
let testInt16ToChar    (x:int16) = char x
let testInt16ToString  (x:int16) = string x

let testUInt16ToByte    (x:uint16) = byte x
let testUInt16ToSByte   (x:uint16) = sbyte x
let testUInt16ToInt16   (x:uint16) = int16 x
let testUInt16ToUInt16  (x:uint16) = uint16 x
let testUInt16ToInt32   (x:uint16) = int x
let testUInt16ToUInt32  (x:uint16) = uint32 x
let testUInt16ToInt64   (x:uint16) = int64 x
let testUInt16ToUInt64  (x:uint16) = uint64 x
let testUInt16ToIntPtr  (x:uint16) = nativeint x
let testUInt16ToUIntPtr (x:uint16) = unativeint x
let testUInt16ToSingle  (x:uint16) = float32 x
let testUInt16ToDouble  (x:uint16) = float x
let testUInt16ToDecimal (x:uint16) = decimal x
let testUInt16ToChar    (x:uint16) = char x
let testUInt16ToString  (x:uint16) = string x

let testInt32ToByte    (x:int) = byte x
let testInt32ToSByte   (x:int) = sbyte x
let testInt32ToInt16   (x:int) = int16 x
let testInt32ToUInt16  (x:int) = uint16 x
let testInt32ToInt32   (x:int) = int x
let testInt32ToUInt32  (x:int) = uint32 x
let testInt32ToInt64   (x:int) = int64 x
let testInt32ToUInt64  (x:int) = uint64 x
let testInt32ToIntPtr  (x:int) = nativeint x
let testInt32ToUIntPtr (x:int) = unativeint x
let testInt32ToSingle  (x:int) = float32 x
let testInt32ToDouble  (x:int) = float x
let testInt32ToDecimal (x:int) = decimal x
let testInt32ToChar    (x:int) = char x
let testInt32ToString  (x:int) = string x

let testUInt32ToByte    (x:uint32) = byte x
let testUInt32ToSByte   (x:uint32) = sbyte x
let testUInt32ToInt16   (x:uint32) = int16 x
let testUInt32ToUInt16  (x:uint32) = uint16 x
let testUInt32ToInt32   (x:uint32) = int x
let testUInt32ToUInt32  (x:uint32) = uint32 x
let testUInt32ToInt64   (x:uint32) = int64 x
let testUInt32ToUInt64  (x:uint32) = uint64 x
let testUInt32ToIntPtr  (x:uint32) = nativeint x
let testUInt32ToUIntPtr (x:uint32) = unativeint x
let testUInt32ToSingle  (x:uint32) = float32 x
let testUInt32ToDouble  (x:uint32) = float x
let testUInt32ToDecimal (x:uint32) = decimal x
let testUInt32ToChar    (x:uint32) = char x
let testUInt32ToString  (x:uint32) = string x

let testInt64ToByte    (x:int64) = byte x
let testInt64ToSByte   (x:int64) = sbyte x
let testInt64ToInt16   (x:int64) = int16 x
let testInt64ToUInt16  (x:int64) = uint16 x
let testInt64ToInt32   (x:int64) = int x
let testInt64ToUInt32  (x:int64) = uint32 x
let testInt64ToInt64   (x:int64) = int64 x
let testInt64ToUInt64  (x:int64) = uint64 x
let testInt64ToIntPtr  (x:int64) = nativeint x
let testInt64ToUIntPtr (x:int64) = unativeint x
let testInt64ToSingle  (x:int64) = float32 x
let testInt64ToDouble  (x:int64) = float x
let testInt64ToDecimal (x:int64) = decimal x
let testInt64ToChar    (x:int64) = char x
let testInt64ToString  (x:int64) = string x

let testUInt64ToByte    (x:uint64) = byte x
let testUInt64ToSByte   (x:uint64) = sbyte x
let testUInt64ToInt16   (x:uint64) = int16 x
let testUInt64ToUInt16  (x:uint64) = uint16 x
let testUInt64ToInt32   (x:uint64) = int x
let testUInt64ToUInt32  (x:uint64) = uint32 x
let testUInt64ToInt64   (x:uint64) = int64 x
let testUInt64ToUInt64  (x:uint64) = uint64 x
let testUInt64ToIntPtr  (x:uint64) = nativeint x
let testUInt64ToUIntPtr (x:uint64) = unativeint x
let testUInt64ToSingle  (x:uint64) = float32 x
let testUInt64ToDouble  (x:uint64) = float x
let testUInt64ToDecimal (x:uint64) = decimal x
let testUInt64ToChar    (x:uint64) = char x
let testUInt64ToString  (x:uint64) = string x

let testIntPtrToByte    (x:nativeint) = byte x
let testIntPtrToSByte   (x:nativeint) = sbyte x
let testIntPtrToInt16   (x:nativeint) = int16 x
let testIntPtrToUInt16  (x:nativeint) = uint16 x
let testIntPtrToInt32   (x:nativeint) = int x
let testIntPtrToUInt32  (x:nativeint) = uint32 x
let testIntPtrToInt64   (x:nativeint) = int64 x
let testIntPtrToUInt64  (x:nativeint) = uint64 x
let testIntPtrToIntPtr  (x:nativeint) = nativeint x
let testIntPtrToUIntPtr (x:nativeint) = unativeint x
let testIntPtrToSingle  (x:nativeint) = float32 x
let testIntPtrToDouble  (x:nativeint) = float x
let testIntPtrToDecimal (x:nativeint) = decimal x
let testIntPtrToChar    (x:nativeint) = char x
let testIntPtrToString  (x:nativeint) = string x

let testUIntPtrToByte    (x:unativeint) = byte x
let testUIntPtrToSByte   (x:unativeint) = sbyte x
let testUIntPtrToInt16   (x:unativeint) = int16 x
let testUIntPtrToUInt16  (x:unativeint) = uint16 x
let testUIntPtrToInt32   (x:unativeint) = int x
let testUIntPtrToUInt32  (x:unativeint) = uint32 x
let testUIntPtrToInt64   (x:unativeint) = int64 x
let testUIntPtrToUInt64  (x:unativeint) = uint64 x
let testUIntPtrToIntPtr  (x:unativeint) = nativeint x
let testUIntPtrToUIntPtr (x:unativeint) = unativeint x
let testUIntPtrToSingle  (x:unativeint) = float32 x
let testUIntPtrToDouble  (x:unativeint) = float x
let testUIntPtrToDecimal (x:unativeint) = decimal x
let testUIntPtrToChar    (x:unativeint) = char x
let testUIntPtrToString  (x:unativeint) = string x

let testSingleToByte    (x:float32) = byte x
let testSingleToSByte   (x:float32) = sbyte x
let testSingleToInt16   (x:float32) = int16 x
let testSingleToUInt16  (x:float32) = uint16 x
let testSingleToInt32   (x:float32) = int x
let testSingleToUInt32  (x:float32) = uint32 x
let testSingleToInt64   (x:float32) = int64 x
let testSingleToUInt64  (x:float32) = uint64 x
let testSingleToIntPtr  (x:float32) = nativeint x
let testSingleToUIntPtr (x:float32) = unativeint x
let testSingleToSingle  (x:float32) = float32 x
let testSingleToDouble  (x:float32) = float x
let testSingleToDecimal (x:float32) = decimal x
let testSingleToChar    (x:float32) = char x
let testSingleToString  (x:float32) = string x

let testDoubleToByte    (x:float) = byte x
let testDoubleToSByte   (x:float) = sbyte x
let testDoubleToInt16   (x:float) = int16 x
let testDoubleToUInt16  (x:float) = uint16 x
let testDoubleToInt32   (x:float) = int x
let testDoubleToUInt32  (x:float) = uint32 x
let testDoubleToInt64   (x:float) = int64 x
let testDoubleToUInt64  (x:float) = uint64 x
let testDoubleToIntPtr  (x:float) = nativeint x
let testDoubleToUIntPtr (x:float) = unativeint x
let testDoubleToSingle  (x:float) = float32 x
let testDoubleToDouble  (x:float) = float x
let testDoubleToDecimal (x:float) = decimal x
let testDoubleToChar    (x:float) = char x
let testDoubleToString  (x:float) = string x

let testDecimalToByte    (x:decimal) = byte x
let testDecimalToSByte   (x:decimal) = sbyte x
let testDecimalToInt16   (x:decimal) = int16 x
let testDecimalToUInt16  (x:decimal) = uint16 x
let testDecimalToInt32   (x:decimal) = int x
let testDecimalToUInt32  (x:decimal) = uint32 x
let testDecimalToInt64   (x:decimal) = int64 x
let testDecimalToUInt64  (x:decimal) = uint64 x
let testDecimalToSingle  (x:decimal) = float32 x
let testDecimalToDouble  (x:decimal) = float x
let testDecimalToDecimal (x:decimal) = decimal x
let testDecimalToChar    (x:decimal) = char x
let testDecimalToString  (x:decimal) = string x

let testCharToByte    (x:char) = byte x
let testCharToSByte   (x:char) = sbyte x
let testCharToInt16   (x:char) = int16 x
let testCharToUInt16  (x:char) = uint16 x
let testCharToInt32   (x:char) = int x
let testCharToUInt32  (x:char) = uint32 x
let testCharToInt64   (x:char) = int64 x
let testCharToUInt64  (x:char) = uint64 x
let testCharToIntPtr  (x:char) = nativeint x
let testCharToUIntPtr (x:char) = unativeint x
let testCharToSingle  (x:char) = float32 x
let testCharToDouble  (x:char) = float x
let testCharToChar    (x:char) = char x
let testCharToString  (x:char) = string x

let testStringToByte    (x:string) = byte x
let testStringToSByte   (x:string) = sbyte x
let testStringToInt16   (x:string) = int16 x
let testStringToUInt16  (x:string) = uint16 x
let testStringToInt32   (x:string) = int x
let testStringToUInt32  (x:string) = uint32 x
let testStringToInt64   (x:string) = int64 x
let testStringToUInt64  (x:string) = uint64 x
let testStringToSingle  (x:string) = float32 x
let testStringToDouble  (x:string) = float x
let testStringToDecimal (x:string) = decimal x
let testStringToChar    (x:string) = char x
let testStringToString  (x:string) = string x

    """
    File.WriteAllText(fileName2, fileSource2)

    let fileNames = [fileName1; fileName2]
    let args = mkProjectCommandLineArgs (dllName, fileNames)
    let options =  checker.GetProjectOptionsFromCommandLineArgs (projFileName, args)

//<@ let x = Some(3) in x.IsSome @>
[<Test>]
let ``Test Declarations project1`` () =
    let wholeProjectResults = exprChecker.ParseAndCheckProject(Project1.options) |> Async.RunSynchronously

    for e in wholeProjectResults.Errors do 
        printfn "Project1 error: <<<%s>>>" e.Message

    wholeProjectResults.Errors.Length |> shouldEqual 3 // recursive value warning
    wholeProjectResults.Errors.[0].Severity |> shouldEqual FSharpErrorSeverity.Warning
    wholeProjectResults.Errors.[1].Severity |> shouldEqual FSharpErrorSeverity.Warning
    wholeProjectResults.Errors.[2].Severity |> shouldEqual FSharpErrorSeverity.Warning

    wholeProjectResults.AssemblyContents.ImplementationFiles.Length |> shouldEqual 2
    let file1 = wholeProjectResults.AssemblyContents.ImplementationFiles.[0]
    let file2 = wholeProjectResults.AssemblyContents.ImplementationFiles.[1]

    // This behaves slightly differently on Mono versions, 'null' is printed somethimes, 'None' other times
    // Presumably this is very small differences in Mono reflection causing F# printing to change behavious
    // For now just disabling this test. See https://github.com/fsharp/FSharp.Compiler.Service/pull/766
    let filterHack l = 
        l |> List.map (fun (s:string) -> 
            s.Replace("ILArrayShape [(Some 0, None)]", "ILArrayShapeFIX")
             .Replace("ILArrayShape [(Some 0, null)]", "ILArrayShapeFIX"))

    let expected = 
          ["type M"; "type IntAbbrev"; "let boolEx1 = True @ (6,14--6,18)";
           "let intEx1 = 1 @ (7,13--7,14)"; "let int64Ex1 = 1 @ (8,15--8,17)";
           "let tupleEx1 = (1,1) @ (9,16--9,21)";
           "let tupleEx2 = (1,1,1) @ (10,16--10,25)";
           "let tupleEx3 = (1,1,1,1) @ (11,16--11,29)";
           "let localExample = let y: Microsoft.FSharp.Core.int = 1 in let z: Microsoft.FSharp.Core.int = 1 in (y,z) @ (14,7--14,8)";
           "let localGenericFunctionExample(unitVar0) = let y: Microsoft.FSharp.Core.int = 1 in let compiledAsLocalGenericFunction: 'a -> 'a = FUN ... -> fun x -> x in (compiledAsLocalGenericFunction<Microsoft.FSharp.Core.int> y,compiledAsLocalGenericFunction<Microsoft.FSharp.Core.float> 1) @ (19,7--19,8)";
           "let funcEx1(x) = x @ (23,23--23,24)";
           "let genericFuncEx1(x) = x @ (24,29--24,30)";
           "let topPair1b = M.patternInput@25 ().Item1 @ (25,4--25,26)";
           "let topPair1a = M.patternInput@25 ().Item0 @ (25,4--25,26)";
           "let testILCall1 = new Object() @ (27,18--27,27)";
           "let testILCall2 = Console.WriteLine (\"176\") @ (28,18--28,49)";
           "let recValNeverUsedAtRuntime = recValNeverUsedAtRuntime@31.Force<Microsoft.FSharp.Core.int>(()) @ (31,8--31,32)";
           "let recFuncIgnoresFirstArg(g) (v) = v @ (32,33--32,34)";
           "let testFun4(unitVar0) = let rec ... in recValNeverUsedAtRuntime @ (36,4--39,28)";
           "type ClassWithImplicitConstructor";
           "member .ctor(compiledAsArg) = (new Object(); (this.compiledAsArg <- compiledAsArg; (this.compiledAsField <- 1; let compiledAsLocal: Microsoft.FSharp.Core.int = 1 in let compiledAsLocal2: Microsoft.FSharp.Core.int = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (compiledAsLocal,compiledAsLocal) in ()))) @ (41,5--41,33)";
           "member .cctor(unitVar) = (compiledAsStaticField <- 1; let compiledAsStaticLocal: Microsoft.FSharp.Core.int = 1 in let compiledAsStaticLocal2: Microsoft.FSharp.Core.int = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (compiledAsStaticLocal,compiledAsStaticLocal) in ()) @ (49,11--49,40)";
           "member M1(__) (unitVar1) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (__.compiledAsField,let x: Microsoft.FSharp.Core.int = __.compiledAsField in __.compiledAsGenericInstanceMethod<Microsoft.FSharp.Core.int>(x)),__.compiledAsArg) @ (55,21--55,102)";
           "member M2(__) (unitVar1) = __.compiledAsInstanceMethod(()) @ (56,21--56,47)";
           "member SM1(unitVar0) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (compiledAsStaticField,let x: Microsoft.FSharp.Core.int = compiledAsStaticField in ClassWithImplicitConstructor.compiledAsGenericStaticMethod<Microsoft.FSharp.Core.int> (x)) @ (57,26--57,101)";
           "member SM2(unitVar0) = ClassWithImplicitConstructor.compiledAsStaticMethod (()) @ (58,26--58,50)";
           "member ToString(__) (unitVar1) = Operators.op_Addition<Microsoft.FSharp.Core.string,Microsoft.FSharp.Core.string,Microsoft.FSharp.Core.string> (base.ToString(),Operators.ToString<Microsoft.FSharp.Core.int> (999)) @ (59,29--59,57)";
           "member TestCallinToString(this) (unitVar1) = this.ToString() @ (60,39--60,54)";
           "type Error"; "let err = {Data0 = 3; Data1 = 4} @ (64,10--64,20)";
           "let matchOnException(err) = match (if err :? M.Error then $0 else $1) targets ... @ (66,33--66,36)";
           "let upwardForLoop(unitVar0) = let mutable a: Microsoft.FSharp.Core.int = 1 in (for-loop; a) @ (69,16--69,17)";
           "let upwardForLoop2(unitVar0) = let mutable a: Microsoft.FSharp.Core.int = 1 in (for-loop; a) @ (74,16--74,17)";
           "let downwardForLoop(unitVar0) = let mutable a: Microsoft.FSharp.Core.int = 1 in (for-loop; a) @ (79,16--79,17)";
           "let quotationTest1(unitVar0) = quote(Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (1,1)) @ (83,24--83,35)";
           "let quotationTest2(v) = quote(Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (ExtraTopLevelOperators.SpliceExpression<Microsoft.FSharp.Core.int> (v),1)) @ (84,24--84,36)";
           "type RecdType"; "type UnionType"; "type ClassWithEventsAndProperties";
           "member .ctor(unitVar0) = (new Object(); (this.ev <- new FSharpEvent`1(()); ())) @ (89,5--89,33)";
           "member .cctor(unitVar) = (sev <- new FSharpEvent`1(()); ()) @ (91,11--91,35)";
           "member get_InstanceProperty(x) (unitVar1) = (x.ev.Trigger(1); 1) @ (92,32--92,48)";
           "member get_StaticProperty(unitVar0) = (sev.Trigger(1); 1) @ (93,35--93,52)";
           "member get_InstanceEvent(x) (unitVar1) = x.ev.get_Publish(()) @ (94,29--94,39)";
           "member get_StaticEvent(x) (unitVar1) = sev.get_Publish(()) @ (95,27--95,38)";
           "let c = new ClassWithEventsAndProperties(()) @ (97,8--97,38)";
           "let v = M.c ().get_InstanceProperty(()) @ (98,8--98,26)";
           "do Console.WriteLine (\"777\")";
           "let functionWithSubmsumption(x) = IntrinsicFunctions.UnboxGeneric<Microsoft.FSharp.Core.string> (x) @ (102,40--102,52)";
           "let functionWithCoercion(x) = Operators.op_PipeRight<Microsoft.FSharp.Core.string,Microsoft.FSharp.Core.string> (Operators.op_PipeRight<Microsoft.FSharp.Core.string,Microsoft.FSharp.Core.string> (IntrinsicFunctions.UnboxGeneric<Microsoft.FSharp.Core.string> (x :> Microsoft.FSharp.Core.obj),fun x -> M.functionWithSubmsumption (x :> Microsoft.FSharp.Core.obj)),fun x -> M.functionWithSubmsumption (x :> Microsoft.FSharp.Core.obj)) @ (103,39--103,116)";
           "type MultiArgMethods";
           "member .ctor(c,d) = (new Object(); ()) @ (105,5--105,20)";
           "member Method(x) (a,b) = 1 @ (106,37--106,38)";
           "member CurriedMethod(x) (a1,b1) (a2,b2) = 1 @ (107,63--107,64)";
           "let testFunctionThatCallsMultiArgMethods(unitVar0) = let m: M.MultiArgMethods = new MultiArgMethods(3,4) in Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (m.Method(7,8),fun tupledArg -> let arg00: Microsoft.FSharp.Core.int = tupledArg.Item0 in let arg01: Microsoft.FSharp.Core.int = tupledArg.Item1 in fun tupledArg -> let arg10: Microsoft.FSharp.Core.int = tupledArg.Item0 in let arg11: Microsoft.FSharp.Core.int = tupledArg.Item1 in m.CurriedMethod(arg00,arg01,arg10,arg11) (9,10) (11,12)) @ (110,8--110,9)";
           "let functionThatUsesObjectExpression(unitVar0) = { new Object() with member x.ToString(unitVar1) = Operators.ToString<Microsoft.FSharp.Core.int> (888)  } @ (114,3--114,55)";
           "let functionThatUsesObjectExpressionWithInterfaceImpl(unitVar0) = { new Object() with member x.ToString(unitVar1) = Operators.ToString<Microsoft.FSharp.Core.int> (888) interface System.IComparable with member x.CompareTo(y) = 0 } :> System.IComparable @ (117,3--120,38)";
           "let testFunctionThatUsesUnitsOfMeasure(x) (y) = Operators.op_Addition<Microsoft.FSharp.Core.float<'u>,Microsoft.FSharp.Core.float<'u>,Microsoft.FSharp.Core.float<'u>> (x,y) @ (122,70--122,75)";
           "let testFunctionThatUsesAddressesAndByrefs(x) = let mutable w: Microsoft.FSharp.Core.int = 4 in let y1: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = x in let y2: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = &w in let arr: Microsoft.FSharp.Core.int Microsoft.FSharp.Core.[] = [|3; 4|] in let r: Microsoft.FSharp.Core.int Microsoft.FSharp.Core.ref = Operators.Ref<Microsoft.FSharp.Core.int> (3) in let y3: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = [I_ldelema (NormalAddress,false,ILArrayShape [(Some 0, None)],TypeVar 0us)](arr,0) in let y4: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = &r.contents in let z: Microsoft.FSharp.Core.int = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x,y1),y2),y3) in (w <- 3; (x <- 4; (y2 <- 4; (y3 <- 5; Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (z,x),y1),y2),y3),y4),IntrinsicFunctions.GetArray<Microsoft.FSharp.Core.int> (arr,0)),r.contents))))) @ (125,16--125,17)";
           "let testFunctionThatUsesStructs1(dt) = dt.AddDays(3) @ (139,57--139,72)";
           "let testFunctionThatUsesStructs2(unitVar0) = let dt1: System.DateTime = DateTime.get_Now () in let mutable dt2: System.DateTime = DateTime.get_Now () in let dt3: System.TimeSpan = Operators.op_Subtraction<System.DateTime,System.DateTime,System.TimeSpan> (dt1,dt2) in let dt4: System.DateTime = dt1.AddDays(3) in let dt5: Microsoft.FSharp.Core.int = dt1.get_Millisecond() in let dt6: Microsoft.FSharp.Core.byref<System.DateTime> = &dt2 in let dt7: System.TimeSpan = Operators.op_Subtraction<System.DateTime,System.DateTime,System.TimeSpan> (dt6,dt4) in dt7 @ (142,7--142,10)";
           "let testFunctionThatUsesWhileLoop(unitVar0) = let mutable x: Microsoft.FSharp.Core.int = 1 in (while Operators.op_LessThan<Microsoft.FSharp.Core.int> (x,100) do x <- Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x,1) done; x) @ (152,15--152,16)";
           "let testFunctionThatUsesTryWith(unitVar0) = try M.testFunctionThatUsesWhileLoop (()) with matchValue -> match (if matchValue :? System.ArgumentException then $0 else $1) targets ... @ (158,3--160,60)";
           "let testFunctionThatUsesTryFinally(unitVar0) = try M.testFunctionThatUsesWhileLoop (()) finally Console.WriteLine (\"8888\") @ (164,3--167,37)";
           "member Console.WriteTwoLines.Static(unitVar0) = (Console.WriteLine (); Console.WriteLine ()) @ (170,36--170,90)";
           "member DateTime.get_TwoMinute(x) (unitVar1) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x.get_Minute(),x.get_Minute()) @ (173,25--173,44)";
           "let testFunctionThatUsesExtensionMembers(unitVar0) = (M.Console.WriteTwoLines.Static (()); let v: Microsoft.FSharp.Core.int = DateTime.get_Now ().DateTime.get_TwoMinute(()) in M.Console.WriteTwoLines.Static (())) @ (176,3--178,33)";
           "let testFunctionThatUsesOptionMembers(unitVar0) = let x: Microsoft.FSharp.Core.int Microsoft.FSharp.Core.option = Some(3) in (x.get_IsSome() (),x.get_IsNone() ()) @ (181,7--181,8)";
           "let testFunctionThatUsesOverAppliedFunction(unitVar0) = Operators.Identity<Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.int> (fun x -> Operators.Identity<Microsoft.FSharp.Core.int> (x)) 3 @ (185,3--185,10)";
           "let testFunctionThatUsesPatternMatchingOnLists(x) = match (if x.Isop_ColonColon then (if x.Tail.Isop_ColonColon then (if x.Tail.Tail.Isop_Nil then $2 else $3) else $1) else $0) targets ... @ (188,10--188,11)";
           "let testFunctionThatUsesPatternMatchingOnOptions(x) = match (if x.IsSome then $1 else $0) targets ... @ (195,10--195,11)";
           "let testFunctionThatUsesPatternMatchingOnOptions2(x) = match (if x.IsSome then $1 else $0) targets ... @ (200,10--200,11)";
           "let testFunctionThatUsesConditionalOnOptions2(x) = (if x.get_IsSome() () then 1 else 2) @ (205,4--205,29)";
           "let f(x) (y) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x,y) @ (207,12--207,15)";
           "let g = let x: Microsoft.FSharp.Core.int = 1 in fun y -> M.f (x,y) @ (208,8--208,11)";
           "let h = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (M.g () 2,3) @ (209,8--209,17)";
           "type TestFuncProp";
           "member .ctor(unitVar0) = (new Object(); ()) @ (211,5--211,17)";
           "member get_Id(this) (unitVar1) = fun x -> x @ (212,21--212,31)";
           "let wrong = Operators.op_Equality<Microsoft.FSharp.Core.int> (new TestFuncProp(()).get_Id(()) 0,0) @ (214,12--214,35)";
           "let start(name) = (name,name) @ (217,4--217,14)";
           "let last(name,values) = Operators.Identity<Microsoft.FSharp.Core.string * Microsoft.FSharp.Core.string> ((name,values)) @ (220,4--220,21)";
           "let last2(name) = Operators.Identity<Microsoft.FSharp.Core.string> (name) @ (223,4--223,11)";
           "let test7(s) = Operators.op_PipeRight<Microsoft.FSharp.Core.string * Microsoft.FSharp.Core.string,Microsoft.FSharp.Core.string * Microsoft.FSharp.Core.string> (M.start (s),fun tupledArg -> let name: Microsoft.FSharp.Core.string = tupledArg.Item0 in let values: Microsoft.FSharp.Core.string = tupledArg.Item1 in M.last (name,values)) @ (226,4--226,19)";
           "let test8(unitVar0) = fun tupledArg -> let name: Microsoft.FSharp.Core.string = tupledArg.Item0 in let values: Microsoft.FSharp.Core.string = tupledArg.Item1 in M.last (name,values) @ (229,4--229,8)";
           "let test9(s) = Operators.op_PipeRight<Microsoft.FSharp.Core.string * Microsoft.FSharp.Core.string,Microsoft.FSharp.Core.string * Microsoft.FSharp.Core.string> ((s,s),fun tupledArg -> let name: Microsoft.FSharp.Core.string = tupledArg.Item0 in let values: Microsoft.FSharp.Core.string = tupledArg.Item1 in M.last (name,values)) @ (232,4--232,17)";
           "let test10(unitVar0) = fun name -> M.last2 (name) @ (235,4--235,9)";
           "let test11(s) = Operators.op_PipeRight<Microsoft.FSharp.Core.string,Microsoft.FSharp.Core.string> (s,fun name -> M.last2 (name)) @ (238,4--238,14)";
           "let badLoop = badLoop@240.Force<Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.int>(()) @ (240,8--240,15)";
           "type LetLambda";
           "let f = ((); fun a -> fun b -> Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (a,b)) @ (246,8--247,24)";
           "let letLambdaRes = Operators.op_PipeRight<(Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int) Microsoft.FSharp.Collections.list,Microsoft.FSharp.Core.int Microsoft.FSharp.Collections.list> (Cons((1,2),Empty()),let mapping: Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.int = fun tupledArg -> let a: Microsoft.FSharp.Core.int = tupledArg.Item0 in let b: Microsoft.FSharp.Core.int = tupledArg.Item1 in (LetLambda.f () a) b in fun list -> ListModule.Map<Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (mapping,list)) @ (249,19--249,71)"] 

    let expected2 = 
      [ "type N"; "type IntAbbrev"; "let bool2 = False @ (6,12--6,17)";
        "let testHashChar(x) = Operators.Hash<Microsoft.FSharp.Core.char> (x) @ (8,28--8,34)";
        "let testHashSByte(x) = Operators.Hash<Microsoft.FSharp.Core.sbyte> (x) @ (9,30--9,36)";
        "let testHashInt16(x) = Operators.Hash<Microsoft.FSharp.Core.int16> (x) @ (10,30--10,36)";
        "let testHashInt64(x) = Operators.Hash<Microsoft.FSharp.Core.int64> (x) @ (11,30--11,36)";
        "let testHashUInt64(x) = Operators.Hash<Microsoft.FSharp.Core.uint64> (x) @ (12,32--12,38)";
        "let testHashIntPtr(x) = Operators.Hash<Microsoft.FSharp.Core.nativeint> (x) @ (13,35--13,41)";
        "let testHashUIntPtr(x) = Operators.Hash<Microsoft.FSharp.Core.unativeint> (x) @ (14,37--14,43)";
        "let testHashString(x) = Operators.Hash<Microsoft.FSharp.Core.string> (x) @ (16,32--16,38)";
        "let testTypeOf(x) = Operators.TypeOf<'T> () @ (17,24--17,30)";
        "let testEqualsOperator(e1) (e2) = Operators.op_Equality<'a> (e1,e2) @ (19,46--19,54)";
        "let testNotEqualsOperator(e1) (e2) = Operators.op_Inequality<'a> (e1,e2) @ (20,46--20,55)";
        "let testLessThanOperator(e1) (e2) = Operators.op_LessThan<'a> (e1,e2) @ (21,46--21,54)";
        "let testLessThanOrEqualsOperator(e1) (e2) = Operators.op_LessThanOrEqual<'a> (e1,e2) @ (22,46--22,55)";
        "let testGreaterThanOperator(e1) (e2) = Operators.op_GreaterThan<'a> (e1,e2) @ (23,46--23,54)";
        "let testGreaterThanOrEqualsOperator(e1) (e2) = Operators.op_GreaterThanOrEqual<'a> (e1,e2) @ (24,46--24,55)";
        "let testAdditionOperator(e1) (e2) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (26,38--26,46)";
        "let testSubtractionOperator(e1) (e2) = Operators.op_Subtraction<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (27,38--27,46)";
        "let testMultiplyOperator(e1) (e2) = Operators.op_Multiply<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (28,37--28,46)";
        "let testDivisionOperator(e1) (e2) = Operators.op_Division<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (29,38--29,46)";
        "let testModulusOperator(e1) (e2) = Operators.op_Modulus<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (30,38--30,46)";
        "let testBitwiseAndOperator(e1) (e2) = Operators.op_BitwiseAnd<Microsoft.FSharp.Core.int> (e1,e2) @ (31,38--31,48)";
        "let testBitwiseOrOperator(e1) (e2) = Operators.op_BitwiseOr<Microsoft.FSharp.Core.int> (e1,e2) @ (32,38--32,48)";
        "let testBitwiseXorOperator(e1) (e2) = Operators.op_ExclusiveOr<Microsoft.FSharp.Core.int> (e1,e2) @ (33,38--33,48)";
        "let testShiftLeftOperator(e1) (e2) = Operators.op_LeftShift<Microsoft.FSharp.Core.int> (e1,e2) @ (34,38--34,48)";
        "let testShiftRightOperator(e1) (e2) = Operators.op_RightShift<Microsoft.FSharp.Core.int> (e1,e2) @ (35,38--35,48)";
        "let testUnaryNegOperator(e1) = Operators.op_UnaryNegation<Microsoft.FSharp.Core.int> (e1) @ (37,33--37,39)";
        "let testUnaryNotOperator(e1) = Operators.Not (e1) @ (38,32--38,38)";
        "let testAdditionChecked(e1) (e2) = Checked.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (40,35--40,52)";
        "let testSubtractionChecked(e1) (e2) = Checked.op_Subtraction<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (41,35--41,52)";
        "let testMultiplyChecked(e1) (e2) = Checked.op_Multiply<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (42,35--42,52)";
        "let testUnaryNegChecked(e1) = Checked.op_UnaryNegation<Microsoft.FSharp.Core.int> (e1) @ (43,32--43,47)";
        "let testToByteChecked(e1) = Checked.ToByte<Microsoft.FSharp.Core.int> (e1) @ (45,32--45,47)";
        "let testToSByteChecked(e1) = Checked.ToSByte<Microsoft.FSharp.Core.int> (e1) @ (46,32--46,48)";
        "let testToInt16Checked(e1) = Checked.ToInt16<Microsoft.FSharp.Core.int> (e1) @ (47,32--47,48)";
        "let testToUInt16Checked(e1) = Checked.ToUInt16<Microsoft.FSharp.Core.int> (e1) @ (48,32--48,49)";
        "let testToIntChecked(e1) = Checked.ToInt<Microsoft.FSharp.Core.int> (e1) @ (49,32--49,46)";
        "let testToInt32Checked(e1) = Checked.ToInt32<Microsoft.FSharp.Core.int> (e1) @ (50,32--50,48)";
        "let testToUInt32Checked(e1) = Checked.ToUInt32<Microsoft.FSharp.Core.int> (e1) @ (51,32--51,49)";
        "let testToInt64Checked(e1) = Checked.ToInt64<Microsoft.FSharp.Core.int> (e1) @ (52,32--52,48)";
        "let testToUInt64Checked(e1) = Checked.ToUInt64<Microsoft.FSharp.Core.int> (e1) @ (53,32--53,49)";
        "let testToIntPtrChecked(e1) = Checked.ToIntPtr<Microsoft.FSharp.Core.int> (e1) @ (54,32--54,52)";
        "let testToUIntPtrChecked(e1) = Checked.ToUIntPtr<Microsoft.FSharp.Core.int> (e1) @ (55,32--55,53)";
        "let testToByteOperator(e1) = Operators.ToByte<Microsoft.FSharp.Core.int> (e1) @ (57,32--57,39)";
        "let testToSByteOperator(e1) = Operators.ToSByte<Microsoft.FSharp.Core.int> (e1) @ (58,32--58,40)";
        "let testToInt16Operator(e1) = Operators.ToInt16<Microsoft.FSharp.Core.int> (e1) @ (59,32--59,40)";
        "let testToUInt16Operator(e1) = Operators.ToUInt16<Microsoft.FSharp.Core.int> (e1) @ (60,32--60,41)";
        "let testToIntOperator(e1) = Operators.ToInt<Microsoft.FSharp.Core.int> (e1) @ (61,32--61,38)";
        "let testToInt32Operator(e1) = Operators.ToInt32<Microsoft.FSharp.Core.int> (e1) @ (62,32--62,40)";
        "let testToUInt32Operator(e1) = Operators.ToUInt32<Microsoft.FSharp.Core.int> (e1) @ (63,32--63,41)";
        "let testToInt64Operator(e1) = Operators.ToInt64<Microsoft.FSharp.Core.int> (e1) @ (64,32--64,40)";
        "let testToUInt64Operator(e1) = Operators.ToUInt64<Microsoft.FSharp.Core.int> (e1) @ (65,32--65,41)";
        "let testToIntPtrOperator(e1) = Operators.ToIntPtr<Microsoft.FSharp.Core.int> (e1) @ (66,32--66,44)";
        "let testToUIntPtrOperator(e1) = Operators.ToUIntPtr<Microsoft.FSharp.Core.int> (e1) @ (67,32--67,45)";
        "let testToSingleOperator(e1) = Operators.ToSingle<Microsoft.FSharp.Core.int> (e1) @ (68,32--68,42)";
        "let testToDoubleOperator(e1) = Operators.ToDouble<Microsoft.FSharp.Core.int> (e1) @ (69,32--69,40)";
        "let testToDecimalOperator(e1) = Operators.ToDecimal<Microsoft.FSharp.Core.int> (e1) @ (70,32--70,42)";
        "let testToCharOperator(e1) = Operators.ToChar<Microsoft.FSharp.Core.int> (e1) @ (71,32--71,39)";
        "let testToStringOperator(e1) = Operators.ToString<Microsoft.FSharp.Core.obj> (e1) @ (72,32--72,41)";
        "let testByteToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.byte> (x) @ (74,33--74,39)";
        "let testByteToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.byte> (x) @ (75,33--75,40)";
        "let testByteToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.byte> (x) @ (76,33--76,40)";
        "let testByteToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.byte> (x) @ (77,33--77,41)";
        "let testByteToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.byte> (x) @ (78,33--78,38)";
        "let testByteToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.byte> (x) @ (79,33--79,41)";
        "let testByteToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.byte> (x) @ (80,33--80,40)";
        "let testByteToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.byte> (x) @ (81,33--81,41)";
        "let testByteToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.byte> (x) @ (82,33--82,44)";
        "let testByteToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.byte> (x) @ (83,33--83,45)";
        "let testByteToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.byte> (x) @ (84,33--84,42)";
        "let testByteToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.byte> (x) @ (85,33--85,40)";
        "let testByteToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.byte> (x) @ (86,33--86,42)";
        "let testByteToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.byte> (x) @ (87,33--87,39)";
        "let testByteToString(x) = Operators.ToString<Microsoft.FSharp.Core.byte> (x) @ (88,33--88,41)";
        "let testSByteToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.sbyte> (x) @ (90,35--90,41)";
        "let testSByteToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.sbyte> (x) @ (91,35--91,42)";
        "let testSByteToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.sbyte> (x) @ (92,35--92,42)";
        "let testSByteToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.sbyte> (x) @ (93,35--93,43)";
        "let testSByteToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.sbyte> (x) @ (94,35--94,40)";
        "let testSByteToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.sbyte> (x) @ (95,35--95,43)";
        "let testSByteToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.sbyte> (x) @ (96,35--96,42)";
        "let testSByteToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.sbyte> (x) @ (97,35--97,43)";
        "let testSByteToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.sbyte> (x) @ (98,35--98,46)";
        "let testSByteToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.sbyte> (x) @ (99,35--99,47)";
        "let testSByteToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.sbyte> (x) @ (100,35--100,44)";
        "let testSByteToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.sbyte> (x) @ (101,35--101,42)";
        "let testSByteToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.sbyte> (x) @ (102,35--102,44)";
        "let testSByteToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.sbyte> (x) @ (103,35--103,41)";
        "let testSByteToString(x) = Operators.ToString<Microsoft.FSharp.Core.sbyte> (x) @ (104,35--104,43)";
        "let testInt16ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.int16> (x) @ (106,35--106,41)";
        "let testInt16ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.int16> (x) @ (107,35--107,42)";
        "let testInt16ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.int16> (x) @ (108,35--108,42)";
        "let testInt16ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int16> (x) @ (109,35--109,43)";
        "let testInt16ToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.int16> (x) @ (110,35--110,40)";
        "let testInt16ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.int16> (x) @ (111,35--111,43)";
        "let testInt16ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int16> (x) @ (112,35--112,42)";
        "let testInt16ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.int16> (x) @ (113,35--113,43)";
        "let testInt16ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int16> (x) @ (114,35--114,46)";
        "let testInt16ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.int16> (x) @ (115,35--115,47)";
        "let testInt16ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.int16> (x) @ (116,35--116,44)";
        "let testInt16ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.int16> (x) @ (117,35--117,42)";
        "let testInt16ToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.int16> (x) @ (118,35--118,44)";
        "let testInt16ToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.int16> (x) @ (119,35--119,41)";
        "let testInt16ToString(x) = Operators.ToString<Microsoft.FSharp.Core.int16> (x) @ (120,35--120,43)";
        "let testUInt16ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.uint16> (x) @ (122,37--122,43)";
        "let testUInt16ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.uint16> (x) @ (123,37--123,44)";
        "let testUInt16ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.uint16> (x) @ (124,37--124,44)";
        "let testUInt16ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint16> (x) @ (125,37--125,45)";
        "let testUInt16ToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.uint16> (x) @ (126,37--126,42)";
        "let testUInt16ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.uint16> (x) @ (127,37--127,45)";
        "let testUInt16ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.uint16> (x) @ (128,37--128,44)";
        "let testUInt16ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.uint16> (x) @ (129,37--129,45)";
        "let testUInt16ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.uint16> (x) @ (130,37--130,48)";
        "let testUInt16ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint16> (x) @ (131,37--131,49)";
        "let testUInt16ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.uint16> (x) @ (132,37--132,46)";
        "let testUInt16ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.uint16> (x) @ (133,37--133,44)";
        "let testUInt16ToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.uint16> (x) @ (134,37--134,46)";
        "let testUInt16ToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.uint16> (x) @ (135,37--135,43)";
        "let testUInt16ToString(x) = Operators.ToString<Microsoft.FSharp.Core.uint16> (x) @ (136,37--136,45)";
        "let testInt32ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.int> (x) @ (138,33--138,39)";
        "let testInt32ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.int> (x) @ (139,33--139,40)";
        "let testInt32ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.int> (x) @ (140,33--140,40)";
        "let testInt32ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int> (x) @ (141,33--141,41)";
        "let testInt32ToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.int> (x) @ (142,33--142,38)";
        "let testInt32ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.int> (x) @ (143,33--143,41)";
        "let testInt32ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int> (x) @ (144,33--144,40)";
        "let testInt32ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.int> (x) @ (145,33--145,41)";
        "let testInt32ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int> (x) @ (146,33--146,44)";
        "let testInt32ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.int> (x) @ (147,33--147,45)";
        "let testInt32ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.int> (x) @ (148,33--148,42)";
        "let testInt32ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.int> (x) @ (149,33--149,40)";
        "let testInt32ToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.int> (x) @ (150,33--150,42)";
        "let testInt32ToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.int> (x) @ (151,33--151,39)";
        "let testInt32ToString(x) = Operators.ToString<Microsoft.FSharp.Core.int> (x) @ (152,33--152,41)";
        "let testUInt32ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.uint32> (x) @ (154,37--154,43)";
        "let testUInt32ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.uint32> (x) @ (155,37--155,44)";
        "let testUInt32ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.uint32> (x) @ (156,37--156,44)";
        "let testUInt32ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint32> (x) @ (157,37--157,45)";
        "let testUInt32ToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.uint32> (x) @ (158,37--158,42)";
        "let testUInt32ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.uint32> (x) @ (159,37--159,45)";
        "let testUInt32ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.uint32> (x) @ (160,37--160,44)";
        "let testUInt32ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.uint32> (x) @ (161,37--161,45)";
        "let testUInt32ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.uint32> (x) @ (162,37--162,48)";
        "let testUInt32ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint32> (x) @ (163,37--163,49)";
        "let testUInt32ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.uint32> (x) @ (164,37--164,46)";
        "let testUInt32ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.uint32> (x) @ (165,37--165,44)";
        "let testUInt32ToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.uint32> (x) @ (166,37--166,46)";
        "let testUInt32ToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.uint32> (x) @ (167,37--167,43)";
        "let testUInt32ToString(x) = Operators.ToString<Microsoft.FSharp.Core.uint32> (x) @ (168,37--168,45)";
        "let testInt64ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.int64> (x) @ (170,35--170,41)";
        "let testInt64ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.int64> (x) @ (171,35--171,42)";
        "let testInt64ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.int64> (x) @ (172,35--172,42)";
        "let testInt64ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int64> (x) @ (173,35--173,43)";
        "let testInt64ToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.int64> (x) @ (174,35--174,40)";
        "let testInt64ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.int64> (x) @ (175,35--175,43)";
        "let testInt64ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int64> (x) @ (176,35--176,42)";
        "let testInt64ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.int64> (x) @ (177,35--177,43)";
        "let testInt64ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int64> (x) @ (178,35--178,46)";
        "let testInt64ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.int64> (x) @ (179,35--179,47)";
        "let testInt64ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.int64> (x) @ (180,35--180,44)";
        "let testInt64ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.int64> (x) @ (181,35--181,42)";
        "let testInt64ToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.int64> (x) @ (182,35--182,44)";
        "let testInt64ToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.int64> (x) @ (183,35--183,41)";
        "let testInt64ToString(x) = Operators.ToString<Microsoft.FSharp.Core.int64> (x) @ (184,35--184,43)";
        "let testUInt64ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.uint64> (x) @ (186,37--186,43)";
        "let testUInt64ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.uint64> (x) @ (187,37--187,44)";
        "let testUInt64ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.uint64> (x) @ (188,37--188,44)";
        "let testUInt64ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint64> (x) @ (189,37--189,45)";
        "let testUInt64ToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.uint64> (x) @ (190,37--190,42)";
        "let testUInt64ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.uint64> (x) @ (191,37--191,45)";
        "let testUInt64ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.uint64> (x) @ (192,37--192,44)";
        "let testUInt64ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.uint64> (x) @ (193,37--193,45)";
        "let testUInt64ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.uint64> (x) @ (194,37--194,48)";
        "let testUInt64ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint64> (x) @ (195,37--195,49)";
        "let testUInt64ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.uint64> (x) @ (196,37--196,46)";
        "let testUInt64ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.uint64> (x) @ (197,37--197,44)";
        "let testUInt64ToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.uint64> (x) @ (198,37--198,46)";
        "let testUInt64ToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.uint64> (x) @ (199,37--199,43)";
        "let testUInt64ToString(x) = Operators.ToString<Microsoft.FSharp.Core.uint64> (x) @ (200,37--200,45)";
        "let testIntPtrToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.nativeint> (x) @ (202,40--202,46)";
        "let testIntPtrToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.nativeint> (x) @ (203,40--203,47)";
        "let testIntPtrToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.nativeint> (x) @ (204,40--204,47)";
        "let testIntPtrToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.nativeint> (x) @ (205,40--205,48)";
        "let testIntPtrToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.nativeint> (x) @ (206,40--206,45)";
        "let testIntPtrToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.nativeint> (x) @ (207,40--207,48)";
        "let testIntPtrToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.nativeint> (x) @ (208,40--208,47)";
        "let testIntPtrToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.nativeint> (x) @ (209,40--209,48)";
        "let testIntPtrToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.nativeint> (x) @ (210,40--210,51)";
        "let testIntPtrToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.nativeint> (x) @ (211,40--211,52)";
        "let testIntPtrToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.nativeint> (x) @ (212,40--212,49)";
        "let testIntPtrToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.nativeint> (x) @ (213,40--213,47)";
        "let testIntPtrToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.nativeint> (x) @ (214,40--214,49)";
        "let testIntPtrToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.nativeint> (x) @ (215,40--215,46)";
        "let testIntPtrToString(x) = Operators.ToString<Microsoft.FSharp.Core.nativeint> (x) @ (216,40--216,48)";
        "let testUIntPtrToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.unativeint> (x) @ (218,42--218,48)";
        "let testUIntPtrToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.unativeint> (x) @ (219,42--219,49)";
        "let testUIntPtrToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.unativeint> (x) @ (220,42--220,49)";
        "let testUIntPtrToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.unativeint> (x) @ (221,42--221,50)";
        "let testUIntPtrToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.unativeint> (x) @ (222,42--222,47)";
        "let testUIntPtrToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.unativeint> (x) @ (223,42--223,50)";
        "let testUIntPtrToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.unativeint> (x) @ (224,42--224,49)";
        "let testUIntPtrToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.unativeint> (x) @ (225,42--225,50)";
        "let testUIntPtrToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.unativeint> (x) @ (226,42--226,53)";
        "let testUIntPtrToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.unativeint> (x) @ (227,42--227,54)";
        "let testUIntPtrToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.unativeint> (x) @ (228,42--228,51)";
        "let testUIntPtrToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.unativeint> (x) @ (229,42--229,49)";
        "let testUIntPtrToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.unativeint> (x) @ (230,42--230,51)";
        "let testUIntPtrToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.unativeint> (x) @ (231,42--231,48)";
        "let testUIntPtrToString(x) = Operators.ToString<Microsoft.FSharp.Core.unativeint> (x) @ (232,42--232,50)";
        "let testSingleToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.float32> (x) @ (234,38--234,44)";
        "let testSingleToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.float32> (x) @ (235,38--235,45)";
        "let testSingleToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.float32> (x) @ (236,38--236,45)";
        "let testSingleToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.float32> (x) @ (237,38--237,46)";
        "let testSingleToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.float32> (x) @ (238,38--238,43)";
        "let testSingleToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.float32> (x) @ (239,38--239,46)";
        "let testSingleToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.float32> (x) @ (240,38--240,45)";
        "let testSingleToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.float32> (x) @ (241,38--241,46)";
        "let testSingleToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.float32> (x) @ (242,38--242,49)";
        "let testSingleToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.float32> (x) @ (243,38--243,50)";
        "let testSingleToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float32> (x) @ (244,38--244,47)";
        "let testSingleToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float32> (x) @ (245,38--245,45)";
        "let testSingleToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.float32> (x) @ (246,38--246,47)";
        "let testSingleToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.float32> (x) @ (247,38--247,44)";
        "let testSingleToString(x) = Operators.ToString<Microsoft.FSharp.Core.float32> (x) @ (248,38--248,46)";
        "let testDoubleToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.float> (x) @ (250,36--250,42)";
        "let testDoubleToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.float> (x) @ (251,36--251,43)";
        "let testDoubleToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.float> (x) @ (252,36--252,43)";
        "let testDoubleToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.float> (x) @ (253,36--253,44)";
        "let testDoubleToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.float> (x) @ (254,36--254,41)";
        "let testDoubleToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.float> (x) @ (255,36--255,44)";
        "let testDoubleToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.float> (x) @ (256,36--256,43)";
        "let testDoubleToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.float> (x) @ (257,36--257,44)";
        "let testDoubleToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.float> (x) @ (258,36--258,47)";
        "let testDoubleToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.float> (x) @ (259,36--259,48)";
        "let testDoubleToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (x) @ (260,36--260,45)";
        "let testDoubleToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (x) @ (261,36--261,43)";
        "let testDoubleToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.float> (x) @ (262,36--262,45)";
        "let testDoubleToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.float> (x) @ (263,36--263,42)";
        "let testDoubleToString(x) = Operators.ToString<Microsoft.FSharp.Core.float> (x) @ (264,36--264,44)";
        "let testDecimalToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.decimal> (x) @ (266,39--266,45)";
        "let testDecimalToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.decimal> (x) @ (267,39--267,46)";
        "let testDecimalToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.decimal> (x) @ (268,39--268,46)";
        "let testDecimalToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.decimal> (x) @ (269,39--269,47)";
        "let testDecimalToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.decimal> (x) @ (270,39--270,44)";
        "let testDecimalToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.decimal> (x) @ (271,39--271,47)";
        "let testDecimalToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.decimal> (x) @ (272,39--272,46)";
        "let testDecimalToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.decimal> (x) @ (273,39--273,47)";
        "let testDecimalToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.decimal> (x) @ (274,39--274,48)";
        "let testDecimalToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.decimal> (x) @ (275,39--275,46)";
        "let testDecimalToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.decimal> (x) @ (276,39--276,48)";
        "let testDecimalToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.decimal> (x) @ (277,39--277,45)";
        "let testDecimalToString(x) = Operators.ToString<Microsoft.FSharp.Core.decimal> (x) @ (278,39--278,47)";
        "let testCharToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.char> (x) @ (280,33--280,39)";
        "let testCharToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.char> (x) @ (281,33--281,40)";
        "let testCharToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.char> (x) @ (282,33--282,40)";
        "let testCharToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.char> (x) @ (283,33--283,41)";
        "let testCharToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.char> (x) @ (284,33--284,38)";
        "let testCharToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.char> (x) @ (285,33--285,41)";
        "let testCharToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.char> (x) @ (286,33--286,40)";
        "let testCharToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.char> (x) @ (287,33--287,41)";
        "let testCharToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.char> (x) @ (288,33--288,44)";
        "let testCharToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.char> (x) @ (289,33--289,45)";
        "let testCharToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.char> (x) @ (290,33--290,42)";
        "let testCharToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.char> (x) @ (291,33--291,40)";
        "let testCharToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.char> (x) @ (292,33--292,39)";
        "let testCharToString(x) = Operators.ToString<Microsoft.FSharp.Core.char> (x) @ (293,33--293,41)";
        "let testStringToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.string> (x) @ (295,37--295,43)";
        "let testStringToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.string> (x) @ (296,37--296,44)";
        "let testStringToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.string> (x) @ (297,37--297,44)";
        "let testStringToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.string> (x) @ (298,37--298,45)";
        "let testStringToInt32(x) = Operators.ToInt<Microsoft.FSharp.Core.string> (x) @ (299,37--299,42)";
        "let testStringToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.string> (x) @ (300,37--300,45)";
        "let testStringToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.string> (x) @ (301,37--301,44)";
        "let testStringToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.string> (x) @ (302,37--302,45)";
        "let testStringToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.string> (x) @ (303,37--303,46)";
        "let testStringToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.string> (x) @ (304,37--304,44)";
        "let testStringToDecimal(x) = Operators.ToDecimal<Microsoft.FSharp.Core.string> (x) @ (305,37--305,46)";
        "let testStringToChar(x) = Operators.ToChar<Microsoft.FSharp.Core.string> (x) @ (306,37--306,43)";
        "let testStringToString(x) = Operators.ToString<Microsoft.FSharp.Core.string> (x) @ (307,37--307,45)" ]

    printDeclarations None (List.ofSeq file1.Declarations) 
      |> Seq.toList 
      |> filterHack
      |> shouldEqual (filterHack expected)

    printDeclarations None (List.ofSeq file2.Declarations) 
      |> Seq.toList 
      |> filterHack
      |> shouldEqual (filterHack expected2)

    ()


[<Test>]
let ``Test Optimized Declarations Project1`` () =
    let wholeProjectResults = exprChecker.ParseAndCheckProject(Project1.options) |> Async.RunSynchronously

    for e in wholeProjectResults.Errors do 
        printfn "Project1 error: <<<%s>>>" e.Message

    wholeProjectResults.Errors.Length |> shouldEqual 3 // recursive value warning
    wholeProjectResults.Errors.[0].Severity |> shouldEqual FSharpErrorSeverity.Warning
    wholeProjectResults.Errors.[1].Severity |> shouldEqual FSharpErrorSeverity.Warning
    wholeProjectResults.Errors.[2].Severity |> shouldEqual FSharpErrorSeverity.Warning

    wholeProjectResults.GetOptimizedAssemblyContents().ImplementationFiles.Length |> shouldEqual 2
    let file1 = wholeProjectResults.GetOptimizedAssemblyContents().ImplementationFiles.[0]
    let file2 = wholeProjectResults.GetOptimizedAssemblyContents().ImplementationFiles.[1]

    // This behaves slightly differently on Mono versions, 'null' is printed somethimes, 'None' other times
    // Presumably this is very small differences in Mono reflection causing F# printing to change behavious
    // For now just disabling this test. See https://github.com/fsharp/FSharp.Compiler.Service/pull/766
    let filterHack l = 
        l |> List.map (fun (s:string) -> 
            s.Replace("ILArrayShape [(Some 0, None)]", "ILArrayShapeFIX")
             .Replace("ILArrayShape [(Some 0, null)]", "ILArrayShapeFIX"))

    let expected = 
          ["type M"; "type IntAbbrev"; "let boolEx1 = True @ (6,14--6,18)";
           "let intEx1 = 1 @ (7,13--7,14)"; "let int64Ex1 = 1 @ (8,15--8,17)";
           "let tupleEx1 = (1,1) @ (9,16--9,21)";
           "let tupleEx2 = (1,1,1) @ (10,16--10,25)";
           "let tupleEx3 = (1,1,1,1) @ (11,16--11,29)";
           "let localExample = let y: Microsoft.FSharp.Core.int = 1 in let z: Microsoft.FSharp.Core.int = 1 in (y,z) @ (14,7--14,8)";
           "let localGenericFunctionExample(unitVar0) = let y: Microsoft.FSharp.Core.int = 1 in let compiledAsLocalGenericFunction: 'a -> 'a = FUN ... -> fun x -> x in (compiledAsLocalGenericFunction<Microsoft.FSharp.Core.int> y,compiledAsLocalGenericFunction<Microsoft.FSharp.Core.float> 1) @ (19,7--19,8)";
           "let funcEx1(x) = x @ (23,23--23,24)";
           "let genericFuncEx1(x) = x @ (24,29--24,30)";
           "let topPair1b = M.patternInput@25 ().Item1 @ (25,4--25,26)";
           "let topPair1a = M.patternInput@25 ().Item0 @ (25,4--25,26)";
           "let testILCall1 = new Object() @ (27,18--27,27)";
           "let testILCall2 = Console.WriteLine (\"176\") @ (28,18--28,49)";
           "let recValNeverUsedAtRuntime = recValNeverUsedAtRuntime@31.Force<Microsoft.FSharp.Core.int>(()) @ (31,8--31,32)";
           "let recFuncIgnoresFirstArg(g) (v) = v @ (32,33--32,34)";
           "let testFun4(unitVar0) = let rec ... in recValNeverUsedAtRuntime @ (36,4--39,28)";
           "type ClassWithImplicitConstructor";
           "member .ctor(compiledAsArg) = (new Object(); (this.compiledAsArg <- compiledAsArg; (this.compiledAsField <- 1; let compiledAsLocal: Microsoft.FSharp.Core.int = 1 in let compiledAsLocal2: Microsoft.FSharp.Core.int = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (compiledAsLocal,compiledAsLocal) in ()))) @ (41,5--41,33)";
           "member .cctor(unitVar) = (compiledAsStaticField <- 1; let compiledAsStaticLocal: Microsoft.FSharp.Core.int = 1 in let compiledAsStaticLocal2: Microsoft.FSharp.Core.int = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (compiledAsStaticLocal,compiledAsStaticLocal) in ()) @ (49,11--49,40)";
           "member M1(__) (unitVar1) = Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (__.compiledAsField,__.compiledAsGenericInstanceMethod<Microsoft.FSharp.Core.int>(__.compiledAsField)),__.compiledAsArg) @ (55,21--55,102)";
           "member M2(__) (unitVar1) = __.compiledAsInstanceMethod(()) @ (56,21--56,47)";
           "member SM1(unitVar0) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (compiledAsStaticField,ClassWithImplicitConstructor.compiledAsGenericStaticMethod<Microsoft.FSharp.Core.int> (compiledAsStaticField)) @ (57,26--57,101)";
           "member SM2(unitVar0) = ClassWithImplicitConstructor.compiledAsStaticMethod (()) @ (58,26--58,50)";
#if NO_PROJECTCRACKER // proxy for COMPILER
           "member ToString(__) (unitVar1) = String.Concat (base.ToString(),let value: Microsoft.FSharp.Core.int = 999 in let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int> (value) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ...) @ (59,29--59,57)";
#else
           "member ToString(__) (unitVar1) = String.Concat (base.ToString(),let x: Microsoft.FSharp.Core.int = 999 in let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ...) @ (59,29--59,57)";
#endif
           "member TestCallinToString(this) (unitVar1) = this.ToString() @ (60,39--60,54)";
           "type Error"; "let err = {Data0 = 3; Data1 = 4} @ (64,10--64,20)";
           "let matchOnException(err) = match (if err :? M.Error then $0 else $1) targets ... @ (66,33--66,36)";
           "let upwardForLoop(unitVar0) = let mutable a: Microsoft.FSharp.Core.int = 1 in (for-loop; a) @ (69,16--69,17)";
           "let upwardForLoop2(unitVar0) = let mutable a: Microsoft.FSharp.Core.int = 1 in (for-loop; a) @ (74,16--74,17)";
           "let downwardForLoop(unitVar0) = let mutable a: Microsoft.FSharp.Core.int = 1 in (for-loop; a) @ (79,16--79,17)";
           "let quotationTest1(unitVar0) = quote(Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (1,1)) @ (83,24--83,35)";
           "let quotationTest2(v) = quote(Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (ExtraTopLevelOperators.SpliceExpression<Microsoft.FSharp.Core.int> (v),1)) @ (84,24--84,36)";
           "type RecdType"; "type UnionType"; "type ClassWithEventsAndProperties";
           "member .ctor(unitVar0) = (new Object(); (this.ev <- new FSharpEvent`1(()); ())) @ (89,5--89,33)";
           "member .cctor(unitVar) = (sev <- new FSharpEvent`1(()); ()) @ (91,11--91,35)";
           "member get_InstanceProperty(x) (unitVar1) = (x.ev.Trigger(1); 1) @ (92,32--92,48)";
           "member get_StaticProperty(unitVar0) = (sev.Trigger(1); 1) @ (93,35--93,52)";
           "member get_InstanceEvent(x) (unitVar1) = x.ev.get_Publish(()) @ (94,29--94,39)";
           "member get_StaticEvent(x) (unitVar1) = sev.get_Publish(()) @ (95,27--95,38)";
           "let c = new ClassWithEventsAndProperties(()) @ (97,8--97,38)";
           "let v = M.c ().get_InstanceProperty(()) @ (98,8--98,26)";
           "do Console.WriteLine (\"777\")";
           "let functionWithSubmsumption(x) = IntrinsicFunctions.UnboxGeneric<Microsoft.FSharp.Core.string> (x) @ (102,40--102,52)";
#if NO_PROJECTCRACKER // proxy for COMPILER
           "let functionWithCoercion(x) = let arg: Microsoft.FSharp.Core.string = let arg: Microsoft.FSharp.Core.string = IntrinsicFunctions.UnboxGeneric<Microsoft.FSharp.Core.string> (x :> Microsoft.FSharp.Core.obj) in M.functionWithSubmsumption (arg :> Microsoft.FSharp.Core.obj) in M.functionWithSubmsumption (arg :> Microsoft.FSharp.Core.obj) @ (103,39--103,116)";
#else
           "let functionWithCoercion(x) = let x: Microsoft.FSharp.Core.string = let x: Microsoft.FSharp.Core.string = IntrinsicFunctions.UnboxGeneric<Microsoft.FSharp.Core.string> (x :> Microsoft.FSharp.Core.obj) in M.functionWithSubmsumption (x :> Microsoft.FSharp.Core.obj) in M.functionWithSubmsumption (x :> Microsoft.FSharp.Core.obj) @ (103,39--103,116)";
#endif
           "type MultiArgMethods";
           "member .ctor(c,d) = (new Object(); ()) @ (105,5--105,20)";
           "member Method(x) (a,b) = 1 @ (106,37--106,38)";
           "member CurriedMethod(x) (a1,b1) (a2,b2) = 1 @ (107,63--107,64)";
           "let testFunctionThatCallsMultiArgMethods(unitVar0) = let m: M.MultiArgMethods = new MultiArgMethods(3,4) in Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (m.Method(7,8),let arg00: Microsoft.FSharp.Core.int = 9 in let arg01: Microsoft.FSharp.Core.int = 10 in let arg10: Microsoft.FSharp.Core.int = 11 in let arg11: Microsoft.FSharp.Core.int = 12 in m.CurriedMethod(arg00,arg01,arg10,arg11)) @ (110,8--110,9)";
#if NO_PROJECTCRACKER // proxy for COMPILER
           "let functionThatUsesObjectExpression(unitVar0) = { new Object() with member x.ToString(unitVar1) = let value: Microsoft.FSharp.Core.int = 888 in let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int> (value) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ...  } @ (114,3--114,55)";
           "let functionThatUsesObjectExpressionWithInterfaceImpl(unitVar0) = { new Object() with member x.ToString(unitVar1) = let value: Microsoft.FSharp.Core.int = 888 in let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int> (value) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... interface System.IComparable with member x.CompareTo(y) = 0 } :> System.IComparable @ (117,3--120,38)";
#else
           "let functionThatUsesObjectExpression(unitVar0) = { new Object() with member x.ToString(unitVar1) = let x: Microsoft.FSharp.Core.int = 888 in let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ...  } @ (114,3--114,55)";
           "let functionThatUsesObjectExpressionWithInterfaceImpl(unitVar0) = { new Object() with member x.ToString(unitVar1) = let x: Microsoft.FSharp.Core.int = 888 in let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... interface System.IComparable with member x.CompareTo(y) = 0 } :> System.IComparable @ (117,3--120,38)";
#endif
           "let testFunctionThatUsesUnitsOfMeasure(x) (y) = Operators.op_Addition<Microsoft.FSharp.Core.float<'u>,Microsoft.FSharp.Core.float<'u>,Microsoft.FSharp.Core.float<'u>> (x,y) @ (122,70--122,75)";
           "let testFunctionThatUsesAddressesAndByrefs(x) = let mutable w: Microsoft.FSharp.Core.int = 4 in let y1: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = x in let y2: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = &w in let arr: Microsoft.FSharp.Core.int Microsoft.FSharp.Core.[] = [|3; 4|] in let r: Microsoft.FSharp.Core.int Microsoft.FSharp.Core.ref = Operators.Ref<Microsoft.FSharp.Core.int> (3) in let y3: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = [I_ldelema (NormalAddress,false,ILArrayShapeFIX,TypeVar 0us)](arr,0) in let y4: Microsoft.FSharp.Core.byref<Microsoft.FSharp.Core.int> = &r.contents in let z: Microsoft.FSharp.Core.int = Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x,y1),y2),y3) in (w <- 3; (x <- 4; (y2 <- 4; (y3 <- 5; Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32,Microsoft.FSharp.Core.int32> (Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (z,x),y1),y2),y3),y4),IntrinsicFunctions.GetArray<Microsoft.FSharp.Core.int> (arr,0)),r.contents))))) @ (125,16--125,17)";
           "let testFunctionThatUsesStructs1(dt) = dt.AddDays(3) @ (139,57--139,72)";
           "let testFunctionThatUsesStructs2(unitVar0) = let dt1: System.DateTime = DateTime.get_Now () in let mutable dt2: System.DateTime = DateTime.get_Now () in let dt3: System.TimeSpan = DateTime.op_Subtraction (dt1,dt2) in let dt4: System.DateTime = dt1.AddDays(3) in let dt5: Microsoft.FSharp.Core.int = dt1.get_Millisecond() in let dt6: Microsoft.FSharp.Core.byref<System.DateTime> = &dt2 in let dt7: System.TimeSpan = DateTime.op_Subtraction (dt6,dt4) in dt7 @ (142,7--142,10)";
           "let testFunctionThatUsesWhileLoop(unitVar0) = let mutable x: Microsoft.FSharp.Core.int = 1 in (while Operators.op_LessThan<Microsoft.FSharp.Core.int> (x,100) do x <- Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x,1) done; x) @ (152,15--152,16)";
           "let testFunctionThatUsesTryWith(unitVar0) = try M.testFunctionThatUsesWhileLoop (()) with matchValue -> match (if matchValue :? System.ArgumentException then $0 else $1) targets ... @ (158,3--160,60)";
           "let testFunctionThatUsesTryFinally(unitVar0) = try M.testFunctionThatUsesWhileLoop (()) finally Console.WriteLine (\"8888\") @ (164,3--167,37)";
           "member Console.WriteTwoLines.Static(unitVar0) = (Console.WriteLine (); Console.WriteLine ()) @ (170,36--170,90)";
           "member DateTime.get_TwoMinute(x) (unitVar1) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x.get_Minute(),x.get_Minute()) @ (173,25--173,44)";
           "let testFunctionThatUsesExtensionMembers(unitVar0) = (M.Console.WriteTwoLines.Static (()); let v: Microsoft.FSharp.Core.int = DateTime.get_Now ().DateTime.get_TwoMinute(()) in M.Console.WriteTwoLines.Static (())) @ (176,3--178,33)";
           "let testFunctionThatUsesOptionMembers(unitVar0) = let x: Microsoft.FSharp.Core.int Microsoft.FSharp.Core.option = Some(3) in (x.get_IsSome() (),x.get_IsNone() ()) @ (181,7--181,8)";
           "let testFunctionThatUsesOverAppliedFunction(unitVar0) = Operators.Identity<Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.int> (fun x -> Operators.Identity<Microsoft.FSharp.Core.int> (x)) 3 @ (185,3--185,10)";
           "let testFunctionThatUsesPatternMatchingOnLists(x) = match (if x.Isop_ColonColon then (if x.Tail.Isop_ColonColon then (if x.Tail.Tail.Isop_Nil then $2 else $3) else $1) else $0) targets ... @ (188,10--188,11)";
           "let testFunctionThatUsesPatternMatchingOnOptions(x) = match (if x.IsSome then $1 else $0) targets ... @ (195,10--195,11)";
           "let testFunctionThatUsesPatternMatchingOnOptions2(x) = match (if x.IsSome then $1 else $0) targets ... @ (200,10--200,11)";
           "let testFunctionThatUsesConditionalOnOptions2(x) = (if x.get_IsSome() () then 1 else 2) @ (205,4--205,29)";
           "let f(x) (y) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (x,y) @ (207,12--207,15)";
           "let g = let x: Microsoft.FSharp.Core.int = 1 in fun y -> M.f (x,y) @ (208,8--208,11)";
           "let h = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (M.g () 2,3) @ (209,8--209,17)";
           "type TestFuncProp";
           "member .ctor(unitVar0) = (new Object(); ()) @ (211,5--211,17)";
           "member get_Id(this) (unitVar1) = fun x -> x @ (212,21--212,31)";
           "let wrong = Operators.op_Equality<Microsoft.FSharp.Core.int> (new TestFuncProp(()).get_Id(()) 0,0) @ (214,12--214,35)";
           "let start(name) = (name,name) @ (217,4--217,14)";
           "let last(name,values) = Operators.Identity<Microsoft.FSharp.Core.string * Microsoft.FSharp.Core.string> ((name,values)) @ (220,4--220,21)";
           "let last2(name) = Operators.Identity<Microsoft.FSharp.Core.string> (name) @ (223,4--223,11)";
           "let test7(s) = let tupledArg: Microsoft.FSharp.Core.string * Microsoft.FSharp.Core.string = M.start (s) in let name: Microsoft.FSharp.Core.string = tupledArg.Item0 in let values: Microsoft.FSharp.Core.string = tupledArg.Item1 in M.last (name,values) @ (226,4--226,19)";
           "let test8(unitVar0) = fun tupledArg -> let name: Microsoft.FSharp.Core.string = tupledArg.Item0 in let values: Microsoft.FSharp.Core.string = tupledArg.Item1 in M.last (name,values) @ (229,4--229,8)";
           "let test9(s) = M.last (s,s) @ (232,4--232,17)";
           "let test10(unitVar0) = fun name -> M.last2 (name) @ (235,4--235,9)";
           "let test11(s) = M.last2 (s) @ (238,4--238,14)";
           "let badLoop = badLoop@240.Force<Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.int>(()) @ (240,8--240,15)";
           "type LetLambda";
           "let f = fun a -> fun b -> Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (a,b) @ (247,8--247,24)";
           "let letLambdaRes = ListModule.Map<Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (fun tupledArg -> let a: Microsoft.FSharp.Core.int = tupledArg.Item0 in let b: Microsoft.FSharp.Core.int = tupledArg.Item1 in (LetLambda.f () a) b,Cons((1,2),Empty())) @ (249,19--249,71)"]

    let expected2 = 
      [ "type N"; "type IntAbbrev"; "let bool2 = False @ (6,12--6,17)";
        "let testHashChar(x) = Operators.op_BitwiseOr<Microsoft.FSharp.Core.int> (Operators.op_LeftShift<Microsoft.FSharp.Core.char> (x,16),x) @ (8,28--8,34)";
        "let testHashSByte(x) = Operators.op_ExclusiveOr<Microsoft.FSharp.Core.int> (Operators.op_LeftShift<Microsoft.FSharp.Core.sbyte> (x,8),x) @ (9,30--9,36)";
        "let testHashInt16(x) = Operators.op_BitwiseOr<Microsoft.FSharp.Core.int> (Operators.ToUInt16<Microsoft.FSharp.Core.int16> (x),Operators.op_LeftShift<Microsoft.FSharp.Core.int16> (x,16)) @ (10,30--10,36)";
        "let testHashInt64(x) = Operators.op_ExclusiveOr<Microsoft.FSharp.Core.int> (Operators.ToInt32<Microsoft.FSharp.Core.int64> (x),Operators.ToInt32<Microsoft.FSharp.Core.int> (Operators.op_RightShift<Microsoft.FSharp.Core.int64> (x,32))) @ (11,30--11,36)";
        "let testHashUInt64(x) = Operators.op_ExclusiveOr<Microsoft.FSharp.Core.int> (Operators.ToInt32<Microsoft.FSharp.Core.uint64> (x),Operators.ToInt32<Microsoft.FSharp.Core.int> (Operators.op_RightShift<Microsoft.FSharp.Core.uint64> (x,32))) @ (12,32--12,38)";
        "let testHashIntPtr(x) = Operators.ToInt32<Microsoft.FSharp.Core.uint64> (Operators.ToUInt64<Microsoft.FSharp.Core.nativeint> (x)) @ (13,35--13,41)";
        "let testHashUIntPtr(x) = Operators.op_BitwiseAnd<Microsoft.FSharp.Core.int> (Operators.ToInt32<Microsoft.FSharp.Core.uint64> (Operators.ToUInt64<Microsoft.FSharp.Core.unativeint> (x)),2147483647) @ (14,37--14,43)";
        "let testHashString(x) = (if Operators.op_Equality<Microsoft.FSharp.Core.string> (x,dflt) then 0 else Operators.Hash<Microsoft.FSharp.Core.string> (x)) @ (16,32--16,38)";
        "let testTypeOf(x) = Operators.TypeOf<'T> () @ (17,24--17,30)";
        "let testEqualsOperator(e1) (e2) = HashCompare.GenericEqualityIntrinsic<'a> (e1,e2) @ (19,46--19,54)";
        "let testNotEqualsOperator(e1) (e2) = Operators.op_Equality<Microsoft.FSharp.Core.bool> (HashCompare.GenericEqualityIntrinsic<'a> (e1,e2),False) @ (20,46--20,55)";
        "let testLessThanOperator(e1) (e2) = HashCompare.GenericLessThanIntrinsic<'a> (e1,e2) @ (21,46--21,54)";
        "let testLessThanOrEqualsOperator(e1) (e2) = HashCompare.GenericLessOrEqualIntrinsic<'a> (e1,e2) @ (22,46--22,55)";
        "let testGreaterThanOperator(e1) (e2) = HashCompare.GenericGreaterThanIntrinsic<'a> (e1,e2) @ (23,46--23,54)";
        "let testGreaterThanOrEqualsOperator(e1) (e2) = HashCompare.GenericGreaterOrEqualIntrinsic<'a> (e1,e2) @ (24,46--24,55)";
        "let testAdditionOperator(e1) (e2) = Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (26,38--26,46)";
        "let testSubtractionOperator(e1) (e2) = Operators.op_Subtraction<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (27,38--27,46)";
        "let testMultiplyOperator(e1) (e2) = Operators.op_Multiply<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (28,37--28,46)";
        "let testDivisionOperator(e1) (e2) = Operators.op_Division<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (29,38--29,46)";
        "let testModulusOperator(e1) (e2) = Operators.op_Modulus<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (30,38--30,46)";
        "let testBitwiseAndOperator(e1) (e2) = Operators.op_BitwiseAnd<Microsoft.FSharp.Core.int> (e1,e2) @ (31,38--31,48)";
        "let testBitwiseOrOperator(e1) (e2) = Operators.op_BitwiseOr<Microsoft.FSharp.Core.int> (e1,e2) @ (32,38--32,48)";
        "let testBitwiseXorOperator(e1) (e2) = Operators.op_ExclusiveOr<Microsoft.FSharp.Core.int> (e1,e2) @ (33,38--33,48)";
        "let testShiftLeftOperator(e1) (e2) = Operators.op_LeftShift<Microsoft.FSharp.Core.int> (e1,Operators.op_BitwiseAnd<Microsoft.FSharp.Core.int> (e2,31)) @ (34,38--34,48)";
        "let testShiftRightOperator(e1) (e2) = Operators.op_RightShift<Microsoft.FSharp.Core.int> (e1,Operators.op_BitwiseAnd<Microsoft.FSharp.Core.int> (e2,31)) @ (35,38--35,48)";
        "let testUnaryNegOperator(e1) = Operators.op_UnaryNegation<Microsoft.FSharp.Core.int> (e1) @ (37,33--37,39)";
        "let testUnaryNotOperator(e1) = Operators.op_Equality<Microsoft.FSharp.Core.bool> (e1,False) @ (38,32--38,35)";
        "let testAdditionChecked(e1) (e2) = Checked.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (40,35--40,52)";
        "let testSubtractionChecked(e1) (e2) = Checked.op_Subtraction<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (41,35--41,52)";
        "let testMultiplyChecked(e1) (e2) = Checked.op_Multiply<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (e1,e2) @ (42,35--42,52)";
        "let testUnaryNegChecked(e1) = Checked.op_Subtraction<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (0,e1) @ (43,32--43,47)";
        "let testToByteChecked(e1) = Checked.ToByte<Microsoft.FSharp.Core.int> (e1) @ (45,32--45,47)";
        "let testToSByteChecked(e1) = Checked.ToSByte<Microsoft.FSharp.Core.int> (e1) @ (46,32--46,48)";
        "let testToInt16Checked(e1) = Checked.ToInt16<Microsoft.FSharp.Core.int> (e1) @ (47,32--47,48)";
        "let testToUInt16Checked(e1) = Checked.ToUInt16<Microsoft.FSharp.Core.int> (e1) @ (48,32--48,49)";
        "let testToIntChecked(e1) = Checked.ToInt32<Microsoft.FSharp.Core.int> (e1) @ (49,32--49,46)";
        "let testToInt32Checked(e1) = Checked.ToInt32<Microsoft.FSharp.Core.int> (e1) @ (50,32--50,48)";
        "let testToUInt32Checked(e1) = Checked.ToUInt32<Microsoft.FSharp.Core.int> (e1) @ (51,32--51,49)";
        "let testToInt64Checked(e1) = Checked.ToInt64<Microsoft.FSharp.Core.int> (e1) @ (52,32--52,48)";
        "let testToUInt64Checked(e1) = Checked.ToUInt64<Microsoft.FSharp.Core.int> (e1) @ (53,32--53,49)";
        "let testToIntPtrChecked(e1) = Checked.ToIntPtr<Microsoft.FSharp.Core.int> (e1) @ (54,32--54,52)";
        "let testToUIntPtrChecked(e1) = Checked.ToUIntPtr<Microsoft.FSharp.Core.int> (e1) @ (55,32--55,53)";
        "let testToByteOperator(e1) = Operators.ToByte<Microsoft.FSharp.Core.int> (e1) @ (57,32--57,39)";
        "let testToSByteOperator(e1) = Operators.ToSByte<Microsoft.FSharp.Core.int> (e1) @ (58,32--58,40)";
        "let testToInt16Operator(e1) = Operators.ToInt16<Microsoft.FSharp.Core.int> (e1) @ (59,32--59,40)";
        "let testToUInt16Operator(e1) = Operators.ToUInt16<Microsoft.FSharp.Core.int> (e1) @ (60,32--60,41)";
        "let testToIntOperator(e1) = e1 @ (61,36--61,38)";
        "let testToInt32Operator(e1) = e1 @ (62,38--62,40)";
        "let testToUInt32Operator(e1) = e1 @ (63,32--63,41)";
        "let testToInt64Operator(e1) = Operators.ToInt64<Microsoft.FSharp.Core.int> (e1) @ (64,32--64,40)";
        "let testToUInt64Operator(e1) = Operators.ToInt64<Microsoft.FSharp.Core.int> (e1) @ (65,32--65,41)";
        "let testToIntPtrOperator(e1) = Operators.ToIntPtr<Microsoft.FSharp.Core.int> (e1) @ (66,32--66,44)";
        "let testToUIntPtrOperator(e1) = Operators.ToIntPtr<Microsoft.FSharp.Core.int> (e1) @ (67,32--67,45)";
        "let testToSingleOperator(e1) = Operators.ToSingle<Microsoft.FSharp.Core.int> (e1) @ (68,32--68,42)";
        "let testToDoubleOperator(e1) = Operators.ToDouble<Microsoft.FSharp.Core.int> (e1) @ (69,32--69,40)";
        "let testToDecimalOperator(e1) = Convert.ToDecimal (e1) @ (70,32--70,42)";
        "let testToCharOperator(e1) = Operators.ToUInt16<Microsoft.FSharp.Core.int> (e1) @ (71,32--71,39)";
        "let testToStringOperator(e1) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.obj> (e1) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (72,32--72,41)";
        "let testByteToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.byte> (x) @ (74,33--74,39)";
        "let testByteToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.byte> (x) @ (75,33--75,40)";
        "let testByteToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.byte> (x) @ (76,33--76,40)";
        "let testByteToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.byte> (x) @ (77,33--77,41)";
        "let testByteToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.byte> (x) @ (78,33--78,38)";
        "let testByteToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.byte> (x) @ (79,33--79,41)";
        "let testByteToInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.byte> (x) @ (80,33--80,40)";
        "let testByteToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.byte> (x) @ (81,33--81,41)";
        "let testByteToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.byte> (x) @ (82,33--82,44)";
        "let testByteToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.byte> (x) @ (83,33--83,45)";
        "let testByteToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.byte> (x)) @ (84,33--84,42)";
        "let testByteToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.byte> (x)) @ (85,33--85,40)";
        "let testByteToDecimal(x) = Convert.ToDecimal (x) @ (86,33--86,42)";
        "let testByteToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.byte> (x) @ (87,33--87,39)";
        "let testByteToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.byte> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (88,33--88,41)";
        "let testSByteToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.sbyte> (x) @ (90,35--90,41)";
        "let testSByteToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.sbyte> (x) @ (91,35--91,42)";
        "let testSByteToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.sbyte> (x) @ (92,35--92,42)";
        "let testSByteToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.sbyte> (x) @ (93,35--93,43)";
        "let testSByteToInt32(x) = x @ (94,35--94,40)";
        "let testSByteToUInt32(x) = x @ (95,35--95,43)";
        "let testSByteToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.sbyte> (x) @ (96,35--96,42)";
        "let testSByteToUInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.sbyte> (x) @ (97,35--97,43)";
        "let testSByteToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.sbyte> (x) @ (98,35--98,46)";
        "let testSByteToUIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.sbyte> (x) @ (99,35--99,47)";
        "let testSByteToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.sbyte> (x) @ (100,35--100,44)";
        "let testSByteToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.sbyte> (x) @ (101,35--101,42)";
        "let testSByteToDecimal(x) = Convert.ToDecimal (x) @ (102,35--102,44)";
        "let testSByteToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.sbyte> (x) @ (103,35--103,41)";
        "let testSByteToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.sbyte> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (104,35--104,43)";
        "let testInt16ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.int16> (x) @ (106,35--106,41)";
        "let testInt16ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.int16> (x) @ (107,35--107,42)";
        "let testInt16ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.int16> (x) @ (108,35--108,42)";
        "let testInt16ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int16> (x) @ (109,35--109,43)";
        "let testInt16ToInt32(x) = x @ (110,35--110,40)";
        "let testInt16ToUInt32(x) = x @ (111,35--111,43)";
        "let testInt16ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int16> (x) @ (112,35--112,42)";
        "let testInt16ToUInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int16> (x) @ (113,35--113,43)";
        "let testInt16ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int16> (x) @ (114,35--114,46)";
        "let testInt16ToUIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int16> (x) @ (115,35--115,47)";
        "let testInt16ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.int16> (x) @ (116,35--116,44)";
        "let testInt16ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.int16> (x) @ (117,35--117,42)";
        "let testInt16ToDecimal(x) = Convert.ToDecimal (x) @ (118,35--118,44)";
        "let testInt16ToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int16> (x) @ (119,35--119,41)";
        "let testInt16ToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int16> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (120,35--120,43)";
        "let testUInt16ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.uint16> (x) @ (122,37--122,43)";
        "let testUInt16ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.uint16> (x) @ (123,37--123,44)";
        "let testUInt16ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.uint16> (x) @ (124,37--124,44)";
        "let testUInt16ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint16> (x) @ (125,37--125,45)";
        "let testUInt16ToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.uint16> (x) @ (126,37--126,42)";
        "let testUInt16ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.uint16> (x) @ (127,37--127,45)";
        "let testUInt16ToInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.uint16> (x) @ (128,37--128,44)";
        "let testUInt16ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.uint16> (x) @ (129,37--129,45)";
        "let testUInt16ToIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint16> (x) @ (130,37--130,48)";
        "let testUInt16ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint16> (x) @ (131,37--131,49)";
        "let testUInt16ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.uint16> (x)) @ (132,37--132,46)";
        "let testUInt16ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.uint16> (x)) @ (133,37--133,44)";
        "let testUInt16ToDecimal(x) = Convert.ToDecimal (x) @ (134,37--134,46)";
        "let testUInt16ToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint16> (x) @ (135,37--135,43)";
        "let testUInt16ToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.uint16> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (136,37--136,45)";
        "let testInt32ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.int> (x) @ (138,33--138,39)";
        "let testInt32ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.int> (x) @ (139,33--139,40)";
        "let testInt32ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.int> (x) @ (140,33--140,40)";
        "let testInt32ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int> (x) @ (141,33--141,41)";
        "let testInt32ToInt32(x) = x @ (142,37--142,38)";
        "let testInt32ToUInt32(x) = x @ (143,33--143,41)";
        "let testInt32ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int> (x) @ (144,33--144,40)";
        "let testInt32ToUInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int> (x) @ (145,33--145,41)";
        "let testInt32ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int> (x) @ (146,33--146,44)";
        "let testInt32ToUIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int> (x) @ (147,33--147,45)";
        "let testInt32ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.int> (x) @ (148,33--148,42)";
        "let testInt32ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.int> (x) @ (149,33--149,40)";
        "let testInt32ToDecimal(x) = Convert.ToDecimal (x) @ (150,33--150,42)";
        "let testInt32ToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int> (x) @ (151,33--151,39)";
        "let testInt32ToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (152,33--152,41)";
        "let testUInt32ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.uint32> (x) @ (154,37--154,43)";
        "let testUInt32ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.uint32> (x) @ (155,37--155,44)";
        "let testUInt32ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.uint32> (x) @ (156,37--156,44)";
        "let testUInt32ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint32> (x) @ (157,37--157,45)";
        "let testUInt32ToInt32(x) = x @ (158,37--158,42)";
        "let testUInt32ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.uint32> (x) @ (159,37--159,45)";
        "let testUInt32ToInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.uint32> (x) @ (160,37--160,44)";
        "let testUInt32ToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.uint32> (x) @ (161,37--161,45)";
        "let testUInt32ToIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint32> (x) @ (162,37--162,48)";
        "let testUInt32ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint32> (x) @ (163,37--163,49)";
        "let testUInt32ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.uint32> (x)) @ (164,37--164,46)";
        "let testUInt32ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.uint32> (x)) @ (165,37--165,44)";
        "let testUInt32ToDecimal(x) = Convert.ToDecimal (x) @ (166,37--166,46)";
        "let testUInt32ToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint32> (x) @ (167,37--167,43)";
        "let testUInt32ToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.uint32> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (168,37--168,45)";
        "let testInt64ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.int64> (x) @ (170,35--170,41)";
        "let testInt64ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.int64> (x) @ (171,35--171,42)";
        "let testInt64ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.int64> (x) @ (172,35--172,42)";
        "let testInt64ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int64> (x) @ (173,35--173,43)";
        "let testInt64ToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.int64> (x) @ (174,35--174,40)";
        "let testInt64ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.int64> (x) @ (175,35--175,43)";
        "let testInt64ToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.int64> (x) @ (176,35--176,42)";
        "let testInt64ToUInt64(x) = x @ (177,35--177,43)";
        "let testInt64ToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int64> (x) @ (178,35--178,46)";
        "let testInt64ToUIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.int64> (x) @ (179,35--179,47)";
        "let testInt64ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.int64> (x) @ (180,35--180,44)";
        "let testInt64ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.int64> (x) @ (181,35--181,42)";
        "let testInt64ToDecimal(x) = Convert.ToDecimal (x) @ (182,35--182,44)";
        "let testInt64ToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.int64> (x) @ (183,35--183,41)";
        "let testInt64ToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.int64> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (184,35--184,43)";
        "let testUInt64ToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.uint64> (x) @ (186,37--186,43)";
        "let testUInt64ToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.uint64> (x) @ (187,37--187,44)";
        "let testUInt64ToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.uint64> (x) @ (188,37--188,44)";
        "let testUInt64ToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint64> (x) @ (189,37--189,45)";
        "let testUInt64ToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.uint64> (x) @ (190,37--190,42)";
        "let testUInt64ToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.uint64> (x) @ (191,37--191,45)";
        "let testUInt64ToInt64(x) = x @ (192,37--192,44)";
        "let testUInt64ToUInt64(x) = x @ (193,44--193,45)";
        "let testUInt64ToIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint64> (x) @ (194,37--194,48)";
        "let testUInt64ToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.uint64> (x) @ (195,37--195,49)";
        "let testUInt64ToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.uint64> (x)) @ (196,37--196,46)";
        "let testUInt64ToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.uint64> (x)) @ (197,37--197,44)";
        "let testUInt64ToDecimal(x) = Convert.ToDecimal (x) @ (198,37--198,46)";
        "let testUInt64ToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.uint64> (x) @ (199,37--199,43)";
        "let testUInt64ToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.uint64> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (200,37--200,45)";
        "let testIntPtrToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.nativeint> (x) @ (202,40--202,46)";
        "let testIntPtrToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.nativeint> (x) @ (203,40--203,47)";
        "let testIntPtrToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.nativeint> (x) @ (204,40--204,47)";
        "let testIntPtrToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.nativeint> (x) @ (205,40--205,48)";
        "let testIntPtrToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.nativeint> (x) @ (206,40--206,45)";
        "let testIntPtrToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.nativeint> (x) @ (207,40--207,48)";
        "let testIntPtrToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.nativeint> (x) @ (208,40--208,47)";
        "let testIntPtrToUInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.nativeint> (x) @ (209,40--209,48)";
        "let testIntPtrToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.nativeint> (x) @ (210,40--210,51)";
        "let testIntPtrToUIntPtr(x) = x @ (211,40--211,52)";
        "let testIntPtrToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.nativeint> (x) @ (212,40--212,49)";
        "let testIntPtrToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.nativeint> (x) @ (213,40--213,47)";
        "let testIntPtrToDecimal(x) = Convert.ToDecimal (Operators.ToInt64<Microsoft.FSharp.Core.nativeint> (x)) @ (214,40--214,49)";
        "let testIntPtrToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.nativeint> (x) @ (215,40--215,46)";
        "let testIntPtrToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.nativeint> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (216,40--216,48)";
        "let testUIntPtrToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.unativeint> (x) @ (218,42--218,48)";
        "let testUIntPtrToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.unativeint> (x) @ (219,42--219,49)";
        "let testUIntPtrToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.unativeint> (x) @ (220,42--220,49)";
        "let testUIntPtrToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.unativeint> (x) @ (221,42--221,50)";
        "let testUIntPtrToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.unativeint> (x) @ (222,42--222,47)";
        "let testUIntPtrToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.unativeint> (x) @ (223,42--223,50)";
        "let testUIntPtrToInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.unativeint> (x) @ (224,42--224,49)";
        "let testUIntPtrToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.unativeint> (x) @ (225,42--225,50)";
        "let testUIntPtrToIntPtr(x) = x @ (226,42--226,53)";
        "let testUIntPtrToUIntPtr(x) = x @ (227,53--227,54)";
        "let testUIntPtrToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.unativeint> (x)) @ (228,42--228,51)";
        "let testUIntPtrToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.unativeint> (x)) @ (229,42--229,49)";
        "let testUIntPtrToDecimal(x) = Convert.ToDecimal (Operators.ToUInt64<Microsoft.FSharp.Core.unativeint> (x)) @ (230,42--230,51)";
        "let testUIntPtrToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.unativeint> (x) @ (231,42--231,48)";
        "let testUIntPtrToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.unativeint> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (232,42--232,50)";
        "let testSingleToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.float32> (x) @ (234,38--234,44)";
        "let testSingleToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.float32> (x) @ (235,38--235,45)";
        "let testSingleToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.float32> (x) @ (236,38--236,45)";
        "let testSingleToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.float32> (x) @ (237,38--237,46)";
        "let testSingleToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.float32> (x) @ (238,38--238,43)";
        "let testSingleToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.float32> (x) @ (239,38--239,46)";
        "let testSingleToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.float32> (x) @ (240,38--240,45)";
        "let testSingleToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.float32> (x) @ (241,38--241,46)";
        "let testSingleToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.float32> (x) @ (242,38--242,49)";
        "let testSingleToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.float32> (x) @ (243,38--243,50)";
        "let testSingleToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float32> (x) @ (244,38--244,47)";
        "let testSingleToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float32> (x) @ (245,38--245,45)";
        "let testSingleToDecimal(x) = Convert.ToDecimal (x) @ (246,38--246,47)";
        "let testSingleToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.float32> (x) @ (247,38--247,44)";
        "let testSingleToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.float32> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (248,38--248,46)";
        "let testDoubleToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.float> (x) @ (250,36--250,42)";
        "let testDoubleToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.float> (x) @ (251,36--251,43)";
        "let testDoubleToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.float> (x) @ (252,36--252,43)";
        "let testDoubleToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.float> (x) @ (253,36--253,44)";
        "let testDoubleToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.float> (x) @ (254,36--254,41)";
        "let testDoubleToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.float> (x) @ (255,36--255,44)";
        "let testDoubleToInt64(x) = Operators.ToInt64<Microsoft.FSharp.Core.float> (x) @ (256,36--256,43)";
        "let testDoubleToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.float> (x) @ (257,36--257,44)";
        "let testDoubleToIntPtr(x) = Operators.ToIntPtr<Microsoft.FSharp.Core.float> (x) @ (258,36--258,47)";
        "let testDoubleToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.float> (x) @ (259,36--259,48)";
        "let testDoubleToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (x) @ (260,36--260,45)";
        "let testDoubleToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (x) @ (261,36--261,43)";
        "let testDoubleToDecimal(x) = Convert.ToDecimal (x) @ (262,36--262,45)";
        "let testDoubleToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.float> (x) @ (263,36--263,42)";
        "let testDoubleToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.float> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (264,36--264,44)";
        "let testDecimalToByte(x) = Decimal.op_Explicit (x) @ (266,39--266,45)";
        "let testDecimalToSByte(x) = Decimal.op_Explicit (x) @ (267,39--267,46)";
        "let testDecimalToInt16(x) = Decimal.op_Explicit (x) @ (268,39--268,46)";
        "let testDecimalToUInt16(x) = Decimal.op_Explicit (x) @ (269,39--269,47)";
        "let testDecimalToInt32(x) = Decimal.op_Explicit (x) @ (270,39--270,44)";
        "let testDecimalToUInt32(x) = Decimal.op_Explicit (x) @ (271,39--271,47)";
        "let testDecimalToInt64(x) = Decimal.op_Explicit (x) @ (272,39--272,46)";
        "let testDecimalToUInt64(x) = Decimal.op_Explicit (x) @ (273,39--273,47)";
        "let testDecimalToSingle(x) = Decimal.op_Explicit (x) @ (274,39--274,48)";
        "let testDecimalToDouble(x) = Convert.ToDouble (x) @ (275,39--275,46)";
        "let testDecimalToDecimal(x) = x @ (276,47--276,48)";
        "let testDecimalToChar(x) = Decimal.op_Explicit (x) @ (277,39--277,45)";
        "let testDecimalToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.decimal> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (278,39--278,47)";
        "let testCharToByte(x) = Operators.ToByte<Microsoft.FSharp.Core.char> (x) @ (280,33--280,39)";
        "let testCharToSByte(x) = Operators.ToSByte<Microsoft.FSharp.Core.char> (x) @ (281,33--281,40)";
        "let testCharToInt16(x) = Operators.ToInt16<Microsoft.FSharp.Core.char> (x) @ (282,33--282,40)";
        "let testCharToUInt16(x) = Operators.ToUInt16<Microsoft.FSharp.Core.char> (x) @ (283,33--283,41)";
        "let testCharToInt32(x) = Operators.ToInt32<Microsoft.FSharp.Core.char> (x) @ (284,33--284,38)";
        "let testCharToUInt32(x) = Operators.ToUInt32<Microsoft.FSharp.Core.char> (x) @ (285,33--285,41)";
        "let testCharToInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.char> (x) @ (286,33--286,40)";
        "let testCharToUInt64(x) = Operators.ToUInt64<Microsoft.FSharp.Core.char> (x) @ (287,33--287,41)";
        "let testCharToIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.char> (x) @ (288,33--288,44)";
        "let testCharToUIntPtr(x) = Operators.ToUIntPtr<Microsoft.FSharp.Core.char> (x) @ (289,33--289,45)";
        "let testCharToSingle(x) = Operators.ToSingle<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.char> (x)) @ (290,33--290,42)";
        "let testCharToDouble(x) = Operators.ToDouble<Microsoft.FSharp.Core.float> (Operators.ToDouble<Microsoft.FSharp.Core.char> (x)) @ (291,33--291,40)";
        "let testCharToChar(x) = Operators.ToUInt16<Microsoft.FSharp.Core.char> (x) @ (292,33--292,39)";
        "let testCharToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.char> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (293,33--293,41)";
        "let testStringToByte(x) = Checked.ToByte<Microsoft.FSharp.Core.uint32> (LanguagePrimitives.ParseUInt32 (x)) @ (295,37--295,43)";
        "let testStringToSByte(x) = Checked.ToSByte<Microsoft.FSharp.Core.int> (LanguagePrimitives.ParseInt32 (x)) @ (296,37--296,44)";
        "let testStringToInt16(x) = Checked.ToInt16<Microsoft.FSharp.Core.int> (LanguagePrimitives.ParseInt32 (x)) @ (297,37--297,44)";
        "let testStringToUInt16(x) = Checked.ToUInt16<Microsoft.FSharp.Core.uint32> (LanguagePrimitives.ParseUInt32 (x)) @ (298,37--298,45)";
        "let testStringToInt32(x) = LanguagePrimitives.ParseInt32 (x) @ (299,37--299,42)";
        "let testStringToUInt32(x) = LanguagePrimitives.ParseUInt32 (x) @ (300,37--300,45)";
        "let testStringToInt64(x) = LanguagePrimitives.ParseInt64 (x) @ (301,37--301,44)";
        "let testStringToUInt64(x) = LanguagePrimitives.ParseUInt64 (x) @ (302,37--302,45)";
        "let testStringToSingle(x) = Single.Parse ((if Operators.op_Equality<Microsoft.FSharp.Core.string> (x,dflt) then dflt else x.Replace(\"_\",\"\")),167,CultureInfo.get_InvariantCulture () :> System.IFormatProvider) @ (303,37--303,46)";
        "let testStringToDouble(x) = Double.Parse ((if Operators.op_Equality<Microsoft.FSharp.Core.string> (x,dflt) then dflt else x.Replace(\"_\",\"\")),167,CultureInfo.get_InvariantCulture () :> System.IFormatProvider) @ (304,37--304,44)";
        "let testStringToDecimal(x) = Decimal.Parse (x,167,CultureInfo.get_InvariantCulture () :> System.IFormatProvider) @ (305,37--305,46)";
        "let testStringToChar(x) = Char.Parse (x) @ (306,37--306,43)";
        "let testStringToString(x) = let matchValue: Microsoft.FSharp.Core.obj = Operators.Box<Microsoft.FSharp.Core.string> (x) in match (if Operators.op_Equality<Microsoft.FSharp.Core.obj> (matchValue,dflt) then $0 else (if matchValue :? System.IFormattable then $1 else $2)) targets ... @ (307,37--307,45)" ]


    // printFSharpDecls "" file2.Declarations |> Seq.iter (printfn "%s")

    printDeclarations None (List.ofSeq file1.Declarations) 
      |> Seq.toList 
      |> filterHack
      |> shouldEqual (filterHack expected)

    printDeclarations None (List.ofSeq file2.Declarations) 
      |> Seq.toList 
      |> filterHack
      |> shouldEqual (filterHack expected2)

    ()


//---------------------------------------------------------------------------------------------------------
// This big list expression was causing us trouble

module internal ProjectStressBigExpressions = 
    open System.IO

    let fileName1 = Path.ChangeExtension(Path.GetTempFileName(), ".fs")
    let base2 = Path.GetTempFileName()
    let dllName = Path.ChangeExtension(base2, ".dll")
    let projFileName = Path.ChangeExtension(base2, ".fsproj")
    let fileSource1 = """
module StressBigExpressions 


let BigListExpression = 

   [("C", "M.C", "file1", ((3, 5), (3, 6)), ["class"]);
    ("( .ctor )", "M.C.( .ctor )", "file1", ((3, 5), (3, 6)),["member"; "ctor"]);
    ("P", "M.C.P", "file1", ((4, 13), (4, 14)), ["member"; "getter"]);
    ("x", "x", "file1", ((4, 11), (4, 12)), []);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file1",((6, 12), (6, 13)), ["val"]);
    ("xxx", "M.xxx", "file1", ((6, 4), (6, 7)), ["val"]);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file1",((7, 17), (7, 18)), ["val"]);
    ("xxx", "M.xxx", "file1", ((7, 13), (7, 16)), ["val"]);
    ("xxx", "M.xxx", "file1", ((7, 19), (7, 22)), ["val"]);
    ("fff", "M.fff", "file1", ((7, 4), (7, 7)), ["val"]);
    ("C", "M.C", "file1", ((9, 15), (9, 16)), ["class"]);
    ("C", "M.C", "file1", ((9, 15), (9, 16)), ["class"]);
    ("C", "M.C", "file1", ((9, 15), (9, 16)), ["class"]);
    ("C", "M.C", "file1", ((9, 15), (9, 16)), ["class"]);
    ("CAbbrev", "M.CAbbrev", "file1", ((9, 5), (9, 12)), ["abbrev"]);
    ("M", "M", "file1", ((1, 7), (1, 8)), ["module"]);
    ("D1", "N.D1", "file2", ((5, 5), (5, 7)), ["class"]);
    ("( .ctor )", "N.D1.( .ctor )", "file2", ((5, 5), (5, 7)),["member"; "ctor"]);
    ("SomeProperty", "N.D1.SomeProperty", "file2", ((6, 13), (6, 25)),["member"; "getter"]); 
    ("x", "x", "file2", ((6, 11), (6, 12)), []);
    ("M", "M", "file2", ((6, 28), (6, 29)), ["module"]);
    ("xxx", "M.xxx", "file2", ((6, 28), (6, 33)), ["val"]);
    ("D2", "N.D2", "file2", ((8, 5), (8, 7)), ["class"]);
    ("( .ctor )", "N.D2.( .ctor )", "file2", ((8, 5), (8, 7)),["member"; "ctor"]);
    ("SomeProperty", "N.D2.SomeProperty", "file2", ((9, 13), (9, 25)),["member"; "getter"]); ("x", "x", "file2", ((9, 11), (9, 12)), []);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file2",((9, 36), (9, 37)), ["val"]);
    ("M", "M", "file2", ((9, 28), (9, 29)), ["module"]);
    ("fff", "M.fff", "file2", ((9, 28), (9, 33)), ["val"]);
    ("D1", "N.D1", "file2", ((9, 38), (9, 40)), ["member"; "ctor"]);
    ("M", "M", "file2", ((12, 27), (12, 28)), ["module"]);
    ("xxx", "M.xxx", "file2", ((12, 27), (12, 32)), ["val"]);
    ("y2", "N.y2", "file2", ((12, 4), (12, 6)), ["val"]);
    ("DefaultValueAttribute", "Microsoft.FSharp.Core.DefaultValueAttribute","file2", ((18, 6), (18, 18)), ["class"]);
    ("DefaultValueAttribute", "Microsoft.FSharp.Core.DefaultValueAttribute","file2", ((18, 6), (18, 18)), ["class"]);
    ("DefaultValueAttribute", "Microsoft.FSharp.Core.DefaultValueAttribute","file2", ((18, 6), (18, 18)), ["member"]);
    ("int", "Microsoft.FSharp.Core.int", "file2", ((19, 20), (19, 23)),["abbrev"]);
    ("DefaultValueAttribute", "Microsoft.FSharp.Core.DefaultValueAttribute","file2", ((18, 6), (18, 18)), ["class"]);
    ("DefaultValueAttribute", "Microsoft.FSharp.Core.DefaultValueAttribute","file2", ((18, 6), (18, 18)), ["class"]);
    ("DefaultValueAttribute", "Microsoft.FSharp.Core.DefaultValueAttribute","file2", ((18, 6), (18, 18)), ["member"]);
    ("x", "N.D3.x", "file2", ((19, 16), (19, 17)),["field"; "default"; "mutable"]);
    ("D3", "N.D3", "file2", ((15, 5), (15, 7)), ["class"]);
    ("int", "Microsoft.FSharp.Core.int", "file2", ((15, 10), (15, 13)),["abbrev"]); ("a", "a", "file2", ((15, 8), (15, 9)), []);
    ("( .ctor )", "N.D3.( .ctor )", "file2", ((15, 5), (15, 7)),["member"; "ctor"]);
    ("SomeProperty", "N.D3.SomeProperty", "file2", ((21, 13), (21, 25)),["member"; "getter"]);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file2",((16, 14), (16, 15)), ["val"]);
    ("a", "a", "file2", ((16, 12), (16, 13)), []);
    ("b", "b", "file2", ((16, 8), (16, 9)), []);
    ("x", "x", "file2", ((21, 11), (21, 12)), []);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file2",((21, 30), (21, 31)), ["val"]);
    ("a", "a", "file2", ((21, 28), (21, 29)), []);
    ("b", "b", "file2", ((21, 32), (21, 33)), []);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file2",((23, 25), (23, 26)), ["val"]);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file2",((23, 21), (23, 22)), ["val"]);
    ("int32", "Microsoft.FSharp.Core.Operators.int32", "file2",((23, 27), (23, 32)), ["val"]);
    ("DateTime", "System.DateTime", "file2", ((23, 40), (23, 48)),["valuetype"]);
    ("System", "System", "file2", ((23, 33), (23, 39)), ["namespace"]);
    ("Now", "System.DateTime.Now", "file2", ((23, 33), (23, 52)),["member"; "prop"]);
    ("Ticks", "System.DateTime.Ticks", "file2", ((23, 33), (23, 58)),["member"; "prop"]);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file2",((23, 62), (23, 63)), ["val"]);
    ("pair2", "N.pair2", "file2", ((23, 10), (23, 15)), ["val"]);
    ("pair1", "N.pair1", "file2", ((23, 4), (23, 9)), ["val"]);
    ("None", "N.SaveOptions.None", "file2", ((27, 4), (27, 8)),["field"; "static"; "0"]);
    ("DisableFormatting", "N.SaveOptions.DisableFormatting", "file2",((28, 4), (28, 21)), ["field"; "static"; "1"]);
    ("SaveOptions", "N.SaveOptions", "file2", ((26, 5), (26, 16)),["enum"; "valuetype"]);
    ("SaveOptions", "N.SaveOptions", "file2", ((30, 16), (30, 27)),["enum"; "valuetype"]);
    ("DisableFormatting", "N.SaveOptions.DisableFormatting", "file2",((30, 16), (30, 45)), ["field"; "static"; "1"]);
    ("enumValue", "N.enumValue", "file2", ((30, 4), (30, 13)), ["val"]);
    ("x", "x", "file2", ((32, 9), (32, 10)), []);
    ("y", "y", "file2", ((32, 11), (32, 12)), []);
    ("( + )", "Microsoft.FSharp.Core.Operators.( + )", "file2",((32, 17), (32, 18)), ["val"]);
    ("x", "x", "file2", ((32, 15), (32, 16)), []);
    ("y", "y", "file2", ((32, 19), (32, 20)), []);
    ("( ++ )", "N.( ++ )", "file2", ((32, 5), (32, 7)), ["val"]);
    ("( ++ )", "N.( ++ )", "file2", ((34, 11), (34, 13)), ["val"]);
    ("c1", "N.c1", "file2", ((34, 4), (34, 6)), ["val"]);
    ("( ++ )", "N.( ++ )", "file2", ((36, 11), (36, 13)), ["val"]);
    ("c2", "N.c2", "file2", ((36, 4), (36, 6)), ["val"]);
    ("M", "M", "file2", ((38, 12), (38, 13)), ["module"]);
    ("C", "M.C", "file2", ((38, 12), (38, 15)), ["class"]);
    ("M", "M", "file2", ((38, 22), (38, 23)), ["module"]);
    ("C", "M.C", "file2", ((38, 22), (38, 25)), ["class"]);
    ("C", "M.C", "file2", ((38, 22), (38, 25)), ["member"; "ctor"]);
    ("mmmm1", "N.mmmm1", "file2", ((38, 4), (38, 9)), ["val"]);
    ("M", "M", "file2", ((39, 12), (39, 13)), ["module"]);
    ("CAbbrev", "M.CAbbrev", "file2", ((39, 12), (39, 21)), ["abbrev"]);
    ("M", "M", "file2", ((39, 28), (39, 29)), ["module"]);
    ("CAbbrev", "M.CAbbrev", "file2", ((39, 28), (39, 37)), ["abbrev"]);
    ("C", "M.C", "file2", ((39, 28), (39, 37)), ["member"; "ctor"]);
    ("mmmm2", "N.mmmm2", "file2", ((39, 4), (39, 9)), ["val"]);
    ("N", "N", "file2", ((1, 7), (1, 8)), ["module"])]

let BigSequenceExpression(outFileOpt,docFileOpt,baseAddressOpt) =    
        [   yield "--simpleresolution"
            yield "--noframework"
            match outFileOpt with
            | None -> ()
            | Some outFile -> yield "--out:" + outFile
            match docFileOpt with
            | None -> ()
            | Some docFile -> yield "--doc:" + docFile
            match baseAddressOpt with
            | None -> ()
            | Some baseAddress -> yield "--baseaddress:" + baseAddress
            match baseAddressOpt with
            | None -> ()
            | Some keyFile -> yield "--keyfile:" + keyFile
            match baseAddressOpt with
            | None -> ()
            | Some sigFile -> yield "--sig:" + sigFile
            match baseAddressOpt with
            | None -> ()
            | Some pdbFile -> yield "--pdb:" + pdbFile
            match baseAddressOpt with
            | None -> ()
            | Some versionFile -> yield "--versionfile:" + versionFile
            match baseAddressOpt with
            | None -> ()
            | Some warnLevel -> yield "--warn:" + warnLevel
            match baseAddressOpt with
            | None -> ()
            | Some s -> yield "--subsystemversion:" + s
            if true then yield "--highentropyva+"
            match baseAddressOpt with
            | None -> ()
            | Some win32Res -> yield "--win32res:" + win32Res
            match baseAddressOpt with
            | None -> ()
            | Some win32Manifest -> yield "--win32manifest:" + win32Manifest
            match baseAddressOpt with
            | None -> ()
            | Some targetProfile -> yield "--targetprofile:" + targetProfile
            yield "--fullpaths"
            yield "--flaterrors"
            if true then yield "--warnaserror"
            yield 
                if true then "--target:library"
                else "--target:exe"
            for symbol in [] do
                if not (System.String.IsNullOrWhiteSpace symbol) then yield "--define:" + symbol
            for nw in [] do
                if not (System.String.IsNullOrWhiteSpace nw) then yield "--nowarn:" + nw
            for nw in [] do
                if not (System.String.IsNullOrWhiteSpace nw) then yield "--warnaserror:" + nw
            yield if true then "--debug+"
                    else "--debug-"
            yield if true then "--optimize+"
                    else "--optimize-"
            yield if true then "--tailcalls+"
                    else "--tailcalls-"
            match baseAddressOpt with
            | None -> ()
            | Some debugType -> 
                match "" with
                | "NONE" -> ()
                | "PDBONLY" -> yield "--debug:pdbonly"
                | "FULL" -> yield "--debug:full"
                | _ -> ()
            match baseAddressOpt |> Option.map (fun o -> ""), true, baseAddressOpt |> Option.map (fun o -> "") with
            | Some "ANYCPU", true, Some "EXE" | Some "ANYCPU", true, Some "WINEXE" -> yield "--platform:anycpu32bitpreferred"
            | Some "ANYCPU", _, _ -> yield "--platform:anycpu"
            | Some "X86", _, _ -> yield "--platform:x86"
            | Some "X64", _, _ -> yield "--platform:x64"
            | Some "ITANIUM", _, _ -> yield "--platform:Itanium"
            | _ -> ()
            match baseAddressOpt |> Option.map (fun o -> "") with
            | Some "LIBRARY" -> yield "--target:library"
            | Some "EXE" -> yield "--target:exe"
            | Some "WINEXE" -> yield "--target:winexe"
            | Some "MODULE" -> yield "--target:module"
            | _ -> ()
            yield! []
            for f in [] do
                yield "--resource:" + f
            for i in [] do
                yield "--lib:" 
            for r in []  do
                yield "-r:" + r 
            yield! [] ]

    
    """
    File.WriteAllText(fileName1, fileSource1)

    let fileNames = [fileName1]
    let args = mkProjectCommandLineArgs (dllName, fileNames)
    let options =  exprChecker.GetProjectOptionsFromCommandLineArgs (projFileName, args)


[<Test>]
let ``Test expressions of declarations stress big expressions`` () =
    let wholeProjectResults = exprChecker.ParseAndCheckProject(ProjectStressBigExpressions.options) |> Async.RunSynchronously
    
    wholeProjectResults.Errors.Length |> shouldEqual 0

    wholeProjectResults.AssemblyContents.ImplementationFiles.Length |> shouldEqual 1
    let file1 = wholeProjectResults.AssemblyContents.ImplementationFiles.[0]

    // This should not stack overflow
    printDeclarations None (List.ofSeq file1.Declarations) |> Seq.toList |> ignore


[<Test>]
let ``Test expressions of optimized declarations stress big expressions`` () =
    let wholeProjectResults = exprChecker.ParseAndCheckProject(ProjectStressBigExpressions.options) |> Async.RunSynchronously
    
    wholeProjectResults.Errors.Length |> shouldEqual 0

    wholeProjectResults.GetOptimizedAssemblyContents().ImplementationFiles.Length |> shouldEqual 1
    let file1 = wholeProjectResults.GetOptimizedAssemblyContents().ImplementationFiles.[0]

    // This should not stack overflow
    printDeclarations None (List.ofSeq file1.Declarations) |> Seq.toList |> ignore


#if NOT_YET_ENABLED

#if !NO_PROJECTCRACKER && !DOTNETCORE

[<Test>]
let ``Check use of type provider that provides calls to F# code`` () = 
    let config = 
#if DEBUG
        ["Configuration", "Debug"]
#else
        ["Configuration", "Release"]
#endif
    let options =
        ProjectCracker.GetProjectOptionsFromProjectFile (Path.Combine(Path.Combine(Path.Combine(__SOURCE_DIRECTORY__, "data"),"TestProject"),"TestProject.fsproj"), config)

    let res =
        options
        |> exprChecker.ParseAndCheckProject 
        |> Async.RunSynchronously

    Assert.AreEqual ([||], res.Errors, sprintf "Should not be errors, but: %A" res.Errors)
                                                                                       
    let results = 
        [ for f in res.AssemblyContents.ImplementationFiles do
               for d in f.Declarations do 
                    for line in d |> printDeclaration None do 
                        yield line ]    
    
    results |> List.iter (printfn "%s")

    results |> shouldEqual
      ["type TestProject"; "type AssemblyInfo"; "type TestProject"; "type T";
       """type Class1""";
       """member .ctor(unitVar0) = (new Object(); ()) @ (5,5--5,11)""";
       """member get_X1(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothing () @ (6,21--6,36)"""
       """member get_X2(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothingGeneric<Microsoft.FSharp.Core.int> (3) @ (7,21--7,43)"""
       """member get_X3(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothingOneArg (3) @ (8,21--8,42)"""
       """member get_X4(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in C.DoNothing () @ (9,21--9,41)"""
       """member get_X5(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in C.DoNothingGeneric<Microsoft.FSharp.Core.int> (3) @ (10,21--10,48)"""
       """member get_X6(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in C.DoNothingOneArg (3) @ (11,21--11,47)"""
       """member get_X7(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in C.DoNothingTwoArg (new C(),3) @ (12,21--12,47)"""
       """member get_X8(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new C().InstanceDoNothing() @ (13,21--13,49)"""
       """member get_X9(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new C().InstanceDoNothingGeneric<Microsoft.FSharp.Core.int>(3) @ (14,21--14,56)"""
       """member get_X10(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new C().InstanceDoNothingOneArg(3) @ (15,22--15,56)"""
       """member get_X11(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new C().InstanceDoNothingTwoArg(new C(),3) @ (16,22--16,56)"""
       """member get_X12(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in G`1<Microsoft.FSharp.Core.int>.DoNothing () @ (17,22--17,49)"""
       """member get_X13(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in G`1<Microsoft.FSharp.Core.int>.DoNothingOneArg (3) @ (18,22--18,55)"""
       """member get_X14(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in G`1<Microsoft.FSharp.Core.int>.DoNothingTwoArg (new C(),3) @ (19,22--19,55)"""
       """member get_X15(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in let matchValue: Microsoft.FSharp.Core.Option<Microsoft.FSharp.Core.int> = FSharpOption`1<Microsoft.FSharp.Core.int>.Some (1) in (if Operators.op_Equality<Microsoft.FSharp.Core.int> (matchValue.Tag,1) then let x: Microsoft.FSharp.Core.int = matchValue.get_Value() in x else 0) @ (20,22--20,54)"""
       """member get_X16(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in let matchValue: Microsoft.FSharp.Core.Choice<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.obj> = Choice1Of2(1) in (if Operators.op_Equality<Microsoft.FSharp.Core.int> (matchValue.Tag,0) then 1 else 0) @ (21,22--21,54)"""
       """member get_X17(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in let r: TestTP.Helper.R = {A = 1; B = 0} in (r.B <- 1; r.A) @ (22,22--22,60)"""
       """member get_X18(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothingTwoArg (3,4) @ (23,22--23,43)"""
       """member get_X19(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothingTwoArgCurried (3,4) @ (24,22--24,50)"""
       """member get_X21(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in (fun arg00 -> fun arg10 -> C.DoNothingTwoArgCurried (arg00,arg10)<TestTP.Helper.C> new C())<Microsoft.FSharp.Core.int> 3 @ (25,22--25,55)"""
       """member get_X23(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in (let objectArg: TestTP.Helper.C = new C() in fun arg00 -> fun arg10 -> objectArg.InstanceDoNothingTwoArgCurried(arg00,arg10)<TestTP.Helper.C> new C())<Microsoft.FSharp.Core.int> 3 @ (26,22--26,63)"""
       """member get_X24(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothingGenericWithConstraint<Microsoft.FSharp.Core.int> (3) @ (27,22--27,58)"""
       """member get_X25(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothingGenericWithTypeConstraint<Microsoft.FSharp.Collections.List<Microsoft.FSharp.Core.int>,Microsoft.FSharp.Core.int> (FSharpList`1<Microsoft.FSharp.Core.int>.Cons (3,FSharpList`1<Microsoft.FSharp.Core.int>.get_Empty ())) @ (28,22--28,62)"""
       """member get_X26(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.doNothingGenericWithTypeConstraint<Microsoft.FSharp.Collections.List<Microsoft.FSharp.Core.int>,Microsoft.FSharp.Core.int> (FSharpList`1<Microsoft.FSharp.Core.int>.Cons (3,FSharpList`1<Microsoft.FSharp.Core.int>.get_Empty ())) @ (29,22--29,62)"""
       """member get_X27(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Helper.DoNothingReally () @ (30,22--30,53)"""
       """member get_X28(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new CSharpClass(0).Method("x") :> Microsoft.FSharp.Core.Unit @ (31,22--31,40)"""
       """member get_X29(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in Operators.op_Addition<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.int> (new CSharpClass(0).Method2("x"),new CSharpClass(0).Method2("empty")) :> Microsoft.FSharp.Core.Unit @ (32,22--32,53)"""
       """member get_X30(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new CSharpClass(0).Method3([|"x"; "y"|]) :> Microsoft.FSharp.Core.Unit @ (33,22--33,50)"""
       """member get_X31(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new CSharpClass(0).GenericMethod<Microsoft.FSharp.Core.int>(2) @ (34,22--34,47)"""
       """member get_X32(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new CSharpClass(0).GenericMethod2<Microsoft.FSharp.Core.obj>(new Object()) @ (35,22--35,61)"""
       """member get_X33(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new CSharpClass(0).GenericMethod3<Microsoft.FSharp.Core.int>(3) @ (36,22--36,65)"""
       """member get_X34(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in C.DoNothingReally () @ (37,22--37,58)"""
       """member get_X35(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new C().DoNothingReallyInst() @ (38,22--38,66)"""
       """member get_X36(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in (new CSharpClass(0) :> FSharp.Compiler.Service.Tests.ICSharpExplicitInterface).ExplicitMethod("x") :> Microsoft.FSharp.Core.Unit @ (39,22--39,62)"""
       """member get_X37(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in (new C() :> TestTP.Helper.I).DoNothing() @ (40,22--40,46)"""
       """member get_X38(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in new C().VirtualDoNothing() @ (41,22--41,45)"""
       """member get_X39(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in let t: Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int = (1,2,3) in let i: Microsoft.FSharp.Core.int = t.Item1 in i @ (42,22--42,51)"""
       """member get_X40(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in (moduleValue <- 1; moduleValue) @ (43,22--43,39)"""
       """member get_X41(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in let x: TestTP.Helper.C = new C() in (x.set_Property(1); x.get_Property()) @ (44,22--44,41)"""
       """member get_X42(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in let x: TestTP.Helper.C = new C() in (x.set_AutoProperty(1); x.get_AutoProperty()) @ (45,22--45,45)"""
       """member get_X43(this) (unitVar1) = let this: Microsoft.FSharp.Core.obj = ("My internal state" :> Microsoft.FSharp.Core.obj) :> ErasedWithConstructor.Provided.MyType in (C.set_StaticAutoProperty (1); C.get_StaticAutoProperty ()) @ (46,22--46,51)"""
      ]

    let members = 
        [ for f in res.AssemblyContents.ImplementationFiles do yield! printMembersOfDeclatations f.Declarations ]

    members |> List.iter (printfn "%s")

    members |> shouldEqual 
      [
       ".ctor: Microsoft.FSharp.Core.unit -> TestProject.Class1"
       ".ctor: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X1: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothing: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X2: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothingGeneric<'T>: 'T -> Microsoft.FSharp.Core.unit"
       "get_X3: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothingOneArg: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "get_X4: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothing: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X5: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingGeneric<'T>: 'T -> Microsoft.FSharp.Core.unit"
       "get_X6: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingOneArg: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "get_X7: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingTwoArg: TestTP.Helper.C * Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "get_X8: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "InstanceDoNothing: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X9: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "InstanceDoNothingGeneric<'T>: 'T -> Microsoft.FSharp.Core.unit"
       "get_X10: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "InstanceDoNothingOneArg: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "get_X11: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "InstanceDoNothingTwoArg: TestTP.Helper.C * Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "get_X12: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothing: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X13: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingOneArg: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "get_X14: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingTwoArg: TestTP.Helper.C * Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "get_X15: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "Some: 'T -> 'T Microsoft.FSharp.Core.option"
       "op_Equality<'T when 'T : equality>: 'T -> 'T -> Microsoft.FSharp.Core.bool"
       "matchValue: Microsoft.FSharp.Core.Option<Microsoft.FSharp.Core.int>"
       "matchValue: Microsoft.FSharp.Core.Option<Microsoft.FSharp.Core.int>"
       "get_Value: Microsoft.FSharp.Core.unit -> 'T"
       "x: Microsoft.FSharp.Core.int"
       "get_X16: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "op_Equality<'T when 'T : equality>: 'T -> 'T -> Microsoft.FSharp.Core.bool"
       "matchValue: Microsoft.FSharp.Core.Choice<Microsoft.FSharp.Core.int,Microsoft.FSharp.Core.obj>"
       "get_X17: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "r: TestTP.Helper.R"
       "r: TestTP.Helper.R"
       "get_X18: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothingTwoArg: Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "get_X19: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothingTwoArgCurried: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "get_X21: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingTwoArgCurried: TestTP.Helper.C -> Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "arg00: TestTP.Helper.C"
       "arg10: Microsoft.FSharp.Core.int"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "get_X23: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "objectArg: TestTP.Helper.C"
       "InstanceDoNothingTwoArgCurried: TestTP.Helper.C -> Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "arg00: TestTP.Helper.C"
       "arg10: Microsoft.FSharp.Core.int"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "get_X24: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothingGenericWithConstraint<'T when 'T : equality>: 'T -> Microsoft.FSharp.Core.unit"
       "get_X25: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothingGenericWithTypeConstraint<'T, _ when 'T :> Microsoft.FSharp.Collections.seq<'a>>: 'T -> Microsoft.FSharp.Core.unit"
       "Cons: 'T * 'T Microsoft.FSharp.Collections.list -> 'T Microsoft.FSharp.Collections.list"
       "get_Empty: Microsoft.FSharp.Core.unit -> 'T Microsoft.FSharp.Collections.list"
       "get_X26: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "doNothingGenericWithTypeConstraint<'T, _ when 'T :> Microsoft.FSharp.Collections.seq<'a>>: 'T -> Microsoft.FSharp.Core.unit"
       "Cons: 'T * 'T Microsoft.FSharp.Collections.list -> 'T Microsoft.FSharp.Collections.list"
       "get_Empty: Microsoft.FSharp.Core.unit -> 'T Microsoft.FSharp.Collections.list"
       "get_X27: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingReally: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X28: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "Method: Microsoft.FSharp.Core.string -> Microsoft.FSharp.Core.int"
       "get_X29: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "op_Addition<^T1, ^T2, ^T3>:  ^T1 ->  ^T2 ->  ^T3"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "Method2: Microsoft.FSharp.Core.string -> Microsoft.FSharp.Core.int"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "Method2: Microsoft.FSharp.Core.string -> Microsoft.FSharp.Core.int"
       "get_X30: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "Method3: Microsoft.FSharp.Core.string Microsoft.FSharp.Core.[] -> Microsoft.FSharp.Core.int"
       "get_X31: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "GenericMethod<'T>: 'T -> Microsoft.FSharp.Core.unit"
       "get_X32: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "GenericMethod2<'T when 'T : class>: 'T -> Microsoft.FSharp.Core.unit"
       ".ctor: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X33: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "GenericMethod3<'T when 'T :> System.IComparable<'T>>: 'T -> Microsoft.FSharp.Core.unit"
       "get_X34: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       "DoNothingReally: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X35: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "DoNothingReallyInst: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X36: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "ExplicitMethod: Microsoft.FSharp.Core.string -> Microsoft.FSharp.Core.int"
       "get_X37: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "DoNothing: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X38: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.Unit"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "VirtualDoNothing: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.unit"
       "get_X39: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "t: Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int * Microsoft.FSharp.Core.int"
       "i: Microsoft.FSharp.Core.int"
       "get_X40: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "moduleValue: Microsoft.FSharp.Core.int"
       "moduleValue: Microsoft.FSharp.Core.int"
       "get_X41: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "x: TestTP.Helper.C"
       "set_Property: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "x: TestTP.Helper.C"
       "get_Property: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "get_X42: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       ".ctor: Microsoft.FSharp.Core.unit -> TestTP.Helper.C"
       "x: TestTP.Helper.C"
       "set_AutoProperty: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "x: TestTP.Helper.C"
       "get_AutoProperty: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "get_X43: TestProject.Class1 -> Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
       "set_StaticAutoProperty: Microsoft.FSharp.Core.int -> Microsoft.FSharp.Core.unit"
       "get_StaticAutoProperty: Microsoft.FSharp.Core.unit -> Microsoft.FSharp.Core.int"
      ]

#endif
#endif


#if SELF_HOST_STRESS
   

[<Test>]
let ``Test Declarations selfhost`` () =
    let projectFile = __SOURCE_DIRECTORY__ + @"/FSharp.Compiler.Service.Tests.fsproj"
    // Check with Configuration = Release
    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile, [("Configuration", "Debug")])
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunSynchronously
    
    wholeProjectResults.Errors.Length |> shouldEqual 0 

    wholeProjectResults.AssemblyContents.ImplementationFiles.Length |> shouldEqual 13

    let textOfAll = [ for file in wholeProjectResults.AssemblyContents.ImplementationFiles -> Array.ofSeq (printDeclarations None (List.ofSeq file.Declarations))   ]

    ()


[<Test>]
let ``Test Declarations selfhost whole compiler`` () =
    
    Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__ +  @"/../../src/fsharp/FSharp.Compiler.Service")
    let projectFile = __SOURCE_DIRECTORY__ + @"/../../src/fsharp/FSharp.Compiler.Service/FSharp.Compiler.Service.fsproj"

    //let v = FSharpProjectFileInfo.Parse(projectFile, [("Configuration", "Debug"); ("NoFsSrGenTask", "true")],enableLogging=true)
    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile, [("Configuration", "Debug"); ("NoFsSrGenTask", "true")])

    // For subsets of the compiler:
    //let options = { options with OtherOptions = options.OtherOptions.[0..51] }

    //for x in options.OtherOptions do printfn "%s" x

    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunSynchronously
    
    (wholeProjectResults.Errors |> Array.filter (fun x -> x.Severity = FSharpErrorSeverity.Error)).Length |> shouldEqual 0 

    for file in (wholeProjectResults.AssemblyContents.ImplementationFiles |> List.toArray) do
        for d in file.Declarations do 
           for s in printDeclaration None d do 
              () //printfn "%s" s

    // Very Quick (1 sec - expressions are computed on demand)
    for file in (wholeProjectResults.AssemblyContents.ImplementationFiles |> List.toArray) do
        for d in file.Declarations do 
           for s in exprsOfDecl d do 
              () 

    // Quickish (~4.5 seconds for all of FSharp.Compiler.Service.dll)
    #time "on"
    for file in (wholeProjectResults.AssemblyContents.ImplementationFiles |> List.toArray) do
        for d in file.Declarations do 
           for (e,m) in exprsOfDecl d do 
              // This forces the computation of the expression
              match e with
              | BasicPatterns.Const _ -> () //printfn "%s" s
              | _ -> () //printfn "%s" s

[<Test>]
let ``Test Declarations selfhost FSharp.Core`` () =
    
    Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__ +  @"/../../../fsharp/src/fsharp/FSharp.Core")
    let projectFile = __SOURCE_DIRECTORY__ + @"/../../../fsharp/src/fsharp/FSharp.Core/FSharp.Core.fsproj"

    let options = ProjectCracker.GetProjectOptionsFromProjectFile(projectFile, [("Configuration", "Debug")])

    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunSynchronously
    
    //(wholeProjectResults.Errors |> Array.filter (fun x -> x.Severity = FSharpErrorSeverity.Error)).Length |> shouldEqual 0 

    for file in (wholeProjectResults.AssemblyContents.ImplementationFiles |> List.toArray) do
        for d in file.Declarations do 
           for s in printDeclaration (Some (HashSet [])) d do 
              printfn "%s" s

    #time "on"

    for file in (wholeProjectResults.AssemblyContents.ImplementationFiles |> List.toArray) do
        for d in file.Declarations do 
           for (e,m) in exprsOfDecl d do 
              // This forces the computation of the expression
              match e with
              | BasicPatterns.Const _ -> () 
              | _ -> () 

#endif

