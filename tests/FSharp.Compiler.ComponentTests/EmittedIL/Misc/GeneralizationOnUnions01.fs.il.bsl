
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly GeneralizationOnUnions01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.GeneralizationOnUnions01
{
  // Offset: 0x00000000 Length: 0x0000070B
  // WARNING: managed resource file FSharpSignatureData.GeneralizationOnUnions01 created
}
.mresource public FSharpOptimizationData.GeneralizationOnUnions01
{
  // Offset: 0x00000710 Length: 0x000001F4
  // WARNING: managed resource file FSharpOptimizationData.GeneralizationOnUnions01 created
}
.module GeneralizationOnUnions01.exe
// MVID: {624E1220-4CA2-8CD1-A745-038320124E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03940000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed GeneralizationOnUnions01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class auto autochar serializable sealed nested public beforefieldinit Weirdo
         extends [mscorlib]System.Object
         implements class [mscorlib]System.IEquatable`1<class GeneralizationOnUnions01/Weirdo>,
                    [mscorlib]System.Collections.IStructuralEquatable,
                    class [mscorlib]System.IComparable`1<class GeneralizationOnUnions01/Weirdo>,
                    [mscorlib]System.IComparable,
                    [mscorlib]System.Collections.IStructuralComparable
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerDisplayAttribute::.ctor(string) = ( 01 00 15 7B 5F 5F 44 65 62 75 67 44 69 73 70 6C   // ...{__DebugDispl
                                                                                                   61 79 28 29 2C 6E 71 7D 00 00 )                   // ay(),nq}..
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 01 00 00 00 00 00 ) 
    .field static assembly initonly class GeneralizationOnUnions01/Weirdo _unique_C
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    .method private specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void GeneralizationOnUnions01/Weirdo::.ctor()
      IL_0005:  stsfld     class GeneralizationOnUnions01/Weirdo GeneralizationOnUnions01/Weirdo::_unique_C
      IL_000a:  ret
    } // end of method Weirdo::.cctor

    .method assembly specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Weirdo::.ctor

    .method public static class GeneralizationOnUnions01/Weirdo 
            get_C() cil managed
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags,
                                                                                                  int32) = ( 01 00 08 00 00 00 00 00 00 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     class GeneralizationOnUnions01/Weirdo GeneralizationOnUnions01/Weirdo::_unique_C
      IL_0005:  ret
    } // end of method Weirdo::get_C

    .method public hidebysig instance int32 
            get_Tag() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  pop
      IL_0002:  ldc.i4.0
      IL_0003:  ret
    } // end of method Weirdo::get_Tag

    .method assembly hidebysig specialname 
            instance object  __DebugDisplay() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+0.8A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,string>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Weirdo::__DebugDisplay

    .method public strict virtual instance string 
            ToString() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldstr      "%+A"
      IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,string>,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string,class GeneralizationOnUnions01/Weirdo>::.ctor(string)
      IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatToString<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,string>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit,string,string>)
      IL_000f:  ldarg.0
      IL_0010:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,string>::Invoke(!0)
      IL_0015:  ret
    } // end of method Weirdo::ToString

    .method public hidebysig virtual final 
            instance int32  CompareTo(class GeneralizationOnUnions01/Weirdo obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_000a

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0008

      IL_0006:  ldc.i4.0
      IL_0007:  ret

      IL_0008:  ldc.i4.1
      IL_0009:  ret

      IL_000a:  ldarg.1
      IL_000b:  brfalse.s  IL_000f

      IL_000d:  ldc.i4.m1
      IL_000e:  ret

      IL_000f:  ldc.i4.0
      IL_0010:  ret
    } // end of method Weirdo::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  unbox.any  GeneralizationOnUnions01/Weirdo
      IL_0007:  callvirt   instance int32 GeneralizationOnUnions01/Weirdo::CompareTo(class GeneralizationOnUnions01/Weirdo)
      IL_000c:  ret
    } // end of method Weirdo::CompareTo

    .method public hidebysig virtual final 
            instance int32  CompareTo(object obj,
                                      class [mscorlib]System.Collections.IComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       34 (0x22)
      .maxstack  3
      .locals init (class GeneralizationOnUnions01/Weirdo V_0)
      IL_0000:  ldarg.1
      IL_0001:  unbox.any  GeneralizationOnUnions01/Weirdo
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  brfalse.s  IL_0016

      IL_000a:  ldarg.1
      IL_000b:  unbox.any  GeneralizationOnUnions01/Weirdo
      IL_0010:  brfalse.s  IL_0014

      IL_0012:  ldc.i4.0
      IL_0013:  ret

      IL_0014:  ldc.i4.1
      IL_0015:  ret

      IL_0016:  ldarg.1
      IL_0017:  unbox.any  GeneralizationOnUnions01/Weirdo
      IL_001c:  brfalse.s  IL_0020

      IL_001e:  ldc.i4.m1
      IL_001f:  ret

      IL_0020:  ldc.i4.0
      IL_0021:  ret
    } // end of method Weirdo::CompareTo

    .method public hidebysig virtual final 
            instance int32  GetHashCode(class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       11 (0xb)
      .maxstack  3
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0009

      IL_0003:  ldc.i4.0
      IL_0004:  stloc.0
      IL_0005:  ldarg.0
      IL_0006:  pop
      IL_0007:  ldc.i4.0
      IL_0008:  ret

      IL_0009:  ldc.i4.0
      IL_000a:  ret
    } // end of method Weirdo::GetHashCode

    .method public hidebysig virtual final 
            instance int32  GetHashCode() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       12 (0xc)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
      IL_0006:  callvirt   instance int32 GeneralizationOnUnions01/Weirdo::GetHashCode(class [mscorlib]System.Collections.IEqualityComparer)
      IL_000b:  ret
    } // end of method Weirdo::GetHashCode

    .method public hidebysig virtual final 
            instance bool  Equals(object obj,
                                  class [mscorlib]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       27 (0x1b)
      .maxstack  4
      .locals init (class GeneralizationOnUnions01/Weirdo V_0,
               class GeneralizationOnUnions01/Weirdo V_1)
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0013

      IL_0003:  ldarg.1
      IL_0004:  isinst     GeneralizationOnUnions01/Weirdo
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0011

      IL_000d:  ldloc.0
      IL_000e:  stloc.1
      IL_000f:  ldc.i4.1
      IL_0010:  ret

      IL_0011:  ldc.i4.0
      IL_0012:  ret

      IL_0013:  ldarg.1
      IL_0014:  ldnull
      IL_0015:  cgt.un
      IL_0017:  ldc.i4.0
      IL_0018:  ceq
      IL_001a:  ret
    } // end of method Weirdo::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(class GeneralizationOnUnions01/Weirdo obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0008

      IL_0003:  ldarg.1
      IL_0004:  ldnull
      IL_0005:  cgt.un
      IL_0007:  ret

      IL_0008:  ldarg.1
      IL_0009:  ldnull
      IL_000a:  cgt.un
      IL_000c:  ldc.i4.0
      IL_000d:  ceq
      IL_000f:  ret
    } // end of method Weirdo::Equals

    .method public hidebysig virtual final 
            instance bool  Equals(object obj) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 )
      // Code size       20 (0x14)
      .maxstack  4
      .locals init (class GeneralizationOnUnions01/Weirdo V_0)
      IL_0000:  ldarg.1
      IL_0001:  isinst     GeneralizationOnUnions01/Weirdo
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  brfalse.s  IL_0012

      IL_000a:  ldarg.0
      IL_000b:  ldloc.0
      IL_000c:  callvirt   instance bool GeneralizationOnUnions01/Weirdo::Equals(class GeneralizationOnUnions01/Weirdo)
      IL_0011:  ret

      IL_0012:  ldc.i4.0
      IL_0013:  ret
    } // end of method Weirdo::Equals

    .property instance int32 Tag()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get instance int32 GeneralizationOnUnions01/Weirdo::get_Tag()
    } // end of property Weirdo::Tag
    .property class GeneralizationOnUnions01/Weirdo
            C()
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
      .get class GeneralizationOnUnions01/Weirdo GeneralizationOnUnions01/Weirdo::get_C()
    } // end of property Weirdo::C
  } // end of class Weirdo

  .class auto ansi serializable sealed nested assembly beforefieldinit f@8
         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,int32>
  {
    .field public int32 C
    .method assembly specialname rtspecialname 
            instance void  .ctor(int32 C) cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,int32>::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      int32 GeneralizationOnUnions01/f@8::C
      IL_000d:  ret
    } // end of method f@8::.ctor

    .method public strict virtual instance int32 
            Invoke(class GeneralizationOnUnions01/Weirdo _arg1) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  5
      .locals init (class GeneralizationOnUnions01/Weirdo V_0)
      IL_0000:  ldarg.1
      IL_0001:  stloc.0
      IL_0002:  ldarg.0
      IL_0003:  ldfld      int32 GeneralizationOnUnions01/f@8::C
      IL_0008:  ret
    } // end of method f@8::Invoke

  } // end of class f@8

  .method public static int32  f(class GeneralizationOnUnions01/Weirdo C) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  3
    .locals init (class GeneralizationOnUnions01/Weirdo V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  ret
  } // end of method GeneralizationOnUnions01::f

  .method public static void  g() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  3
    .locals init (int32 V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class GeneralizationOnUnions01/Weirdo,int32> V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  newobj     instance void GeneralizationOnUnions01/f@8::.ctor(int32)
    IL_0008:  stloc.1
    IL_0009:  ret
  } // end of method GeneralizationOnUnions01::g

} // end of class GeneralizationOnUnions01

.class private abstract auto ansi sealed '<StartupCode$GeneralizationOnUnions01>'.$GeneralizationOnUnions01
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.Operators::Exit<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(int32)
    IL_0006:  pop
    IL_0007:  ret
  } // end of method $GeneralizationOnUnions01::main@

} // end of class '<StartupCode$GeneralizationOnUnions01>'.$GeneralizationOnUnions01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\kevinransom\fsharp\tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\GeneralizationOnUnions01_fs\GeneralizationOnUnions01.res
