




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
  .class auto ansi serializable nested public C
         extends [runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
    .field assembly string x
    .field assembly string x@8
    .method public specialname rtspecialname instance void  .ctor() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [runtime]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  call       string [runtime]System.Console::ReadLine()
      IL_000e:  stfld      string assembly/C::x
      IL_0013:  ldarg.0
      IL_0014:  call       string [runtime]System.Console::ReadLine()
      IL_0019:  stfld      string assembly/C::x@8
      IL_001e:  ret
    } 

    .method public hidebysig instance string M() cil managed
    {
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/C::x@8
      IL_0006:  ldarg.0
      IL_0007:  callvirt   instance string assembly/C::g()
      IL_000c:  call       string [runtime]System.String::Concat(string,
                                                                  string)
      IL_0011:  ret
    } 

    .method assembly hidebysig instance string g() cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string assembly/C::x
      IL_0006:  ret
    } 

  } 

  .method assembly static void  g@12(class [FSharp.Core]Microsoft.FSharp.Core.Unit unitVar0) cil managed
  {
    
    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0)
    IL_0000:  ldstr      "Hello"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_000a:  stloc.0
    IL_000b:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0010:  ldloc.0
    IL_0011:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [runtime]System.IO.TextWriter,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_0016:  pop
    IL_0017:  ldstr      "Hello"
    IL_001c:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(string)
    IL_0021:  stloc.0
    IL_0022:  call       class [netstandard]System.IO.TextWriter [netstandard]System.Console::get_Out()
    IL_0027:  ldloc.0
    IL_0028:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.PrintfModule::PrintFormatLineToTextWriter<class [FSharp.Core]Microsoft.FSharp.Core.Unit>(class [runtime]System.IO.TextWriter,
                                                                                                                                                         class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_002d:  pop
    IL_002e:  ret
  } 

  .method public static class [runtime]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit> f<a>(!!a x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  call       void assembly::g@12(class [FSharp.Core]Microsoft.FSharp.Core.Unit)
    IL_0006:  nop
    IL_0007:  ldnull
    IL_0008:  ldnull
    IL_0009:  call       void assembly::g@12(class [FSharp.Core]Microsoft.FSharp.Core.Unit)
    IL_000e:  nop
    IL_000f:  ldnull
    IL_0010:  newobj     instance void class [runtime]System.Tuple`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::.ctor(!0,
                                                                                                                                                                          !1)
    IL_0015:  ret
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






