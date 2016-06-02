// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// ILX extensions to Abstract IL types and instructions F# 
module internal Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.IL 

// -------------------------------------------------------------------- 
// Union references 
// -------------------------------------------------------------------- 

[<Sealed>]
type IlxUnionField = 
    new : ILFieldDef -> IlxUnionField
    member Type : ILType
    member Name : string
    /// The name used for the field in parameter or IL field position.
    member LowerName : string 
    member ILField : ILFieldDef
    
type IlxUnionAlternative = 
    { altName: string;
      altFields: IlxUnionField[];
      altCustomAttrs: ILAttributes }

    member FieldDefs : IlxUnionField[]
    member FieldDef : int -> IlxUnionField
    member Name : string
    member IsNullary  : bool
    member FieldTypes : ILType[]


type IlxUnionHasHelpers = 
   | NoHelpers
   | AllHelpers
   | SpecialFSharpListHelpers 
   | SpecialFSharpOptionHelpers 
   
type IlxUnionRef = 
    | IlxUnionRef of ILTypeRef * IlxUnionAlternative[] * bool (* cudNullPermitted *)  * IlxUnionHasHelpers (* cudHasHelpers *)

type IlxUnionSpec = 
    | IlxUnionSpec of IlxUnionRef * ILGenericArgs
    member EnclosingType : ILType
    member GenericArgs : ILGenericArgs
    member Alternatives : IlxUnionAlternative list
    member AlternativesArray : IlxUnionAlternative[]
    member TypeRef : ILTypeRef 
    member IsNullPermitted : bool
    member HasHelpers : IlxUnionHasHelpers
    member Alternative : int -> IlxUnionAlternative
    member FieldDef : int -> int -> IlxUnionField

// -------------------------------------------------------------------- 
// Closure references 
// -------------------------------------------------------------------- 

type IlxClosureLambdas = 
    | Lambdas_forall of ILGenericParameterDef * IlxClosureLambdas
    | Lambdas_lambda of ILParameter * IlxClosureLambdas
    | Lambdas_return of ILType

type IlxClosureFreeVar = 
    { fvName: string ; 
      fvCompilerGenerated:bool; 
      fvType: ILType }

type IlxClosureRef = 
    | IlxClosureRef of ILTypeRef * IlxClosureLambdas * IlxClosureFreeVar[]

type IlxClosureSpec = 
    | IlxClosureSpec of IlxClosureRef * ILGenericArgs * ILType

    member TypeRef : ILTypeRef
    member ILType : ILType
    member ClosureRef : IlxClosureRef
    member FormalLambdas : IlxClosureLambdas
    member GenericArgs : ILGenericArgs
    static member Create : IlxClosureRef * ILGenericArgs -> IlxClosureSpec
    member Constructor : ILMethodSpec


/// IlxClosureApps - i.e. types being applied at a callsite.
type IlxClosureApps = 
    | Apps_tyapp of ILType * IlxClosureApps 
    | Apps_app of ILType * IlxClosureApps 
    | Apps_done of ILType

/// ILX extensions to the instruction set.
type IlxInstr = 
    | EI_lddata of avoidHelpers:bool * IlxUnionSpec * int * int
    | EI_isdata of avoidHelpers:bool * IlxUnionSpec * int
    | EI_brisdata of avoidHelpers:bool * IlxUnionSpec * int * ILCodeLabel * ILCodeLabel
    | EI_castdata of bool * IlxUnionSpec * int
    | EI_stdata of IlxUnionSpec * int * int
    | EI_datacase of avoidHelpers:bool * IlxUnionSpec * (int * ILCodeLabel) list * ILCodeLabel
    | EI_lddatatag of avoidHelpers:bool * IlxUnionSpec
    | EI_newdata of IlxUnionSpec * int

val mkIlxExtInstr: (IlxInstr -> IlxExtensionInstr)
val isIlxExtInstr: (IlxExtensionInstr -> bool)
val destIlxExtInstr: (IlxExtensionInstr -> IlxInstr)

val mkIlxInstr: IlxInstr -> ILInstr

// -------------------------------------------------------------------- 
// ILX extensions to the kinds of type definitions available
// -------------------------------------------------------------------- 

type IlxClosureInfo = 
    { cloStructure: IlxClosureLambdas;
      cloFreeVars: IlxClosureFreeVar[];  
      cloCode: Lazy<ILMethodBody>;
      cloSource: ILSourceMarker option}

and IlxUnionInfo = 
    { /// Is the representation public? 
      cudReprAccess: ILMemberAccess; 
      /// Are the representation helpers public? 
      cudHelpersAccess: ILMemberAccess; 
      /// Generate the helpers? 
      cudHasHelpers: IlxUnionHasHelpers; 
      cudDebugProxies: bool; 
      cudDebugDisplayAttributes: ILAttribute list;
      cudAlternatives: IlxUnionAlternative[];
      cudNullPermitted: bool;
      /// Debug info for generated code for classunions.
      cudWhere: ILSourceMarker option;  
    }

type IlxTypeDefKind = 
   | Closure of IlxClosureInfo
   | Union of IlxUnionInfo

val mkIlxExtTypeDefKind: (IlxTypeDefKind -> IlxExtensionTypeKind)
val isIlxExtTypeDefKind: (IlxExtensionTypeKind -> bool)
val destIlxExtTypeDefKind: (IlxExtensionTypeKind -> IlxTypeDefKind)

val mkIlxTypeDefKind: IlxTypeDefKind -> ILTypeDefKind

// -------------------------------------------------------------------- 
// MS-ILX constructs: Closures, thunks, classunions
// -------------------------------------------------------------------- 

val instAppsAux: int -> ILGenericArgs -> IlxClosureApps -> IlxClosureApps
val destTyFuncApp: IlxClosureApps -> ILType * IlxClosureApps

val mkILFormalCloRef: ILGenericParameterDefs -> IlxClosureRef -> IlxClosureSpec


// -------------------------------------------------------------------- 
// MS-ILX: Unions
// -------------------------------------------------------------------- 


val actualTypOfIlxUnionField: IlxUnionSpec -> int -> int -> ILType

val mkILFreeVar: string * bool * ILType -> IlxClosureFreeVar
