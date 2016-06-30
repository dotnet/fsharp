// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Defines an extension of the IL algebra
module internal Microsoft.FSharp.Compiler.AbstractIL.Extensions.ILX.Types

open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 

// --------------------------------------------------------------------
// Define an extension of the IL instruction algebra
// -------------------------------------------------------------------- 

let mkLowerName (nm: string) =
    // Use the lower case name of a field or constructor as the field/parameter name if it differs from the uppercase name
    let lowerName = String.uncapitalize nm
    if lowerName = nm then "_" + nm else lowerName

[<Sealed>]
type IlxUnionField(fd: ILFieldDef) =
    let lowerName = mkLowerName fd.Name
    member x.ILField = fd
    member x.Type = x.ILField.Type
    member x.Name = x.ILField.Name
    member x.LowerName = lowerName
    

type IlxUnionAlternative = 
    { altName: string
      altFields: IlxUnionField[]
      altCustomAttrs: ILAttributes }

    member x.FieldDefs = x.altFields
    member x.FieldDef n = x.altFields.[n]
    member x.Name = x.altName
    member x.IsNullary  = (x.FieldDefs.Length = 0)
    member x.FieldTypes = x.FieldDefs |> Array.map (fun fd -> fd.Type) 

type IlxUnionHasHelpers = 
   | NoHelpers
   | AllHelpers
   | SpecialFSharpListHelpers 
   | SpecialFSharpOptionHelpers 
   
type IlxUnionRef = 
    | IlxUnionRef of boxity: ILBoxity * ILTypeRef * IlxUnionAlternative[] * bool * (* hasHelpers: *) IlxUnionHasHelpers 

type IlxUnionSpec = 
    | IlxUnionSpec of IlxUnionRef * ILGenericArgs
    member x.EnclosingType = let (IlxUnionSpec(IlxUnionRef(bx,tref,_,_,_),inst)) = x in mkILNamedTy bx tref inst
    member x.Boxity = let (IlxUnionSpec(IlxUnionRef(bx,_,_,_,_),_)) = x in bx 
    member x.TypeRef = let (IlxUnionSpec(IlxUnionRef(_,tref,_,_,_),_)) = x in tref
    member x.GenericArgs = let (IlxUnionSpec(_,inst)) = x in inst
    member x.AlternativesArray = let (IlxUnionSpec(IlxUnionRef(_,_,alts,_,_),_)) = x in alts
    member x.IsNullPermitted = let (IlxUnionSpec(IlxUnionRef(_,_,_,np,_),_)) = x in np
    member x.HasHelpers = let (IlxUnionSpec(IlxUnionRef(_,_,_,_,b),_)) = x in b
    member x.Alternatives = Array.toList x.AlternativesArray
    member x.Alternative idx = x.AlternativesArray.[idx]
    member x.FieldDef idx fidx = x.Alternative(idx).FieldDef(fidx)


type IlxClosureLambdas = 
    | Lambdas_forall of ILGenericParameterDef * IlxClosureLambdas
    | Lambdas_lambda of ILParameter * IlxClosureLambdas
    | Lambdas_return of ILType

type IlxClosureApps = 
  | Apps_tyapp of ILType * IlxClosureApps 
  | Apps_app of ILType * IlxClosureApps 
  | Apps_done of ILType

let rec instAppsAux n inst = function
    Apps_tyapp (ty,rty) -> Apps_tyapp(instILTypeAux n inst ty, instAppsAux n inst rty)
  | Apps_app (dty,rty) ->  Apps_app(instILTypeAux n inst dty, instAppsAux n inst rty)
  | Apps_done rty ->  Apps_done(instILTypeAux n inst rty)

let rec instLambdasAux n inst = function
  | Lambdas_forall (b,rty) -> 
      Lambdas_forall(b, instLambdasAux n inst rty)
  | Lambdas_lambda (p,rty) ->  
      Lambdas_lambda({ p with Type=instILTypeAux n inst p.Type},instLambdasAux n inst rty)
  | Lambdas_return rty ->  Lambdas_return(instILTypeAux n inst rty)

let instLambdas i t = instLambdasAux 0 i t

type IlxClosureFreeVar = 
    { fvName: string  
      fvCompilerGenerated:bool 
      fvType: ILType }

let mkILFreeVar (name,compgen,ty) = 
    { fvName=name
      fvCompilerGenerated=compgen
      fvType=ty }


type IlxClosureRef = 
    | IlxClosureRef of ILTypeRef * IlxClosureLambdas * IlxClosureFreeVar[]
    
type IlxClosureSpec = 
    | IlxClosureSpec of IlxClosureRef * ILGenericArgs * ILType
    member x.TypeRef = let (IlxClosureRef(tref,_,_)) = x.ClosureRef in tref
    member x.ILType = let (IlxClosureSpec(_,_,ty)) = x in ty
    member x.ClosureRef = let (IlxClosureSpec(cloref,_,_)) = x in cloref 
    member x.FormalFreeVars = let (IlxClosureRef(_,_,fvs)) = x.ClosureRef in fvs
    member x.FormalLambdas = let (IlxClosureRef(_,lambdas,_)) = x.ClosureRef in lambdas
    member x.GenericArgs = let (IlxClosureSpec(_,inst,_)) = x in inst
    static member Create (cloref, inst) = 
        let (IlxClosureRef(tref,_,_)) = cloref
        IlxClosureSpec(cloref, inst, mkILBoxedType (mkILTySpecRaw(tref, inst)))
    member clospec.Constructor = 
        let cloTy = clospec.ILType
        let fields = clospec.FormalFreeVars
        mkILCtorMethSpecForTy (cloTy,fields |> Array.map (fun fv -> fv.fvType) |> Array.toList)


// Define an extension of the IL algebra of type definitions
type IlxClosureInfo = 
    { cloStructure: IlxClosureLambdas
      cloFreeVars: IlxClosureFreeVar[]  
      cloCode: Lazy<ILMethodBody>
      cloSource: ILSourceMarker option}

type IlxUnionInfo = 
    { /// is the representation public? 
      cudReprAccess: ILMemberAccess 
      /// are the representation public? 
      cudHelpersAccess: ILMemberAccess 
      /// generate the helpers? 
      cudHasHelpers: IlxUnionHasHelpers 
      /// generate the helpers? 
      cudDebugProxies: bool 
      cudDebugDisplayAttributes: ILAttribute list
      cudAlternatives: IlxUnionAlternative[]
      cudNullPermitted: bool
      /// debug info for generated code for classunions 
      cudWhere: ILSourceMarker option }

// --------------------------------------------------------------------
// Define these as extensions of the IL types
// -------------------------------------------------------------------- 

let destTyFuncApp = function Apps_tyapp (b,c) -> b,c | _ -> failwith "destTyFuncApp"

let mkILFormalCloRef gparams csig = IlxClosureSpec.Create(csig, mkILFormalGenericArgsRaw gparams)

let actualTypOfIlxUnionField (cuspec : IlxUnionSpec) idx fidx =
  instILType cuspec.GenericArgs (cuspec.FieldDef idx fidx).Type

