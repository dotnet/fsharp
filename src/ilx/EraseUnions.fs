// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// -------------------------------------------------------------------- 
// Erase discriminated unions.
// -------------------------------------------------------------------- 


module internal Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.EraseUnions

open System.Collections.Generic
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX
open Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types
open Microsoft.FSharp.Compiler.AbstractIL.Morphs 


[<Literal>]
let TagNil = 0
[<Literal>]
let TagCons = 1
[<Literal>]
let ALT_NAME_CONS = "Cons"

type DiscriminationTechnique =
   /// Indicates a special representation for the F# list type where the "empty" value has a tail field of value null
   | TailOrNull
   /// Indicates a type with either number of cases < 4, and not a single-class type with an integer tag (IntegerTag)
   | RuntimeTypes
   /// Indicates a type with a single case, e.g. ``type X = ABC of string * int``
   | SingleCase
   /// Indicates a type with either cases >= 4, or a type like
   //     type X = A | B | C 
   //  or type X = A | B | C of string
   // where at most one case is non-nullary.  These can be represented using a single
   // class (no subclasses), but an integer tag is stored to discriminate between the objects.
   | IntegerTag

// A potentially useful additional representation trades an extra integer tag in the root type
// for faster discrimination, and in the important single-non-nullary constructor case
//
//     type Tree = Tip | Node of int * Tree * Tree
//
// it also flattens so the fields for "Node" are stored in the base class, meanign that no type casts
// are needed to access the data.  
//
// However, it can't be enabled because it suppresses the generation 
// of C#-facing nested types for the non-nullary case. This could be enabled
// in a binary compatible way by ensuring we continue to generate the C# facing types and use 
// them as the instance types, but still store all field elements in the base type. Additional
// accessors would be needed to access these fields directly, akin to HeadOrDefault and TailOrNull.

// This functor helps us make representation decisions for F# union type compilation
type UnionReprDecisions<'Union,'Alt,'Type>
          (getAlternatives: 'Union->'Alt[],
           nullPermitted:'Union->bool,
           isNullary:'Alt->bool,
           isList:'Union->bool,
           isStruct:'Union->bool,
           nameOfAlt : 'Alt -> string,
           makeRootType: 'Union -> 'Type,
           makeNestedType: 'Union * string -> 'Type) =

    static let TaggingThresholdFixedConstant = 4

    member repr.RepresentAllAlternativesAsConstantFieldsInRootClass cu = 
        cu |> getAlternatives |> Array.forall isNullary

    member repr.DiscriminationTechnique cu = 
        if isList cu then 
            TailOrNull
        else
            let alts = getAlternatives cu
            if alts.Length = 1 then 
                SingleCase
            elif 
                not (isStruct cu) &&
                alts.Length < TaggingThresholdFixedConstant &&
                not (repr.RepresentAllAlternativesAsConstantFieldsInRootClass cu)  then 
                RuntimeTypes
            else
                IntegerTag

    // WARNING: this must match IsUnionTypeWithNullAsTrueValue in the F# compiler 
    member repr.RepresentAlternativeAsNull (cu,alt) = 
        let alts = getAlternatives cu
        nullPermitted cu &&
        (repr.DiscriminationTechnique cu  = RuntimeTypes) && (* don't use null for tags, lists or single-case  *)
        Array.existsOne isNullary alts  &&
        Array.exists (isNullary >> not) alts  &&
        isNullary alt  (* is this the one? *)

    member repr.RepresentOneAlternativeAsNull cu = 
        let alts = getAlternatives cu
        nullPermitted cu &&
        alts |> Array.existsOne (fun alt -> repr.RepresentAlternativeAsNull (cu,alt))

    member repr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull (cu,alt) = 
        // Check all nullary constructors are being represented without using sub-classes 
        let alts = getAlternatives cu
        not (isStruct cu) &&
        not (isNullary alt) &&
        (alts |> Array.forall (fun alt2 -> not (isNullary alt2) || repr.RepresentAlternativeAsNull (cu,alt2))) &&
        // Check this is the one and only non-nullary constructor 
        Array.existsOne (isNullary >> not) alts

    member repr.RepresentAlternativeAsFreshInstancesOfRootClass (cu,alt) = 
        // Flattening
        isStruct cu ||
        // Check all nullary constructors are being represented without using sub-classes 
        (isList cu  && nameOfAlt alt = ALT_NAME_CONS) ||
        repr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull (cu, alt) 

    member repr.RepresentAlternativeAsConstantFieldInTaggedRootClass (cu,alt) = 
        not (isStruct cu) &&
        isNullary alt &&
        not (repr.RepresentAlternativeAsNull (cu,alt))  &&
        (repr.DiscriminationTechnique cu <> RuntimeTypes)

    member repr.Flatten cu = 
        isStruct cu

    member repr.OptimizeAlternativeToRootClass (cu,alt) = 
        // The list type always collapses to the root class 
        isList cu ||
        // Structs are always flattened
        repr.Flatten cu ||
        repr.RepresentAllAlternativesAsConstantFieldsInRootClass cu ||
        repr.RepresentAlternativeAsConstantFieldInTaggedRootClass (cu,alt) ||
        repr.RepresentAlternativeAsFreshInstancesOfRootClass(cu,alt)
      
    member repr.MaintainPossiblyUniqueConstantFieldForAlternative(cu,alt) = 
        not (isStruct cu) && 
        not (repr.RepresentAlternativeAsNull (cu,alt)) &&
        isNullary alt

    member repr.TypeForAlternative (cuspec,alt) =
        if repr.OptimizeAlternativeToRootClass (cuspec,alt) || repr.RepresentAlternativeAsNull (cuspec,alt) then 
            makeRootType cuspec
        else 
            let altName = nameOfAlt alt 
            // Add "_" if the thing is nullary or if it is 'List._Cons', which is special because it clashes with the name of the static method "Cons"
            let nm = if isNullary alt || isList cuspec then "_"+altName else altName
            makeNestedType (cuspec, nm)


let baseTyOfUnionSpec (cuspec : IlxUnionSpec) = 
    mkILNamedTyRaw cuspec.Boxity cuspec.TypeRef cuspec.GenericArgs

let mkMakerName (cuspec: IlxUnionSpec) nm = 
    match cuspec.HasHelpers with
    | SpecialFSharpListHelpers 
    | SpecialFSharpOptionHelpers ->  nm // Leave 'Some', 'None', 'Cons', 'Empty' as is
    | AllHelpers 
    | NoHelpers -> "New" + nm

let mkCasesTypeRef (cuspec: IlxUnionSpec) = cuspec.TypeRef

let cuspecRepr = 
    UnionReprDecisions
        ((fun (cuspec:IlxUnionSpec) -> cuspec.AlternativesArray),
         (fun (cuspec:IlxUnionSpec) -> cuspec.IsNullPermitted), 
         (fun (alt:IlxUnionAlternative) -> alt.IsNullary),
         (fun cuspec -> cuspec.HasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers),
         (fun cuspec -> cuspec.Boxity = ILBoxity.AsValue),
         (fun (alt:IlxUnionAlternative) -> alt.Name),
         (fun cuspec -> cuspec.EnclosingType),
         (fun (cuspec,nm) -> mkILNamedTyRaw cuspec.Boxity (mkILTyRefInTyRef (mkCasesTypeRef cuspec, nm)) cuspec.GenericArgs))

type NoTypesGeneratedViaThisReprDecider = NoTypesGeneratedViaThisReprDecider
let cudefRepr = 
    UnionReprDecisions
        ((fun (_td,cud) -> cud.cudAlternatives),
         (fun (_td,cud) -> cud.cudNullPermitted), 
         (fun (alt:IlxUnionAlternative) -> alt.IsNullary),
         (fun (_td,cud) -> cud.cudHasHelpers = IlxUnionHasHelpers.SpecialFSharpListHelpers),
         (fun (td,_cud) -> match td.tdKind with ILTypeDefKind.ValueType -> true | _ -> false),
         (fun (alt:IlxUnionAlternative) -> alt.Name),
         (fun (_td,_cud) -> NoTypesGeneratedViaThisReprDecider),
         (fun ((_td,_cud),_nm) -> NoTypesGeneratedViaThisReprDecider))


let mkTesterName nm = "Is" + nm
let tagPropertyName = "Tag"

let mkUnionCaseFieldId (fdef: IlxUnionField) = 
    // Use the lower case name of a field or constructor as the field/parameter name if it differs from the uppercase name
    fdef.LowerName, fdef.Type

let refToFieldInTy ty (nm, fldTy) = mkILFieldSpecInTy (ty, nm, fldTy)

let formalTypeArgs (baseTy:ILType) = ILList.mapi (fun i _ -> mkILTyvarTy (uint16 i)) baseTy.GenericArgs
let constFieldName nm = "_unique_" + nm 
let constFormalFieldTy (baseTy:ILType) = 
    mkILNamedTyRaw baseTy.Boxity baseTy.TypeRef (formalTypeArgs baseTy)

let mkConstFieldSpecFromId (baseTy:ILType) constFieldId = 
    refToFieldInTy baseTy constFieldId

let mkConstFieldSpec nm (baseTy:ILType) = 
    mkConstFieldSpecFromId baseTy (constFieldName nm, constFormalFieldTy baseTy) 


let tyForAlt cuspec alt = cuspecRepr.TypeForAlternative(cuspec,alt)

let GetILTypeForAlternative cuspec alt = cuspecRepr.TypeForAlternative(cuspec,cuspec.Alternative alt) 

let mkTagFieldType ilg _cuspec = ilg.typ_Int32
let mkTagFieldFormalType ilg _cuspec = ilg.typ_Int32
let mkTagFieldId ilg cuspec = "_tag", mkTagFieldType ilg cuspec
let mkTailOrNullId baseTy = "tail", constFormalFieldTy baseTy


let altOfUnionSpec (cuspec:IlxUnionSpec) cidx =
    try cuspec.Alternative cidx 
    with _ -> failwith ("alternative " + string cidx + " not found") 

// Nullary cases on types with helpers do not reveal their underlying type even when 
// using runtime type discrimination, because the underlying type is never needed from 
// C# code and pollutes the visible API surface. In this case we must discriminate by 
// calling the IsFoo helper. This only applies to discriminations outside the 
// assembly where the type is defined (indicated by 'avoidHelpers' flag - if this is true
// then the reference is intra-assembly).
let doesRuntimeTypeDiscriminateUseHelper avoidHelpers (cuspec: IlxUnionSpec) (alt: IlxUnionAlternative) = 
    not avoidHelpers && alt.IsNullary && cuspec.HasHelpers = IlxUnionHasHelpers.AllHelpers

let mkRuntimeTypeDiscriminate ilg avoidHelpers cuspec alt altName altTy = 
    let useHelper = doesRuntimeTypeDiscriminateUseHelper avoidHelpers cuspec alt
    if useHelper then 
        let baseTy = baseTyOfUnionSpec cuspec
        [ mkNormalCall (mkILNonGenericInstanceMethSpecInTy (baseTy, "get_" + mkTesterName altName, [], ilg.typ_Bool))  ]
    else
        [ I_isinst altTy; AI_ldnull; AI_cgt_un ]

let mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy after = 
    let useHelper = doesRuntimeTypeDiscriminateUseHelper avoidHelpers cuspec alt
    match after with 
    | I_brcmp (BI_brfalse,_) 
    | I_brcmp (BI_brtrue,_) when not useHelper -> 
        [ I_isinst altTy; after ]
    | _ -> 
        mkRuntimeTypeDiscriminate ilg avoidHelpers cuspec alt altName altTy @ [ after ]

let mkGetTagFromField ilg cuspec baseTy = 
    [ mkNormalLdfld (refToFieldInTy baseTy (mkTagFieldId ilg cuspec)) ]

let adjustFieldName hasHelpers nm = 
    match hasHelpers, nm  with
    | SpecialFSharpListHelpers, "Head" -> "HeadOrDefault" 
    | SpecialFSharpListHelpers, "Tail" -> "TailOrNull" 
    | _ -> nm

let mkLdData (avoidHelpers, cuspec, cidx, fidx) = 
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let fieldDef = alt.FieldDef fidx
    if avoidHelpers then 
        [ mkNormalLdfld (mkILFieldSpecInTy(altTy,fieldDef.LowerName, fieldDef.Type))  ]
    else
        [ mkNormalCall (mkILNonGenericInstanceMethSpecInTy(altTy,"get_" + adjustFieldName cuspec.HasHelpers fieldDef.Name,[],fieldDef.Type))  ]

let mkLdDataAddr (avoidHelpers, cuspec, cidx, fidx) = 
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let fieldDef = alt.FieldDef fidx
    if avoidHelpers then 
        [ mkNormalLdflda (mkILFieldSpecInTy(altTy,fieldDef.LowerName, fieldDef.Type))  ]
    else
        failwith (sprintf "can't load address using helpers, for fieldDef %s" fieldDef.LowerName)

let mkGetTailOrNull avoidHelpers cuspec = 
    mkLdData (avoidHelpers, cuspec, 1, 1) (* tail is in alternative 1, field number 1 *)
        

let mkGetTagFromHelpers ilg (cuspec: IlxUnionSpec) = 
    let baseTy = baseTyOfUnionSpec cuspec
    if cuspecRepr.RepresentOneAlternativeAsNull cuspec then
        mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, "Get" + tagPropertyName, [baseTy], mkTagFieldFormalType ilg cuspec))  
    else
        mkNormalCall (mkILNonGenericInstanceMethSpecInTy(baseTy, "get_" + tagPropertyName, [], mkTagFieldFormalType ilg cuspec))  

let mkGetTag ilg (cuspec: IlxUnionSpec) = 
    match cuspec.HasHelpers with
    | AllHelpers -> [ mkGetTagFromHelpers ilg cuspec ]
    | _hasHelpers -> mkGetTagFromField ilg cuspec (baseTyOfUnionSpec cuspec)

let mkCeqThen after = 
    match after with 
    | I_brcmp (BI_brfalse,a) -> [I_brcmp (BI_bne_un,a)]
    | I_brcmp (BI_brtrue,a) ->  [I_brcmp (BI_beq,a)]
    | _ -> [AI_ceq; after]


let mkTagDiscriminate ilg cuspec _baseTy cidx = 
    mkGetTag ilg cuspec @ [ mkLdcInt32 cidx; AI_ceq ]

let mkTagDiscriminateThen ilg cuspec cidx after = 
    mkGetTag ilg cuspec @ [ mkLdcInt32 cidx ] @ mkCeqThen after

let convNewDataInstrInternal ilg cuspec cidx = 
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let altName = alt.Name

    if cuspecRepr.RepresentAlternativeAsNull (cuspec,alt) then 
        [ AI_ldnull  ]
    elif cuspecRepr.MaintainPossiblyUniqueConstantFieldForAlternative (cuspec,alt) then 
        let baseTy = baseTyOfUnionSpec cuspec
        [ I_ldsfld (Nonvolatile,mkConstFieldSpec altName baseTy) ]
    elif cuspecRepr.RepresentAlternativeAsFreshInstancesOfRootClass (cuspec,alt) then 
        let baseTy = baseTyOfUnionSpec cuspec
        let instrs, tagfields = 
            match cuspecRepr.DiscriminationTechnique cuspec with
            | IntegerTag -> [ mkLdcInt32 cidx ], [mkTagFieldType ilg cuspec]
            | _ -> [], []
        let ctorFieldTys = alt.FieldTypes |> Array.toList
        instrs @ [ mkNormalNewobj(mkILCtorMethSpecForTy (baseTy,(ctorFieldTys @ tagfields))) ]
    else 
        [ mkNormalNewobj(mkILCtorMethSpecForTy (altTy,Array.toList alt.FieldTypes)) ]

// The stdata 'instruction' is only ever used for the F# "List" type within FSharp.Core.dll
let mkStData (cuspec, cidx, fidx) = 
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let fieldDef = alt.FieldDef fidx
    [ mkNormalStfld (mkILFieldSpecInTy(altTy,fieldDef.LowerName, fieldDef.Type)) ]

let mkNewData ilg (cuspec, cidx) =
    let alt = altOfUnionSpec cuspec cidx
    let altName = alt.Name
    let baseTy = baseTyOfUnionSpec cuspec
    // If helpers exist, use them
    match cuspec.HasHelpers with
    | AllHelpers 
    | SpecialFSharpListHelpers 
    | SpecialFSharpOptionHelpers -> 
        if cuspecRepr.RepresentAlternativeAsNull (cuspec,alt) then 
            [ AI_ldnull  ]
        elif alt.IsNullary then 
            [ mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, "get_" + altName, [], constFormalFieldTy baseTy)) ]
        else
            [ mkNormalCall (mkILNonGenericStaticMethSpecInTy (baseTy, mkMakerName cuspec altName, Array.toList alt.FieldTypes, constFormalFieldTy baseTy)) ]

    | NoHelpers -> 
        if cuspecRepr.MaintainPossiblyUniqueConstantFieldForAlternative (cuspec,alt) then 
            // This method is only available if not AllHelpers. It fetches the unique object for the alternative
            // without exposing direct access to the underlying field
            [ mkNormalCall (mkILNonGenericStaticMethSpecInTy(baseTy, "get_" + altName, [], constFormalFieldTy baseTy)) ]
        else
            convNewDataInstrInternal ilg cuspec cidx 

let mkIsData ilg (avoidHelpers, cuspec, cidx) = 
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let altName = alt.Name
    if cuspecRepr.RepresentAlternativeAsNull (cuspec,alt) then 
        [ AI_ldnull; AI_ceq ] 
    elif cuspecRepr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull (cuspec,alt) then 
        // in this case we can use a null test
        [ AI_ldnull; AI_cgt_un ] 
    else 
        match cuspecRepr.DiscriminationTechnique cuspec with 
        | SingleCase -> [ mkLdcInt32 1 ] 
        | RuntimeTypes -> mkRuntimeTypeDiscriminate ilg avoidHelpers cuspec alt altName  altTy
        | IntegerTag -> mkTagDiscriminate ilg cuspec (baseTyOfUnionSpec cuspec) cidx
        | TailOrNull -> 
            match cidx with 
            | TagNil -> mkGetTailOrNull avoidHelpers cuspec @  [ AI_ldnull; AI_ceq ]
            | TagCons -> mkGetTailOrNull avoidHelpers cuspec @ [ AI_ldnull; AI_cgt_un  ]
            | _ -> failwith "unexpected"

type ICodeGen<'Mark> = 
    abstract CodeLabel: 'Mark -> ILCodeLabel
    abstract GenerateDelayMark: unit -> 'Mark
    abstract GenLocal: ILType -> uint16
    abstract SetMarkToHere: 'Mark  -> unit
    abstract EmitInstr : ILInstr -> unit
    abstract EmitInstrs : ILInstr list -> unit

let genWith g : ILCode = 
    let instrs = ResizeArray() 
    let lab2pc = Dictionary() 
    g { new ICodeGen<ILCodeLabel> with 
            member __.CodeLabel(m) = m
            member __.GenerateDelayMark() = generateCodeLabel()
            member __.GenLocal(ilty) = failwith "not needed"
            member __.SetMarkToHere(m) = lab2pc.[m] <- instrs.Count
            member __.EmitInstr x = instrs.Add x
            member cg.EmitInstrs xs = for i in xs do cg.EmitInstr i }

    { Labels = lab2pc
      Instrs = instrs.ToArray()
      Exceptions = []
      Locals = [] }


let mkBrIsNotData ilg (avoidHelpers, cuspec,cidx,tg) = 
    let alt = altOfUnionSpec cuspec cidx
    let altTy = tyForAlt cuspec alt
    let altName = alt.Name
    if cuspecRepr.RepresentAlternativeAsNull (cuspec,alt) then 
        [ I_brcmp (BI_brtrue,tg) ] 
    elif cuspecRepr.RepresentSingleNonNullaryAlternativeAsInstancesOfRootClassAndAnyOtherAlternativesAsNull (cuspec,alt) then 
        // in this case we can use a null test
        [ I_brcmp (BI_brfalse,tg) ] 
    else
        match cuspecRepr.DiscriminationTechnique cuspec  with 
        | SingleCase -> [ ]
        | RuntimeTypes ->  mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy (I_brcmp (BI_brfalse,tg))
        | IntegerTag -> mkTagDiscriminateThen ilg cuspec cidx (I_brcmp (BI_brfalse,tg))
        | TailOrNull -> 
            match cidx with 
            | TagNil -> mkGetTailOrNull avoidHelpers cuspec @ [I_brcmp (BI_brtrue,tg)]
            | TagCons -> mkGetTailOrNull avoidHelpers cuspec @ [ I_brcmp (BI_brfalse,tg)]
            | _ -> failwith "unexpected"


let emitLdDataTagPrim ilg ldOpt (cg: ICodeGen<'Mark>) (avoidHelpers,cuspec: IlxUnionSpec)  = 
        // If helpers exist, use them
    match cuspec.HasHelpers with
    | (SpecialFSharpListHelpers | AllHelpers) when not avoidHelpers -> 
        ldOpt |> Option.iter cg.EmitInstr 
        cg.EmitInstr (mkGetTagFromHelpers ilg cuspec)
    | _ -> 
                    
        let alts = cuspec.Alternatives
        match cuspecRepr.DiscriminationTechnique cuspec with
        | TailOrNull ->
            // leaves 1 if cons, 0 if not
            ldOpt |> Option.iter cg.EmitInstr 
            cg.EmitInstrs (mkGetTailOrNull avoidHelpers cuspec @ [ AI_ldnull; AI_cgt_un])
        | IntegerTag -> 
            let baseTy = baseTyOfUnionSpec cuspec
            ldOpt |> Option.iter cg.EmitInstr 
            cg.EmitInstrs (mkGetTagFromField ilg cuspec baseTy)
        | SingleCase -> 
            ldOpt |> Option.iter cg.EmitInstr 
            cg.EmitInstrs [ AI_pop; mkLdcInt32 0 ] 
        | RuntimeTypes -> 
            let baseTy = baseTyOfUnionSpec cuspec
            let ld = 
                match ldOpt with 
                | None -> 
                    let locn = cg.GenLocal baseTy 
                    // Add on a branch to the first input label.  This gets optimized away by the printer/emitter. 
                    cg.EmitInstr (mkStloc locn)
                    mkLdloc locn 
                | Some i -> i

            let outlab = cg.GenerateDelayMark()

            let emitCase cidx = 
                let alt = altOfUnionSpec cuspec cidx
                let internalLab = cg.GenerateDelayMark()
                let failLab = cg.GenerateDelayMark ()
                let cmpNull = cuspecRepr.RepresentAlternativeAsNull (cuspec, alt)
                let test = I_brcmp ((if cmpNull then BI_brtrue else BI_brfalse),cg.CodeLabel failLab)
                let testBlock = 
                    if cmpNull || cuspecRepr.RepresentAlternativeAsFreshInstancesOfRootClass (cuspec,alt) then 
                        [ test ]
                    else
                        let altName = alt.Name
                        let altTy = tyForAlt cuspec alt
                        mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy test
                cg.EmitInstrs (ld :: testBlock)
                cg.SetMarkToHere internalLab
                cg.EmitInstrs [mkLdcInt32 cidx; I_br (cg.CodeLabel outlab) ]
                cg.SetMarkToHere failLab

            // Make the blocks for the remaining tests. 
            for n in alts.Length-1 .. -1 .. 1 do 
                emitCase n 

            // Make the block for the last test. 
            cg.EmitInstr (mkLdcInt32 0)
            cg.SetMarkToHere outlab

let emitLdDataTag ilg (cg: ICodeGen<'Mark>) (avoidHelpers,cuspec: IlxUnionSpec)  = 
    emitLdDataTagPrim ilg None cg (avoidHelpers,cuspec)  

let emitCastData ilg (cg: ICodeGen<'Mark>) (canfail,avoidHelpers,cuspec,cidx) = 
    let alt = altOfUnionSpec cuspec cidx
    if cuspecRepr.RepresentAlternativeAsNull (cuspec,alt) then 
        if canfail then 
            let outlab = cg.GenerateDelayMark ()
            let internal1 = cg.GenerateDelayMark ()
            cg.EmitInstrs [AI_dup; I_brcmp (BI_brfalse, cg.CodeLabel outlab) ]
            cg.SetMarkToHere internal1
            cg.EmitInstrs  [mkPrimaryAssemblyExnNewobj ilg "System.InvalidCastException"; I_throw ]
            cg.SetMarkToHere outlab
        else
            // If it can't fail, it's still verifiable just to leave the value on the stack unchecked 
            ()
    elif cuspecRepr.Flatten cuspec then
        if canfail then
            let outlab = cg.GenerateDelayMark ()
            let internal1 = cg.GenerateDelayMark ()
            cg.EmitInstrs [ AI_dup ]
            emitLdDataTagPrim ilg None cg (avoidHelpers,cuspec)
            cg.EmitInstrs [ mkLdcInt32 cidx; I_brcmp (BI_beq, cg.CodeLabel outlab) ]
            cg.SetMarkToHere internal1
            cg.EmitInstrs  [mkPrimaryAssemblyExnNewobj ilg "System.InvalidCastException"; I_throw ]
            cg.SetMarkToHere outlab
        else
            // If it can't fail, it's still verifiable just to leave the value on the stack unchecked 
            ()
    elif cuspecRepr.OptimizeAlternativeToRootClass (cuspec,alt) then 
        ()
    else 
        let altTy = tyForAlt cuspec alt
        cg.EmitInstr (I_castclass altTy)
              
let emitDataSwitch ilg (cg: ICodeGen<'Mark>) (avoidHelpers, cuspec, cases) =
    let baseTy = baseTyOfUnionSpec cuspec
        
    match cuspecRepr.DiscriminationTechnique cuspec with 
    | RuntimeTypes ->  
        let locn = cg.GenLocal baseTy 

        cg.EmitInstr (mkStloc locn)

        for (cidx,tg) in cases do 
            let alt = altOfUnionSpec cuspec cidx
            let altTy = tyForAlt cuspec alt
            let altName = alt.Name
            let failLab = cg.GenerateDelayMark ()
            let cmpNull = cuspecRepr.RepresentAlternativeAsNull (cuspec,alt)

            cg.EmitInstr (mkLdloc locn)
            let testInstr = I_brcmp ((if cmpNull then BI_brfalse else BI_brtrue),tg) 
            if cmpNull || cuspecRepr.RepresentAlternativeAsFreshInstancesOfRootClass (cuspec,alt) then 
                 cg.EmitInstr testInstr 
            else 
                 cg.EmitInstrs (mkRuntimeTypeDiscriminateThen ilg avoidHelpers cuspec alt altName altTy testInstr)
            cg.SetMarkToHere failLab
                

    | IntegerTag -> 
        match cases with 
        | [] -> cg.EmitInstrs  [ AI_pop ]
        | _ ->
        // Use a dictionary to avoid quadratic lookup in case list
        let dict = System.Collections.Generic.Dictionary<int,_>()
        for (i,case) in cases do dict.[i] <- case
        let failLab = cg.GenerateDelayMark ()
        let emitCase i _ = 
            let mutable res = Unchecked.defaultof<_>
            let ok = dict.TryGetValue(i, &res)
            if ok then res else cg.CodeLabel failLab

        let dests = Array.mapi emitCase cuspec.AlternativesArray
        cg.EmitInstrs (mkGetTag ilg cuspec)
        cg.EmitInstr (I_switch (Array.toList dests))
        cg.SetMarkToHere failLab

    | SingleCase ->
        match cases with 
        | [(0,tg)] -> cg.EmitInstrs [ AI_pop; I_br tg ]
        | [] -> cg.EmitInstrs  [ AI_pop ]
        | _ -> failwith "unexpected: strange switch on single-case unions should not be present"

    | TailOrNull -> 
        failwith "unexpected: switches on lists should have been eliminated to brisdata tests"
                


//---------------------------------------------------
// Generate the union classes

let mkHiddenGeneratedInstanceFieldDef ilg (nm,ty,init,access) = 
     mkILInstanceField (nm,ty,init,access)
            |> addFieldNeverAttrs ilg
            |> addFieldGeneratedAttrs ilg

let mkHiddenGeneratedStaticFieldDef ilg (a,b,c,d,e) = 
     mkILStaticField (a,b,c,d,e)
            |> addFieldNeverAttrs ilg
            |> addFieldGeneratedAttrs ilg


let mkMethodsAndPropertiesForFields ilg access attr hasHelpers (typ: ILType) (fields: IlxUnionField[]) = 
    let basicProps = 
        fields 
        |> Array.map (fun field -> 
            { Name=adjustFieldName hasHelpers field.Name
              IsRTSpecialName=false
              IsSpecialName=false
              SetMethod=None
              GetMethod = Some (mkILMethRef (typ.TypeRef, ILCallingConv.Instance, "get_" + adjustFieldName hasHelpers field.Name, 0, [], field.Type))
              CallingConv=ILThisConvention.Instance
              Type=field.Type          
              Init=None
              Args=mkILTypes []
              CustomAttrs= field.ILField.CustomAttrs }
            |> addPropertyGeneratedAttrs ilg
        )
        |> Array.toList

    let basicMethods = 
        [ for field in fields do 
              let fspec = mkILFieldSpecInTy(typ,field.LowerName,field.Type)
              yield 
                  mkILNonGenericInstanceMethod
                     ("get_" + adjustFieldName hasHelpers field.Name,
                      access, [], mkILReturn field.Type,
                      mkMethodBody(true,emptyILLocals,2,nonBranchingInstrsToCode [ mkLdarg 0us; mkNormalLdfld fspec ], attr))
                  |> addMethodGeneratedAttrs ilg  ]
    
    basicProps, basicMethods

    
let convAlternativeDef ilg num (td:ILTypeDef) cud info cuspec (baseTy:ILType) (alt:IlxUnionAlternative) =
    let attr = cud.cudWhere
    let altName = alt.Name
    let fields = alt.FieldDefs
    let altTy = tyForAlt cuspec alt
    let repr = cudefRepr 

    // Attributes on unions get attached to the construction methods in the helpers
    let addAltAttribs (mdef: ILMethodDef) = { mdef with CustomAttrs=alt.altCustomAttrs }

    // The stdata instruction is only ever used for the F# "List" type
    //
    // Microsoft.FSharp.Collections.List`1 is indeed logically immutable, but we use mutation on this type internally
    // within FSharp.Core.dll on fresh unpublished cons cells.
    let isTotallyImmutable = (cud.cudHasHelpers <> SpecialFSharpListHelpers)
    
    let altUniqObjMeths  = 

         // This method is only generated if helpers are not available. It fetches the unique object for the alternative
         // without exposing direct access to the underlying field
         match cud.cudHasHelpers with 
         | AllHelpers  
         | SpecialFSharpOptionHelpers  
         | SpecialFSharpListHelpers  -> []
         | _ -> 
             if alt.IsNullary && repr.MaintainPossiblyUniqueConstantFieldForAlternative (info,alt) then 
                 let methName = "get_" + altName
                 let meth = 
                     mkILNonGenericStaticMethod
                           (methName,
                            cud.cudReprAccess,[],mkILReturn(baseTy),
                            mkMethodBody(true,emptyILLocals,fields.Length,
                                    nonBranchingInstrsToCode 
                                      [ I_ldsfld (Nonvolatile,mkConstFieldSpec altName baseTy) ], attr))
                         |> addMethodGeneratedAttrs ilg
                 [meth]
                     
             else
                []

    let baseMakerMeths, baseMakerProps = 

        match cud.cudHasHelpers with 
        | AllHelpers
        | SpecialFSharpOptionHelpers  
        | SpecialFSharpListHelpers  -> 

            let baseTesterMeths, baseTesterProps = 
                if cud.cudAlternatives.Length <= 1 then [], []
                elif repr.RepresentOneAlternativeAsNull info then [], []
                else
                    [ mkILNonGenericInstanceMethod
                         ("get_" + mkTesterName altName,
                          cud.cudHelpersAccess,[],
                          mkILReturn ilg.typ_bool,
                          mkMethodBody(true,emptyILLocals,2,nonBranchingInstrsToCode 
                                    ([ mkLdarg0 ] @ mkIsData ilg (true, cuspec, num)), attr))
                      |> addMethodGeneratedAttrs ilg ],
                    [ { Name=mkTesterName altName
                        IsRTSpecialName=false
                        IsSpecialName=false
                        SetMethod=None
                        GetMethod = Some (mkILMethRef (baseTy.TypeRef, ILCallingConv.Instance, "get_" + mkTesterName altName, 0, [], ilg.typ_bool))
                        CallingConv=ILThisConvention.Instance
                        Type=ilg.typ_bool          
                        Init=None
                        Args=mkILTypes []
                        CustomAttrs=emptyILCustomAttrs }
                      |> addPropertyGeneratedAttrs ilg
                      |> addPropertyNeverAttrs ilg ]

          

            let baseMakerMeths, baseMakerProps = 

                if alt.IsNullary then 

                    let nullaryMeth = 
                        mkILNonGenericStaticMethod
                          ("get_" + altName,
                           cud.cudHelpersAccess, [], mkILReturn baseTy,
                           mkMethodBody(true,emptyILLocals,fields.Length, nonBranchingInstrsToCode (convNewDataInstrInternal ilg cuspec num), attr))
                        |> addMethodGeneratedAttrs ilg
                        |> addAltAttribs

                    let nullaryProp = 
                         
                        { Name=altName
                          IsRTSpecialName=false
                          IsSpecialName=false
                          SetMethod=None
                          GetMethod = Some (mkILMethRef (baseTy.TypeRef, ILCallingConv.Static, "get_" + altName, 0, [], baseTy))
                          CallingConv=ILThisConvention.Static
                          Type=baseTy          
                          Init=None
                          Args=mkILTypes []
                          CustomAttrs=emptyILCustomAttrs }
                        |> addPropertyGeneratedAttrs ilg
                        |> addPropertyNeverAttrs ilg

                    [nullaryMeth],[nullaryProp]
                  
                else
                    let mdef = 
                         mkILNonGenericStaticMethod
                           (mkMakerName cuspec altName,
                            cud.cudHelpersAccess,
                            fields |> Array.map (fun fd -> mkILParamNamed (fd.LowerName, fd.Type)) |> Array.toList,
                            mkILReturn baseTy,
                            mkMethodBody(true,emptyILLocals,fields.Length,
                                    nonBranchingInstrsToCode 
                                      (Array.toList (Array.mapi (fun i _ -> mkLdarg (uint16 i)) fields) @
                                       (convNewDataInstrInternal ilg cuspec num)), attr))
                         |> addMethodGeneratedAttrs ilg
                         |> addAltAttribs

                    [mdef],[]

            (baseMakerMeths@baseTesterMeths), (baseMakerProps@baseTesterProps)

        | NoHelpers ->
            [], []

    let typeDefs, altDebugTypeDefs, altNullaryFields = 
        if repr.RepresentAlternativeAsNull (info,alt) then [], [], [] 
        elif repr.RepresentAlternativeAsFreshInstancesOfRootClass (info,alt) then [], [], [] 
        else
          let altNullaryFields = 
              if repr.MaintainPossiblyUniqueConstantFieldForAlternative(info,alt) then 
                  let basic = mkHiddenGeneratedStaticFieldDef ilg (constFieldName altName, baseTy, None, None, ILMemberAccess.Assembly)
                  let uniqObjField = { basic with IsInitOnly=true }
                  let inRootClass = cuspecRepr.OptimizeAlternativeToRootClass (cuspec,alt)
                  [ (info,alt, altTy,num,uniqObjField,inRootClass) ] 
              else 
                  []

          let typeDefs, altDebugTypeDefs = 
              if repr.OptimizeAlternativeToRootClass (info,alt) then [], [] else
                
              let altDebugTypeDefs, debugAttrs = 
                  if not cud.cudDebugProxies then  [],  []
                  else
                    
                    let debugProxyTypeName = altTy.TypeSpec.Name + "@DebugTypeProxy"
                    let debugProxyTy = mkILBoxedTyRaw (mkILNestedTyRef(altTy.TypeSpec.Scope,altTy.TypeSpec.Enclosing, debugProxyTypeName)) altTy.GenericArgs
                    let debugProxyFieldName = "_obj"
                    
                    let debugProxyFields = 
                        [ mkHiddenGeneratedInstanceFieldDef ilg (debugProxyFieldName,altTy, None, ILMemberAccess.Assembly) ]

                    let debugProxyCtor = 
                        mkILCtor(ILMemberAccess.Public (* must always be public - see jared parson blog entry on implementing debugger type proxy *),
                                [ mkILParamNamed ("obj",altTy) ],
                                mkMethodBody
                                  (false,emptyILLocals,3,
                                   nonBranchingInstrsToCode
                                     [ yield mkLdarg0 
                                       yield mkNormalCall (mkILCtorMethSpecForTy (ilg.typ_Object,[]))  
                                       yield mkLdarg0 
                                       yield mkLdarg 1us
                                       yield mkNormalStfld (mkILFieldSpecInTy (debugProxyTy,debugProxyFieldName,altTy)) ],None))

                        |> addMethodGeneratedAttrs ilg

                    let debugProxyGetterMeths = 
                        fields 
                        |> Array.map (fun field -> 
                            let fldName,fldTy = mkUnionCaseFieldId field
                            mkILNonGenericInstanceMethod
                               ("get_" + field.Name,
                                ILMemberAccess.Public,[],
                                mkILReturn field.Type,
                                mkMethodBody(true,emptyILLocals,2,
                                        nonBranchingInstrsToCode 
                                          [ mkLdarg0
                                            (match td.tdKind with ILTypeDefKind.ValueType -> mkNormalLdflda | _ -> mkNormalLdfld)  
                                                (mkILFieldSpecInTy (debugProxyTy,debugProxyFieldName,altTy)) 
                                            mkNormalLdfld (mkILFieldSpecInTy(altTy,fldName,fldTy))],None))
                            |> addMethodGeneratedAttrs ilg)
                        |> Array.toList

                    let debugProxyGetterProps =
                        fields 
                        |> Array.map (fun fdef -> 
                            { Name=fdef.Name
                              IsRTSpecialName=false
                              IsSpecialName=false
                              SetMethod=None
                              GetMethod=Some(mkILMethRef(debugProxyTy.TypeRef,ILCallingConv.Instance,"get_" + fdef.Name,0,[],fdef.Type))
                              CallingConv=ILThisConvention.Instance
                              Type=fdef.Type          
                              Init=None
                              Args=mkILTypes []
                              CustomAttrs= fdef.ILField.CustomAttrs }
                            |> addPropertyGeneratedAttrs ilg)
                        |> Array.toList

                    let debugProxyTypeDef = 
                        mkILGenericClass (debugProxyTypeName, 
                                          ILTypeDefAccess.Nested ILMemberAccess.Assembly, 
                                          td.GenericParams, 
                                          ilg.typ_Object, [], 
                                          mkILMethods ([debugProxyCtor] @ debugProxyGetterMeths), 
                                          mkILFields debugProxyFields,
                                          emptyILTypeDefs,
                                          mkILProperties debugProxyGetterProps,
                                          emptyILEvents,
                                          emptyILCustomAttrs,
                                          ILTypeInit.BeforeField)

                    [ { debugProxyTypeDef with IsSpecialName=true } ],
                    ( [ilg.mkDebuggerTypeProxyAttribute debugProxyTy] @ cud.cudDebugDisplayAttributes)
                                    
              let altTypeDef = 
                  let basicFields = 
                      fields 
                      |> Array.map (fun field -> 
                          let fldName,fldTy = mkUnionCaseFieldId field
                          let fdef = mkHiddenGeneratedInstanceFieldDef ilg (fldName,fldTy, None, ILMemberAccess.Assembly)
                          { fdef with IsInitOnly=isTotallyImmutable })
                      |> Array.toList


                  let basicProps, basicMethods = mkMethodsAndPropertiesForFields ilg cud.cudReprAccess attr cud.cudHasHelpers altTy fields 

                  
                  let basicCtorMeth = 
                      mkILStorageCtor 
                         (attr  ,
                          [ yield mkLdarg0 
                            match repr.DiscriminationTechnique info with 
                            | IntegerTag -> 
                                yield mkLdcInt32 num
                                yield mkNormalCall (mkILCtorMethSpecForTy (baseTy,[mkTagFieldType ilg cuspec])) 
                            | SingleCase 
                            | RuntimeTypes ->
                                yield mkNormalCall (mkILCtorMethSpecForTy (baseTy,[])) 
                            | TailOrNull -> 
                                failwith "unreachable" ],
                          altTy,
                          (basicFields |> List.map (fun fdef -> fdef.Name, fdef.Type) ),
                          (if cuspec.HasHelpers = AllHelpers then ILMemberAccess.Assembly else cud.cudReprAccess))
                      |> addMethodGeneratedAttrs ilg

                  let altTypeDef = 
                      mkILGenericClass (altTy.TypeSpec.Name, 
                                        // Types for nullary's become private, they also have names like _Empty
                                        ILTypeDefAccess.Nested (if alt.IsNullary && cud.cudHasHelpers = IlxUnionHasHelpers.AllHelpers then ILMemberAccess.Assembly else cud.cudReprAccess), 
                                        td.GenericParams, 
                                        baseTy, [], 
                                        mkILMethods ([basicCtorMeth] @ basicMethods), 
                                        mkILFields basicFields,
                                        emptyILTypeDefs,
                                        mkILProperties basicProps,
                                        emptyILEvents,
                                        mkILCustomAttrs debugAttrs,
                                        ILTypeInit.BeforeField)

                  { altTypeDef with IsSerializable=td.IsSerializable; IsSpecialName=true }

              [ altTypeDef ], altDebugTypeDefs 


          typeDefs,altDebugTypeDefs,altNullaryFields

    baseMakerMeths, baseMakerProps, altUniqObjMeths, typeDefs, altDebugTypeDefs, altNullaryFields
        
  
let mkClassUnionDef ilg tref td cud = 
    let boxity = match td.tdKind with ILTypeDefKind.ValueType -> ILBoxity.AsValue | _ -> ILBoxity.AsObject
    let baseTy = mkILFormalNamedTy boxity tref td.GenericParams
    let cuspec = IlxUnionSpec(IlxUnionRef(boxity,baseTy.TypeRef, cud.cudAlternatives, cud.cudNullPermitted, cud.cudHasHelpers), baseTy.GenericArgs)
    let info = (td,cud)
    let repr = cudefRepr 
    let isTotallyImmutable = (cud.cudHasHelpers <> SpecialFSharpListHelpers)

    let results = 
        cud.cudAlternatives 
        |> List.ofArray 
        |> List.mapi (fun i alt -> convAlternativeDef ilg i td cud info cuspec baseTy alt)

    let baseMethsFromAlt = results |> List.collect (fun (a,_,_,_,_,_) -> a) 
    let basePropsFromAlt = results |> List.collect (fun (_,a,_,_,_,_) -> a) 
    let altUniqObjMeths  = results |> List.collect (fun (_,_,a,_,_,_) -> a) 
    let altTypeDefs      = results |> List.collect (fun (_,_,_,a,_,_) -> a) 
    let altDebugTypeDefs = results |> List.collect (fun (_,_,_,_,a,_) -> a) 
    let altNullaryFields = results |> List.collect (fun (_,_,_,_,_,a) -> a) 
       
    let tagFieldsInObject = 
        match repr.DiscriminationTechnique info with 
        | SingleCase | RuntimeTypes | TailOrNull -> []
        | IntegerTag -> [ mkTagFieldId ilg cuspec ] 

    let isStruct = match td.tdKind with ILTypeDefKind.ValueType -> true | _ -> false

    let selfFields, selfMeths, selfProps = 

        [ for alt in cud.cudAlternatives do 
           if repr.RepresentAlternativeAsFreshInstancesOfRootClass (info,alt) then
        // TODO
            let fields = alt.FieldDefs |> Array.toList |> List.map mkUnionCaseFieldId 
            let baseInit = 
                if isStruct then None else
                match td.Extends with 
                | None -> Some ilg.tspec_Object 
                | Some typ -> Some typ.TypeSpec

            let ctor = 
                mkILSimpleStorageCtor 
                   (cud.cudWhere,
                    baseInit,
                    baseTy,
                    (fields @ tagFieldsInObject),
                    (if cuspec.HasHelpers = AllHelpers then ILMemberAccess.Assembly else cud.cudReprAccess))
                |> addMethodGeneratedAttrs ilg

            let props, meths = mkMethodsAndPropertiesForFields ilg cud.cudReprAccess cud.cudWhere cud.cudHasHelpers baseTy alt.FieldDefs                 
            yield (fields,([ctor] @ meths),props) ]
         |> List.unzip3
         |> (fun (a,b,c) -> List.concat a, List.concat b, List.concat c)

    let selfAndTagFields = 
        [ for (fldName,fldTy) in (selfFields @ tagFieldsInObject)  do
              let fdef = mkHiddenGeneratedInstanceFieldDef ilg (fldName,fldTy, None, ILMemberAccess.Assembly)
              yield { fdef with IsInitOnly= (not isStruct && isTotallyImmutable) } ]

    let ctorMeths =
        if (isNil selfFields && isNil tagFieldsInObject && nonNil selfMeths)
            ||  cud.cudAlternatives |> Array.forall (fun alt -> repr.RepresentAlternativeAsFreshInstancesOfRootClass (info,alt))  then 

            [] (* no need for a second ctor in these cases *)

        else 
            [ mkILSimpleStorageCtor 
                 (cud.cudWhere,
                  (match td.Extends with None -> Some ilg.tspec_Object | Some typ -> Some typ.TypeSpec),
                  baseTy,
                  tagFieldsInObject,
                  ILMemberAccess.Assembly) // cud.cudReprAccess)
              |> addMethodGeneratedAttrs ilg ]

    // Now initialize the constant fields wherever they are stored... 
    let addConstFieldInit cd = 
        if isNil altNullaryFields then 
           cd 
        else 
           prependInstrsToClassCtor 
              [ for (info,_alt,altTy,fidx,fd,inRootClass) in altNullaryFields do 
                  let constFieldId = (fd.Name,baseTy)
                  let constFieldSpec = mkConstFieldSpecFromId baseTy constFieldId
                  match repr.DiscriminationTechnique info with 
                  | SingleCase  
                  | RuntimeTypes  
                  | TailOrNull ->
                      yield mkNormalNewobj (mkILCtorMethSpecForTy (altTy,[])) 
                  | IntegerTag -> 
                      if inRootClass then
                          yield mkLdcInt32 fidx 
                          yield mkNormalNewobj (mkILCtorMethSpecForTy (altTy,[mkTagFieldType ilg cuspec] ))
                      else
                          yield mkNormalNewobj (mkILCtorMethSpecForTy (altTy,[])) 
                  yield mkNormalStsfld constFieldSpec ]
              cud.cudWhere
              cd

    let tagMeths, tagProps, tagEnumFields = 
        let tagFieldType = mkTagFieldType ilg cuspec
        let tagEnumFields = 
            cud.cudAlternatives 
            |> Array.mapi (fun num alt -> mkILLiteralField (alt.Name, tagFieldType, ILFieldInit.Int32 num, None, ILMemberAccess.Public))
            |> Array.toList

        
        let tagMeths,tagProps = 

          let body = mkMethodBody(true,emptyILLocals,2,genWith (fun cg -> emitLdDataTagPrim ilg (Some mkLdarg0) cg (true, cuspec); cg.EmitInstr I_ret), cud.cudWhere)
          // // If we are using NULL as a representation for an element of this type then we cannot 
          // // use an instance method 
          if (repr.RepresentOneAlternativeAsNull info) then
              [ mkILNonGenericStaticMethod("Get" + tagPropertyName,cud.cudHelpersAccess,[mkILParamAnon baseTy],mkILReturn tagFieldType,body)
                |> addMethodGeneratedAttrs ilg ], 
              [] 

          else
              [ mkILNonGenericInstanceMethod("get_" + tagPropertyName,cud.cudHelpersAccess,[],mkILReturn tagFieldType,body) 
                |> addMethodGeneratedAttrs ilg ], 
          
              [ { Name=tagPropertyName
                  IsRTSpecialName=false
                  IsSpecialName=false
                  SetMethod=None
                  GetMethod=Some(mkILMethRef(baseTy.TypeRef,ILCallingConv.Instance,"get_" + tagPropertyName,0,[], tagFieldType))
                  CallingConv=ILThisConvention.Instance
                  Type=tagFieldType          
                  Init=None
                  Args=mkILTypes []
                  CustomAttrs=emptyILCustomAttrs }
                |> addPropertyGeneratedAttrs ilg 
                |> addPropertyNeverAttrs ilg  ]

        tagMeths, tagProps, tagEnumFields

    // The class can be abstract if each alternative is represented by a derived type
    let isAbstract = (altTypeDefs.Length = cud.cudAlternatives.Length)        

    let existingMeths = td.Methods.AsList 
    let existingProps = td.Properties.AsList

    let enumTypeDef = 
        // The nested Tags type is elided if there is only one tag
        // The Tag property is NOT elided if there is only one tag
        if tagEnumFields.Length <= 1 then 
            None
        else
            Some 
                { Name = "Tags"
                  NestedTypes = emptyILTypeDefs
                  GenericParams= td.GenericParams
                  Access = ILTypeDefAccess.Nested cud.cudReprAccess
                  IsAbstract = true
                  IsSealed = true
                  IsSerializable=false
                  IsComInterop=false
                  Layout=ILTypeDefLayout.Auto 
                  IsSpecialName=false
                  Encoding=ILDefaultPInvokeEncoding.Ansi
                  Implements = mkILTypes []
                  Extends= Some ilg.typ_Object 
                  Methods= emptyILMethods
                  SecurityDecls=emptyILSecurityDecls
                  HasSecurity=false 
                  Fields=mkILFields tagEnumFields
                  MethodImpls=emptyILMethodImpls
                  InitSemantics=ILTypeInit.OnAny
                  Events=emptyILEvents
                  Properties=emptyILProperties
                  CustomAttrs= emptyILCustomAttrs
                  tdKind = ILTypeDefKind.Enum }

    let baseTypeDef = 
       { td with 
          NestedTypes = mkILTypeDefs (Option.toList enumTypeDef @ altTypeDefs @ altDebugTypeDefs @ td.NestedTypes.AsList)
          IsAbstract = isAbstract
          IsSealed = altTypeDefs.IsEmpty
          IsComInterop=false
          Extends= (match td.Extends with None -> Some ilg.typ_Object | _ -> td.Extends) 
          Methods= mkILMethods (ctorMeths @ baseMethsFromAlt @ selfMeths @ tagMeths @ altUniqObjMeths @ existingMeths)
          Fields=mkILFields (selfAndTagFields @ List.map (fun (_,_,_,_,fdef,_) -> fdef) altNullaryFields @ td.Fields.AsList)
          InitSemantics=ILTypeInit.BeforeField
          Properties=mkILProperties (tagProps @ basePropsFromAlt @ selfProps @ existingProps)
          tdKind = ILTypeDefKind.Class }
       // The .cctor goes on the Cases type since that's where the constant fields for nullary constructors live
       |> addConstFieldInit 

    baseTypeDef


