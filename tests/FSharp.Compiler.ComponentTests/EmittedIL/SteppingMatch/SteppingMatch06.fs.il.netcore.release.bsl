
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly SteppingMatch06
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch06
{
  // Offset: 0x00000000 Length: 0x000006B7
  // WARNING: managed resource file FSharpSignatureData.SteppingMatch06 created
}
.mresource public FSharpOptimizationData.SteppingMatch06
{
  // Offset: 0x000006C0 Length: 0x000001DF
  // WARNING: managed resource file FSharpOptimizationData.SteppingMatch06 created
}
.module SteppingMatch06.exe
// MVID: {624CDB59-FA41-309E-A745-038359DB4C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000233862B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch06
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit Discr
         extends [System.Runtime]System.Object
         implements class [System.Runtime]System.IEquatable`1<class SteppingMatch06/Discr>,
                    [System.Runtime]System.Collections.IStructuralEquatable,
                    class [System.Runtime]System.IComparable`1<class SteppingMatch06/Discr>,
                    [System.Runtime]System.IComparable,
                    [System.Runtime]System.Collections.IStructuralComparable
  {
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                         61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .class abstract auto ansi sealed nested public Tags
           extends [System.Runtime]System.Object
    {
      .field public static literal int32 CaseA = int32(0x00000000)
      .field public static literal int32 CaseB = int32(0x00000001)
    } // end of class Tags

    .field assembly initonly int32 _tag
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SteppingMatch06/Discr _unique_CaseA
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .field static assembly initonly class SteppingMatch06/Discr _unique_CaseB
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  newobj     instance void SteppingMatch06/Discr::.ctor(int32)
      IL_0006:  stsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseA
      IL_000b:  ldc.i4.1
      IL_000c:  newobj     instance void SteppingMatch06/Discr::.ctor(int32)
      IL_0011:  stsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseB
      IL_0016:  ret
    } // end of method Discr::.cctor

    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 _tag) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 SteppingMatch06/Discr::_tag
      IL_000d:  ret
    } // end of method Discr::.ctor

    .method public static class SteppingMatch06/Discr 
            get_CaseA() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseA
      IL_0005:  ret
    } // end of method Discr::get_CaseA

    .method public hidebysig instance bool 
            get_IsCaseA() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 SteppingMatch06/Discr::get_Tag()
      IL_0006:  ldc.i4.0
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Discr::get_IsCaseA

    .method public static class SteppingMatch06/Discr 
            get_CaseB() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 01 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class SteppingMatch06/Discr SteppingMatch06/Discr::_unique_CaseB
      IL_0005:  ret
    } // end of method Discr::get_CaseB

    .method public hidebysig instance bool 
            get_IsCaseB() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance int32 SteppingMatch06/Discr::get_Tag()
      IL_0006:  ldc.i4.1
      IL_0007:  ceq
      IL_0009:  ret
    } // end of method Discr::get_IsCaseB

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_0006:  ret
    } // end of method Discr::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Discr::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class SteppingMatch06/Discr>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class SteppingMatch06/Discr,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Discr::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class SteppingMatch06/Discr obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       39 (0x27)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0020

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_001e

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldloc.1
      IL_0016:  bne.un.s   IL_001a

      IL_0018:  ldc.i4.0
      IL_0019:  ret

      IL_001a:  ldloc.0
      IL_001b:  ldloc.1
      IL_001c:  sub
      IL_001d:  ret

      IL_001e:  ldc.i4.1
      IL_001f:  ret

      IL_0020:  ldarg.1
      IL_0021:  brfalse.s  IL_0025

      IL_0023:  ldc.i4.m1
      IL_0024:  ret

      IL_0025:  ldc.i4.0
      IL_0026:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  SteppingMatch06/Discr
      IL_0007:  callvirt   instance int32 SteppingMatch06/Discr::CompareTo(class SteppingMatch06/Discr)
      IL_000c:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [System.Runtime]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       56 (0x38)
      .maxstack  4
      .locals init (class SteppingMatch06/Discr V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  SteppingMatch06/Discr
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_002c

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  SteppingMatch06/Discr
      IL_0010:  brfalse.s  IL_002a

      IL_0012:  ldarg.0
      IL_0013:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_0018:  stloc.1
      IL_0019:  ldloc.0
      IL_001a:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_001f:  stloc.2
      IL_0020:  ldloc.1
      IL_0021:  ldloc.2
      IL_0022:  bne.un.s   IL_0026

      IL_0024:  ldc.i4.0
      IL_0025:  ret

      IL_0026:  ldloc.1
      IL_0027:  ldloc.2
      IL_0028:  sub
      IL_0029:  ret

      IL_002a:  ldc.i4.1
      IL_002b:  ret

      IL_002c:  ldarg.1
      IL_002d:  unbox.any  SteppingMatch06/Discr
      IL_0032:  brfalse.s  IL_0036

      IL_0034:  ldc.i4.m1
      IL_0035:  ret

      IL_0036:  ldc.i4.0
      IL_0037:  ret
    } // end of method Discr::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_000c

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldarg.0
      IL_0006:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_000b:  ret

      IL_000c:  ldc.i4.0
      IL_000d:  ret
    } // end of method Discr::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [System.Runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 SteppingMatch06/Discr::GetHashCode(class [System.Runtime]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Discr::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [System.Runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       44 (0x2c)
      .maxstack  4
      .locals init (class SteppingMatch06/Discr V_0,
               class SteppingMatch06/Discr V_1,
               int32 V_2,
               int32 V_3)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0024

      IL_0003:  ldarg.1
      IL_0004:  isinst     SteppingMatch06/Discr
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0022

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldarg.0
      IL_0010:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_0015:  stloc.2
      IL_0016:  ldloc.1
      IL_0017:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_001c:  stloc.3
      IL_001d:  ldloc.2
      IL_001e:  ldloc.3
      IL_001f:  ceq
      IL_0021:  ret

      IL_0022:  ldc.i4.0
      IL_0023:  ret

      IL_0024:  ldarg.1
      IL_0025:  ldnull
      IL_0026:  cgt.un
      IL_0028:  ldc.i4.0
      IL_0029:  ceq
      IL_002b:  ret
    } // end of method Discr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class SteppingMatch06/Discr obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       35 (0x23)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_001b

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0019

      IL_0006:  ldarg.0
      IL_0007:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_000c:  stloc.0
      IL_000d:  ldarg.1
      IL_000e:  ldfld      int32 SteppingMatch06/Discr::_tag
      IL_0013:  stloc.1
      IL_0014:  ldloc.0
      IL_0015:  ldloc.1
      IL_0016:  ceq
      IL_0018:  ret

      IL_0019:  ldc.i4.0
      IL_001a:  ret

      IL_001b:  ldarg.1
      IL_001c:  ldnull
      IL_001d:  cgt.un
      IL_001f:  ldc.i4.0
      IL_0020:  ceq
      IL_0022:  ret
    } // end of method Discr::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class SteppingMatch06/Discr V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     SteppingMatch06/Discr
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool SteppingMatch06/Discr::Equals(class SteppingMatch06/Discr)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method Discr::Equals

    .property instance int32 Tag()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 SteppingMatch06/Discr::get_Tag()
    } // end of property Discr::Tag
    .property class SteppingMatch06/Discr
            CaseA()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SteppingMatch06/Discr SteppingMatch06/Discr::get_CaseA()
    } // end of property Discr::CaseA
    .property instance bool IsCaseA()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SteppingMatch06/Discr::get_IsCaseA()
    } // end of property Discr::IsCaseA
    .property class SteppingMatch06/Discr
            CaseB()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class SteppingMatch06/Discr SteppingMatch06/Discr::get_CaseB()
    } // end of property Discr::CaseB
    .property instance bool IsCaseB()
    {
      .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [System.Runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance bool SteppingMatch06/Discr::get_IsCaseB()
    } // end of property Discr::IsCaseB
  } // end of class Discr

  .method public static void  funcD(class SteppingMatch06/Discr n) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32 SteppingMatch06/Discr::get_Tag()
    IL_0007:  ldc.i4.0
    IL_0008:  bne.un.s   IL_000c

    IL_000a:  br.s       IL_0017

    IL_000c:  ldstr      "B"
    IL_0011:  call       void [System.Console]System.Console::WriteLine(string)
    IL_0016:  ret

    IL_0017:  ldstr      "A"
    IL_001c:  call       void [System.Console]System.Console::WriteLine(string)
    IL_0021:  ret
  } // end of method SteppingMatch06::funcD

} // end of class SteppingMatch06

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch06>'.$SteppingMatch06
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $SteppingMatch06::main@

} // end of class '<StartupCode$SteppingMatch06>'.$SteppingMatch06


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net6.0\tests\EmittedIL\SteppingMatch\SteppingMatch06_fs\SteppingMatch06.res
