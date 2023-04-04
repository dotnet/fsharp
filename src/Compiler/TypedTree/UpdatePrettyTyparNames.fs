﻿/// The Typars of a Val in the signature data should also be pretty named.
/// This will happen for the implementation file contents, but not for the signature data.
/// In this module some helpers will traverse the ModuleOrNamespaceType and update all the typars of each found Val.
module internal FSharp.Compiler.UpdatePrettyTyparNames

open Internal.Utilities.Library
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

let updateVal (v: Val) =
    if not (List.isEmpty v.Typars) then
        let nms = PrettyTypes.PrettyTyparNames (fun _ -> true) List.empty v.Typars

        (v.Typars, nms)
        ||> List.iter2 (fun tp nm ->
            if tp.typar_id.idText = unassignedTyparName then
                tp.typar_id <- ident (nm, tp.Range))

let rec updateEntity (entity: Entity) =
    for e in entity.ModuleOrNamespaceType.AllEntities do
        updateEntity e

    for v in entity.ModuleOrNamespaceType.AllValsAndMembers do
        updateVal v

let updateModuleOrNamespaceType (signatureData: ModuleOrNamespaceType) =
    for e in signatureData.ModuleAndNamespaceDefinitions do
        updateEntity e
