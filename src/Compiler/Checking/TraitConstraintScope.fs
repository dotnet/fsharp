// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.TraitConstraintScope

open Internal.Utilities.Library

open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Infos
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

let rec CreateTraitContext selectExtensionMethods (nenv: Lazy<NameResolutionEnv>) ad : ITraitContext<AccessorDomain, MethInfo, InfoReader> =
    { new ITraitContext<AccessorDomain, MethInfo, InfoReader> with
        member _.SelectExtensionMethods(traitInfo, m, infoReader) =
            selectExtensionMethods (traitInfo, m, nenv.Value, infoReader)

        member _.AccessRights = ad

        member _.Remap(remapType, remapValRef, remapStamp) =
            // Cloned values are fixed up after their constraints are remapped.
            let nenvR =
                lazy
                    (let nenv = nenv.Value
                     let g = nenv.DisplayEnv.g

                     let remapTyconRef tcref =
                         tcrefOfAppTy g (remapType (generalizedTyconRef g tcref))

                     let remapMethInfo (minfo: MethInfo) = minfo.Remap(remapType, remapValRef)

                     let remapExtensionMember emem =
                         match emem with
                         | FSExtMem(vref, pri) -> FSExtMem(remapValRef vref, pri)
                         | ILExtMem(tcref, minfos, pri) -> ILExtMem(remapTyconRef tcref, List.map remapMethInfo minfos, pri)

                     { nenv with
                         eIndexedExtensionMembers = nenv.eIndexedExtensionMembers.Remap(remapStamp, remapExtensionMember)
                         eUnindexedExtensionMembers = List.map remapExtensionMember nenv.eUnindexedExtensionMembers
                         eOpenedTypeOperators = NameMultiMap.map remapMethInfo nenv.eOpenedTypeOperators
                     })

            CreateTraitContext selectExtensionMethods nenvR ad :> ITraitContext
    }
