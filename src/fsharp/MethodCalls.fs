// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Logic associated with resolving method calls.
module internal FSharp.Compiler.MethodCalls

open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler.AbstractIL.Internal.Library 
open FSharp.Compiler.Range
open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lib
open FSharp.Compiler.Infos
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.NameResolution
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.Tastops.DebugPrint
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypeRelations
open FSharp.Compiler.AttributeChecking

#if !NO_EXTENSIONTYPING
open FSharp.Compiler.ExtensionTyping
#endif


//-------------------------------------------------------------------------
// Sets of methods involved in overload resolution and trait constraint
// satisfaction.
//------------------------------------------------------------------------- 

/// In the following, 'T gets instantiated to: 
///   1. the expression being supplied for an argument 
///   2. "unit", when simply checking for the existence of an overload that satisfies 
///      a signature, or when finding the corresponding witness. 
/// Note the parametricity helps ensure that overload resolution doesn't depend on the 
/// expression on the callside (though it is in some circumstances allowed 
/// to depend on some type information inferred syntactically from that 
/// expression, e.g. a lambda expression may be converted to a delegate as 
/// an adhoc conversion. 
///
/// The bool indicates if named using a '?' 
type CallerArg<'T> = 
    /// CallerArg(ty, range, isOpt, exprInfo)
    | CallerArg of TType * range * bool * 'T  
    member x.Type = (let (CallerArg(ty, _, _, _)) = x in ty)
    member x.Range = (let (CallerArg(_, m, _, _)) = x in m)
    member x.IsOptional = (let (CallerArg(_, _, isOpt, _)) = x in isOpt)
    member x.Expr = (let (CallerArg(_, _, _, expr)) = x in expr)
    
/// Represents the information about an argument in the method being called
type CalledArg = 
    { Position: (int * int)
      IsParamArray : bool
      OptArgInfo : OptionalArgInfo
      CallerInfo : CallerInfo
      IsInArg: bool
      IsOutArg: bool
      ReflArgInfo: ReflectedArgInfo
      NameOpt: Ident option
      CalledArgumentType : TType }

let CalledArg (pos, isParamArray, optArgInfo, callerInfo, isInArg, isOutArg, nameOpt, reflArgInfo, calledArgTy) =
    { Position=pos
      IsParamArray=isParamArray
      OptArgInfo=optArgInfo
      CallerInfo=callerInfo
      IsInArg=isInArg
      IsOutArg=isOutArg
      ReflArgInfo=reflArgInfo
      NameOpt=nameOpt
      CalledArgumentType=calledArgTy }

/// Represents a match between a caller argument and a called argument, arising from either
/// a named argument or an unnamed argument.
type AssignedCalledArg<'T> = 
    { /// The identifier for a named argument, if any
      NamedArgIdOpt : Ident option
      /// The called argument in the method
      CalledArg: CalledArg 
      /// The argument on the caller side
      CallerArg: CallerArg<'T> }
    member x.Position = x.CalledArg.Position

/// Represents the possibilities for a named-setter argument (a property, field, or a record field setter)
type AssignedItemSetterTarget = 
    | AssignedPropSetter of PropInfo * MethInfo * TypeInst   (* the MethInfo is a non-indexer setter property *)
    | AssignedILFieldSetter of ILFieldInfo 
    | AssignedRecdFieldSetter of RecdFieldInfo 

/// Represents the resolution of a caller argument as a named-setter argument
type AssignedItemSetter<'T> = AssignedItemSetter of Ident * AssignedItemSetterTarget * CallerArg<'T> 

type CallerNamedArg<'T> = 
    | CallerNamedArg of Ident * CallerArg<'T>  
    member x.Ident = (let (CallerNamedArg(id, _)) = x in id)
    member x.Name = x.Ident.idText
    member x.CallerArg = (let (CallerNamedArg(_, a)) = x in a)

//-------------------------------------------------------------------------
// Callsite conversions
//------------------------------------------------------------------------- 

// F# supports three adhoc conversions at method callsites (note C# supports more, though ones 
// such as implicit conversions interact badly with type inference). 
//
// 1. The use of "(fun x y -> ...)" when  a delegate it expected. This is not part of 
// the ":>" coercion relationship or inference constraint problem as 
// such, but is a special rule applied only to method arguments. 
// 
// The function AdjustCalledArgType detects this case based on types and needs to know that the type being applied 
// is a function type. 
// 
// 2. The use of "(fun x y -> ...)" when Expression<delegate> it expected. This is similar to above.
// 
// 3. Two ways to pass a value where a byref is expected. The first (default) 
// is to use a reference cell, and the interior address is taken automatically 
// The second is an explicit use of the "address-of" operator "&e". Here we detect the second case,
// and record the presence of the syntax "&e" in the pre-inferred actual type for the method argument. 
// The function AdjustCalledArgType detects this and refuses to apply the default byref-to-ref transformation. 
//
// The function AdjustCalledArgType also adjusts for optional arguments. 
let AdjustCalledArgType (infoReader: InfoReader) isConstraint (calledArg: CalledArg) (callerArg: CallerArg<_>)  =
    let g = infoReader.g
    // #424218 - when overload resolution is part of constraint solving - do not perform type-directed conversions
    let calledArgTy = calledArg.CalledArgumentType
    let callerArgTy = callerArg.Type
    let m = callerArg.Range
    if isConstraint then 
        calledArgTy 
    else

        // If the called method argument is an inref type, then the caller may provide a byref or value
        if isInByrefTy g calledArgTy then
#if IMPLICIT_ADDRESS_OF
            if isByrefTy g callerArgTy then 
                calledArgTy
            else 
                destByrefTy g calledArgTy
#else
            calledArgTy
#endif

        // If the called method argument is a (non inref) byref type, then the caller may provide a byref or ref.
        elif isByrefTy g calledArgTy then
            if isByrefTy g callerArgTy then 
                calledArgTy
            else
                mkRefCellTy g (destByrefTy g calledArgTy)  

        else 
            // If the called method argument is a delegate type, then the caller may provide a function 
            let calledArgTy = 
                let adjustDelegateTy calledTy =
                    let (SigOfFunctionForDelegate(_, delArgTys, _, fty)) = GetSigOfFunctionForDelegate infoReader calledTy m  AccessibleFromSomewhere
                    let delArgTys = if isNil delArgTys then [g.unit_ty] else delArgTys
                    if (fst (stripFunTy g callerArgTy)).Length = delArgTys.Length
                    then fty 
                    else calledArgTy 

                if isDelegateTy g calledArgTy && isFunTy g callerArgTy then 
                    adjustDelegateTy calledArgTy

                elif isLinqExpressionTy g calledArgTy && isFunTy g callerArgTy then 
                    let origArgTy = calledArgTy
                    let calledArgTy = destLinqExpressionTy g calledArgTy
                    if isDelegateTy g calledArgTy then 
                        adjustDelegateTy calledArgTy
                    else
                        // BUG 435170: called arg is Expr<'t> where 't is not delegate - such conversion is not legal -> return original type
                        origArgTy

                elif calledArg.ReflArgInfo.AutoQuote && isQuotedExprTy g calledArgTy && not (isQuotedExprTy g callerArgTy) then 
                    destQuotedExprTy g calledArgTy

                else calledArgTy

            // Adjust the called argument type to take into account whether the caller's argument is M(?arg=Some(3)) or M(arg=1) 
            // If the called method argument is optional with type Option<T>, then the caller may provide a T, unless their argument is propagating-optional (i.e. isOptCallerArg) 
            let calledArgTy = 
                match calledArg.OptArgInfo with 
                | NotOptional                    -> calledArgTy
                | CalleeSide when not callerArg.IsOptional && isOptionTy g calledArgTy  -> destOptionTy g calledArgTy
                | CalleeSide | CallerSide _ -> calledArgTy
            calledArgTy
        

//-------------------------------------------------------------------------
// CalledMeth
//------------------------------------------------------------------------- 

type CalledMethArgSet<'T> = 
    { /// The called arguments corresponding to "unnamed" arguments
      UnnamedCalledArgs : CalledArg list
      /// Any unnamed caller arguments not otherwise assigned 
      UnnamedCallerArgs :  CallerArg<'T> list
      /// The called "ParamArray" argument, if any
      ParamArrayCalledArgOpt : CalledArg option 
      /// Any unnamed caller arguments assigned to a "param array" argument
      ParamArrayCallerArgs : CallerArg<'T> list
      /// Named args
      AssignedNamedArgs: AssignedCalledArg<'T> list  }
    member x.NumUnnamedCallerArgs = x.UnnamedCallerArgs.Length
    member x.NumAssignedNamedArgs = x.AssignedNamedArgs.Length
    member x.NumUnnamedCalledArgs = x.UnnamedCalledArgs.Length


let MakeCalledArgs amap m (minfo: MethInfo) minst =
    // Mark up the arguments with their position, so we can sort them back into order later 
    let paramDatas = minfo.GetParamDatas(amap, m, minst)
    paramDatas |> List.mapiSquared (fun i j (ParamData(isParamArrayArg, isInArg, isOutArg, optArgInfo, callerInfoFlags, nmOpt, reflArgInfo, typeOfCalledArg))  -> 
      { Position=(i,j)
        IsParamArray=isParamArrayArg
        OptArgInfo=optArgInfo
        CallerInfo = callerInfoFlags
        IsInArg=isInArg
        IsOutArg=isOutArg
        ReflArgInfo=reflArgInfo
        NameOpt=nmOpt
        CalledArgumentType=typeOfCalledArg })

/// Represents the syntactic matching between a caller of a method and the called method.
///
/// The constructor takes all the information about the caller and called side of a method, match up named arguments, property setters etc.,
/// and returns a CalledMeth object for further analysis.
type CalledMeth<'T>
      (infoReader: InfoReader,
       nameEnv: NameResolutionEnv option,
       isCheckingAttributeCall,
       freshenMethInfo, // a function to help generate fresh type variables the property setters methods in generic classes 
       m,
       ad,                // the access domain of the place where the call is taking place
       minfo: MethInfo,    // the method we're attempting to call 
       calledTyArgs,      // the 'called type arguments', i.e. the fresh generic instantiation of the method we're attempting to call 
       callerTyArgs: TType list, // the 'caller type arguments', i.e. user-given generic instantiation of the method we're attempting to call 
       pinfoOpt: PropInfo option,   // the property related to the method we're attempting to call, if any  
       callerObjArgTys: TType list,   // the types of the actual object argument, if any 
       curriedCallerArgs: (CallerArg<'T> list * CallerNamedArg<'T> list) list,     // the data about any arguments supplied by the caller 
       allowParamArgs: bool,       // do we allow the use of a param args method in its "expanded" form?
       allowOutAndOptArgs: bool,  // do we allow the use of the transformation that converts out arguments as tuple returns?
       tyargsOpt : TType option) // method parameters
    =
    let g = infoReader.g
    let methodRetTy = minfo.GetFSharpReturnTy(infoReader.amap, m, calledTyArgs)

    let fullCurriedCalledArgs = MakeCalledArgs infoReader.amap m minfo calledTyArgs
    do assert (fullCurriedCalledArgs.Length = fullCurriedCalledArgs.Length)
 
    let argSetInfos = 
        (curriedCallerArgs, fullCurriedCalledArgs) ||> List.map2 (fun (unnamedCallerArgs, namedCallerArgs) fullCalledArgs -> 
            // Find the arguments not given by name 
            let unnamedCalledArgs = 
                fullCalledArgs |> List.filter (fun calledArg -> 
                    match calledArg.NameOpt with 
                    | Some nm -> namedCallerArgs |> List.forall (fun (CallerNamedArg(nm2, _e)) -> nm.idText <> nm2.idText)   
                    | None -> true)

            // See if any of them are 'out' arguments being returned as part of a return tuple 
            let minArgs, unnamedCalledArgs, unnamedCalledOptArgs, unnamedCalledOutArgs = 
                let nUnnamedCallerArgs = unnamedCallerArgs.Length
                let nUnnamedCalledArgs = unnamedCalledArgs.Length
                if allowOutAndOptArgs && nUnnamedCallerArgs < nUnnamedCalledArgs then
                    let unnamedCalledArgsTrimmed, unnamedCalledOptOrOutArgs = List.splitAt nUnnamedCallerArgs unnamedCalledArgs
                    
                    // Check if all optional/out arguments are byref-out args
                    if unnamedCalledOptOrOutArgs |> List.forall (fun x -> x.IsOutArg && isByrefTy g x.CalledArgumentType) then 
                        nUnnamedCallerArgs - 1, unnamedCalledArgsTrimmed, [], unnamedCalledOptOrOutArgs 
                    // Check if all optional/out arguments are optional args
                    elif unnamedCalledOptOrOutArgs |> List.forall (fun x -> x.OptArgInfo.IsOptional) then 
                        nUnnamedCallerArgs - 1, unnamedCalledArgsTrimmed, unnamedCalledOptOrOutArgs, []
                    // Otherwise drop them on the floor
                    else
                        nUnnamedCalledArgs - 1, unnamedCalledArgs, [], []
                else 
                    nUnnamedCalledArgs - 1, unnamedCalledArgs, [], []

            let (unnamedCallerArgs, paramArrayCallerArgs), unnamedCalledArgs, paramArrayCalledArgOpt = 
                let supportsParamArgs = 
                    allowParamArgs && 
                    minArgs >= 0 && 
                    unnamedCalledArgs |> List.last |> (fun calledArg -> calledArg.IsParamArray && isArray1DTy g calledArg.CalledArgumentType)

                if supportsParamArgs  && unnamedCallerArgs.Length >= minArgs then
                    let a, b = List.frontAndBack unnamedCalledArgs
                    List.splitAt minArgs unnamedCallerArgs, a, Some(b)
                else
                    (unnamedCallerArgs, []), unnamedCalledArgs, None

            let assignedNamedArgs = 
                fullCalledArgs |> List.choose (fun calledArg ->
                    match calledArg.NameOpt with 
                    | Some nm -> 
                        namedCallerArgs |> List.tryPick (fun (CallerNamedArg(nm2, callerArg)) -> 
                            if nm.idText = nm2.idText then Some { NamedArgIdOpt = Some nm2; CallerArg=callerArg; CalledArg=calledArg } 
                            else None) 
                    | _ -> None)

            let unassignedNamedItems = 
                namedCallerArgs |> List.filter (fun (CallerNamedArg(nm, _e)) -> 
                    fullCalledArgs |> List.forall (fun calledArg -> 
                        match calledArg.NameOpt with 
                        | Some nm2 -> nm.idText <> nm2.idText
                        | None -> true))

            let attributeAssignedNamedItems = 
                if isCheckingAttributeCall then 
                    // The process for assigning names-->properties is substantially different for attribute specifications 
                    // because it permits the bindings of names to immutable fields. So we use the old 
                    // code for this.
                    unassignedNamedItems
                 else 
                    []

            let assignedNamedProps, unassignedNamedItems = 
                let returnedObjTy = if minfo.IsConstructor then minfo.ApparentEnclosingType else methodRetTy
                unassignedNamedItems |> List.splitChoose (fun (CallerNamedArg(id, e) as arg) -> 
                    let nm = id.idText
                    let pinfos = GetIntrinsicPropInfoSetsOfType infoReader (Some nm) ad AllowMultiIntfInstantiations.Yes IgnoreOverrides id.idRange returnedObjTy
                    let pinfos = pinfos |> ExcludeHiddenOfPropInfos g infoReader.amap m 
                    match pinfos with 
                    | [pinfo] when pinfo.HasSetter && not pinfo.IsIndexer -> 
                        let pminfo = pinfo.SetterMethod
                        let pminst = freshenMethInfo m pminfo
                        Choice1Of2(AssignedItemSetter(id, AssignedPropSetter(pinfo, pminfo, pminst), e))
                    | _ ->
                        let epinfos = 
                            match nameEnv with  
                            | Some ne -> ExtensionPropInfosOfTypeInScope ResultCollectionSettings.AllResults infoReader ne (Some nm) ad m returnedObjTy
                            | _ -> []
                        match epinfos with 
                        | [pinfo] when pinfo.HasSetter && not pinfo.IsIndexer -> 
                            let pminfo = pinfo.SetterMethod
                            let pminst = match minfo with
                                         | MethInfo.FSMeth(_, TType.TType_app(_, types), _, _) -> types
                                         | _ -> freshenMethInfo m pminfo

                            let pminst = match tyargsOpt with
                                         | Some(TType.TType_app(_, types)) -> types
                                         | _ -> pminst
                            Choice1Of2(AssignedItemSetter(id, AssignedPropSetter(pinfo, pminfo, pminst), e))
                        |  _ ->    
                            match infoReader.GetILFieldInfosOfType(Some(nm), ad, m, returnedObjTy) with
                            | finfo :: _ -> 
                                Choice1Of2(AssignedItemSetter(id, AssignedILFieldSetter(finfo), e))
                            | _ ->              
                              match infoReader.TryFindRecdOrClassFieldInfoOfType(nm, m, returnedObjTy) with
                              | ValueSome rfinfo -> 
                                  Choice1Of2(AssignedItemSetter(id, AssignedRecdFieldSetter(rfinfo), e))
                              | _ -> 
                                  Choice2Of2(arg))

            let names = namedCallerArgs |> List.map (fun (CallerNamedArg(nm, _)) -> nm.idText) 

            if (List.noRepeats String.order names).Length <> namedCallerArgs.Length then
                errorR(Error(FSComp.SR.typrelNamedArgumentHasBeenAssignedMoreThenOnce(), m))
                
            let argSet = { UnnamedCalledArgs=unnamedCalledArgs; UnnamedCallerArgs=unnamedCallerArgs; ParamArrayCalledArgOpt=paramArrayCalledArgOpt; ParamArrayCallerArgs=paramArrayCallerArgs; AssignedNamedArgs=assignedNamedArgs }

            (argSet, assignedNamedProps, unassignedNamedItems, attributeAssignedNamedItems, unnamedCalledOptArgs, unnamedCalledOutArgs))

    let argSets                     = argSetInfos |> List.map     (fun (x, _, _, _, _, _) -> x)
    let assignedNamedProps          = argSetInfos |> List.collect (fun (_, x, _, _, _, _) -> x)
    let unassignedNamedItems        = argSetInfos |> List.collect (fun (_, _, x, _, _, _) -> x)
    let attributeAssignedNamedItems = argSetInfos |> List.collect (fun (_, _, _, x, _, _) -> x)
    let unnamedCalledOptArgs        = argSetInfos |> List.collect (fun (_, _, _, _, x, _) -> x)
    let unnamedCalledOutArgs        = argSetInfos |> List.collect (fun (_, _, _, _, _, x) -> x)

    member x.infoReader = infoReader

    member x.amap = infoReader.amap

      /// the method we're attempting to call 
    member x.Method = minfo

      /// the instantiation of the method we're attempting to call 
    member x.CalledTyArgs = calledTyArgs

      /// the instantiation of the method we're attempting to call 
    member x.CalledTyparInst = 
        let tps = minfo.FormalMethodTypars 
        if tps.Length = calledTyArgs.Length then mkTyparInst tps calledTyArgs else []

      /// the formal instantiation of the method we're attempting to call 
    member x.CallerTyArgs = callerTyArgs

      /// The types of the actual object arguments, if any
    member x.CallerObjArgTys = callerObjArgTys

      /// The argument analysis for each set of curried arguments
    member x.ArgSets = argSets

      /// return type after implicit deference of byref returns is taken into account
    member x.CalledReturnTypeAfterByrefDeref = 
        let retTy = methodRetTy
        if isByrefTy g retTy then destByrefTy g retTy else retTy

      /// return type after tupling of out args is taken into account
    member x.CalledReturnTypeAfterOutArgTupling = 
        let retTy = x.CalledReturnTypeAfterByrefDeref
        if isNil unnamedCalledOutArgs then 
            retTy 
        else 
            let outArgTys = unnamedCalledOutArgs |> List.map (fun calledArg -> destByrefTy g calledArg.CalledArgumentType) 
            if isUnitTy g retTy then mkRefTupledTy g outArgTys
            else mkRefTupledTy g (retTy :: outArgTys)

      /// named setters
    member x.AssignedItemSetters = assignedNamedProps

      /// the property related to the method we're attempting to call, if any  
    member x.AssociatedPropertyInfo = pinfoOpt

      /// unassigned args
    member x.UnassignedNamedArgs = unassignedNamedItems

      /// args assigned to specify values for attribute fields and properties (these are not necessarily "property sets")
    member x.AttributeAssignedNamedArgs = attributeAssignedNamedItems

      /// unnamed called optional args: pass defaults for these
    member x.UnnamedCalledOptArgs = unnamedCalledOptArgs

      /// unnamed called out args: return these as part of the return tuple
    member x.UnnamedCalledOutArgs = unnamedCalledOutArgs

    static member GetMethod (x: CalledMeth<'T>) = x.Method

    member x.NumArgSets = x.ArgSets.Length

    member x.HasOptArgs = not (isNil x.UnnamedCalledOptArgs)

    member x.HasOutArgs = not (isNil x.UnnamedCalledOutArgs)

    member x.UsesParamArrayConversion = x.ArgSets |> List.exists (fun argSet -> argSet.ParamArrayCalledArgOpt.IsSome)

    member x.ParamArrayCalledArgOpt = x.ArgSets |> List.tryPick (fun argSet -> argSet.ParamArrayCalledArgOpt)

    member x.ParamArrayCallerArgs = x.ArgSets |> List.tryPick (fun argSet -> if Option.isSome argSet.ParamArrayCalledArgOpt then Some argSet.ParamArrayCallerArgs else None )

    member x.ParamArrayElementType = 
        assert (x.UsesParamArrayConversion)
        x.ParamArrayCalledArgOpt.Value.CalledArgumentType |> destArrayTy x.amap.g 

    member x.NumAssignedProps = x.AssignedItemSetters.Length

    member x.CalledObjArgTys(m) = 
        match x.Method.GetObjArgTypes(x.amap, m, x.CalledTyArgs) with 
        | [ thisArgTy ] when isByrefTy g thisArgTy -> [ destByrefTy g thisArgTy ]
        | res -> res

    member x.NumCalledTyArgs = x.CalledTyArgs.Length

    member x.NumCallerTyArgs = x.CallerTyArgs.Length 

    member x.AssignsAllNamedArgs = isNil x.UnassignedNamedArgs

    member x.HasCorrectArity =
      (x.NumCalledTyArgs = x.NumCallerTyArgs)  &&
      x.ArgSets |> List.forall (fun argSet -> argSet.NumUnnamedCalledArgs = argSet.NumUnnamedCallerArgs) 

    member x.HasCorrectGenericArity =
      (x.NumCalledTyArgs = x.NumCallerTyArgs)  

    member x.IsAccessible(m, ad) = 
        IsMethInfoAccessible x.amap m ad x.Method 

    member x.HasCorrectObjArgs(m) = 
        x.CalledObjArgTys(m).Length = x.CallerObjArgTys.Length 

    member x.IsCandidate(m, ad) =
        x.IsAccessible(m, ad) &&
        x.HasCorrectArity && 
        x.HasCorrectObjArgs(m) &&
        x.AssignsAllNamedArgs

    member x.AssignedUnnamedArgs = 
       // We use Seq.map2 to tolerate there being mismatched caller/called args
       x.ArgSets |> List.map (fun argSet -> 
           (argSet.UnnamedCalledArgs, argSet.UnnamedCallerArgs) ||> Seq.map2 (fun calledArg callerArg -> 
               { NamedArgIdOpt=None; CalledArg=calledArg; CallerArg=callerArg }) |> Seq.toList)

    member x.AssignedNamedArgs = 
       x.ArgSets |> List.map (fun argSet -> argSet.AssignedNamedArgs)

    member x.AllUnnamedCalledArgs = x.ArgSets |> List.collect (fun x -> x.UnnamedCalledArgs)

    member x.TotalNumUnnamedCalledArgs = x.ArgSets |> List.sumBy (fun x -> x.NumUnnamedCalledArgs)

    member x.TotalNumUnnamedCallerArgs = x.ArgSets |> List.sumBy (fun x -> x.NumUnnamedCallerArgs)

    member x.TotalNumAssignedNamedArgs = x.ArgSets |> List.sumBy (fun x -> x.NumAssignedNamedArgs)

let NamesOfCalledArgs (calledArgs: CalledArg list) = 
    calledArgs |> List.choose (fun x -> x.NameOpt) 

//-------------------------------------------------------------------------
// Helpers dealing with propagating type information in method overload resolution
//------------------------------------------------------------------------- 

type ArgumentAnalysis = 
    | NoInfo
    | ArgDoesNotMatch 
    | CallerLambdaHasArgTypes of TType list
    | CalledArgMatchesType of TType

let InferLambdaArgsForLambdaPropagation origRhsExpr = 
    let rec loop e = 
        match e with 
        | SynExpr.Lambda (_, _, _, rest, _) -> 1 + loop rest
        | SynExpr.MatchLambda _ -> 1
        | _ -> 0
    loop origRhsExpr

let ExamineArgumentForLambdaPropagation (infoReader: InfoReader) (arg: AssignedCalledArg<SynExpr>) =
    let g = infoReader.g
    // Find the explicit lambda arguments of the caller. Ignore parentheses.
    let argExpr = match arg.CallerArg.Expr with SynExpr.Paren (x, _, _, _) -> x  | x -> x
    let countOfCallerLambdaArg = InferLambdaArgsForLambdaPropagation argExpr
    // Adjust for Expression<_>, Func<_, _>, ...
    let adjustedCalledArgTy = AdjustCalledArgType infoReader false arg.CalledArg arg.CallerArg
    if countOfCallerLambdaArg > 0 then 
        // Decompose the explicit function type of the target
        let calledLambdaArgTys, _calledLambdaRetTy = Tastops.stripFunTy g adjustedCalledArgTy
        if calledLambdaArgTys.Length >= countOfCallerLambdaArg then 
            // success 
            CallerLambdaHasArgTypes calledLambdaArgTys
        elif isDelegateTy g (if isLinqExpressionTy g adjustedCalledArgTy then destLinqExpressionTy g adjustedCalledArgTy else adjustedCalledArgTy) then
            ArgDoesNotMatch  // delegate arity mismatch
        else
            NoInfo   // not a function type on the called side - no information
    else CalledArgMatchesType(adjustedCalledArgTy)  // not a lambda on the caller side - push information from caller to called

let ExamineMethodForLambdaPropagation (x: CalledMeth<SynExpr>) =
    let unnamedInfo = x.AssignedUnnamedArgs |> List.mapSquared (ExamineArgumentForLambdaPropagation x.infoReader)
    let namedInfo = x.AssignedNamedArgs |> List.mapSquared (fun arg -> (arg.NamedArgIdOpt.Value, ExamineArgumentForLambdaPropagation x.infoReader arg))
    if unnamedInfo |> List.existsSquared (function CallerLambdaHasArgTypes _ -> true | _ -> false) || 
       namedInfo |> List.existsSquared (function (_, CallerLambdaHasArgTypes _) -> true | _ -> false) then 
        Some (unnamedInfo, namedInfo)
    else
        None

//-------------------------------------------------------------------------
// Additional helpers for building method calls and doing TAST generation
//------------------------------------------------------------------------- 

/// Is this a 'base' call (in the sense of C#) 
let IsBaseCall objArgs = 
    match objArgs with 
    | [Expr.Val (v, _, _)] when v.BaseOrThisInfo  = BaseVal -> true
    | _ -> false
    
/// Compute whether we insert a 'coerce' on the 'this' pointer for an object model call 
/// For example, when calling an interface method on a struct, or a method on a constrained 
/// variable type. 
let ComputeConstrainedCallInfo g amap m (objArgs, minfo: MethInfo) =
    match objArgs with 
    | [objArgExpr] when not minfo.IsExtensionMember -> 
        let methObjTy = minfo.ApparentEnclosingType
        let objArgTy = tyOfExpr g objArgExpr
        if TypeDefinitelySubsumesTypeNoCoercion 0 g amap m methObjTy objArgTy 
           // Constrained calls to class types can only ever be needed for the three class types that 
           // are base types of value types
           || (isClassTy g methObjTy && 
                 (not (typeEquiv g methObjTy g.system_Object_ty || 
                       typeEquiv g methObjTy g.system_Value_ty ||
                       typeEquiv g methObjTy g.system_Enum_ty))) then 
            None
        else
            // The object argument is a value type or variable type and the target method is an interface or System.Object
            // type. A .NET 2.0 generic constrained call is required
            Some objArgTy
    | _ -> 
        None


/// Adjust the 'this' pointer before making a call 
/// Take the address of a struct, and coerce to an interface/base/constraint type if necessary 
let TakeObjAddrForMethodCall g amap (minfo: MethInfo) isMutable m objArgs f =
    let ccallInfo = ComputeConstrainedCallInfo g amap m (objArgs, minfo)

    let wrap, objArgs = 
        match objArgs with
        | [objArgExpr] ->
            let hasCallInfo = ccallInfo.IsSome
            let mustTakeAddress = hasCallInfo || minfo.ObjArgNeedsAddress(amap, m)
            let objArgTy = tyOfExpr g objArgExpr
            let wrap, objArgExpr', isReadOnly, _isWriteOnly = mkExprAddrOfExpr g mustTakeAddress hasCallInfo isMutable objArgExpr None m
            
            // Extension members and calls to class constraints may need a coercion for their object argument
            let objArgExpr' = 
              if not hasCallInfo &&
                 not (TypeDefinitelySubsumesTypeNoCoercion 0 g amap m minfo.ApparentEnclosingType objArgTy) then 
                  mkCoerceExpr(objArgExpr', minfo.ApparentEnclosingType, m, objArgTy)
              else
                  objArgExpr'

            // Check to see if the extension member uses the extending type as a byref.
            //     If so, make sure we don't allow readonly/immutable values to be passed byref from an extension member. 
            //     An inref will work though.
            if isReadOnly && mustTakeAddress && minfo.IsExtensionMember then
                minfo.TryObjArgByrefType(amap, m, minfo.FormalMethodInst)
                |> Option.iter (fun ty ->
                    if not (isInByrefTy g ty) then
                        errorR(Error(FSComp.SR.tcCannotCallExtensionMethodInrefToByref(minfo.DisplayName), m)))
                        

            wrap, [objArgExpr'] 

        | _ -> 
            id, objArgs
    let e, ety = f ccallInfo objArgs
    wrap e, ety

//-------------------------------------------------------------------------
// Build method calls.
//------------------------------------------------------------------------- 

//-------------------------------------------------------------------------
// Build calls 
//------------------------------------------------------------------------- 


/// Build an expression node that is a call to a .NET method. 
let BuildILMethInfoCall g amap m isProp (minfo: ILMethInfo) valUseFlags minst direct args = 
    let valu = isStructTy g minfo.ApparentEnclosingType
    let ctor = minfo.IsConstructor
    if minfo.IsClassConstructor then 
        error (InternalError (minfo.ILName+": cannot call a class constructor", m))
    let useCallvirt = 
        not valu && not direct && minfo.IsVirtual
    let isProtected = minfo.IsProtectedAccessibility
    let ilMethRef = minfo.ILMethodRef
    let newobj = ctor && (match valUseFlags with NormalValUse -> true | _ -> false)
    let exprTy = if ctor then minfo.ApparentEnclosingType else minfo.GetFSharpReturnTy(amap, m, minst)
    let retTy = if not ctor && ilMethRef.ReturnType = ILType.Void then [] else [exprTy]
    let isDllImport = minfo.IsDllImport g
    Expr.Op (TOp.ILCall (useCallvirt, isProtected, valu, newobj, valUseFlags, isProp, isDllImport, ilMethRef, minfo.DeclaringTypeInst, minst, retTy), [], args, m),
    exprTy

/// Build a call to the System.Object constructor taking no arguments,
let BuildObjCtorCall (g: TcGlobals) m =
    let ilMethRef = (mkILCtorMethSpecForTy(g.ilg.typ_Object, [])).MethodRef
    Expr.Op (TOp.ILCall (false, false, false, false, CtorValUsedAsSuperInit, false, true, ilMethRef, [], [], [g.obj_ty]), [], [], m)


/// Build a call to an F# method.
///
/// Consume the arguments in chunks and build applications.  This copes with various F# calling signatures
/// all of which ultimately become 'methods'.
///
/// QUERY: this looks overly complex considering that we are doing a fundamentally simple 
/// thing here. 
let BuildFSharpMethodApp g m (vref: ValRef) vexp vexprty (args: Exprs) =
    let arities =  (arityOfVal vref.Deref).AritiesOfArgs
    
    let args3, (leftover, retTy) = 
        ((args, vexprty), arities) ||> List.mapFold (fun (args, fty) arity -> 
            match arity, args with 
            | (0|1), [] when typeEquiv g (domainOfFunTy g fty) g.unit_ty -> mkUnit g m, (args, rangeOfFunTy g fty)
            | 0, (arg :: argst) -> 
                let msg = Layout.showL (Layout.sepListL (Layout.rightL (Layout.TaggedTextOps.tagText ";")) (List.map exprL args))
                warning(InternalError(sprintf "Unexpected zero arity, args = %s" msg, m))
                arg, (argst, rangeOfFunTy g fty)
            | 1, (arg :: argst) -> arg, (argst, rangeOfFunTy g fty)
            | 1, [] -> error(InternalError("expected additional arguments here", m))
            | _ -> 
                if args.Length < arity then
                    error(InternalError("internal error in getting arguments, n = "+string arity+", #args = "+string args.Length, m))
                let tupargs, argst = List.splitAt arity args
                let tuptys = tupargs |> List.map (tyOfExpr g) 
                (mkRefTupled g m tupargs tuptys),
                (argst, rangeOfFunTy g fty) )
    if not leftover.IsEmpty then error(InternalError("Unexpected "+string(leftover.Length)+" remaining arguments in method application", m))
    mkApps g ((vexp, vexprty), [], args3, m),
    retTy
    
/// Build a call to an F# method.
let BuildFSharpMethodCall g m (ty, vref: ValRef) valUseFlags minst args =
    let vexp = Expr.Val (vref, valUseFlags, m)
    let vexpty = vref.Type
    let tpsorig, tau =  vref.TypeScheme
    let vtinst = argsOfAppTy g ty @ minst
    if tpsorig.Length <> vtinst.Length then error(InternalError("BuildFSharpMethodCall: unexpected List.length mismatch", m))
    let expr = mkTyAppExpr m (vexp, vexpty) vtinst
    let exprty = instType (mkTyparInst tpsorig vtinst) tau
    BuildFSharpMethodApp g m vref expr exprty args
    

/// Make a call to a method info. Used by the optimizer and code generator to build 
/// calls to the type-directed solutions to member constraints.
let MakeMethInfoCall amap m minfo minst args =
    let valUseFlags = NormalValUse // correct unless if we allow wild trait constraints like "T has a ctor and can be used as a parent class" 
    match minfo with 
    | ILMeth(g, ilminfo, _) -> 
        let direct = not minfo.IsVirtual
        let isProp = false // not necessarily correct, but this is only used post-creflect where this flag is irrelevant 
        BuildILMethInfoCall g amap m isProp ilminfo valUseFlags minst  direct args |> fst
    | FSMeth(g, ty, vref, _) -> 
        BuildFSharpMethodCall g m (ty, vref) valUseFlags minst args |> fst
    | DefaultStructCtor(_, ty) -> 
       mkDefault (m, ty)
#if !NO_EXTENSIONTYPING
    | ProvidedMeth(amap, mi, _, m) -> 
        let isProp = false // not necessarily correct, but this is only used post-creflect where this flag is irrelevant 
        let ilMethodRef = Import.ImportProvidedMethodBaseAsILMethodRef amap m mi
        let isConstructor = mi.PUntaint((fun c -> c.IsConstructor), m)
        let valu = mi.PUntaint((fun c -> c.DeclaringType.IsValueType), m)
        let actualTypeInst = [] // GENERIC TYPE PROVIDERS: for generics, we would have something here
        let actualMethInst = [] // GENERIC TYPE PROVIDERS: for generics, we would have something here
        let ilReturnTys = Option.toList (minfo.GetCompiledReturnTy(amap, m, []))  // GENERIC TYPE PROVIDERS: for generics, we would have more here
        // REVIEW: Should we allow protected calls?
        Expr.Op (TOp.ILCall (false, false, valu, isConstructor, valUseFlags, isProp, false, ilMethodRef, actualTypeInst, actualMethInst, ilReturnTys), [], args, m)

#endif

#if !NO_EXTENSIONTYPING
// This imports a provided method, and checks if it is a known compiler intrinsic like "1 + 2"
let TryImportProvidedMethodBaseAsLibraryIntrinsic (amap: Import.ImportMap, m: range, mbase: Tainted<ProvidedMethodBase>) = 
    let methodName = mbase.PUntaint((fun x -> x.Name), m)
    let declaringType = Import.ImportProvidedType amap m (mbase.PApply((fun x -> x.DeclaringType), m))
    if isAppTy amap.g declaringType then 
        let declaringEntity = tcrefOfAppTy amap.g declaringType
        if not declaringEntity.IsLocalRef && ccuEq declaringEntity.nlr.Ccu amap.g.fslibCcu then
            match amap.g.knownIntrinsics.TryGetValue ((declaringEntity.LogicalName, methodName)) with 
            | true, vref -> Some vref
            | _ -> 
            match amap.g.knownFSharpCoreModules.TryGetValue declaringEntity.LogicalName with
            | true, modRef -> 
                modRef.ModuleOrNamespaceType.AllValsByLogicalName 
                |> Seq.tryPick (fun (KeyValue(_, v)) -> if v.CompiledName = methodName then Some (mkNestedValRef modRef v) else None)
            | _ -> None
        else
            None
    else
        None
#endif
        

/// Build an expression that calls a given method info. 
/// This is called after overload resolution, and also to call other 
/// methods such as 'setters' for properties. 
//   tcVal: used to convert an F# value into an expression. See tc.fs. 
//   isProp: is it a property get? 
//   minst: the instantiation to apply for a generic method 
//   objArgs: the 'this' argument, if any 
//   args: the arguments, if any 
let BuildMethodCall tcVal g amap isMutable m isProp minfo valUseFlags minst objArgs args =
    let direct = IsBaseCall objArgs

    TakeObjAddrForMethodCall g amap minfo isMutable m objArgs (fun ccallInfo objArgs -> 
        let allArgs = objArgs @ args
        let valUseFlags = 
            if direct && (match valUseFlags with NormalValUse -> true | _ -> false) then 
                VSlotDirectCall 
            else 
                match ccallInfo with
                | Some ty -> 
                    // printfn "possible constrained call to '%s' at %A" minfo.LogicalName m
                    PossibleConstrainedCall ty
                | None -> 
                    valUseFlags

        match minfo with 
#if !NO_EXTENSIONTYPING
        // By this time this is an erased method info, e.g. one returned from an expression
        // REVIEW: copied from tastops, which doesn't allow protected methods
        | ProvidedMeth (amap, providedMeth, _, _) -> 
            // TODO: there  is a fair bit of duplication here with mk_il_minfo_call. We should be able to merge these
                
            /// Build an expression node that is a call to a extension method in a generated assembly
            let enclTy = minfo.ApparentEnclosingType
            // prohibit calls to methods that are declared in specific array types (Get, Set, Address)
            // these calls are provided by the runtime and should not be called from the user code
            if isArrayTy g enclTy then
                let tpe = TypeProviderError(FSComp.SR.tcRuntimeSuppliedMethodCannotBeUsedInUserCode(minfo.DisplayName), providedMeth.TypeProviderDesignation, m)
                error tpe
            let valu = isStructTy g enclTy
            let isCtor = minfo.IsConstructor
            if minfo.IsClassConstructor then 
                error (InternalError (minfo.LogicalName + ": cannot call a class constructor", m))
            let useCallvirt = not valu && not direct && minfo.IsVirtual
            let isProtected = minfo.IsProtectedAccessiblity
            let exprTy = if isCtor then enclTy else minfo.GetFSharpReturnTy(amap, m, minst)
            match TryImportProvidedMethodBaseAsLibraryIntrinsic (amap, m, providedMeth) with 
            | Some fsValRef -> 
                //reraise() calls are converted to TOp.Reraise in the type checker. So if a provided expression includes a reraise call
                // we must put it in that form here.
                if valRefEq amap.g fsValRef amap.g.reraise_vref then
                    mkReraise m exprTy, exprTy
                else
                    let vexp, vexpty = tcVal fsValRef valUseFlags (minfo.DeclaringTypeInst @ minst) m
                    BuildFSharpMethodApp g m fsValRef vexp vexpty allArgs
            | None -> 
                let ilMethRef = Import.ImportProvidedMethodBaseAsILMethodRef amap m providedMeth
                let isNewObj = isCtor && (match valUseFlags with NormalValUse -> true | _ -> false)
                let actualTypeInst = 
                    if isRefTupleTy g enclTy then argsOfAppTy g (mkCompiledTupleTy g false (destRefTupleTy g enclTy))  // provided expressions can include method calls that get properties of tuple types
                    elif isFunTy g enclTy then [ domainOfFunTy g enclTy; rangeOfFunTy g enclTy ]  // provided expressions can call Invoke
                    else minfo.DeclaringTypeInst
                let actualMethInst = minst
                let retTy = if not isCtor && (ilMethRef.ReturnType = ILType.Void) then [] else [exprTy]
                let noTailCall = false
                let expr = Expr.Op (TOp.ILCall (useCallvirt, isProtected, valu, isNewObj, valUseFlags, isProp, noTailCall, ilMethRef, actualTypeInst, actualMethInst, retTy), [], allArgs, m)
                expr, exprTy

#endif
            
        // Build a call to a .NET method 
        | ILMeth(_, ilMethInfo, _) -> 
            BuildILMethInfoCall g amap m isProp ilMethInfo valUseFlags minst direct allArgs

        // Build a call to an F# method 
        | FSMeth(_, _, vref, _) -> 

            // Go see if this is a use of a recursive definition... Note we know the value instantiation 
            // we want to use so we pass that in order not to create a new one. 
            let vexp, vexpty = tcVal vref valUseFlags (minfo.DeclaringTypeInst @ minst) m
            BuildFSharpMethodApp g m vref vexp vexpty allArgs

        // Build a 'call' to a struct default constructor 
        | DefaultStructCtor (g, ty) -> 
            if not (TypeHasDefaultValue g m ty) then 
                errorR(Error(FSComp.SR.tcDefaultStructConstructorCall(), m))
            mkDefault (m, ty), ty)

//-------------------------------------------------------------------------
// Build delegate constructions (lambdas/functions to delegates)
//------------------------------------------------------------------------- 

/// Implements the elaborated form of adhoc conversions from functions to delegates at member callsites
let BuildNewDelegateExpr (eventInfoOpt: EventInfo option, g, amap, delegateTy, invokeMethInfo: MethInfo, delArgTys, f, fty, m) =
    let slotsig = invokeMethInfo.GetSlotSig(amap, m)
    let delArgVals, expr = 
        let topValInfo = ValReprInfo([], List.replicate (max 1 (List.length delArgTys)) ValReprInfo.unnamedTopArg, ValReprInfo.unnamedRetVal)

        // Try to pull apart an explicit lambda and use it directly 
        // Don't do this in the case where we're adjusting the arguments of a function used to build a .NET-compatible event handler 
        let lambdaContents = 
            if Option.isSome eventInfoOpt then 
                None 
            else 
                tryDestTopLambda g amap topValInfo (f, fty)        

        match lambdaContents with 
        | None -> 
        
            if List.exists (isByrefTy g) delArgTys then
                    error(Error(FSComp.SR.tcFunctionRequiresExplicitLambda(List.length delArgTys), m)) 

            let delArgVals = delArgTys |> List.mapi (fun i argty -> fst (mkCompGenLocal m ("delegateArg" + string i) argty)) 
            let expr = 
                let args = 
                    match eventInfoOpt with 
                    | Some einfo -> 
                        match delArgVals with 
                        | [] -> error(nonStandardEventError einfo.EventName m)
                        | h :: _ when not (isObjTy g h.Type) -> error(nonStandardEventError einfo.EventName m)
                        | h :: t -> [exprForVal m h; mkRefTupledVars g m t] 
                    | None -> 
                        if isNil delArgTys then [mkUnit g m] else List.map (exprForVal m) delArgVals
                mkApps g ((f, fty), [], args, m)
            delArgVals, expr
            
        | Some _ -> 
            let _, _, _, vsl, body, _ = IteratedAdjustArityOfLambda g amap topValInfo f
            List.concat vsl, body
            
    let meth = TObjExprMethod(slotsig, [], [], [delArgVals], expr, m)
    mkObjExpr(delegateTy, None, BuildObjCtorCall g m, [meth], [], m)

let CoerceFromFSharpFuncToDelegate g amap infoReader ad callerArgTy m callerArgExpr delegateTy =    
    let (SigOfFunctionForDelegate(invokeMethInfo, delArgTys, _, _)) = GetSigOfFunctionForDelegate infoReader delegateTy m ad
    BuildNewDelegateExpr (None, g, amap, delegateTy, invokeMethInfo, delArgTys, callerArgExpr, callerArgTy, m)


//-------------------------------------------------------------------------
// Import provided expressions
//------------------------------------------------------------------------- 


#if !NO_EXTENSIONTYPING
// This file is not a great place for this functionality to sit, it's here because of BuildMethodCall
module ProvidedMethodCalls =

    let private convertConstExpr g amap m (constant : Tainted<obj * ProvidedType>) =
        let (obj, objTy) = constant.PApply2(id, m)
        let ty = Import.ImportProvidedType amap m objTy
        let normTy = normalizeEnumTy g ty
        obj.PUntaint((fun v ->
            let fail() = raise (TypeProviderError(FSComp.SR.etUnsupportedConstantType(v.GetType().ToString()), constant.TypeProviderDesignation, m))
            try 
                if isNull v then mkNull m ty else
                let c = 
                    match v with
                    | _ when typeEquiv g normTy g.bool_ty -> Const.Bool(v :?> bool)
                    | _ when typeEquiv g normTy g.sbyte_ty -> Const.SByte(v :?> sbyte)
                    | _ when typeEquiv g normTy g.byte_ty -> Const.Byte(v :?> byte)
                    | _ when typeEquiv g normTy g.int16_ty -> Const.Int16(v :?> int16)
                    | _ when typeEquiv g normTy g.uint16_ty -> Const.UInt16(v :?> uint16)
                    | _ when typeEquiv g normTy g.int32_ty -> Const.Int32(v :?> int32)
                    | _ when typeEquiv g normTy g.uint32_ty -> Const.UInt32(v :?> uint32)
                    | _ when typeEquiv g normTy g.int64_ty -> Const.Int64(v :?> int64)
                    | _ when typeEquiv g normTy g.uint64_ty -> Const.UInt64(v :?> uint64)
                    | _ when typeEquiv g normTy g.nativeint_ty -> Const.IntPtr(v :?> int64)
                    | _ when typeEquiv g normTy g.unativeint_ty -> Const.UIntPtr(v :?> uint64)
                    | _ when typeEquiv g normTy g.float32_ty -> Const.Single(v :?> float32)
                    | _ when typeEquiv g normTy g.float_ty -> Const.Double(v :?> float)
                    | _ when typeEquiv g normTy g.char_ty -> Const.Char(v :?> char)
                    | _ when typeEquiv g normTy g.string_ty -> Const.String(v :?> string)
                    | _ when typeEquiv g normTy g.decimal_ty -> Const.Decimal(v :?> decimal)
                    | _ when typeEquiv g normTy g.unit_ty -> Const.Unit
                    | _ -> fail()
                Expr.Const (c, m, ty)
             with _ -> fail()
            ), range=m)

    /// Erasure over System.Type.
    ///
    /// This is a reimplementation of the logic of provided-type erasure, working entirely over (tainted, provided) System.Type
    /// values. This is used when preparing ParameterInfo objects to give to the provider in GetInvokerExpression. 
    /// These ParameterInfo have erased ParameterType - giving the provider an erased type makes it considerably easier 
    /// to implement a correct GetInvokerExpression.
    ///
    /// Ideally we would implement this operation by converting to an F# TType using ImportSystemType, and then erasing, and then converting
    /// back to System.Type. However, there is currently no way to get from an arbitrary F# TType (even the TType for 
    /// System.Object) to a System.Type to give to the type provider.
    let eraseSystemType (amap, m, inputType) = 
        let rec loop (st: Tainted<ProvidedType>) = 
            if st.PUntaint((fun st -> st.IsGenericParameter), m) then st
            elif st.PUntaint((fun st -> st.IsArray), m) then 
                let et = st.PApply((fun st -> st.GetElementType()), m)
                let rank = st.PUntaint((fun st -> st.GetArrayRank()), m)
                (loop et).PApply((fun st -> ProvidedType.CreateNoContext(if rank = 1 then st.RawSystemType.MakeArrayType() else st.RawSystemType.MakeArrayType(rank))), m)
            elif st.PUntaint((fun st -> st.IsByRef), m) then 
                let et = st.PApply((fun st -> st.GetElementType()), m)
                (loop et).PApply((fun st -> ProvidedType.CreateNoContext(st.RawSystemType.MakeByRefType())), m)
            elif st.PUntaint((fun st -> st.IsPointer), m) then 
                let et = st.PApply((fun st -> st.GetElementType()), m)
                (loop et).PApply((fun st -> ProvidedType.CreateNoContext(st.RawSystemType.MakePointerType())), m)
            else
                let isGeneric = st.PUntaint((fun st -> st.IsGenericType), m)
                let headType = if isGeneric then st.PApply((fun st -> st.GetGenericTypeDefinition()), m) else st
                // We import in order to use IsProvidedErasedTycon, to make sure we at least don't reinvent that 
                let headTypeAsFSharpType = Import.ImportProvidedNamedType amap m headType
                if headTypeAsFSharpType.IsProvidedErasedTycon then 
                    let baseType = 
                        st.PApply((fun st -> 
                            match st.BaseType with 
                            | null -> ProvidedType.CreateNoContext(typeof<obj>)  // it might be an interface
                            | st -> st), m)
                    loop baseType
                else
                    if isGeneric then 
                        let genericArgs = st.PApplyArray((fun st -> st.GetGenericArguments()), "GetGenericArguments", m) 
                        let typars = headTypeAsFSharpType.Typars(m)
                        // Drop the generic arguments that don't correspond to type arguments, i.e. are units-of-measure
                        let genericArgs = 
                            [| for (genericArg, tp) in Seq.zip genericArgs typars do
                                   if tp.Kind = TyparKind.Type then 
                                       yield genericArg |]

                        if genericArgs.Length = 0 then 
                            headType
                        else
                            let erasedArgTys = genericArgs |> Array.map loop
                            headType.PApply((fun st -> 
                                let erasedArgTys = erasedArgTys |> Array.map (fun a -> a.PUntaintNoFailure (fun x -> x.RawSystemType))
                                ProvidedType.CreateNoContext(st.RawSystemType.MakeGenericType erasedArgTys)), m)
                    else   
                        st
        loop inputType

    let convertProvidedExpressionToExprAndWitness tcVal (thisArg: Expr option,
                                                         allArgs: Exprs,
                                                         paramVars: Tainted<ProvidedVar>[],
                                                         g, amap, mut, isProp, isSuperInit, m,
                                                         expr: Tainted<ProvidedExpr>) = 
        let varConv =
            // note: using paramVars.Length as assumed initial size, but this might not 
            // be the optimal value; this wasn't checked before obsoleting Dictionary.ofList
            let dict = Dictionary.newWithSize paramVars.Length
            for v, e in Seq.zip (paramVars |> Seq.map (fun x -> x.PUntaint(id, m))) (Option.toList thisArg @ allArgs) do
                dict.Add(v, (None, e))
            dict
        let rec exprToExprAndWitness top (ea: Tainted<ProvidedExpr>) =
            let fail() = error(Error(FSComp.SR.etUnsupportedProvidedExpression(ea.PUntaint((fun etree -> etree.UnderlyingExpressionString), m)), m))
            match ea with
            | Tainted.Null -> error(Error(FSComp.SR.etNullProvidedExpression(ea.TypeProviderDesignation), m))
            |  _ ->
            match ea.PApplyOption((function ProvidedTypeAsExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let (expr, targetTy) = info.PApply2(id, m)
                let srcExpr = exprToExpr expr
                let targetTy = Import.ImportProvidedType amap m (targetTy.PApply(id, m)) 
                let sourceTy = Import.ImportProvidedType amap m (expr.PApply ((fun e -> e.Type), m)) 
                let te = mkCoerceIfNeeded g targetTy sourceTy srcExpr
                None, (te, tyOfExpr g te)
            | None -> 
            match ea.PApplyOption((function ProvidedTypeTestExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let (expr, targetTy) = info.PApply2(id, m)
                let srcExpr = exprToExpr expr
                let targetTy = Import.ImportProvidedType amap m (targetTy.PApply(id, m)) 
                let te = mkCallTypeTest g m targetTy srcExpr
                None, (te, tyOfExpr g te)
            | None -> 
            match ea.PApplyOption((function ProvidedIfThenElseExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let test, thenBranch, elseBranch = info.PApply3(id, m)
                let testExpr = exprToExpr test
                let ifTrueExpr = exprToExpr thenBranch
                let ifFalseExpr = exprToExpr elseBranch
                let te = mkCond NoSequencePointAtStickyBinding SuppressSequencePointAtTarget m (tyOfExpr g ifTrueExpr) testExpr ifTrueExpr ifFalseExpr
                None, (te, tyOfExpr g te)
            | None -> 
            match ea.PApplyOption((function ProvidedVarExpr x -> Some x | _ -> None), m) with
            | Some info ->  
                let _, vTe = varToExpr info
                None, (vTe, tyOfExpr g vTe)
            | None -> 
            match ea.PApplyOption((function ProvidedConstantExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let ce = convertConstExpr g amap m info
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedNewTupleExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let elems = info.PApplyArray(id, "GetInvokerExpresson", m)
                let elemsT = elems |> Array.map exprToExpr |> Array.toList
                let exprT = mkRefTupledNoTypes g m elemsT
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedNewArrayExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let ty, elems = info.PApply2(id, m)
                let tyT = Import.ImportProvidedType amap m ty
                let elems = elems.PApplyArray(id, "GetInvokerExpresson", m)
                let elemsT = elems |> Array.map exprToExpr |> Array.toList
                let exprT = Expr.Op (TOp.Array, [tyT], elemsT, m)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedTupleGetExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let inp, n = info.PApply2(id, m)
                let inpT = inp |> exprToExpr 
                // if type of expression is erased type then we need convert it to the underlying base type
                let typeOfExpr = 
                    let t = tyOfExpr g inpT
                    stripTyEqnsWrtErasure EraseMeasures g t
                let tupInfo, tysT = tryDestAnyTupleTy g typeOfExpr
                let exprT = mkTupleFieldGet g (tupInfo, inpT, tysT, n.PUntaint(id, m), m)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedLambdaExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v, b = info.PApply2(id, m)
                let vT = addVar v
                let bT = exprToExpr b
                removeVar v
                let exprT = mkLambda m vT (bT, tyOfExpr g bT)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedLetExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v, e, b = info.PApply3(id, m)
                let eT = exprToExpr  e
                let vT = addVar v
                let bT = exprToExpr  b
                removeVar v
                let exprT = mkCompGenLet m vT eT bT
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedVarSetExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v, e = info.PApply2(id, m)
                let eT = exprToExpr  e
                let vTopt, _ = varToExpr v
                match vTopt with 
                | None -> 
                    fail()
                | Some vT -> 
                    let exprT = mkValSet m (mkLocalValRef vT) eT 
                    None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedWhileLoopExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let guardExpr, bodyExpr = info.PApply2(id, m)
                let guardExprT = exprToExpr guardExpr
                let bodyExprT = exprToExpr bodyExpr
                let exprT = mkWhile g (SequencePointInfoForWhileLoop.NoSequencePointAtWhileLoop, SpecialWhileLoopMarker.NoSpecialWhileLoopMarker, guardExprT, bodyExprT, m)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedForIntegerRangeLoopExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let v, e1, e2, e3 = info.PApply4(id, m)
                let e1T = exprToExpr  e1
                let e2T = exprToExpr  e2
                let vT = addVar v
                let e3T = exprToExpr  e3
                removeVar v
                let exprT = mkFastForLoop g (SequencePointInfoForForLoop.NoSequencePointAtForLoop, m, vT, e1T, true, e2T, e3T)
                None, (exprT, tyOfExpr g exprT)
            | None -> 
            match ea.PApplyOption((function ProvidedNewDelegateExpr x -> Some x | _ -> None), m) with
            | Some info -> 
                let delegateTy, boundVars, delegateBodyExpr = info.PApply3(id, m)
                let delegateTyT = Import.ImportProvidedType amap m delegateTy
                let vs = boundVars.PApplyArray(id, "GetInvokerExpresson", m) |> Array.toList 
                let vsT = List.map addVar vs
                let delegateBodyExprT = exprToExpr delegateBodyExpr
                List.iter removeVar vs
                let lambdaExpr = mkLambdas m [] vsT (delegateBodyExprT, tyOfExpr g delegateBodyExprT)
                let lambdaExprTy = tyOfExpr g lambdaExpr
                let infoReader = InfoReader(g, amap)
                let exprT = CoerceFromFSharpFuncToDelegate g amap infoReader AccessorDomain.AccessibleFromSomewhere lambdaExprTy m lambdaExpr delegateTyT
                None, (exprT, tyOfExpr g exprT)
            | None -> 
#if PROVIDED_ADDRESS_OF
            match ea.PApplyOption((function ProvidedAddressOfExpr x -> Some x | _ -> None), m) with
            | Some e -> 
                let eT =  exprToExpr e
                let wrap,ce, _readonly, _writeonly = mkExprAddrOfExpr g true false DefinitelyMutates eT None m
                let ce = wrap ce
                None, (ce, tyOfExpr g ce)
            | None -> 
#endif
            match ea.PApplyOption((function ProvidedDefaultExpr x -> Some x | _ -> None), m) with
            | Some pty -> 
                let ty = Import.ImportProvidedType amap m pty
                let ce = mkDefault (m, ty)
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedCallExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                methodCallToExpr top ea info
            | None -> 
            match ea.PApplyOption((function ProvidedSequentialExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                let e1, e2 = info.PApply2(id, m)
                let e1T = exprToExpr e1
                let e2T = exprToExpr e2
                let ce = mkCompGenSequential m e1T e2T
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedTryFinallyExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                let e1, e2 = info.PApply2(id, m)
                let e1T = exprToExpr e1
                let e2T = exprToExpr e2
                let ce = mkTryFinally g (e1T, e2T, m, tyOfExpr g e1T, SequencePointInfoForTry.NoSequencePointAtTry, SequencePointInfoForFinally.NoSequencePointAtFinally)
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedTryWithExpr c -> Some c | _ -> None), m) with 
            | Some info ->
                let bT = exprToExpr (info.PApply((fun (x, _, _, _, _) -> x), m))
                let v1 = info.PApply((fun (_, x, _, _, _) -> x), m)
                let v1T = addVar v1
                let e1T = exprToExpr (info.PApply((fun (_, _, x, _, _) -> x), m))
                removeVar v1
                let v2 = info.PApply((fun (_, _, _, x, _) -> x), m)
                let v2T = addVar v2
                let e2T = exprToExpr (info.PApply((fun (_, _, _, _, x) -> x), m))
                removeVar v2
                let ce = mkTryWith g (bT, v1T, e1T, v2T, e2T, m, tyOfExpr g bT, SequencePointInfoForTry.NoSequencePointAtTry, SequencePointInfoForWith.NoSequencePointAtWith)
                None, (ce, tyOfExpr g ce)
            | None -> 
            match ea.PApplyOption((function ProvidedNewObjectExpr c -> Some c | _ -> None), m) with 
            | Some info -> 
                None, ctorCallToExpr info
            | None -> 
                fail()


        and ctorCallToExpr (ne: Tainted<_>) =    
            let (ctor, args) = ne.PApply2(id, m)
            let targetMethInfo = ProvidedMeth(amap, ctor.PApply((fun ne -> upcast ne), m), None, m)
            let objArgs = [] 
            let arguments = [ for ea in args.PApplyArray(id, "GetInvokerExpresson", m) -> exprToExpr ea ]
            let callExpr = BuildMethodCall tcVal g amap Mutates.PossiblyMutates m false targetMethInfo isSuperInit [] objArgs arguments
            callExpr

        and addVar (v: Tainted<ProvidedVar>) =    
            let nm = v.PUntaint ((fun v -> v.Name), m)
            let mut = v.PUntaint ((fun v -> v.IsMutable), m)
            let vRaw = v.PUntaint (id, m)
            let tyT = Import.ImportProvidedType amap m (v.PApply ((fun v -> v.Type), m))
            let vT, vTe = if mut then mkMutableCompGenLocal m nm tyT else mkCompGenLocal m nm tyT
            varConv.[vRaw] <- (Some vT, vTe)
            vT

        and removeVar (v: Tainted<ProvidedVar>) =    
            let vRaw = v.PUntaint (id, m)
            varConv.Remove vRaw |> ignore

        and methodCallToExpr top _origExpr (mce: Tainted<_>) =    
            let (objOpt, meth, args) = mce.PApply3(id, m)
            let targetMethInfo = ProvidedMeth(amap, meth.PApply((fun mce -> upcast mce), m), None, m)
            let objArgs = 
                match objOpt.PApplyOption(id, m) with
                | None -> []
                | Some objExpr -> [exprToExpr objExpr]

            let arguments = [ for ea in args.PApplyArray(id, "GetInvokerExpresson", m) -> exprToExpr ea ]
            let genericArguments = 
                if meth.PUntaint((fun m -> m.IsGenericMethod), m) then 
                    meth.PApplyArray((fun m -> m.GetGenericArguments()), "GetGenericArguments", m)  
                else 
                    [| |]
            let replacementGenericArguments = genericArguments |> Array.map (fun t->Import.ImportProvidedType amap m t) |> List.ofArray

            let mut         = if top then mut else PossiblyMutates
            let isSuperInit = if top then isSuperInit else ValUseFlag.NormalValUse
            let isProp      = if top then isProp else false
            let callExpr = BuildMethodCall tcVal g amap mut m isProp targetMethInfo isSuperInit replacementGenericArguments objArgs arguments
            Some meth, callExpr

        and varToExpr (pe: Tainted<ProvidedVar>) =    
            // sub in the appropriate argument
            // REVIEW: "thisArg" pointer should be first, if present
            let vRaw = pe.PUntaint(id, m)
            match varConv.TryGetValue vRaw with
            | true, v -> v
            | _ ->
                let typeProviderDesignation = ExtensionTyping.DisplayNameOfTypeProvider (pe.TypeProvider, m)
                error(NumberedError(FSComp.SR.etIncorrectParameterExpression(typeProviderDesignation, vRaw.Name), m))
                
        and exprToExpr expr =
            let _, (resExpr, _) = exprToExprAndWitness false expr
            resExpr

        exprToExprAndWitness true expr

        
    // fill in parameter holes in the expression   
    let TranslateInvokerExpressionForProvidedMethodCall tcVal (g, amap, mut, isProp, isSuperInit, mi: Tainted<ProvidedMethodBase>, objArgs, allArgs, m) =        
        let parameters = 
            mi.PApplyArray((fun mi -> mi.GetParameters()), "GetParameters", m)
        let paramTys = 
            parameters
            |> Array.map (fun p -> p.PApply((fun st -> st.ParameterType), m))
        let erasedParamTys = 
            paramTys
            |> Array.map (fun pty -> eraseSystemType (amap, m, pty))
        let paramVars = 
            erasedParamTys
            |> Array.mapi (fun i erasedParamTy -> erasedParamTy.PApply((fun ty -> ProvidedVar.Fresh("arg" + i.ToString(), ty)), m))


        // encode "this" as the first ParameterExpression, if applicable
        let thisArg, paramVars = 
            match objArgs with
            | [objArg] -> 
                let erasedThisTy = eraseSystemType (amap, m, mi.PApply((fun mi -> mi.DeclaringType), m))
                let thisVar = erasedThisTy.PApply((fun ty -> ProvidedVar.Fresh("this", ty)), m)
                Some objArg, Array.append [| thisVar |] paramVars
            | [] -> None, paramVars
            | _ -> failwith "multiple objArgs?"
            
        let ea = mi.PApplyWithProvider((fun (methodInfo, provider) -> ExtensionTyping.GetInvokerExpression(provider, methodInfo, [| for p in paramVars -> p.PUntaintNoFailure id |])), m)

        convertProvidedExpressionToExprAndWitness tcVal (thisArg, allArgs, paramVars, g, amap, mut, isProp, isSuperInit, m, ea)

            
    let BuildInvokerExpressionForProvidedMethodCall tcVal (g, amap, mi: Tainted<ProvidedMethodBase>, objArgs, mut, isProp, isSuperInit, allArgs, m) =
        try                   
            let methInfoOpt, (expr, retTy) = TranslateInvokerExpressionForProvidedMethodCall tcVal (g, amap, mut, isProp, isSuperInit, mi, objArgs, allArgs, m)

            let exprty = GetCompiledReturnTyOfProvidedMethodInfo amap m mi |> GetFSharpViewOfReturnType g
            let expr = mkCoerceIfNeeded g exprty retTy expr
            methInfoOpt, expr, exprty
        with
            | :? TypeProviderError as tpe ->
                let typeName = mi.PUntaint((fun mb -> mb.DeclaringType.FullName), m)
                let methName = mi.PUntaint((fun mb -> mb.Name), m)
                raise( tpe.WithContext(typeName, methName) )  // loses original stack trace
#endif



let RecdFieldInstanceChecks g amap ad m (rfinfo: RecdFieldInfo) = 
    if rfinfo.IsStatic then error (Error (FSComp.SR.tcStaticFieldUsedWhenInstanceFieldExpected(), m))
    CheckRecdFieldInfoAttributes g rfinfo m |> CommitOperationResult        
    CheckRecdFieldInfoAccessible amap m ad rfinfo

let ILFieldInstanceChecks  g amap ad m (finfo : ILFieldInfo) =
    if finfo.IsStatic then error (Error (FSComp.SR.tcStaticFieldUsedWhenInstanceFieldExpected(), m))
    CheckILFieldInfoAccessible g amap m ad finfo
    CheckILFieldAttributes g finfo m

let MethInfoChecks g amap isInstance tyargsOpt objArgs ad m (minfo: MethInfo)  =
    if minfo.IsInstance <> isInstance then
      if isInstance then 
        error (Error (FSComp.SR.csMethodIsNotAnInstanceMethod(minfo.LogicalName), m))
      else        
        error (Error (FSComp.SR.csMethodIsNotAStaticMethod(minfo.LogicalName), m))

    // keep the original accessibility domain to determine type accessibility
    let adOriginal = ad
    // Eliminate the 'protected' portion of the accessibility domain for instance accesses    
    let ad = 
        match objArgs, ad with 
        | [objArg], AccessibleFrom(paths, Some tcref) -> 
            let objArgTy = tyOfExpr g objArg 
            let ty = generalizedTyconRef tcref
            // We get to keep our rights if the type we're in subsumes the object argument type
            if TypeFeasiblySubsumesType 0 g amap m ty CanCoerce objArgTy then
                ad
            // We get to keep our rights if this is a base call
            elif IsBaseCall objArgs then 
                ad
            else
                AccessibleFrom(paths, None) 
        | _ -> ad

    if not (IsTypeAndMethInfoAccessible amap m adOriginal ad minfo) then 
      error (Error (FSComp.SR.tcMethodNotAccessible(minfo.LogicalName), m))

    if isAnyTupleTy g minfo.ApparentEnclosingType && not minfo.IsExtensionMember &&
        (minfo.LogicalName.StartsWithOrdinal("get_Item") || minfo.LogicalName.StartsWithOrdinal("get_Rest")) then
      warning (Error (FSComp.SR.tcTupleMemberNotNormallyUsed(), m))

    CheckMethInfoAttributes g m tyargsOpt minfo |> CommitOperationResult

exception FieldNotMutable of DisplayEnv * Tast.RecdFieldRef * range

let CheckRecdFieldMutation m denv (rfinfo: RecdFieldInfo) = 
    if not rfinfo.RecdField.IsMutable then error (FieldNotMutable(denv, rfinfo.RecdFieldRef, m))

