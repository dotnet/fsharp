// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test.Compiler

module ClassTypeVisibilityNamespaceRootWithFsi =

    let withRealInternalSignature realSig compilation =
        compilation
        |> withOptions [if realSig then "--realsig+" else "--realsig-" ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - constructors`` (realSig) =
        Fsi """
namespace RealInternalSignature

type public TypeOne = public new: unit -> TypeOne
type public TypeTwo = internal new: unit -> TypeTwo
type public TypeThree = private new: unit -> TypeThree
type public TypeFour = public new: unit -> TypeFour
"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TypeOne () = class end
type TypeTwo () = class end
type TypeThree () = class end
type TypeFour () = class end
type HiddenType () = class end
        """))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                """
.class public auto ansi serializable RealInternalSignature.TypeOne
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
.class public auto ansi serializable RealInternalSignature.TypeTwo
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .method assembly specialname rtspecialname instance void  .ctor() cil managed
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
.class public auto ansi serializable RealInternalSignature.TypeThree
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .method private specialname rtspecialname instance void  .ctor() cil managed
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
.class public auto ansi serializable RealInternalSignature.TypeFour
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
.class private auto ansi serializable RealInternalSignature.HiddenType
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

            else
                """
.class public auto ansi serializable RealInternalSignature.TypeOne
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
.class public auto ansi serializable RealInternalSignature.TypeTwo
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .method assembly specialname rtspecialname instance void  .ctor() cil managed
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
.class public auto ansi serializable RealInternalSignature.TypeThree
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
  .method assembly specialname rtspecialname instance void  .ctor() cil managed
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
.class public auto ansi serializable RealInternalSignature.TypeFour
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
.class private auto ansi serializable RealInternalSignature.HiddenType
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

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - constructors`` (realSig) =
        Fsi """
namespace RealInternalSignature

type private TypeOne = public new: unit -> TypeOne
type private TypeTwo = internal new: unit -> TypeTwo
type private TypeThree = private new: unit -> TypeThree
type private TypeFour = public new: unit -> TypeFour
"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TypeOne () = class end
type TypeTwo () = class end
type TypeThree () = class end
type TypeFour () = class end
type HiddenType () = class end
        """))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
                """
.class private auto ansi serializable RealInternalSignature.TypeOne
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
.class private auto ansi serializable RealInternalSignature.TypeTwo
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
.class private auto ansi serializable RealInternalSignature.TypeThree
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
.class private auto ansi serializable RealInternalSignature.TypeFour
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
.class private auto ansi serializable RealInternalSignature.HiddenType
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

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - instance methods`` (realSig) =
        Fsi """
namespace RealInternalSignature
    type public TestType =
        new: unit -> TestType
        member DefaultMethod: unit -> unit
        member internal InternalMethod: unit -> unit
        member private PrivateMethod: unit -> unit
        member PublicMethod: unit -> unit"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    member _.PublicMethod() = ()
    member _.InternalMethod() = ()
    member _.PrivateMethod() = ()
    member _.DefaultMethod() = ()
    member _.HiddenMethod() = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method private hidebysig instance void PrivateMethod() cil managed"
                ".method public hidebysig instance void DefaultMethod() cil managed"
                ".method assembly hidebysig instance void HiddenMethod() cil managed"

            else
                ".method public hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method assembly hidebysig instance void PrivateMethod() cil managed"
                ".method public hidebysig instance void DefaultMethod() cil managed"
                ".method assembly hidebysig instance void HiddenMethod() cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - instance methods`` (realSig) =
        Fsi """
namespace RealInternalSignature
    type private TestType =
        new: unit -> TestType
        member DefaultMethod: unit -> unit
        member internal InternalMethod: unit -> unit
        member private PrivateMethod: unit -> unit
        member PublicMethod: unit -> unit"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    member _.PublicMethod() = ()
    member _.InternalMethod() = ()
    member _.PrivateMethod() = ()
    member _.DefaultMethod() = ()
    member _.HiddenMethod() = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method private hidebysig instance void PrivateMethod() cil managed"
                ".method public hidebysig instance void DefaultMethod() cil managed"
                ".method assembly hidebysig instance void HiddenMethod() cil managed"

            else
                ".method assembly hidebysig instance void PublicMethod() cil managed"
                ".method assembly hidebysig instance void InternalMethod() cil managed"
                ".method assembly hidebysig instance void PrivateMethod() cil managed"
                ".method assembly hidebysig instance void DefaultMethod() cil managed"
                ".method assembly hidebysig instance void HiddenMethod() cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - instance properties`` (realSig) =
        Fsi """
namespace RealInternalSignature
type public TestType =
    new: unit -> TestType
    member public PublicProperty: int with get
    member public PublicProperty: int with set
    member internal InternalProperty: int with get
    member internal InternalProperty: int with set
    member private PrivateProperty: int with get
    member private PrivateProperty: int with set
    member DefaultProperty: int with get
    member DefaultProperty: int with set"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    member val PublicProperty = 0 with get, set
    member val InternalProperty = 0 with get, set
    member val PrivateProperty = 0 with get, set
    member val DefaultProperty = 0 with get, set
    member val HiddenProperty = 0 with get, set"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public hidebysig specialname instance int32 get_PublicProperty() cil managed"
                ".method public hidebysig specialname instance void set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method private hidebysig specialname instance int32 get_PrivateProperty() cil managed"
                ".method private hidebysig specialname instance void set_PrivateProperty(int32 v) cil managed"
                ".method public hidebysig specialname instance int32 get_DefaultProperty() cil managed"
                ".method public hidebysig specialname instance void set_DefaultProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_HiddenProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_HiddenProperty(int32 v) cil managed"

            else
                ".method public hidebysig specialname instance int32 get_PublicProperty() cil managed"
                ".method public hidebysig specialname instance void set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_PrivateProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_PrivateProperty(int32 v) cil managed"
                ".method public hidebysig specialname instance int32 get_DefaultProperty() cil managed"
                ".method public hidebysig specialname instance void set_DefaultProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_HiddenProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_HiddenProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - instance properties`` (realSig) =
        Fsi """
namespace RealInternalSignature
type private TestType =
    new: unit -> TestType
    member public PublicProperty: int with get
    member public PublicProperty: int with set
    member internal InternalProperty: int with get
    member internal InternalProperty: int with set
    member private PrivateProperty: int with get
    member private PrivateProperty: int with set
    member DefaultProperty: int with get
    member DefaultProperty: int with set"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    member val PublicProperty = 0 with get, set
    member val InternalProperty = 0 with get, set
    member val PrivateProperty = 0 with get, set
    member val DefaultProperty = 0 with get, set
    member val HiddenProperty = 0 with get, set"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public hidebysig specialname instance int32 get_PublicProperty() cil managed"
                ".method public hidebysig specialname instance void set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method private hidebysig specialname instance int32 get_PrivateProperty() cil managed"
                ".method private hidebysig specialname instance void set_PrivateProperty(int32 v) cil managed"
                ".method public hidebysig specialname instance int32 get_DefaultProperty() cil managed"
                ".method public hidebysig specialname instance void set_DefaultProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_HiddenProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_HiddenProperty(int32 v) cil managed"

            else
                ".method assembly hidebysig specialname instance int32  get_PublicProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_PublicProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_InternalProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_InternalProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_PrivateProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_PrivateProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_DefaultProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_DefaultProperty(int32 v) cil managed"
                ".method assembly hidebysig specialname instance int32  get_HiddenProperty() cil managed"
                ".method assembly hidebysig specialname instance void  set_HiddenProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - instance mixed properties`` (realSig) =
        Fsi """
namespace RealInternalSignature
    type TestType =
        new: unit -> TestType
        member public MixedPropertyOne: int with get
        member internal MixedPropertyOne: int with set
        member public MixedPropertyTwo: int with get
        member private MixedPropertyTwo: int with set
        member private MixedPropertyThree: int with get
        member public MixedPropertyThree: int with set
        member private MixedPropertyFour: int with get
        member internal MixedPropertyFour: int with set
        member internal MixedPropertyFive: int with get
        member public MixedPropertyFive: int with set
        member internal MixedPropertySix: int with get
        member private MixedPropertySix: int with set
        member public MixedPropertySeven: int with get
        member public MixedPropertySeven: int with set
        member public MixedPropertyEight: int with get
        member internal MixedPropertyEight: int with set
        member public MixedPropertyNine: int with get
        member private MixedPropertyNine: int with set
        member MixedPropertyTen: int with get
        member MixedPropertyTen: int with set
        member internal MixedPropertyEleven: int with get
        member public MixedPropertyEleven: int with set
        member private MixedPropertyTwelve: int with get
        member public MixedPropertyTwelve: int with set"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type public TestType () =
    member _.MixedPropertyOne with get() = 0 and set (_:int) = ()
    member _.MixedPropertyTwo with get() = 0 and set (_:int) = ()
    member _.MixedPropertyThree with get() = 0 and set (_:int) = ()
    member _.MixedPropertyFour with get() = 0 and set (_:int) = ()
    member _.MixedPropertyFive with get() = 0 and set (_:int) = ()
    member _.MixedPropertySix with get() = 0 and set (_:int) = ()
    member _.MixedPropertySeven with get() = 0 and set (_:int) = ()
    member _.MixedPropertyEight with get() = 0 and set (_:int) = ()
    member _.MixedPropertyNine with get() = 0 and set (_:int) = ()
    member _.MixedPropertyTen with get() = 0 and set (_:int) = ()
    member _.MixedPropertyEleven with get() = 0 and set (_:int) = ()
    member _.MixedPropertyTwelve with get() = 0 and set (_:int) = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public specialname rtspecialname instance void  .ctor() cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyOne() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyTwo() cil managed"
                ".method private hidebysig specialname instance void set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method private hidebysig specialname instance int32 get_MixedPropertyThree() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method private hidebysig specialname instance int32 get_MixedPropertyFour() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFive() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySix() cil managed"
                ".method private hidebysig specialname instance void set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertySeven() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyEight() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyNine() cil managed"
                ".method private hidebysig specialname instance void set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyTen() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEleven() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method private hidebysig specialname instance int32 get_MixedPropertyTwelve() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyTwelve(int32 _arg12) cil managed"

            else
                ".method public specialname rtspecialname instance void  .ctor() cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyOne() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyTwo() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyThree() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFour() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFive() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySix() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertySeven() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyEight() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyNine() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyTen() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEleven() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyTwelve() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyTwelve(int32 _arg12) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - instance mixed properties`` (realSig) =
        Fsi """
namespace RealInternalSignature
    type private TestType =
        new: unit -> TestType
        member public MixedPropertyOne: int with get
        member internal MixedPropertyOne: int with set
        member public MixedPropertyTwo: int with get
        member private MixedPropertyTwo: int with set
        member private MixedPropertyThree: int with get
        member public MixedPropertyThree: int with set
        member private MixedPropertyFour: int with get
        member internal MixedPropertyFour: int with set
        member internal MixedPropertyFive: int with get
        member public MixedPropertyFive: int with set
        member internal MixedPropertySix: int with get
        member private MixedPropertySix: int with set
        member public MixedPropertySeven: int with get
        member public MixedPropertySeven: int with set
        member public MixedPropertyEight: int with get
        member internal MixedPropertyEight: int with set
        member public MixedPropertyNine: int with get
        member private MixedPropertyNine: int with set
        member public MixedPropertyTen: int with get
        member public MixedPropertyTen: int with set
        member internal MixedPropertyEleven: int with get
        member public MixedPropertyEleven: int with set
        member private MixedPropertyTwelve: int with get
        member public MixedPropertyTwelve: int with set"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type public TestType () =
    member _.MixedPropertyOne with get() = 0 and set (_:int) = ()
    member _.MixedPropertyTwo with get() = 0 and set (_:int) = ()
    member _.MixedPropertyThree with get() = 0 and set (_:int) = ()
    member _.MixedPropertyFour with get() = 0 and set (_:int) = ()
    member _.MixedPropertyFive with get() = 0 and set (_:int) = ()
    member _.MixedPropertySix with get() = 0 and set (_:int) = ()
    member _.MixedPropertySeven with get() = 0 and set (_:int) = ()
    member _.MixedPropertyEight with get() = 0 and set (_:int) = ()
    member _.MixedPropertyNine with get() = 0 and set (_:int) = ()
    member _.MixedPropertyTen with get() = 0 and set (_:int) = ()
    member _.MixedPropertyEleven with get() = 0 and set (_:int) = ()
    member _.MixedPropertyTwelve with get() = 0 and set (_:int) = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public hidebysig specialname instance int32 get_MixedPropertyOne() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyOne(int32 _arg1) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyTwo() cil managed"
                ".method private hidebysig specialname instance void set_MixedPropertyTwo(int32 _arg2) cil managed"
                ".method private hidebysig specialname instance int32 get_MixedPropertyThree() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyThree(int32 _arg3) cil managed"
                ".method private hidebysig specialname instance int32 get_MixedPropertyFour() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyFour(int32 _arg4) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyFive() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyFive(int32 _arg5) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertySix() cil managed"
                ".method private hidebysig specialname instance void set_MixedPropertySix(int32 _arg6) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertySeven() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertySeven(int32 _arg7) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyEight() cil managed"
                ".method assembly hidebysig specialname instance void  set_MixedPropertyEight(int32 _arg8) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyNine() cil managed"
                ".method private hidebysig specialname instance void set_MixedPropertyNine(int32 _arg9) cil managed"
                ".method public hidebysig specialname instance int32 get_MixedPropertyTen() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyTen(int32 _arg10) cil managed"
                ".method assembly hidebysig specialname instance int32  get_MixedPropertyEleven() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyEleven(int32 _arg11) cil managed"
                ".method private hidebysig specialname instance int32 get_MixedPropertyTwelve() cil managed"
                ".method public hidebysig specialname instance void set_MixedPropertyTwelve(int32 _arg12) cil managed"
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
        Fsi """
namespace RealInternalSignature
    type public TestType =
        new: unit -> TestType
        static member DefaultMethod: unit -> unit
        static member internal InternalMethod: unit -> unit
        static member private PrivateMethod: unit -> unit
        static member PublicMethod: unit -> unit"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    static member PublicMethod() = ()
    static member InternalMethod() = ()
    static member PrivateMethod() = ()
    static member DefaultMethod() = ()
    static member HiddenMethod() = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method private static void  PrivateMethod() cil managed"
                ".method public static void  DefaultMethod() cil managed"
                ".method assembly static void  HiddenMethod() cil managed"

            else
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method assembly static void  PrivateMethod() cil managed"
                ".method public static void  DefaultMethod() cil managed"
                ".method assembly static void  HiddenMethod() cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - static methods`` (realSig) =
        FSharp """
namespace RealInternalSignature

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
    let ``public type - static properties`` (realSig) =
        Fsi """
namespace RealInternalSignature
type public TestType =
    new: unit -> TestType
    static member public PublicProperty: int with get
    static member public PublicProperty: int with set
    static member internal InternalProperty: int with get
    static member internal InternalProperty: int with set
    static member private PrivateProperty: int with get
    static member private PrivateProperty: int with set
    static member DefaultProperty: int with get
    static member DefaultProperty: int with set"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    static member val PublicProperty = 0 with get, set
    static member val InternalProperty = 0 with get, set
    static member val PrivateProperty = 0 with get, set
    static member val DefaultProperty = 0 with get, set
    static member val HiddenProperty = 0 with get, set"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
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
                ".method assembly specialname static int32 get_HiddenProperty() cil managed"
                ".method assembly specialname static void set_HiddenProperty(int32 v) cil managed"

            else
                ".method public specialname static int32 get_PublicProperty() cil managed"
                ".method public specialname static void set_PublicProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_InternalProperty() cil managed"
                ".method assembly specialname static void set_InternalProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_PrivateProperty() cil managed"
                ".method assembly specialname static void set_PrivateProperty(int32 v) cil managed"
                ".method public specialname static int32 get_DefaultProperty() cil managed"
                ".method public specialname static void set_DefaultProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_HiddenProperty() cil managed"
                ".method assembly specialname static void set_HiddenProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``private type - static properties`` (realSig) =
        Fsi """
namespace RealInternalSignature
type private TestType =
    new: unit -> TestType
    static member PublicProperty: int with get
    static member PublicProperty: int with set
    static member internal InternalProperty: int with get
    static member internal InternalProperty: int with set
    static member private PrivateProperty: int with get
    static member private PrivateProperty: int with set
    static member DefaultProperty: int with get
    static member DefaultProperty: int with set"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    static member val PublicProperty = 0 with get, set
    static member val InternalProperty = 0 with get, set
    static member val PrivateProperty = 0 with get, set
    static member val DefaultProperty = 0 with get, set
    static member val HiddenProperty = 0 with get, set"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
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
                ".method assembly specialname static int32 get_HiddenProperty() cil managed"
                ".method assembly specialname static void set_HiddenProperty(int32 v) cil managed"

            else
                ".method assembly specialname static int32 get_PublicProperty() cil managed"
                ".method assembly specialname static void set_PublicProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_InternalProperty() cil managed"
                ".method assembly specialname static void set_InternalProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_PrivateProperty() cil managed"
                ".method assembly specialname static void set_PrivateProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_DefaultProperty() cil managed"
                ".method assembly specialname static void set_DefaultProperty(int32 v) cil managed"
                ".method assembly specialname static int32 get_HiddenProperty() cil managed"
                ".method assembly specialname static void set_HiddenProperty(int32 v) cil managed"

            ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - static mixed properties`` (realSig) =
        Fsi """
namespace RealInternalSignature
    type public TestType =
        new: unit -> TestType
        static member public MixedPropertyOne: int with get
        static member internal MixedPropertyOne: int with set
        static member public MixedPropertyTwo: int with get        
        static member private MixedPropertyTwo: int with set
        static member private MixedPropertyThree: int with get
        static member public MixedPropertyThree: int with set
        static member private MixedPropertyFour: int with get
        static member internal MixedPropertyFour: int with set
        static member internal MixedPropertyFive: int with get
        static member public MixedPropertyFive: int with set
        static member internal MixedPropertySix: int with get
        static member private MixedPropertySix: int with set
        static member public MixedPropertySeven: int with get
        static member public MixedPropertySeven: int with set
        static member MixedPropertyEight: int with get
        static member internal MixedPropertyEight: int with set
        static member MixedPropertyNine: int with get
        static member private MixedPropertyNine: int with set
        static member MixedPropertyTen: int with get
        static member MixedPropertyTen: int with set
        static member internal MixedPropertyEleven: int with get
        static member MixedPropertyEleven: int with set
        static member private MixedPropertyTwelve: int with get
        static member MixedPropertyTwelve: int with set
"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

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
    static member MixedPropertyTwelve with private get() = 0 and set (_:int) = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
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
        Fsi """
namespace RealInternalSignature
type private TestType =
    new: unit -> TestType
    static member public MixedPropertyOne: int with get
    static member internal MixedPropertyOne: int with set
    static member public MixedPropertyTwo: int with get
    static member private MixedPropertyTwo: int with set
    static member private MixedPropertyThree: int with get
    static member public MixedPropertyThree: int with set
    static member private MixedPropertyFour: int with get
    static member internal MixedPropertyFour: int with set
    static member internal MixedPropertyFive: int with get
    static member public MixedPropertyFive: int with set
    static member internal MixedPropertySix: int with get
    static member private MixedPropertySix: int with set
    static member MixedPropertySeven: int with get
    static member MixedPropertySeven: int with set
    static member public MixedPropertySeven: int with set
    static member MixedPropertyEight: int with get
    static member internal MixedPropertyEight: int with set
    static member MixedPropertyNine: int with get
    static member private MixedPropertyNine: int with set
    static member MixedPropertyTen: int with get
    static member MixedPropertyTen: int with set
    static member internal MixedPropertyEleven: int with get
    static member MixedPropertyEleven: int with set
    static member private MixedPropertyTwelve: int with get
    static member MixedPropertyTwelve: int with set
"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

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
    static member MixedPropertyTwelve with private get() = 0 and set (_:int) = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
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


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - static methods - hide by omission`` (realSig) =
        Fsi """
namespace RealInternalSignature
    type public TestType =
        new: unit -> TestType
        static member PublicMethod: unit -> unit"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    static member PublicMethod() = ()
    static member InternalMethod() = ()
    static member PrivateMethod() = ()
    static member DefaultMethod() = ()
    static member HiddenMethod() = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method assembly static void  PrivateMethod() cil managed"
                ".method assembly static void  DefaultMethod() cil managed"
                ".method assembly static void  HiddenMethod() cil managed"

            else
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method assembly static void  PrivateMethod() cil managed"
                ".method assembly static void  DefaultMethod() cil managed"
                ".method assembly static void  HiddenMethod() cil managed"

            ]
        |> shouldSucceed


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``public type - public nested type - hide by omission`` (realSig) =
        Fsi """
namespace RealInternalSignature
    type public TestType =
        new: unit -> TestType
        type public NestedTestType =
            new: unit -> NestedTestType
            static member PublicMethod: unit -> unit"""
        |> withAdditionalSourceFile (FsSource ("""
namespace RealInternalSignature

type TestType () =
    static member PublicMethod() = ()
    type NestedTestType () =
        static member PublicMethod() = ()
        static member InternalMethod() = ()
        static member PrivateMethod() = ()
        static member DefaultMethod() = ()
        static member HiddenMethod() = ()"""))
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> withILContains [
            if realSig then
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method assembly static void  PrivateMethod() cil managed"
                ".method assembly static void  DefaultMethod() cil managed"
                ".method assembly static void  HiddenMethod() cil managed"
            else
                ".method public static void  PublicMethod() cil managed"
                ".method assembly static void  InternalMethod() cil managed"
                ".method assembly static void  PrivateMethod() cil managed"
                ".method assembly static void  DefaultMethod() cil managed"
                ".method assembly static void  HiddenMethod() cil managed"

            ]
        |> shouldSucceed

