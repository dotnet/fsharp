




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
    .method public static string  'string sbyte'(int8 'value') cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarga.s   'value'
      IL_0002:  ldnull
      IL_0003:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0008:  call       instance string [netstandard]System.SByte::ToString(string,
                                                                               class [netstandard]System.IFormatProvider)
      IL_000d:  ret
    } 

    .method public static string  'string int16'(int16 'value') cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarga.s   'value'
      IL_0002:  ldnull
      IL_0003:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0008:  call       instance string [netstandard]System.Int16::ToString(string,
                                                                               class [netstandard]System.IFormatProvider)
      IL_000d:  ret
    } 

    .method public static string  'string int32'(int32 'value') cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarga.s   'value'
      IL_0002:  ldnull
      IL_0003:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0008:  call       instance string [netstandard]System.Int32::ToString(string,
                                                                               class [netstandard]System.IFormatProvider)
      IL_000d:  ret
    } 

    .method public static string  'string int64'(int64 'value') cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarga.s   'value'
      IL_0002:  ldnull
      IL_0003:  call       class [netstandard]System.Globalization.CultureInfo [netstandard]System.Globalization.CultureInfo::get_InvariantCulture()
      IL_0008:  call       instance string [netstandard]System.Int64::ToString(string,
                                                                               class [netstandard]System.IFormatProvider)
      IL_000d:  ret
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






