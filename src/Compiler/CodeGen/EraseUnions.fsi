// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// --------------------------------------------------------------------
// Compiler use only.  Erase discriminated unions.
// --------------------------------------------------------------------

module internal FSharp.Compiler.AbstractIL.ILX.EraseUnions

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILX.Types
open FSharp.Compiler.TcGlobals

/// Make the type definition for a union type
val mkClassUnionDef:
    addMethodGeneratedAttrs: (ILMethodDef -> ILMethodDef) *
    addPropertyGeneratedAttrs: (ILPropertyDef -> ILPropertyDef) *
    addPropertyNeverAttrs: (ILPropertyDef -> ILPropertyDef) *
    addFieldGeneratedAttrs: (ILFieldDef -> ILFieldDef) *
    addFieldNeverAttrs: (ILFieldDef -> ILFieldDef) *
    mkDebuggerTypeProxyAttribute: (ILType -> ILAttribute) ->
        g: TcGlobals ->
        tref: ILTypeRef ->
        td: ILTypeDef ->
        cud: IlxUnionInfo ->
            ILTypeDef
