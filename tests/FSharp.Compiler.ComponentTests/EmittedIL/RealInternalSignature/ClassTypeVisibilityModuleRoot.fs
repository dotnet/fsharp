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
    let ``public type - constructors`` (realSig) =
        FSharp """
module RealInternalSignature

type public TypeOne public () = class end
type public TypeTwo internal () = class end
type public TypeThree private () = class end
type public TypeFour () = class end
"""
        |> withName "PublicTypeConstructors"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeConstructors.dll Verified."
            ]
        |> withILContains [
            if realSig then
                """  .class auto ansi serializable nested public TypeOne
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed"""

                """  .class auto ansi serializable nested public TypeTwo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor() cil managed"""

                """  .class auto ansi serializable nested public TypeThree
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method private specialname rtspecialname instance void  .ctor() cil managed"""

                """  .class auto ansi serializable nested public TypeFour
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed"""

            else
                """  .class auto ansi serializable nested public TypeOne
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed"""

                """  .class auto ansi serializable nested public TypeTwo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor() cil managed"""

                """  .class auto ansi serializable nested public TypeThree
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method assembly specialname rtspecialname instance void  .ctor() cil managed"""

                """  .class auto ansi serializable nested public TypeFour
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed"""

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private Type - constructors`` (realSig) =
        FSharp """
module MyLibrary
type private TypeOne public () = class end
type private TypeTwo internal () = class end
type private TypeThree private () = class end
type private TypeFour () = class end"""
        |> withName "PrivateTypeConstructors"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeConstructors.dll Verified."
            ]
        |> withILContains [
            if realSig then
                """
  .class auto ansi serializable nested private TypeOne
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  }"""

                """
  .class auto ansi serializable nested private TypeTwo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } """

                """
  .class auto ansi serializable nested private TypeThree
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } """
                """
  .class auto ansi serializable nested private TypeFour
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } """
            else
                """
  .class auto ansi serializable nested assembly TypeOne
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  }"""

                """
  .class auto ansi serializable nested assembly TypeTwo
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } """

                """
  .class auto ansi serializable nested assembly TypeThree
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } """
                """
  .class auto ansi serializable nested assembly TypeFour
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    } 

  } """

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - instance methods`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member public _.PublicMethod() = ()
    member internal _.InternalMethod() = ()
    member private _.PrivateMethod() = ()
    member _.DefaultMethod() = ()
"""
        |> withName "PublicTypeInstanceMethods"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeInstanceMethods.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method private hidebysig instance void PrivateMethod() cil managed"
                ".method public hidebysig instance void DefaultMethod() cil managed"
            else
                ".method public hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method assembly hidebysig instance void PrivateMethod() cil managed"
                ".method public hidebysig instance void DefaultMethod() cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - instance methods`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member public _.PublicMethod() = ()
    member internal _.InternalMethod() = ()
    member private _.PrivateMethod() = ()
    member _.DefaultMethod() = ()
"""
        |> withName "PrivateTypeInstanceMethods"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeInstanceMethods.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method private hidebysig instance void PrivateMethod() cil managed"
                ".method public hidebysig instance void DefaultMethod() cil managed"

            else
                ".method public hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method assembly hidebysig instance void PrivateMethod() cil managed"
                ".method public hidebysig instance void DefaultMethod() cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - instance properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member val public PublicProperty = 0 with get, set
    member val internal InternalProperty = 0 with get, set
    member val private PrivateProperty = 0 with get, set
    member val DefaultProperty = 0 with get, set
"""
        |> withName "PublicTypeInstanceProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeInstanceProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public hidebysig specialname instance int32  get_PublicProperty() cil managed"
                ".method public hidebysig specialname instance void  set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method private hidebysig specialname instance int32  get_PrivateProperty() cil managed"
                ".method private hidebysig specialname instance void  set_PrivateProperty(int32 v) cil managed"
                ".method public hidebysig specialname instance int32  get_DefaultProperty() cil managed"
                ".method public hidebysig specialname instance void  set_DefaultProperty(int32 v) cil managed"

            else
                ".method public hidebysig specialname instance int32  get_PublicProperty() cil managed"
                ".method public hidebysig specialname instance void  set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_PrivateProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_PrivateProperty(int32 v) cil managed"
                ".method public hidebysig specialname instance int32  get_DefaultProperty() cil managed"
                ".method public hidebysig specialname instance void  set_DefaultProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - instance properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    member val public PublicProperty = 0 with get, set
    member val internal InternalProperty = 0 with get, set
    member val private PrivateProperty = 0 with get, set
    member val DefaultProperty = 0 with get, set
"""
        |> withName "PrivateTypeInstanceMixedProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeInstanceMixedProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public hidebysig specialname instance int32  get_PublicProperty() cil managed"
                ".method public hidebysig specialname instance void  set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method private hidebysig specialname instance int32  get_PrivateProperty() cil managed"
                ".method private hidebysig specialname instance void  set_PrivateProperty(int32 v) cil managed"
                ".method public hidebysig specialname instance int32  get_DefaultProperty() cil managed"
                ".method public hidebysig specialname instance void  set_DefaultProperty(int32 v) cil managed"

            else
                ".method public hidebysig specialname instance int32  get_PublicProperty() cil managed"
                ".method public hidebysig specialname instance void  set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_PrivateProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_PrivateProperty(int32 v) cil managed"
                ".method public hidebysig specialname instance int32  get_DefaultProperty() cil managed"
                ".method public hidebysig specialname instance void  set_DefaultProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - instance mixed properties`` (realSig) =
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
        |> withName "PublicTypeInstanceMixedProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeInstanceMixedProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public hidebysig specialname instance int32  get_MixedPropertyOne() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyTwo() cil managed"
                ".method private hidebysig specialname instance void  set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method private hidebysig specialname instance int32  get_MixedPropertyThree() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method private hidebysig specialname instance int32  get_MixedPropertyFour() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFive() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySix() cil managed"
                ".method private hidebysig specialname instance void  set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertySeven() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyEight() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyNine() cil managed"
                ".method private hidebysig specialname instance void  set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyTen() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEleven() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method private hidebysig specialname instance int32  get_MixedPropertyTwelve() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed"

            else
                ".method public hidebysig specialname instance int32  get_MixedPropertyOne() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyTwo() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyThree() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFour() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFive() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySix() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertySeven() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyEight() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyNine() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyTen() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEleven() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyTwelve() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed"
            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - instance mixed properties`` (realSig) =
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
        |> withName "PrivateTypeInstanceMixedProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeInstanceMixedProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public hidebysig specialname instance int32  get_MixedPropertyOne() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyTwo() cil managed"
                ".method private hidebysig specialname instance void  set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method private hidebysig specialname instance int32  get_MixedPropertyThree() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method private hidebysig specialname instance int32  get_MixedPropertyFour() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFive() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySix() cil managed"
                ".method private hidebysig specialname instance void  set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertySeven() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyEight() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyNine() cil managed"
                ".method private hidebysig specialname instance void  set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public hidebysig specialname instance int32  get_MixedPropertyTen() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEleven() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method private hidebysig specialname instance int32  get_MixedPropertyTwelve() cil managed"
                ".method public hidebysig specialname instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed"

            else
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyOne() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyTwo() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyThree() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFour() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFive() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySix() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertySix(int32 _arg6) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySeven() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEight() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyNine() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyTen() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEleven() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyTwelve() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyTwelve(int32 _arg12) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - static methods`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    static member public PublicMethod() = ()
    static member internal InternalMethod() = ()
    static member private PrivateMethod() = ()
    static member DefaultMethod() = ()
"""
        |> withName "PublicTypeStaticMethods"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeStaticMethods.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method private static void  PrivateMethod() cil managed"
                ".method public static void  DefaultMethod() cil managed"

            else
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method assembly static void  PrivateMethod() cil managed"
                ".method public static void  DefaultMethod() cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - static methods`` (realSig) =
        FSharp """
module RealInternalSignature

type private TestType () =
    static member public PublicMethod() = ()
    static member internal InternalMethod() = ()
    static member private PrivateMethod() = ()
    static member DefaultMethod() = ()
"""
        |> withName "PrivateTypeStaticMethods"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeStaticMethods.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method private static void  PrivateMethod() cil managed"
                ".method public static void  DefaultMethod() cil managed"
            else
                ".method assembly static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method assembly static void  PrivateMethod() cil managed"
                ".method assembly static void  DefaultMethod() cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - static properties`` (realSig) =
        FSharp """
module RealInternalSignature

type public TestType () =
    static member val public PublicProperty = 0 with get, set
    static member val internal InternalProperty = 0 with get, set
    static member val private PrivateProperty = 0 with get, set
    static member val DefaultProperty = 0 with get, set"""
        |> withName "PublicTypeStaticProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeStaticProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public specialname static int32 get_PublicProperty() cil managed"
                ".method public specialname static void set_PublicProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_InternalProperty() cil managed"
                ".method assembly specialname static void set_InternalProperty(int32 v) cil managed"
                ".method private specialname static int32 get_PrivateProperty() cil managed"
                ".method private specialname static void set_PrivateProperty(int32 v) cil managed"
                ".method public specialname static int32 get_DefaultProperty() cil managed"
                ".method public specialname static void set_DefaultProperty(int32 v) cil managed"
            else
                ".method public specialname static int32 get_PublicProperty() cil managed"
                ".method public specialname static void set_PublicProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_InternalProperty() cil managed"
                ".method assembly specialname static void set_InternalProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_PrivateProperty() cil managed"
                ".method assembly specialname static void set_PrivateProperty(int32 v) cil managed"
                ".method public specialname static int32 get_DefaultProperty() cil managed"
                ".method public specialname static void set_DefaultProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - static properties`` (realSig) =
        FSharp """
module RealInternalSignature

type private TestType () =
    static member val public PublicProperty = 0 with get, set
    static member val internal InternalProperty = 0 with get, set
    static member val private PrivateProperty = 0 with get, set
    static member val DefaultProperty = 0 with get, set"""
        |> withName "PrivateTypeStaticProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeStaticProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public specialname static int32 get_PublicProperty() cil managed"
                ".method public specialname static void set_PublicProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_InternalProperty() cil managed"
                ".method assembly specialname static void set_InternalProperty(int32 v) cil managed"
                ".method private specialname static int32 get_PrivateProperty() cil managed"
                ".method private specialname static void set_PrivateProperty(int32 v) cil managed"
                ".method public specialname static int32 get_DefaultProperty() cil managed"
                ".method public specialname static void set_DefaultProperty(int32 v) cil managed"

            else
                ".method assembly specialname static int32 get_PublicProperty() cil managed"
                ".method assembly specialname static void set_PublicProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_InternalProperty() cil managed"
                ".method assembly specialname static int32 get_PrivateProperty() cil managed"
                ".method assembly specialname static void set_PrivateProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_DefaultProperty() cil managed"
                ".method assembly specialname static void set_DefaultProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - static mixed properties`` (realSig) =
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
        |> withName "PublicTypeStaticMixedProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PublicTypeStaticMixedProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public specialname static int32 get_MixedPropertyOne() cil managed"
                ".method assembly specialname static void set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public specialname static int32 get_MixedPropertyTwo() cil managed"
                ".method private specialname static void set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method private specialname static int32 get_MixedPropertyThree() cil managed"
                ".method public specialname static void set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method private specialname static int32 get_MixedPropertyFour() cil managed"
                ".method assembly specialname static void set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyFive() cil managed"
                ".method public specialname static void set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly specialname static int32 get_MixedPropertySix() cil managed"
                ".method private specialname static void set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public specialname static int32 get_MixedPropertySeven() cil managed"
                ".method public specialname static void set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public specialname static int32 get_MixedPropertyEight() cil managed"
                ".method assembly specialname static void set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public specialname static int32 get_MixedPropertyNine() cil managed"
                ".method private specialname static void set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public specialname static int32 get_MixedPropertyTen() cil managed"
                ".method public specialname static void set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyEleven() cil managed"
                ".method public specialname static void set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method private specialname static int32 get_MixedPropertyTwelve() cil managed"
                ".method public specialname static void set_MixedPropertyTwelve(int32 _arg12) cil managed"

            else
                ".method public specialname static int32 get_MixedPropertyOne() cil managed"
                ".method assembly specialname static void set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public specialname static int32 get_MixedPropertyTwo() cil managed"
                ".method assembly specialname static void set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyThree() cil managed"
                ".method public specialname static void set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyFour() cil managed"
                ".method assembly specialname static void set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyFive() cil managed"
                ".method public specialname static void set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly specialname static int32 get_MixedPropertySix() cil managed"
                ".method assembly specialname static void set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public specialname static int32 get_MixedPropertySeven() cil managed"
                ".method public specialname static void set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public specialname static int32 get_MixedPropertyEight() cil managed"
                ".method assembly specialname static void set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public specialname static int32 get_MixedPropertyNine() cil managed"
                ".method assembly specialname static void set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public specialname static int32 get_MixedPropertyTen() cil managed"
                ".method public specialname static void set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyEleven() cil managed"
                ".method public specialname static void set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyTwelve() cil managed"
                ".method public specialname static void set_MixedPropertyTwelve(int32 _arg12) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - static mixed properties`` (realSig) =
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
        |> withName "PrivateTypeStaticMixedProperties"
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*PrivateTypeStaticMixedProperties.dll Verified."
            ]
        |> withILContains [
            if realSig then
                ".method public specialname static int32 get_MixedPropertyOne() cil managed"
                ".method assembly specialname static void set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public specialname static int32 get_MixedPropertyTwo() cil managed"
                ".method private specialname static void set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method private specialname static int32 get_MixedPropertyThree() cil managed"
                ".method public specialname static void set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method private specialname static int32 get_MixedPropertyFour() cil managed"
                ".method assembly specialname static void set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyFive() cil managed"
                ".method public specialname static void set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly specialname static int32 get_MixedPropertySix() cil managed"
                ".method private specialname static void set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public specialname static int32 get_MixedPropertySeven() cil managed"
                ".method public specialname static void set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public specialname static int32 get_MixedPropertyEight() cil managed"
                ".method assembly specialname static void set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public specialname static int32 get_MixedPropertyNine() cil managed"
                ".method private specialname static void set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public specialname static int32 get_MixedPropertyTen() cil managed"
                ".method public specialname static void set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyEleven() cil managed"
                ".method public specialname static void set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method private specialname static int32 get_MixedPropertyTwelve() cil managed"
                ".method public specialname static void set_MixedPropertyTwelve(int32 _arg12) cil managed"

            else
                ".method assembly specialname static int32 get_MixedPropertyOne() cil managed"
                ".method assembly specialname static void set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyTwo() cil managed"
                ".method assembly specialname static void set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyThree() cil managed"
                ".method assembly specialname static void set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyFour() cil managed"
                ".method assembly specialname static void set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyFive() cil managed"
                ".method assembly specialname static void set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly specialname static int32 get_MixedPropertySix() cil managed"
                ".method assembly specialname static void set_MixedPropertySix(int32 _arg6) cil managed"
                ".method assembly specialname static int32 get_MixedPropertySeven() cil managed"
                ".method assembly specialname static void set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyEight() cil managed"
                ".method assembly specialname static void set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyNine() cil managed"
                ".method assembly specialname static void set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyTen() cil managed"
                ".method assembly specialname static void set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyEleven() cil managed"
                ".method assembly specialname static void set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method assembly specialname static int32 get_MixedPropertyTwelve() cil managed"
                ".method assembly specialname static void set_MixedPropertyTwelve(int32 _arg12) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true, "private")>]         // RealSig
    [<InlineData(false, "private")>]        // Regular
    [<InlineData(true, "internal")>]        // RealSig
    [<InlineData(false, "internal")>]       // Regular
    [<InlineData(true, "public")>]          // RealSig
    [<InlineData(false, "public")>]         // Regular
    [<Theory>]
    let ``lazy operation - member with various visibilities`` (realSig, scope) =
        FSharp $"""
module internal SR =
    let {scope} lazyThing = lazy ( () )
    let getLazyThing () = lazyThing.Force()
SR.getLazyThing ()
"""
        |> withName "LazyOperationMemberWithVariousVisibilities"
        |> asExe
        |> withOptimize
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyPEFileWithSystemDlls
        |> withOutputContainsAllInOrderWithWildcards [
            "All Classes and Methods in*LazyOperationMemberWithVariousVisibilities.exe Verified."
            ]
