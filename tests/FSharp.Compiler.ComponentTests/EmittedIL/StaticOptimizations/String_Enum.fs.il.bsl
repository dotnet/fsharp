




.assembly extern runtime { }
.assembly extern FSharp.Core { }
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         
  .ver 2:1:0:0
}
.assembly assembly
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  
  

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module assembly.exe

.imagebase {value}
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       
.corflags 0x00000001    





.class public abstract auto ansi sealed assembly
       extends [runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public String
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .class auto ansi serializable sealed nested public CharEnum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname char value__
      .field public static literal valuetype assembly/String/CharEnum Char = char(0x0061)
    } 

    .class auto ansi serializable sealed nested public SByteEnum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname int8 value__
      .field public static literal valuetype assembly/String/SByteEnum SByte = int8(0x01)
    } 

    .class auto ansi serializable sealed nested public Int16Enum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname int16 value__
      .field public static literal valuetype assembly/String/Int16Enum Int16 = int16(0x0001)
    } 

    .class auto ansi serializable sealed nested public Int32Enum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname int32 value__
      .field public static literal valuetype assembly/String/Int32Enum Int32 = int32(0x00000001)
    } 

    .class auto ansi serializable sealed nested public Int64Enum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname int64 value__
      .field public static literal valuetype assembly/String/Int64Enum Int64 = int64(0x1)
    } 

    .class auto ansi serializable sealed nested public ByteEnum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname uint8 value__
      .field public static literal valuetype assembly/String/ByteEnum Byte = uint8(0x01)
    } 

    .class auto ansi serializable sealed nested public UInt16Enum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname uint16 value__
      .field public static literal valuetype assembly/String/UInt16Enum UInt16 = uint16(0x0001)
    } 

    .class auto ansi serializable sealed nested public UInt32Enum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname uint32 value__
      .field public static literal valuetype assembly/String/UInt32Enum UInt32 = uint32(0x00000001)
    } 

    .class auto ansi serializable sealed nested public UInt64Enum
           extends [runtime]System.Enum
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
      .field public specialname rtspecialname uint64 value__
      .field public static literal valuetype assembly/String/UInt64Enum UInt64 = uint64(0x1)
    } 

    .method public static string  'string<CharEnum>'(valuetype assembly/String/CharEnum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/CharEnum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/CharEnum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<SByteEnum>'(valuetype assembly/String/SByteEnum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/SByteEnum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/SByteEnum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<Int16Enum>'(valuetype assembly/String/Int16Enum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/Int16Enum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/Int16Enum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<Int32Enum>'(valuetype assembly/String/Int32Enum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/Int32Enum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/Int32Enum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<Int64Enum>'(valuetype assembly/String/Int64Enum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/Int64Enum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/Int64Enum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<ByteEnum>'(valuetype assembly/String/ByteEnum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/ByteEnum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/ByteEnum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<UInt16Enum>'(valuetype assembly/String/UInt16Enum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/UInt16Enum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/UInt16Enum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<UInt32Enum>'(valuetype assembly/String/UInt32Enum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/UInt32Enum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/UInt32Enum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<UInt64Enum>'(valuetype assembly/String/UInt64Enum 'enum') cil managed
    {
      
      .maxstack  3
      .locals init (valuetype assembly/String/UInt64Enum V_0)
      IL_0000:  ldarg.0
      IL_0001:  stloc.0
      IL_0002:  ldloca.s   V_0
      IL_0004:  constrained. assembly/String/UInt64Enum
      IL_000a:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_000f:  ret
    } 

    .method public static string  'string<#Enum>'<([runtime]System.Enum) a>(!!a 'enum') cil managed
    {
      
      .maxstack  5
      .locals init (object V_0,
               class [runtime]System.IFormattable V_1,
               string V_2,
               !!a V_3)
      IL_0000:  ldarg.0
      IL_0001:  box        !!a
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  isinst     [runtime]System.IFormattable
      IL_000d:  brtrue.s   IL_0014

      IL_000f:  ldloc.0
      IL_0010:  brfalse.s  IL_0033

      IL_0012:  br.s       IL_0039

      IL_0014:  ldloc.0
      IL_0015:  unbox.any  [runtime]System.IFormattable
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  ldnull
      IL_001d:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0022:  callvirt   instance string [netstandard]System.IFormattable::ToString(string,
                                                                                      class [netstandard]System.IFormatProvider)
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  brtrue.s   IL_0031

      IL_002b:  ldstr      ""
      IL_0030:  ret

      IL_0031:  ldloc.2
      IL_0032:  ret

      IL_0033:  ldstr      ""
      IL_0038:  ret

      IL_0039:  ldarg.0
      IL_003a:  stloc.3
      IL_003b:  ldloca.s   V_3
      IL_003d:  constrained. !!a
      IL_0043:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_0048:  stloc.2
      IL_0049:  ldloc.2
      IL_004a:  brtrue.s   IL_0052

      IL_004c:  ldstr      ""
      IL_0051:  ret

      IL_0052:  ldloc.2
      IL_0053:  ret
    } 

    .method public static string  'string<\'T :> Enum>'<([runtime]System.Enum) T>(!!T 'enum') cil managed
    {
      
      .maxstack  5
      .locals init (object V_0,
               class [runtime]System.IFormattable V_1,
               string V_2,
               !!T V_3)
      IL_0000:  ldarg.0
      IL_0001:  box        !!T
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  isinst     [runtime]System.IFormattable
      IL_000d:  brtrue.s   IL_0014

      IL_000f:  ldloc.0
      IL_0010:  brfalse.s  IL_0033

      IL_0012:  br.s       IL_0039

      IL_0014:  ldloc.0
      IL_0015:  unbox.any  [runtime]System.IFormattable
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  ldnull
      IL_001d:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0022:  callvirt   instance string [netstandard]System.IFormattable::ToString(string,
                                                                                      class [netstandard]System.IFormatProvider)
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  brtrue.s   IL_0031

      IL_002b:  ldstr      ""
      IL_0030:  ret

      IL_0031:  ldloc.2
      IL_0032:  ret

      IL_0033:  ldstr      ""
      IL_0038:  ret

      IL_0039:  ldarg.0
      IL_003a:  stloc.3
      IL_003b:  ldloca.s   V_3
      IL_003d:  constrained. !!T
      IL_0043:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_0048:  stloc.2
      IL_0049:  ldloc.2
      IL_004a:  brtrue.s   IL_0052

      IL_004c:  ldstr      ""
      IL_0051:  ret

      IL_0052:  ldloc.2
      IL_0053:  ret
    } 

    .method public static string  'string<\'T when \'T : enum<\'U>>'<T,U>(!!T 'enum') cil managed
    {
      
      .maxstack  5
      .locals init (object V_0,
               class [runtime]System.IFormattable V_1,
               string V_2,
               !!T V_3)
      IL_0000:  ldarg.0
      IL_0001:  box        !!T
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  isinst     [runtime]System.IFormattable
      IL_000d:  brtrue.s   IL_0014

      IL_000f:  ldloc.0
      IL_0010:  brfalse.s  IL_0033

      IL_0012:  br.s       IL_0039

      IL_0014:  ldloc.0
      IL_0015:  unbox.any  [runtime]System.IFormattable
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  ldnull
      IL_001d:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0022:  callvirt   instance string [netstandard]System.IFormattable::ToString(string,
                                                                                      class [netstandard]System.IFormatProvider)
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  brtrue.s   IL_0031

      IL_002b:  ldstr      ""
      IL_0030:  ret

      IL_0031:  ldloc.2
      IL_0032:  ret

      IL_0033:  ldstr      ""
      IL_0038:  ret

      IL_0039:  ldarg.0
      IL_003a:  stloc.3
      IL_003b:  ldloca.s   V_3
      IL_003d:  constrained. !!T
      IL_0043:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_0048:  stloc.2
      IL_0049:  ldloc.2
      IL_004a:  brtrue.s   IL_0052

      IL_004c:  ldstr      ""
      IL_0051:  ret

      IL_0052:  ldloc.2
      IL_0053:  ret
    } 

    .method public static string  'string<\'T when \'T : enum<int>>'<T>(!!T 'enum') cil managed
    {
      
      .maxstack  5
      .locals init (object V_0,
               class [runtime]System.IFormattable V_1,
               string V_2,
               !!T V_3)
      IL_0000:  ldarg.0
      IL_0001:  box        !!T
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  isinst     [runtime]System.IFormattable
      IL_000d:  brtrue.s   IL_0014

      IL_000f:  ldloc.0
      IL_0010:  brfalse.s  IL_0033

      IL_0012:  br.s       IL_0039

      IL_0014:  ldloc.0
      IL_0015:  unbox.any  [runtime]System.IFormattable
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  ldnull
      IL_001d:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0022:  callvirt   instance string [netstandard]System.IFormattable::ToString(string,
                                                                                      class [netstandard]System.IFormatProvider)
      IL_0027:  stloc.2
      IL_0028:  ldloc.2
      IL_0029:  brtrue.s   IL_0031

      IL_002b:  ldstr      ""
      IL_0030:  ret

      IL_0031:  ldloc.2
      IL_0032:  ret

      IL_0033:  ldstr      ""
      IL_0038:  ret

      IL_0039:  ldarg.0
      IL_003a:  stloc.3
      IL_003b:  ldloca.s   V_3
      IL_003d:  constrained. !!T
      IL_0043:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_0048:  stloc.2
      IL_0049:  ldloc.2
      IL_004a:  brtrue.s   IL_0052

      IL_004c:  ldstr      ""
      IL_0051:  ret

      IL_0052:  ldloc.2
      IL_0053:  ret
    } 

    .method public static string  'string Unchecked.defaultof<System.Enum>'() cil managed
    {
      
      .maxstack  5
      .locals init (class [runtime]System.Enum V_0,
               object V_1,
               class [runtime]System.IFormattable V_2,
               string V_3,
               class [runtime]System.Enum V_4)
      IL_0000:  ldnull
      IL_0001:  stloc.0
      IL_0002:  ldloc.0
      IL_0003:  box        [runtime]System.Enum
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  isinst     [runtime]System.IFormattable
      IL_000f:  brtrue.s   IL_0016

      IL_0011:  ldloc.1
      IL_0012:  brfalse.s  IL_0035

      IL_0014:  br.s       IL_003b

      IL_0016:  ldloc.1
      IL_0017:  unbox.any  [runtime]System.IFormattable
      IL_001c:  stloc.2
      IL_001d:  ldloc.2
      IL_001e:  ldnull
      IL_001f:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0024:  callvirt   instance string [netstandard]System.IFormattable::ToString(string,
                                                                                      class [netstandard]System.IFormatProvider)
      IL_0029:  stloc.3
      IL_002a:  ldloc.3
      IL_002b:  brtrue.s   IL_0033

      IL_002d:  ldstr      ""
      IL_0032:  ret

      IL_0033:  ldloc.3
      IL_0034:  ret

      IL_0035:  ldstr      ""
      IL_003a:  ret

      IL_003b:  ldloc.0
      IL_003c:  stloc.s    V_4
      IL_003e:  ldloca.s   V_4
      IL_0040:  constrained. [runtime]System.Enum
      IL_0046:  callvirt   instance string [netstandard]System.Object::ToString()
      IL_004b:  stloc.3
      IL_004c:  ldloc.3
      IL_004d:  brtrue.s   IL_0055

      IL_004f:  ldstr      ""
      IL_0054:  ret

      IL_0055:  ldloc.3
      IL_0056:  ret
    } 

  } 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
       extends [runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    
    .maxstack  8
    IL_0000:  ret
  } 

} 






