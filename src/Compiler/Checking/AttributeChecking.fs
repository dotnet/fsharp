// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Logic associated with checking "ObsoleteAttribute" and other attributes 
/// on items from name resolution
module internal FSharp.Compiler.AttributeChecking

open System
open System.Collections.Generic
open Internal.Utilities.Library
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler 
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Import
open FSharp.Compiler.Infos
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy

#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
open FSharp.Core.CompilerServices
#endif

let fail() = failwith "This custom attribute has an argument that cannot yet be converted using this API"

let rec private evalILAttribElem elem = 
    match elem with 
    | ILAttribElem.String (Some x) -> box x
    | ILAttribElem.String None -> null
    | ILAttribElem.Bool x -> box x
    | ILAttribElem.Char x -> box x
    | ILAttribElem.SByte x -> box x
    | ILAttribElem.Int16 x -> box x
    | ILAttribElem.Int32 x -> box x
    | ILAttribElem.Int64 x -> box x
    | ILAttribElem.Byte x -> box x
    | ILAttribElem.UInt16 x -> box x
    | ILAttribElem.UInt32 x -> box x
    | ILAttribElem.UInt64 x -> box x
    | ILAttribElem.Single x -> box x
    | ILAttribElem.Double x -> box x
    | ILAttribElem.Null -> null
    | ILAttribElem.Array (_, a) -> box [| for i in a -> evalILAttribElem i |]
    // TODO: typeof<..> in attribute values
    | ILAttribElem.Type (Some _t) -> fail() 
    | ILAttribElem.Type None -> null
    | ILAttribElem.TypeRef (Some _t) -> fail()
    | ILAttribElem.TypeRef None -> null

let rec private evalFSharpAttribArg g attribExpr = 
    match stripDebugPoints attribExpr with
    | Expr.Const (c, _, _) -> 
        match c with 
        | Const.Bool b -> box b
        | Const.SByte i -> box i
        | Const.Int16 i -> box  i
        | Const.Int32 i -> box i
        | Const.Int64 i -> box i  
        | Const.Byte i -> box i
        | Const.UInt16 i -> box i
        | Const.UInt32 i -> box i
        | Const.UInt64 i -> box i
        | Const.Single i -> box i
        | Const.Double i -> box i
        | Const.Char i -> box i
        | Const.Zero -> null
        | Const.String s ->  box s
        | _ -> fail()
    | Expr.Op (TOp.Array, _, a, _) -> box [| for i in a -> evalFSharpAttribArg g i |]
    | TypeOfExpr g ty -> box ty
    // TODO: | TypeDefOfExpr g ty
    | _ -> fail()

type AttribInfo = 
    | FSAttribInfo of TcGlobals * Attrib
    | ILAttribInfo of TcGlobals * Import.ImportMap * ILScopeRef * ILAttribute * range

    member x.Range = 
         match x with 
         | FSAttribInfo(_, attrib) -> attrib.Range
         | ILAttribInfo (_, _, _, _, m) -> m

    member x.TyconRef = 
         match x with 
         | FSAttribInfo(_g, Attrib(tcref, _, _, _, _, _, _)) -> tcref
         | ILAttribInfo (g, amap, scoref, a, m) -> 
             // We are skipping nullness check here because this reference is an attribute usage, nullness does not apply.
             let ty = RescopeAndImportILTypeSkipNullness scoref amap m [] a.Method.DeclaringType
             tcrefOfAppTy g ty

    member x.ConstructorArguments = 
         match x with 
         | FSAttribInfo(g, Attrib(_, _, unnamedArgs, _, _, _, _)) -> 
             unnamedArgs
             |> List.map (fun (AttribExpr(origExpr, evaluatedExpr)) -> 
                    let ty = tyOfExpr g origExpr
                    let obj = evalFSharpAttribArg g evaluatedExpr
                    ty, obj) 
         | ILAttribInfo (_g, amap, scoref, cattr, m) -> 
              let params_, _args = decodeILAttribData cattr 
              [ for argTy, arg in Seq.zip cattr.Method.FormalArgTypes params_ ->
                    // We are skipping nullness check here because this reference is an attribute usage, nullness does not apply.
                    let ty = RescopeAndImportILTypeSkipNullness scoref amap m [] argTy
                    let obj = evalILAttribElem arg
                    ty, obj ]

    member x.NamedArguments = 
         match x with 
         | FSAttribInfo(g, Attrib(_, _, _, namedArgs, _, _, _)) -> 
             namedArgs
             |> List.map (fun (AttribNamedArg(nm, _, isField, AttribExpr(origExpr, evaluatedExpr))) -> 
                    let ty = tyOfExpr g origExpr
                    let obj = evalFSharpAttribArg g evaluatedExpr
                    ty, nm, isField, obj) 
         | ILAttribInfo (_g, amap, scoref, cattr, m) -> 
              let _params_, namedArgs = decodeILAttribData cattr 
              [ for nm, argTy, isProp, arg in namedArgs ->
                    // We are skipping nullness check here because this reference is an attribute usage, nullness does not apply.
                    let ty = RescopeAndImportILTypeSkipNullness scoref amap m [] argTy
                    let obj = evalILAttribElem arg
                    let isField = not isProp 
                    ty, nm, isField, obj ]


/// Check custom attributes. This is particularly messy because custom attributes come in in three different
/// formats.
let AttribInfosOfIL g amap scoref m (attribs: ILAttributes) = 
    attribs.AsList()  |> List.map (fun a -> ILAttribInfo (g, amap, scoref, a, m))

let AttribInfosOfFS g attribs = 
    attribs |> List.map (fun a -> FSAttribInfo (g, a))

let GetAttribInfosOfEntity g amap m (tcref:TyconRef) = 
    match metadataOfTycon tcref.Deref with 
#if !NO_TYPEPROVIDERS
    // TODO: provided attributes
    | ProvidedTypeMetadata _info -> []
        //let provAttribs = info.ProvidedType.PApply((fun a -> (a :> IProvidedCustomAttributeProvider)), m)
        //match provAttribs.PUntaint((fun a -> a. .GetAttributeConstructorArgs(provAttribs.TypeProvider.PUntaintNoFailure(id), atref.FullName)), m) with
        //| Some args -> f3 args
        //| None -> None
#endif
    | ILTypeMetadata (TILObjectReprData(scoref, _, tdef)) -> 
        tdef.CustomAttrs |> AttribInfosOfIL g amap scoref m
    | FSharpOrArrayOrByrefOrTupleOrExnTypeMetadata -> 
        tcref.Attribs |> List.map (fun a -> FSAttribInfo (g, a))


let rec GetAttribInfosOfMethod amap m minfo = 
    match minfo with 
    | ILMeth (g, ilminfo, _) -> ilminfo.RawMetadata.CustomAttrs  |> AttribInfosOfIL g amap ilminfo.MetadataScope m
    | FSMeth (g, _, vref, _) -> vref.Attribs |> AttribInfosOfFS g 
    | MethInfoWithModifiedReturnType(mi,_) -> GetAttribInfosOfMethod amap m mi
    | DefaultStructCtor _ -> []
#if !NO_TYPEPROVIDERS
    // TODO: provided attributes
    | ProvidedMeth (_, _mi, _, _m) -> 
            []

#endif

let GetAttribInfosOfProp amap m pinfo = 
    match pinfo with 
    | ILProp ilpinfo -> ilpinfo.RawMetadata.CustomAttrs |> AttribInfosOfIL ilpinfo.TcGlobals amap ilpinfo.ILTypeInfo.ILScopeRef m
    | FSProp(g, _, Some vref, _) 
    | FSProp(g, _, _, Some vref) -> vref.Attribs |> AttribInfosOfFS g 
    | FSProp _ -> failwith "GetAttribInfosOfProp: unreachable"
#if !NO_TYPEPROVIDERS
    // TODO: provided attributes
    | ProvidedProp _ ->  []
#endif

let GetAttribInfosOfEvent amap m einfo = 
    match einfo with 
    | ILEvent ileinfo -> ileinfo.RawMetadata.CustomAttrs |> AttribInfosOfIL einfo.TcGlobals amap ileinfo.ILTypeInfo.ILScopeRef m
    | FSEvent(_, pi, _vref1, _vref2) -> GetAttribInfosOfProp amap m pi
#if !NO_TYPEPROVIDERS
    // TODO: provided attributes
    | ProvidedEvent _ -> []
#endif

/// Analyze three cases for attributes declared on methods: IL-declared attributes, F#-declared attributes and
/// provided attributes.
let rec BindMethInfoAttributes m minfo f1 f2 f3 = 
    ignore m; ignore f3
    match minfo with 
    | ILMeth (_, x, _) -> f1 x.RawMetadata.CustomAttrs 
    | FSMeth (_, _, vref, _) -> f2 vref.Attribs
    | MethInfoWithModifiedReturnType(mi,_) -> BindMethInfoAttributes m mi f1 f2 f3
    | DefaultStructCtor _ -> f2 []
#if !NO_TYPEPROVIDERS
    | ProvidedMeth (_, mi, _, _) -> f3 (mi.PApply((fun st -> (st :> IProvidedCustomAttributeProvider)), m))
#endif

/// Analyze three cases for attributes declared on methods: IL-declared attributes, F#-declared attributes and
/// provided attributes.
let TryBindMethInfoAttribute g (m: range) (AttribInfo(atref, _) as attribSpec) minfo f1 f2 f3 = 
#if NO_TYPEPROVIDERS
    // to prevent unused parameter warning
    ignore f3
#endif
    BindMethInfoAttributes m minfo 
        (fun ilAttribs -> TryDecodeILAttribute atref ilAttribs |> Option.bind f1)
        (fun fsAttribs -> TryFindFSharpAttribute g attribSpec fsAttribs |> Option.bind f2)
#if !NO_TYPEPROVIDERS
        (fun provAttribs -> 
            match provAttribs.PUntaint((fun a -> a.GetAttributeConstructorArgs(provAttribs.TypeProvider.PUntaintNoFailure(id), atref.FullName)), m) with
            | Some args -> f3 args
            | None -> None)
#else
        (fun _provAttribs -> None)
#endif

/// Try to find a specific attribute on a method, where the attribute accepts a string argument.
///
/// This is just used for the 'ConditionalAttribute' attribute
let TryFindMethInfoStringAttribute g (m: range) attribSpec minfo  =
    TryBindMethInfoAttribute g m attribSpec minfo 
                    (function [ILAttribElem.String (Some msg) ], _ -> Some msg | _ -> None) 
                    (function Attrib(_, _, [ AttribStringArg msg ], _, _, _, _) -> Some msg | _ -> None)
                    (function [ Some (:? string as msg : obj) ], _ -> Some msg | _ -> None)

/// Check if a method has a specific attribute.
let MethInfoHasAttribute g m attribSpec minfo  =
    TryBindMethInfoAttribute g m attribSpec minfo 
                    (fun _ -> Some ()) 
                    (fun _ -> Some ())
                    (fun _ -> Some ())
        |> Option.isSome

let private CheckCompilerFeatureRequiredAttribute (g: TcGlobals) cattrs msg m =
    // In some cases C# will generate both ObsoleteAttribute and CompilerFeatureRequiredAttribute.
    // Specifically, when default constructor is generated for class with any required members in them.
    // ObsoleteAttribute should be ignored if CompilerFeatureRequiredAttribute is present, and its name is "RequiredMembers".
    let (AttribInfo(tref,_)) = g.attrib_CompilerFeatureRequiredAttribute
    match TryDecodeILAttribute tref cattrs with
    | Some([ILAttribElem.String (Some featureName) ], _) when featureName = "RequiredMembers" ->
        CompleteD
    | _ ->
        ErrorD (ObsoleteDiagnostic(true, None, msg, None, m))
        
let private extractILAttribValueFrom name namedArgs   =
    match namedArgs with 
    | ExtractILAttributeNamedArg name (AttribElemStringArg v) -> Some v 
    | _ -> None

let private extractILAttributeInfo namedArgs =
    let diagnosticId = extractILAttribValueFrom "DiagnosticId" namedArgs
    let urlFormat = extractILAttribValueFrom "UrlFormat" namedArgs
    (diagnosticId, urlFormat)

let private CheckILExperimentalAttributes (g: TcGlobals) cattrs m =
    let (AttribInfo(tref,_)) = g.attrib_IlExperimentalAttribute
    match TryDecodeILAttribute tref cattrs with
    // [Experimental("DiagnosticId")]
    // [Experimental(diagnosticId: "DiagnosticId")]
    // [Experimental("DiagnosticId", UrlFormat = "UrlFormat")]
    // [Experimental(diagnosticId = "DiagnosticId", UrlFormat = "UrlFormat")]
    // Constructors deciding on DiagnosticId and UrlFormat properties.
    | Some ([ attribElement ], namedArgs) ->
        let diagnosticId = 
            match attribElement with 
            | ILAttribElem.String (Some msg) -> Some msg
            | ILAttribElem.String None
            | _ -> None

        let message = extractILAttribValueFrom "Message" namedArgs
        let urlFormat = extractILAttribValueFrom "UrlFormat" namedArgs

        WarnD(Experimental(message, diagnosticId, urlFormat, m))
    // Empty constructor or only UrlFormat property are not allowed.
    | Some _
    | None -> CompleteD

let private CheckILObsoleteAttributes (g: TcGlobals) isByrefLikeTyconRef cattrs m =
    if isByrefLikeTyconRef then
        CompleteD
    else
        let (AttribInfo(tref,_)) = g.attrib_SystemObsolete
        match TryDecodeILAttribute tref cattrs with
        // [Obsolete]
        // [Obsolete("Message")]
        // [Obsolete("Message", true)]
        // [Obsolete("Message", DiagnosticId = "DiagnosticId")]
        // [Obsolete("Message", DiagnosticId = "DiagnosticId", UrlFormat = "UrlFormat")]
        // [Obsolete(DiagnosticId = "DiagnosticId")]
        // [Obsolete(DiagnosticId = "DiagnosticId", UrlFormat = "UrlFormat")]
        // [Obsolete("Message", true, DiagnosticId = "DiagnosticId")]
        // [Obsolete("Message", true, DiagnosticId = "DiagnosticId", UrlFormat = "UrlFormat")]
        // Constructors deciding on IsError and Message properties.
        | Some ([ attribElement ], namedArgs) ->
            let diagnosticId, urlFormat = extractILAttributeInfo namedArgs
            let msg = 
                match attribElement with 
                | ILAttribElem.String (Some msg) -> Some msg
                | ILAttribElem.String None
                | _ -> None

            WarnD (ObsoleteDiagnostic(false, diagnosticId, msg, urlFormat, m))
        | Some ([ILAttribElem.String msg; ILAttribElem.Bool isError ], namedArgs) ->
            let diagnosticId, urlFormat = extractILAttributeInfo namedArgs
            if isError then
                if g.langVersion.SupportsFeature(LanguageFeature.RequiredPropertiesSupport) then
                    CheckCompilerFeatureRequiredAttribute g cattrs msg m
                else
                    ErrorD (ObsoleteDiagnostic(true, diagnosticId, msg, urlFormat, m))
            else
                WarnD (ObsoleteDiagnostic(false, diagnosticId, msg, urlFormat, m))
        // Only DiagnosticId, UrlFormat
        | Some (_, namedArgs) ->
            let diagnosticId, urlFormat = extractILAttributeInfo namedArgs
            WarnD(ObsoleteDiagnostic(false, diagnosticId, None, urlFormat, m))
        // No arguments
        | None -> CompleteD

/// Check IL attributes for Experimental, warnings as data
let private CheckILAttributes (g: TcGlobals) isByrefLikeTyconRef cattrs m =
    trackErrors {
        do! CheckILObsoleteAttributes g isByrefLikeTyconRef cattrs m
        do! CheckILExperimentalAttributes g cattrs m
    }

let private extractObsoleteAttributeInfo namedArgs =
    let extractILAttribValueFrom name namedArgs   =
        match namedArgs with 
        | ExtractAttribNamedArg name (AttribStringArg v) -> Some v 
        | _ -> None
    let diagnosticId = extractILAttribValueFrom "DiagnosticId" namedArgs
    let urlFormat = extractILAttribValueFrom "UrlFormat" namedArgs
    (diagnosticId, urlFormat)

let private CheckObsoleteAttributes g attribs m =
    trackErrors {
        match TryFindFSharpAttribute g g.attrib_SystemObsolete attribs with
        // [<Obsolete>]
        // [<Obsolete("Message")>]
        // [<Obsolete("Message", true)>]
        // [<Obsolete("Message", DiagnosticId = "DiagnosticId")>]
        // [<Obsolete("Message", DiagnosticId = "DiagnosticId", UrlFormat = "UrlFormat")>]
        // [<Obsolete(DiagnosticId = "DiagnosticId")>]
        // [<Obsolete(DiagnosticId = "DiagnosticId", UrlFormat = "UrlFormat")>]
        // [<Obsolete("Message", true, DiagnosticId = "DiagnosticId")>]
        // [<Obsolete("Message", true, DiagnosticId = "DiagnosticId", UrlFormat = "UrlFormat")>]
        // Constructors deciding on IsError and Message properties.
        | Some(Attrib(unnamedArgs= [ AttribStringArg s ]; propVal= namedArgs)) ->
            let diagnosticId, urlFormat = extractObsoleteAttributeInfo namedArgs
            do! WarnD(ObsoleteDiagnostic(false, diagnosticId, Some s, urlFormat, m))
        | Some(Attrib(unnamedArgs= [ AttribStringArg s; AttribBoolArg(isError) ]; propVal= namedArgs)) -> 
            let diagnosticId, urlFormat = extractObsoleteAttributeInfo namedArgs
            if isError then
                do! ErrorD (ObsoleteDiagnostic(true, diagnosticId, Some s, urlFormat, m))
            else
                do! WarnD (ObsoleteDiagnostic(false, diagnosticId, Some s, urlFormat, m))
        // Only DiagnosticId, UrlFormat
        | Some(Attrib(propVal= namedArgs)) ->
            let diagnosticId, urlFormat = extractObsoleteAttributeInfo namedArgs
            do! WarnD(ObsoleteDiagnostic(false, diagnosticId, None, urlFormat, m))
        | None ->  ()
    }
    
let private CheckCompilerMessageAttribute g attribs m =
    trackErrors {
        match TryFindFSharpAttribute g g.attrib_CompilerMessageAttribute attribs with
        | Some(Attrib(unnamedArgs= [ AttribStringArg s ; AttribInt32Arg n ]; propVal= namedArgs)) ->
            let msg = UserCompilerMessage(s, n, m)
            let isError = 
                match namedArgs with 
                | ExtractAttribNamedArg "IsError" (AttribBoolArg v) -> v 
                | _ -> false 
            // If we are using a compiler that supports nameof then error 3501 is always suppressed.
            // See attribute on FSharp.Core 'nameof'
            if n = 3501 then
                ()
            elif isError && (not g.compilingFSharpCore || n <> 1204) then
                do! ErrorD msg 
            else
                do! WarnD msg
        | _ -> 
            ()
    }
    
let private CheckFSharpExperimentalAttribute g attribs m =
    trackErrors {
        match TryFindFSharpAttribute g g.attrib_ExperimentalAttribute attribs with
        // [<Experimental("Message")>]
        | Some(Attrib(unnamedArgs= [ AttribStringArg(s) ])) ->
            let isExperimentalAttributeDisabled (s:string) =
                if g.compilingFSharpCore then
                    true
                else
                    g.langVersion.IsPreviewEnabled && (s.IndexOf("--langversion:preview", StringComparison.OrdinalIgnoreCase) >= 0)
            if not (isExperimentalAttributeDisabled s) then
                do! WarnD(Experimental(Some s, None, None, m))
        // Empty constructor is not allowed.
        | Some _
        | _ -> ()
    }
    
let private CheckUnverifiableAttribute g attribs m  =
    trackErrors {
        match TryFindFSharpAttribute g g.attrib_UnverifiableAttribute attribs with
        | Some _ -> 
            do! WarnD(PossibleUnverifiableCode(m))
        | _ -> ()
    }

/// Check F# attributes for 'ObsoleteAttribute', 'CompilerMessageAttribute' and 'ExperimentalAttribute',
/// returning errors and warnings as data
let CheckFSharpAttributes (g:TcGlobals) attribs m =
    if isNil attribs then CompleteD
    else
        trackErrors {
            do! CheckObsoleteAttributes g attribs m
            do! CheckCompilerMessageAttribute g attribs m
            do! CheckFSharpExperimentalAttribute g attribs m
            do! CheckUnverifiableAttribute g attribs m
        }

#if !NO_TYPEPROVIDERS
/// Check a list of provided attributes for 'ObsoleteAttribute', returning errors and warnings as data
let private CheckProvidedAttributes (g: TcGlobals) m (provAttribs: Tainted<IProvidedCustomAttributeProvider>)  = 
    let (AttribInfo(tref, _)) = g.attrib_SystemObsolete
    match provAttribs.PUntaint((fun a -> a.GetAttributeConstructorArgs(provAttribs.TypeProvider.PUntaintNoFailure(id), tref.FullName)), m) with
    | Some ([ Some (:? string as msg) ], _) -> WarnD(ObsoleteDiagnostic(false, None, Some msg, None, m))
    | Some ([ Some (:? string as msg); Some (:?bool as isError) ], _) ->
        if isError then 
            ErrorD (ObsoleteDiagnostic(true, None, Some msg, None, m))
        else 
            WarnD (ObsoleteDiagnostic(false, None, Some msg, None, m))
    | Some ([ None ], _) -> 
        WarnD(ObsoleteDiagnostic(false, None, None, None, m))
    | Some _ -> 
        WarnD(ObsoleteDiagnostic(false, None, None, None, m))
    | None -> 
        CompleteD
#endif

/// Indicate if a list of IL attributes contains 'ObsoleteAttribute'. Used to suppress the item in intellisense.
let CheckILAttributesForUnseen (g: TcGlobals) cattrs _m = 
    let (AttribInfo(tref, _)) = g.attrib_SystemObsolete
    Option.isSome (TryDecodeILAttribute tref cattrs)

/// Checks the attributes for CompilerMessageAttribute, which has an IsHidden argument that allows
/// items to be suppressed from intellisense.
let CheckFSharpAttributesForHidden g attribs = 
    not (isNil attribs) &&         
    (match TryFindFSharpAttribute g g.attrib_CompilerMessageAttribute attribs with
        | Some(Attrib(_, _, [AttribStringArg _; AttribInt32Arg messageNumber],
                    ExtractAttribNamedArg "IsHidden" (AttribBoolArg v), _, _, _)) -> 
            // Message number 62 is for "ML Compatibility". Items labelled with this are visible in intellisense
            // when mlCompatibility is set.
            v && not (messageNumber = 62 && g.mlCompatibility)
        | _ -> false)
    || 
    (match TryFindFSharpAttribute g g.attrib_ComponentModelEditorBrowsableAttribute attribs with
     | Some(Attrib(_, _, [AttribInt32Arg state], _, _, _, _)) -> state = int System.ComponentModel.EditorBrowsableState.Never
     | _ -> false)

/// Indicate if a list of F# attributes contains 'ObsoleteAttribute'. Used to suppress the item in intellisense.
let CheckFSharpAttributesForObsolete g attribs = 
    not (isNil attribs) && (HasFSharpAttribute g g.attrib_SystemObsolete attribs)

/// Indicate if a list of F# attributes contains 'ObsoleteAttribute'. Used to suppress the item in intellisense.
/// Also check the attributes for CompilerMessageAttribute, which has an IsHidden argument that allows
/// items to be suppressed from intellisense.
let CheckFSharpAttributesForUnseen g attribs _m = 
    not (isNil attribs) &&         
    (CheckFSharpAttributesForObsolete g attribs ||
        CheckFSharpAttributesForHidden g attribs)
      
#if !NO_TYPEPROVIDERS
/// Indicate if a list of provided attributes contains 'ObsoleteAttribute'. Used to suppress the item in intellisense.
let CheckProvidedAttributesForUnseen (provAttribs: Tainted<IProvidedCustomAttributeProvider>) m = 
    provAttribs.PUntaint((fun a -> a.GetAttributeConstructorArgs(provAttribs.TypeProvider.PUntaintNoFailure(id), !! typeof<ObsoleteAttribute>.FullName).IsSome), m)
#endif

/// Check the attributes associated with a property, returning warnings and errors as data.
let CheckPropInfoAttributes pinfo m = 
    match pinfo with
    | ILProp(ILPropInfo(_, pdef)) -> CheckILAttributes pinfo.TcGlobals false pdef.CustomAttrs m
    | FSProp(g, _, Some vref, _) 
    | FSProp(g, _, _, Some vref) -> CheckFSharpAttributes g vref.Attribs m
    | FSProp _ -> failwith "CheckPropInfoAttributes: unreachable"
#if !NO_TYPEPROVIDERS
    | ProvidedProp (amap, pi, m) ->  
        CheckProvidedAttributes amap.g m (pi.PApply((fun st -> (st :> IProvidedCustomAttributeProvider)), m))
         
#endif

      
/// Check the attributes associated with a IL field, returning warnings and errors as data.
let CheckILFieldAttributes g (finfo:ILFieldInfo) m = 
    match finfo with 
    | ILFieldInfo(_, pd) -> 
        CheckILAttributes g false pd.CustomAttrs m |> CommitOperationResult
#if !NO_TYPEPROVIDERS
    | ProvidedField (amap, fi, m) -> 
        CheckProvidedAttributes amap.g m (fi.PApply((fun st -> (st :> IProvidedCustomAttributeProvider)), m)) |> CommitOperationResult
#endif

/// Check the attributes on an entity, returning errors and warnings as data.
let CheckEntityAttributes g (tcref: TyconRef) m =    
    if tcref.IsILTycon then 
        CheckILAttributes g (isByrefLikeTyconRef g m tcref) tcref.ILTyconRawMetadata.CustomAttrs m
    else 
        CheckFSharpAttributes g tcref.Attribs m
        
let CheckILEventAttributes g (tcref: TyconRef) cattrs m  =    
    CheckILAttributes g (isByrefLikeTyconRef g m tcref) cattrs m

let CheckUnitOfMeasureAttributes g (measure: Measure) = 
    let checkAttribs tm m =
        let attribs =
            ListMeasureConOccsWithNonZeroExponents g true tm
            |> List.map fst
            |> List.map(_.Attribs)
            |> List.concat

        CheckFSharpAttributes g attribs m |> CommitOperationResult
                
    match measure with
    | Measure.Const(range = m) -> checkAttribs measure m
    | Measure.Inv ms -> checkAttribs measure ms.Range
    | Measure.One(m) -> checkAttribs measure m
    | Measure.RationalPower(measure = ms1) -> checkAttribs measure ms1.Range
    | Measure.Prod(measure1= ms1; measure2= ms2) ->
        checkAttribs ms1 ms1.Range
        checkAttribs ms2 ms2.Range
    | Measure.Var(typar) -> checkAttribs measure typar.Range

/// Check the attributes associated with a method, returning warnings and errors as data.
let CheckMethInfoAttributes g m tyargsOpt (minfo: MethInfo) =
    trackErrors {
        match stripTyEqns g minfo.ApparentEnclosingAppType with
        | TType_app(tcref, _, _) -> do! CheckEntityAttributes g tcref m 
        | _ -> ()

        let search =
            BindMethInfoAttributes m minfo 
                (fun ilAttribs -> Some(CheckILAttributes g false ilAttribs m)) 
                (fun fsAttribs -> 
                    let res =
                        trackErrors {
                             do! CheckFSharpAttributes g fsAttribs m
                             if Option.isNone tyargsOpt && HasFSharpAttribute g g.attrib_RequiresExplicitTypeArgumentsAttribute fsAttribs then
                                do! ErrorD(Error(FSComp.SR.tcFunctionRequiresExplicitTypeArguments(minfo.LogicalName), m))
                        }
                        
                    Some res) 
#if !NO_TYPEPROVIDERS
                (fun provAttribs -> Some (CheckProvidedAttributes g m provAttribs)) 
#else
                (fun _provAttribs -> None)
#endif 
        match search with
        | Some res -> do! res
        | None -> () // no attribute = no errors 
}

/// Indicate if a method has 'Obsolete', 'CompilerMessageAttribute' or 'TypeProviderEditorHideMethodsAttribute'. 
/// Used to suppress the item in intellisense.
let MethInfoIsUnseen g (m: range) (ty: TType) minfo = 
    let isUnseenByObsoleteAttrib () = 
        match BindMethInfoAttributes m minfo 
                (fun ilAttribs -> Some(CheckILAttributesForUnseen g ilAttribs m)) 
                (fun fsAttribs -> Some(CheckFSharpAttributesForUnseen g fsAttribs m))
#if !NO_TYPEPROVIDERS
                (fun provAttribs -> Some(CheckProvidedAttributesForUnseen provAttribs m))
#else
                (fun _provAttribs -> None)
#endif
                    with
        | Some res -> res
        | None -> false

    let isUnseenByHidingAttribute () = 
#if !NO_TYPEPROVIDERS
        not (isObjTyAnyNullness g ty) &&
        isAppTy g ty &&
        isObjTyAnyNullness g minfo.ApparentEnclosingType &&
        let tcref = tcrefOfAppTy g ty 
        match tcref.TypeReprInfo with 
        | TProvidedTypeRepr info -> 
            info.ProvidedType.PUntaint((fun st -> (st :> IProvidedCustomAttributeProvider).GetHasTypeProviderEditorHideMethodsAttribute(info.ProvidedType.TypeProvider.PUntaintNoFailure(id))), m)
        | _ -> 
        // This attribute check is done by name to ensure compilation doesn't take a dependency 
        // on Microsoft.FSharp.Core.CompilerServices.TypeProviderEditorHideMethodsAttribute.
        //
        // We are only interested in filtering out the method on System.Object, so it is sufficient
        // just to look at the attributes on IL methods.
        if tcref.IsILTycon then 
                tcref.ILTyconRawMetadata.CustomAttrs.AsArray()
                |> Array.exists (fun attr -> attr.Method.DeclaringType.TypeSpec.Name = !! typeof<TypeProviderEditorHideMethodsAttribute>.FullName)
        else 
            false
#else
        ty |> ignore
        false
#endif

    isUnseenByObsoleteAttrib () || isUnseenByHidingAttribute ()

/// Indicate if a property has 'Obsolete' or 'CompilerMessageAttribute'.
/// Used to suppress the item in intellisense.
let PropInfoIsUnseen m pinfo = 
    match pinfo with
    | ILProp (ILPropInfo(_, pdef) as ilpinfo) -> 
        // Properties on .NET tuple types are resolvable but unseen
        isAnyTupleTy pinfo.TcGlobals ilpinfo.ILTypeInfo.ToType || 
        CheckILAttributesForUnseen pinfo.TcGlobals pdef.CustomAttrs m
    | FSProp (g, _, Some vref, _) 
    | FSProp (g, _, _, Some vref) -> CheckFSharpAttributesForUnseen g vref.Attribs m
    | FSProp _ -> failwith "CheckPropInfoAttributes: unreachable"
#if !NO_TYPEPROVIDERS
    | ProvidedProp (_amap, pi, m) -> 
        CheckProvidedAttributesForUnseen (pi.PApply((fun st -> (st :> IProvidedCustomAttributeProvider)), m)) m
#endif

/// Check the attributes on a union case, returning errors and warnings as data.
let CheckUnionCaseAttributes g (x:UnionCaseRef) m =
    trackErrors {
        do! CheckEntityAttributes g x.TyconRef m
        do! CheckFSharpAttributes g x.Attribs m
    }

/// Check the attributes on a record field, returning errors and warnings as data.
let CheckRecdFieldAttributes g (x:RecdFieldRef) m =
    trackErrors {
        do! CheckEntityAttributes g x.TyconRef m 
        do! CheckFSharpAttributes g x.PropertyAttribs m
        do! CheckFSharpAttributes g x.RecdField.FieldAttribs m
    }

/// Check the attributes on an F# value, returning errors and warnings as data.
let CheckValAttributes g (x:ValRef) m =
    CheckFSharpAttributes g x.Attribs m

/// Check the attributes on a record field, returning errors and warnings as data.
let CheckRecdFieldInfoAttributes g (x:RecdFieldInfo) m =
    CheckRecdFieldAttributes g x.RecdFieldRef m

// Identify any security attributes
let IsSecurityAttribute (g: TcGlobals) amap (casmap : IDictionary<Stamp, bool>) (Attrib(tcref, _, _, _, _, _, _)) m =
    // There's no CAS on Silverlight, so we have to be careful here
    match g.attrib_SecurityAttribute with
    | None -> false
    | Some attr ->
        match attr.TyconRef.TryDeref with
        | ValueSome _ -> 
            let tcs = tcref.Stamp
            match casmap.TryGetValue tcs with
            | true, c -> c
            | _ ->
                let exists = ExistsInEntireHierarchyOfType (fun t -> typeEquiv g t (mkWoNullAppTy attr.TyconRef [])) g amap m AllowMultiIntfInstantiations.Yes (mkWoNullAppTy tcref [])          
                casmap[tcs] <- exists
                exists
        | ValueNone -> false  

let IsSecurityCriticalAttribute g (Attrib(tcref, _, _, _, _, _, _)) =
    (tyconRefEq g tcref g.attrib_SecurityCriticalAttribute.TyconRef || tyconRefEq g tcref g.attrib_SecuritySafeCriticalAttribute.TyconRef)

// Identify any AssemblyVersion attributes
let IsAssemblyVersionAttribute (g: TcGlobals) (Attrib(tcref, _, _, _, _, _, _)) =

    match g.TryFindSysAttrib("System.Reflection.AssemblyVersionAttribute") with
    | None -> false
    | Some attr ->
        attr.TyconRef.CompiledRepresentationForNamedType.QualifiedName = tcref.CompiledRepresentationForNamedType.QualifiedName
