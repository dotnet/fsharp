// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ClassTypeVisibilityModuleRoot =

    let withRealInternalSignature realSig compilation =
        compilation
        |> withOptions [if realSig then "--realsig+" else "--realsig-" ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - various constructors`` (realSig) =
        FSharp """
module RealInternalSignature

type public TypeOne public () = class end
type public TypeTwo internal () = class end
type public TypeThree private () = class end
type public TypeFour () = class end
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            """  .class auto ansi serializable nested public TypeOne
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

            """  .class auto ansi serializable nested public TypeTwo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed"""

            """  .class auto ansi serializable nested public TypeThree
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed"""

            """  .class auto ansi serializable nested public TypeFour
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - various constructors`` (realSig) =
        FSharp """
module RealInternalSignature

type private TypeOne public () = class end
type private TypeTwo internal () = class end
type private TypeThree private () = class end
type private TypeFour () = class end
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                //type private TypeOne public () = class end
                """.class auto ansi serializable nested private TypeOne
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

                //type private TypeTwo internal () = class end
                """.class auto ansi serializable nested private TypeTwo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

                //type private TypeThree private () = class end
                """.class auto ansi serializable nested private TypeThree
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

                //type private TypeFour () = class end
                """.class auto ansi serializable nested private TypeFour
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

        else

                //type private TypeOne public () = class end
                """.class auto ansi serializable nested assembly TypeOne
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

                //type private TypeTwo internal () = class end
                """.class auto ansi serializable nested assembly TypeTwo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

                //type private TypeThree private () = class end
                """.class auto ansi serializable nested assembly TypeThree
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

                //type private TypeFour () = class end
                """.class auto ansi serializable nested assembly TypeFour
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname 
            instance void  .ctor() cil managed"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - various methods`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member public _.PublicMethod() = ()
    member internal _.InternalMethod() = ()
    member private _.PrivateMethod() = ()
    member _.DefaultMethod() = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - various methods`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member public _.PublicMethod() = ()
    member internal _.InternalMethod() = ()
    member private _.PrivateMethod() = ()
    member _.DefaultMethod() = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - various properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member val public PublicProperty = 0 with get, set
    member val internal InternalProperty = 0 with get, set
    member val private PrivateProperty = 0 with get, set
    member val DefaultProperty = 0 with get, set
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig specialname 
            instance int32  get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0007:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0006:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0007:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0007:  ret
    } 
"""
            else
                """
    .method public hidebysig specialname 
            instance int32  get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0007:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0007:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - various properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member val public PublicProperty = 0 with get, set
    member val internal InternalProperty = 0 with get, set
    member val private PrivateProperty = 0 with get, set
    member val DefaultProperty = 0 with get, set
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig specialname 
            instance int32  get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0007:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0006:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0007:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0007:  ret
    } 
"""
            else
                """
    .method public hidebysig specialname 
            instance int32  get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0007:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0007:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - various mixed properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member _.MixedPropertyOne with public get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyTwo with public get() = 0 and private set (_:int) = ()
    member _.MixedPropertyThree with private get() = 0 and public set (_:int) = ()
    member _.MixedPropertyFour with private get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyFive with internal get() = 0 and public set (_:int) = ()
    member _.MixedPropertySix with internal get() = 0 and private set (_:int) = ()
    member _.MixedPropertySeven with get() = 0 and public set (_:int) = ()
    member _.MixedPropertyEight with get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyNine with get() = 0 and private set (_:int) = ()
    member _.MixedPropertyTen with public get() = 0 and set (_:int) = ()
    member _.MixedPropertyEleven with internal get() = 0 and set (_:int) = ()
    member _.MixedPropertyTwelve with private get() = 0 and set (_:int) = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method public hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - various mixed properties`` (realSig) =
        FSharp """
module RealInternalSignature

type private TestType () =
    member _.MixedPropertyOne with public get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyTwo with public get() = 0 and private set (_:int) = ()
    member _.MixedPropertyThree with private get() = 0 and public set (_:int) = ()
    member _.MixedPropertyFour with private get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyFive with internal get() = 0 and public set (_:int) = ()
    member _.MixedPropertySix with internal get() = 0 and private set (_:int) = ()
    member _.MixedPropertySeven with get() = 0 and public set (_:int) = ()
    member _.MixedPropertyEight with get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyNine with get() = 0 and private set (_:int) = ()
    member _.MixedPropertyTen with public get() = 0 and set (_:int) = ()
    member _.MixedPropertyEleven with internal get() = 0 and set (_:int) = ()
    member _.MixedPropertyTwelve with private get() = 0 and set (_:int) = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - various static methods`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    static member public PublicMethod() = ()
    static member internal InternalMethod() = ()
    static member private PrivateMethod() = ()
    static member DefaultMethod() = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - various static methods`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    static member public PublicMethod() = ()
    static member internal InternalMethod() = ()
    static member private PrivateMethod() = ()
    static member DefaultMethod() = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method public hidebysig instance void 
            PublicMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            InternalMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig instance void 
            PrivateMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig instance void 
            DefaultMethod() cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - various static properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    static member val public PublicProperty = 0 with get, set
    static member val internal InternalProperty = 0 with get, set
    static member val private PrivateProperty = 0 with get, set
    static member val DefaultProperty = 0 with get, set"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .me  thod public specialname static int32 
            get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::PublicProperty@
      IL_0016:  ret
    } 

    .method public specialname static void 
            set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::PublicProperty@
      IL_0017:  ret
    } 

    .method assembly specialname static int32 
            get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.2
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::InternalProperty@
      IL_0016:  ret
    } 

    .method assembly specialname static void 
            set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.2
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::InternalProperty@
      IL_0017:  ret
    } 

    .method private specialname static int32 
            get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.3
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0016:  ret
    } 

    .method private specialname static void 
            set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.3
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0017:  ret
    } 

    .method public specialname static int32 
            get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.4
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0016:  ret
    } 

    .method public specialname static void 
            set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.4
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0017:  ret
    } 
"""
            else
                """
    .method public specialname static int32 
            get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::PublicProperty@
      IL_0016:  ret
    } 

    .method public specialname static void 
            set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::PublicProperty@
      IL_0017:  ret
    } 

    .method assembly specialname static int32 
            get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.2
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::InternalProperty@
      IL_0016:  ret
    } 

    .method assembly specialname static void 
            set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.2
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::InternalProperty@
      IL_0017:  ret
    } 

    .method assembly specialname static int32 
            get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.3
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0016:  ret
    } 

    .method assembly specialname static void 
            set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.3
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0017:  ret
    } 

    .method public specialname static int32 
            get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.4
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldsfld     int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0016:  ret
    } 

    .method public specialname static void 
            set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 RealInternalSignature/TestType::init@4
      IL_0007:  ldc.i4.4
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  stsfld     int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0017:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - various static properties`` (realSig) =
        FSharp """
module RealInternalSignature

type private TestType () =
    static member val public PublicProperty = 0 with get, set
    static member val internal InternalProperty = 0 with get, set
    static member val private PrivateProperty = 0 with get, set
    static member val DefaultProperty = 0 with get, set"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig specialname 
            instance int32  get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0007:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0006:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0007:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0007:  ret
    } 
"""
            else
                """
    .method public hidebysig specialname 
            instance int32  get_PublicProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_PublicProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PublicProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_InternalProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_InternalProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::InternalProperty@
      IL_0007:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_PrivateProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0006:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_PrivateProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::PrivateProperty@
      IL_0007:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_DefaultProperty() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0006:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_DefaultProperty(int32 v) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 RealInternalSignature/TestType::DefaultProperty@
      IL_0007:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - various static mixed properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    static member MixedPropertyOne with public get() = 0 and internal set (_:int) = ()
    static member MixedPropertyTwo with public get() = 0 and private set (_:int) = ()
    static member MixedPropertyThree with private get() = 0 and public set (_:int) = ()
    static member MixedPropertyFour with private get() = 0 and internal set (_:int) = ()
    static member MixedPropertyFive with internal get() = 0 and public set (_:int) = ()
    static member MixedPropertySix with internal get() = 0 and private set (_:int) = ()
    static member MixedPropertySeven with get() = 0 and public set (_:int) = ()
    static member MixedPropertyEight with get() = 0 and internal set (_:int) = ()
    static member MixedPropertyNine with get() = 0 and private set (_:int) = ()
    static member MixedPropertyTen with public get() = 0 and set (_:int) = ()
    static member MixedPropertyEleven with internal get() = 0 and set (_:int) = ()
    static member MixedPropertyTwelve with private get() = 0 and set (_:int) = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method public hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method public hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method public hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method public hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - various static mixed properties`` (realSig) =
        FSharp """
module RealInternalSignature

type private TestType () =
    static member MixedPropertyOne with public get() = 0 and internal set (_:int) = ()
    static member MixedPropertyTwo with public get() = 0 and private set (_:int) = ()
    static member MixedPropertyThree with private get() = 0 and public set (_:int) = ()
    static member MixedPropertyFour with private get() = 0 and internal set (_:int) = ()
    static member MixedPropertyFive with internal get() = 0 and public set (_:int) = ()
    static member MixedPropertySix with internal get() = 0 and private set (_:int) = ()
    static member MixedPropertySeven with get() = 0 and public set (_:int) = ()
    static member MixedPropertyEight with get() = 0 and internal set (_:int) = ()
    static member MixedPropertyNine with get() = 0 and private set (_:int) = ()
    static member MixedPropertyTen with public get() = 0 and set (_:int) = ()
    static member MixedPropertyEleven with internal get() = 0 and set (_:int) = ()
    static member MixedPropertyTwelve with private get() = 0 and set (_:int) = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method private hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method private hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""
            else
                """
    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyOne() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyOne(int32 _arg1) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwo() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwo(int32 _arg2) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyThree() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyThree(int32 _arg3) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFour() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFour(int32 _arg4) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyFive() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyFive(int32 _arg5) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySix() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySix(int32 _arg6) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertySeven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertySeven(int32 _arg7) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEight() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEight(int32 _arg8) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyNine() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyNine(int32 _arg9) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTen() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTen(int32 _arg10) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyEleven() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyEleven(int32 _arg11) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 

    .method assembly hidebysig specialname 
            instance int32  get_MixedPropertyTwelve() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } 

    .method assembly hidebysig specialname 
            instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed
    {
      
      .maxstack  8
      IL_0000:  ret
    } 
"""

            ]
        |> shouldSucceed

