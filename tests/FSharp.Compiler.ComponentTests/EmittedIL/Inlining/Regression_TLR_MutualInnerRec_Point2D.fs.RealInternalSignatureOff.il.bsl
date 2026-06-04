.assembly extern runtime { }
.assembly extern FSharp.Core { }
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
.class sequential ansi serializable sealed nested public Point2D
extends [runtime]System.ValueType
implements class [runtime]System.IEquatable`1<valuetype assembly/Point2D>,
[runtime]System.Collections.IStructuralEquatable,
class [runtime]System.IComparable`1<valuetype assembly/Point2D>,
[runtime]System.IComparable,
[runtime]System.Collections.IStructuralComparable
{
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
.field assembly float64 x
.field assembly float64 y
.method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/Point2D obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  5
.locals init (int32 V_0,
class [runtime]System.Collections.IComparer V_1,
float64 V_2,
float64 V_3)
IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
IL_0005:  stloc.1
IL_0006:  ldarg.0
IL_0007:  ldfld      float64 assembly/Point2D::x
IL_000c:  stloc.2
IL_000d:  ldarga.s   obj
IL_000f:  ldfld      float64 assembly/Point2D::x
IL_0014:  stloc.3
IL_0015:  ldloc.2
IL_0016:  ldloc.3
IL_0017:  clt
IL_0019:  brfalse.s  IL_001f

IL_001b:  ldc.i4.m1
IL_001c:  nop
IL_001d:  br.s       IL_003e

IL_001f:  ldloc.2
IL_0020:  ldloc.3
IL_0021:  cgt
IL_0023:  brfalse.s  IL_0029

IL_0025:  ldc.i4.1
IL_0026:  nop
IL_0027:  br.s       IL_003e

IL_0029:  ldloc.2
IL_002a:  ldloc.3
IL_002b:  ceq
IL_002d:  brfalse.s  IL_0033

IL_002f:  ldc.i4.0
IL_0030:  nop
IL_0031:  br.s       IL_003e

IL_0033:  ldloc.1
IL_0034:  ldloc.2
IL_0035:  ldloc.3
IL_0036:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_003b:  nop
IL_003c:  br.s       IL_003e

IL_003e:  stloc.0
IL_003f:  ldloc.0
IL_0040:  ldc.i4.0
IL_0041:  bge.s      IL_0045

IL_0043:  ldloc.0
IL_0044:  ret

IL_0045:  ldloc.0
IL_0046:  ldc.i4.0
IL_0047:  ble.s      IL_004b

IL_0049:  ldloc.0
IL_004a:  ret

IL_004b:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
IL_0050:  stloc.1
IL_0051:  ldarg.0
IL_0052:  ldfld      float64 assembly/Point2D::y
IL_0057:  stloc.2
IL_0058:  ldarga.s   obj
IL_005a:  ldfld      float64 assembly/Point2D::y
IL_005f:  stloc.3
IL_0060:  ldloc.2
IL_0061:  ldloc.3
IL_0062:  clt
IL_0064:  brfalse.s  IL_0068

IL_0066:  ldc.i4.m1
IL_0067:  ret

IL_0068:  ldloc.2
IL_0069:  ldloc.3
IL_006a:  cgt
IL_006c:  brfalse.s  IL_0070

IL_006e:  ldc.i4.1
IL_006f:  ret

IL_0070:  ldloc.2
IL_0071:  ldloc.3
IL_0072:  ceq
IL_0074:  brfalse.s  IL_0078

IL_0076:  ldc.i4.0
IL_0077:  ret

IL_0078:  ldloc.1
IL_0079:  ldloc.2
IL_007a:  ldloc.3
IL_007b:  tail.
IL_007d:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_0082:  ret
} 

.method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldarg.1
IL_0002:  unbox.any  assembly/Point2D
IL_0007:  call       instance int32 assembly/Point2D::CompareTo(valuetype assembly/Point2D)
IL_000c:  ret
} 

.method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  5
.locals init (valuetype assembly/Point2D V_0,
int32 V_1,
float64 V_2,
float64 V_3)
IL_0000:  ldarg.1
IL_0001:  unbox.any  assembly/Point2D
IL_0006:  stloc.0
IL_0007:  ldarg.0
IL_0008:  ldfld      float64 assembly/Point2D::x
IL_000d:  stloc.2
IL_000e:  ldloca.s   V_0
IL_0010:  ldfld      float64 assembly/Point2D::x
IL_0015:  stloc.3
IL_0016:  ldloc.2
IL_0017:  ldloc.3
IL_0018:  clt
IL_001a:  brfalse.s  IL_0020

IL_001c:  ldc.i4.m1
IL_001d:  nop
IL_001e:  br.s       IL_003f

IL_0020:  ldloc.2
IL_0021:  ldloc.3
IL_0022:  cgt
IL_0024:  brfalse.s  IL_002a

IL_0026:  ldc.i4.1
IL_0027:  nop
IL_0028:  br.s       IL_003f

IL_002a:  ldloc.2
IL_002b:  ldloc.3
IL_002c:  ceq
IL_002e:  brfalse.s  IL_0034

IL_0030:  ldc.i4.0
IL_0031:  nop
IL_0032:  br.s       IL_003f

IL_0034:  ldarg.2
IL_0035:  ldloc.2
IL_0036:  ldloc.3
IL_0037:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_003c:  nop
IL_003d:  br.s       IL_003f

IL_003f:  stloc.1
IL_0040:  ldloc.1
IL_0041:  ldc.i4.0
IL_0042:  bge.s      IL_0046

IL_0044:  ldloc.1
IL_0045:  ret

IL_0046:  ldloc.1
IL_0047:  ldc.i4.0
IL_0048:  ble.s      IL_004c

IL_004a:  ldloc.1
IL_004b:  ret

IL_004c:  ldarg.0
IL_004d:  ldfld      float64 assembly/Point2D::y
IL_0052:  stloc.2
IL_0053:  ldloca.s   V_0
IL_0055:  ldfld      float64 assembly/Point2D::y
IL_005a:  stloc.3
IL_005b:  ldloc.2
IL_005c:  ldloc.3
IL_005d:  clt
IL_005f:  brfalse.s  IL_0063

IL_0061:  ldc.i4.m1
IL_0062:  ret

IL_0063:  ldloc.2
IL_0064:  ldloc.3
IL_0065:  cgt
IL_0067:  brfalse.s  IL_006b

IL_0069:  ldc.i4.1
IL_006a:  ret

IL_006b:  ldloc.2
IL_006c:  ldloc.3
IL_006d:  ceq
IL_006f:  brfalse.s  IL_0073

IL_0071:  ldc.i4.0
IL_0072:  ret

IL_0073:  ldarg.2
IL_0074:  ldloc.2
IL_0075:  ldloc.3
IL_0076:  tail.
IL_0078:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_007d:  ret
} 

.method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  7
.locals init (int32 V_0)
IL_0000:  ldc.i4.0
IL_0001:  stloc.0
IL_0002:  ldc.i4     0x9e3779b9
IL_0007:  ldarg.1
IL_0008:  ldarg.0
IL_0009:  ldfld      float64 assembly/Point2D::y
IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<float64>(class [runtime]System.Collections.IEqualityComparer,
!!0)
IL_0013:  ldloc.0
IL_0014:  ldc.i4.6
IL_0015:  shl
IL_0016:  ldloc.0
IL_0017:  ldc.i4.2
IL_0018:  shr
IL_0019:  add
IL_001a:  add
IL_001b:  add
IL_001c:  stloc.0
IL_001d:  ldc.i4     0x9e3779b9
IL_0022:  ldarg.1
IL_0023:  ldarg.0
IL_0024:  ldfld      float64 assembly/Point2D::x
IL_0029:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<float64>(class [runtime]System.Collections.IEqualityComparer,
!!0)
IL_002e:  ldloc.0
IL_002f:  ldc.i4.6
IL_0030:  shl
IL_0031:  ldloc.0
IL_0032:  ldc.i4.2
IL_0033:  shr
IL_0034:  add
IL_0035:  add
IL_0036:  add
IL_0037:  stloc.0
IL_0038:  ldloc.0
IL_0039:  ret
} 

.method public hidebysig virtual final instance int32  GetHashCode() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
IL_0006:  call       instance int32 assembly/Point2D::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
IL_000b:  ret
} 

.method public hidebysig instance bool Equals(valuetype assembly/Point2D obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::x
IL_0006:  ldarga.s   obj
IL_0008:  ldfld      float64 assembly/Point2D::x
IL_000d:  ceq
IL_000f:  brfalse.s  IL_0021

IL_0011:  ldarg.0
IL_0012:  ldfld      float64 assembly/Point2D::y
IL_0017:  ldarga.s   obj
IL_0019:  ldfld      float64 assembly/Point2D::y
IL_001e:  ceq
IL_0020:  ret

IL_0021:  ldc.i4.0
IL_0022:  ret
} 

.method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  5
.locals init (valuetype assembly/Point2D V_0)
IL_0000:  ldarg.1
IL_0001:  isinst     assembly/Point2D
IL_0006:  brfalse.s  IL_0018

IL_0008:  ldarg.1
IL_0009:  unbox.any  assembly/Point2D
IL_000e:  stloc.0
IL_000f:  ldarg.0
IL_0010:  ldloc.0
IL_0011:  ldarg.2
IL_0012:  call       instance bool assembly/Point2D::Equals(valuetype assembly/Point2D,
class [runtime]System.Collections.IEqualityComparer)
IL_0017:  ret

IL_0018:  ldc.i4.0
IL_0019:  ret
} 

.method public specialname rtspecialname instance void  .ctor(float64 x, float64 y) cil managed
{

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldarg.1
IL_0002:  stfld      float64 assembly/Point2D::x
IL_0007:  ldarg.0
IL_0008:  ldarg.2
IL_0009:  stfld      float64 assembly/Point2D::y
IL_000e:  ret
} 

.method public hidebysig specialname instance float64  get_X() cil managed
{

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::x
IL_0006:  ret
} 

.method public hidebysig specialname instance float64  get_Y() cil managed
{

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::y
IL_0006:  ret
} 

.method public hidebysig virtual final instance bool  Equals(valuetype assembly/Point2D obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  4
.locals init (float64 V_0,
float64 V_1)
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::x
IL_0006:  stloc.0
IL_0007:  ldarga.s   obj
IL_0009:  ldfld      float64 assembly/Point2D::x
IL_000e:  stloc.1
IL_000f:  ldloc.0
IL_0010:  ldloc.1
IL_0011:  ceq
IL_0013:  brfalse.s  IL_0019

IL_0015:  ldc.i4.1
IL_0016:  nop
IL_0017:  br.s       IL_002b

IL_0019:  ldloc.0
IL_001a:  ldloc.0
IL_001b:  beq.s      IL_0027

IL_001d:  ldloc.1
IL_001e:  ldloc.1
IL_001f:  ceq
IL_0021:  ldc.i4.0
IL_0022:  ceq
IL_0024:  nop
IL_0025:  br.s       IL_002b

IL_0027:  ldc.i4.0
IL_0028:  nop
IL_0029:  br.s       IL_002b

IL_002b:  brfalse.s  IL_0052

IL_002d:  ldarg.0
IL_002e:  ldfld      float64 assembly/Point2D::y
IL_0033:  stloc.0
IL_0034:  ldarga.s   obj
IL_0036:  ldfld      float64 assembly/Point2D::y
IL_003b:  stloc.1
IL_003c:  ldloc.0
IL_003d:  ldloc.1
IL_003e:  ceq
IL_0040:  brfalse.s  IL_0044

IL_0042:  ldc.i4.1
IL_0043:  ret

IL_0044:  ldloc.0
IL_0045:  ldloc.0
IL_0046:  beq.s      IL_0050

IL_0048:  ldloc.1
IL_0049:  ldloc.1
IL_004a:  ceq
IL_004c:  ldc.i4.0
IL_004d:  ceq
IL_004f:  ret

IL_0050:  ldc.i4.0
IL_0051:  ret

IL_0052:  ldc.i4.0
IL_0053:  ret
} 

.method public hidebysig virtual final instance bool  Equals(object obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.1
IL_0001:  isinst     assembly/Point2D
IL_0006:  brfalse.s  IL_0015

IL_0008:  ldarg.0
IL_0009:  ldarg.1
IL_000a:  unbox.any  assembly/Point2D
IL_000f:  call       instance bool assembly/Point2D::Equals(valuetype assembly/Point2D)
IL_0014:  ret

IL_0015:  ldc.i4.0
IL_0016:  ret
} 

.property instance float64 X()
{
.get instance float64 assembly/Point2D::get_X()
} 
.property instance float64 Y()
{
.get instance float64 assembly/Point2D::get_Y()
} 
} 

.method public static int32  fifth() cil managed
{

.maxstack  8
.locals init (valuetype assembly/Point2D V_0,
valuetype assembly/Point2D V_1)
IL_0000:  ldc.r8     10.
IL_0009:  ldc.r8     20.
IL_0012:  newobj     instance void assembly/Point2D::.ctor(float64,
float64)
IL_0017:  stloc.0
IL_0018:  ldc.r8     30.
IL_0021:  ldc.r8     40.
IL_002a:  newobj     instance void assembly/Point2D::.ctor(float64,
float64)
IL_002f:  stloc.1
IL_0030:  ldc.i4     0xf4240
IL_0035:  ldloc.0
IL_0036:  ldloc.1
IL_0037:  ldloc.0
IL_0038:  ldloc.1
IL_0039:  ldloc.0
IL_003a:  tail.
IL_003c:  call       int32 assembly::firstCallee@9(int32,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D)
IL_0041:  ret
} 

.method public static int32  main(string[] _argv) cil managed
{
.entrypoint
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  tail.
IL_0002:  call       int32 assembly::fifth()
IL_0007:  ret
} 

.method assembly static int32  firstCallee@9(int32 n,
valuetype assembly/Point2D a,
valuetype assembly/Point2D b,
valuetype assembly/Point2D c,
valuetype assembly/Point2D d,
valuetype assembly/Point2D e) cil managed
{

.maxstack  8
IL_0000:  nop
IL_0001:  ldarga.s   a
IL_0003:  ldfld      float64 assembly/Point2D::x
IL_0008:  ldc.r8     10.
IL_0011:  beq.s      IL_0016

IL_0013:  ldc.i4.s   -100
IL_0015:  ret

IL_0016:  nop
IL_0017:  ldarg.0
IL_0018:  brtrue.s   IL_001d

IL_001a:  ldc.i4.s   100
IL_001c:  ret

IL_001d:  nop
IL_001e:  ldarg.0
IL_001f:  ldc.i4.2
IL_0020:  rem
IL_0021:  brtrue.s   IL_0035

IL_0023:  ldarg.0
IL_0024:  ldc.i4.1
IL_0025:  sub
IL_0026:  ldarg.1
IL_0027:  ldarg.2
IL_0028:  ldarg.3
IL_0029:  ldarg.s    d
IL_002b:  ldarg.s    e
IL_002d:  tail.
IL_002f:  call       int32 assembly::secondCallee@14(int32,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D)
IL_0034:  ret

IL_0035:  ldarg.0
IL_0036:  ldc.i4.1
IL_0037:  sub
IL_0038:  ldarg.1
IL_0039:  ldarg.2
IL_003a:  ldarg.3
IL_003b:  ldarg.s    d
IL_003d:  ldarg.s    e
IL_003f:  starg.s    e
IL_0041:  starg.s    d
IL_0043:  starg.s    c
IL_0045:  starg.s    b
IL_0047:  starg.s    a
IL_0049:  starg.s    n
IL_004b:  br.s       IL_0000
} 

.method assembly static int32  secondCallee@14(int32 n,
valuetype assembly/Point2D a,
valuetype assembly/Point2D b,
valuetype assembly/Point2D c,
valuetype assembly/Point2D d,
valuetype assembly/Point2D e) cil managed
{

.maxstack  8
IL_0000:  nop
IL_0001:  ldarg.0
IL_0002:  brtrue.s   IL_0007

IL_0004:  ldc.i4.s   101
IL_0006:  ret

IL_0007:  nop
IL_0008:  ldarg.0
IL_0009:  ldc.i4.2
IL_000a:  rem
IL_000b:  brtrue.s   IL_0025

IL_000d:  ldarg.0
IL_000e:  ldc.i4.1
IL_000f:  sub
IL_0010:  ldarg.1
IL_0011:  ldarg.2
IL_0012:  ldarg.3
IL_0013:  ldarg.s    d
IL_0015:  ldarg.s    e
IL_0017:  starg.s    e
IL_0019:  starg.s    d
IL_001b:  starg.s    c
IL_001d:  starg.s    b
IL_001f:  starg.s    a
IL_0021:  starg.s    n
IL_0023:  br.s       IL_0000

IL_0025:  ldarg.0
IL_0026:  ldc.i4.1
IL_0027:  sub
IL_0028:  ldarg.1
IL_0029:  ldarg.2
IL_002a:  ldarg.3
IL_002b:  ldarg.s    d
IL_002d:  ldarg.s    e
IL_002f:  tail.
IL_0031:  call       int32 assembly::firstCallee@9(int32,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D)
IL_0036:  ret
} 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
extends [runtime]System.Object
{
} 







==
Name: '.assembly extern runtime { }'

Expected:	 IL_0000:  ldc.r8     10
Actual:		 IL_0000:  ldc.r8     10.
==

==
Name: '.assembly extern runtime { }'

Expected:	 IL_0009:  ldc.r8     20
Actual:		 IL_0009:  ldc.r8     20.
==

==
Name: '.assembly extern runtime { }'

Expected:	 IL_0018:  ldc.r8     30
Actual:		 IL_0018:  ldc.r8     30.
==

==
Name: '.assembly extern runtime { }'

Expected:	 IL_0021:  ldc.r8     40
Actual:		 IL_0021:  ldc.r8     40.
==

==
Name: '.assembly extern runtime { }'

Expected:	 IL_0008:  ldc.r8     10
Actual:		 IL_0008:  ldc.r8     10.
==

\Expected:
.assembly extern runtime { }
.assembly extern FSharp.Core { }
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
.class sequential ansi serializable sealed nested public Point2D
extends [runtime]System.ValueType
implements class [runtime]System.IEquatable`1<valuetype assembly/Point2D>,
[runtime]System.Collections.IStructuralEquatable,
class [runtime]System.IComparable`1<valuetype assembly/Point2D>,
[runtime]System.IComparable,
[runtime]System.Collections.IStructuralComparable
{
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
.field assembly float64 x
.field assembly float64 y
.method public hidebysig virtual final instance int32  CompareTo(valuetype assembly/Point2D obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  5
.locals init (int32 V_0,
class [runtime]System.Collections.IComparer V_1,
float64 V_2,
float64 V_3)
IL_0000:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
IL_0005:  stloc.1
IL_0006:  ldarg.0
IL_0007:  ldfld      float64 assembly/Point2D::x
IL_000c:  stloc.2
IL_000d:  ldarga.s   obj
IL_000f:  ldfld      float64 assembly/Point2D::x
IL_0014:  stloc.3
IL_0015:  ldloc.2
IL_0016:  ldloc.3
IL_0017:  clt
IL_0019:  brfalse.s  IL_001f

IL_001b:  ldc.i4.m1
IL_001c:  nop
IL_001d:  br.s       IL_003e

IL_001f:  ldloc.2
IL_0020:  ldloc.3
IL_0021:  cgt
IL_0023:  brfalse.s  IL_0029

IL_0025:  ldc.i4.1
IL_0026:  nop
IL_0027:  br.s       IL_003e

IL_0029:  ldloc.2
IL_002a:  ldloc.3
IL_002b:  ceq
IL_002d:  brfalse.s  IL_0033

IL_002f:  ldc.i4.0
IL_0030:  nop
IL_0031:  br.s       IL_003e

IL_0033:  ldloc.1
IL_0034:  ldloc.2
IL_0035:  ldloc.3
IL_0036:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_003b:  nop
IL_003c:  br.s       IL_003e

IL_003e:  stloc.0
IL_003f:  ldloc.0
IL_0040:  ldc.i4.0
IL_0041:  bge.s      IL_0045

IL_0043:  ldloc.0
IL_0044:  ret

IL_0045:  ldloc.0
IL_0046:  ldc.i4.0
IL_0047:  ble.s      IL_004b

IL_0049:  ldloc.0
IL_004a:  ret

IL_004b:  call       class [runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
IL_0050:  stloc.1
IL_0051:  ldarg.0
IL_0052:  ldfld      float64 assembly/Point2D::y
IL_0057:  stloc.2
IL_0058:  ldarga.s   obj
IL_005a:  ldfld      float64 assembly/Point2D::y
IL_005f:  stloc.3
IL_0060:  ldloc.2
IL_0061:  ldloc.3
IL_0062:  clt
IL_0064:  brfalse.s  IL_0068

IL_0066:  ldc.i4.m1
IL_0067:  ret

IL_0068:  ldloc.2
IL_0069:  ldloc.3
IL_006a:  cgt
IL_006c:  brfalse.s  IL_0070

IL_006e:  ldc.i4.1
IL_006f:  ret

IL_0070:  ldloc.2
IL_0071:  ldloc.3
IL_0072:  ceq
IL_0074:  brfalse.s  IL_0078

IL_0076:  ldc.i4.0
IL_0077:  ret

IL_0078:  ldloc.1
IL_0079:  ldloc.2
IL_007a:  ldloc.3
IL_007b:  tail.
IL_007d:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_0082:  ret
} 

.method public hidebysig virtual final instance int32  CompareTo(object obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldarg.1
IL_0002:  unbox.any  assembly/Point2D
IL_0007:  call       instance int32 assembly/Point2D::CompareTo(valuetype assembly/Point2D)
IL_000c:  ret
} 

.method public hidebysig virtual final instance int32  CompareTo(object obj, class [runtime]System.Collections.IComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  5
.locals init (valuetype assembly/Point2D V_0,
int32 V_1,
float64 V_2,
float64 V_3)
IL_0000:  ldarg.1
IL_0001:  unbox.any  assembly/Point2D
IL_0006:  stloc.0
IL_0007:  ldarg.0
IL_0008:  ldfld      float64 assembly/Point2D::x
IL_000d:  stloc.2
IL_000e:  ldloca.s   V_0
IL_0010:  ldfld      float64 assembly/Point2D::x
IL_0015:  stloc.3
IL_0016:  ldloc.2
IL_0017:  ldloc.3
IL_0018:  clt
IL_001a:  brfalse.s  IL_0020

IL_001c:  ldc.i4.m1
IL_001d:  nop
IL_001e:  br.s       IL_003f

IL_0020:  ldloc.2
IL_0021:  ldloc.3
IL_0022:  cgt
IL_0024:  brfalse.s  IL_002a

IL_0026:  ldc.i4.1
IL_0027:  nop
IL_0028:  br.s       IL_003f

IL_002a:  ldloc.2
IL_002b:  ldloc.3
IL_002c:  ceq
IL_002e:  brfalse.s  IL_0034

IL_0030:  ldc.i4.0
IL_0031:  nop
IL_0032:  br.s       IL_003f

IL_0034:  ldarg.2
IL_0035:  ldloc.2
IL_0036:  ldloc.3
IL_0037:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_003c:  nop
IL_003d:  br.s       IL_003f

IL_003f:  stloc.1
IL_0040:  ldloc.1
IL_0041:  ldc.i4.0
IL_0042:  bge.s      IL_0046

IL_0044:  ldloc.1
IL_0045:  ret

IL_0046:  ldloc.1
IL_0047:  ldc.i4.0
IL_0048:  ble.s      IL_004c

IL_004a:  ldloc.1
IL_004b:  ret

IL_004c:  ldarg.0
IL_004d:  ldfld      float64 assembly/Point2D::y
IL_0052:  stloc.2
IL_0053:  ldloca.s   V_0
IL_0055:  ldfld      float64 assembly/Point2D::y
IL_005a:  stloc.3
IL_005b:  ldloc.2
IL_005c:  ldloc.3
IL_005d:  clt
IL_005f:  brfalse.s  IL_0063

IL_0061:  ldc.i4.m1
IL_0062:  ret

IL_0063:  ldloc.2
IL_0064:  ldloc.3
IL_0065:  cgt
IL_0067:  brfalse.s  IL_006b

IL_0069:  ldc.i4.1
IL_006a:  ret

IL_006b:  ldloc.2
IL_006c:  ldloc.3
IL_006d:  ceq
IL_006f:  brfalse.s  IL_0073

IL_0071:  ldc.i4.0
IL_0072:  ret

IL_0073:  ldarg.2
IL_0074:  ldloc.2
IL_0075:  ldloc.3
IL_0076:  tail.
IL_0078:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [runtime]System.Collections.IComparer,
!!0,
!!0)
IL_007d:  ret
} 

.method public hidebysig virtual final instance int32  GetHashCode(class [runtime]System.Collections.IEqualityComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  7
.locals init (int32 V_0)
IL_0000:  ldc.i4.0
IL_0001:  stloc.0
IL_0002:  ldc.i4     0x9e3779b9
IL_0007:  ldarg.1
IL_0008:  ldarg.0
IL_0009:  ldfld      float64 assembly/Point2D::y
IL_000e:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<float64>(class [runtime]System.Collections.IEqualityComparer,
!!0)
IL_0013:  ldloc.0
IL_0014:  ldc.i4.6
IL_0015:  shl
IL_0016:  ldloc.0
IL_0017:  ldc.i4.2
IL_0018:  shr
IL_0019:  add
IL_001a:  add
IL_001b:  add
IL_001c:  stloc.0
IL_001d:  ldc.i4     0x9e3779b9
IL_0022:  ldarg.1
IL_0023:  ldarg.0
IL_0024:  ldfld      float64 assembly/Point2D::x
IL_0029:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashWithComparerIntrinsic<float64>(class [runtime]System.Collections.IEqualityComparer,
!!0)
IL_002e:  ldloc.0
IL_002f:  ldc.i4.6
IL_0030:  shl
IL_0031:  ldloc.0
IL_0032:  ldc.i4.2
IL_0033:  shr
IL_0034:  add
IL_0035:  add
IL_0036:  add
IL_0037:  stloc.0
IL_0038:  ldloc.0
IL_0039:  ret
} 

.method public hidebysig virtual final instance int32  GetHashCode() cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
IL_0006:  call       instance int32 assembly/Point2D::GetHashCode(class [runtime]System.Collections.IEqualityComparer)
IL_000b:  ret
} 

.method public hidebysig instance bool Equals(valuetype assembly/Point2D obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::x
IL_0006:  ldarga.s   obj
IL_0008:  ldfld      float64 assembly/Point2D::x
IL_000d:  ceq
IL_000f:  brfalse.s  IL_0021

IL_0011:  ldarg.0
IL_0012:  ldfld      float64 assembly/Point2D::y
IL_0017:  ldarga.s   obj
IL_0019:  ldfld      float64 assembly/Point2D::y
IL_001e:  ceq
IL_0020:  ret

IL_0021:  ldc.i4.0
IL_0022:  ret
} 

.method public hidebysig virtual final instance bool  Equals(object obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  5
.locals init (valuetype assembly/Point2D V_0)
IL_0000:  ldarg.1
IL_0001:  isinst     assembly/Point2D
IL_0006:  brfalse.s  IL_0018

IL_0008:  ldarg.1
IL_0009:  unbox.any  assembly/Point2D
IL_000e:  stloc.0
IL_000f:  ldarg.0
IL_0010:  ldloc.0
IL_0011:  ldarg.2
IL_0012:  call       instance bool assembly/Point2D::Equals(valuetype assembly/Point2D,
class [runtime]System.Collections.IEqualityComparer)
IL_0017:  ret

IL_0018:  ldc.i4.0
IL_0019:  ret
} 

.method public specialname rtspecialname instance void  .ctor(float64 x, float64 y) cil managed
{

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldarg.1
IL_0002:  stfld      float64 assembly/Point2D::x
IL_0007:  ldarg.0
IL_0008:  ldarg.2
IL_0009:  stfld      float64 assembly/Point2D::y
IL_000e:  ret
} 

.method public hidebysig specialname instance float64  get_X() cil managed
{

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::x
IL_0006:  ret
} 

.method public hidebysig specialname instance float64  get_Y() cil managed
{

.maxstack  8
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::y
IL_0006:  ret
} 

.method public hidebysig virtual final instance bool  Equals(valuetype assembly/Point2D obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  4
.locals init (float64 V_0,
float64 V_1)
IL_0000:  ldarg.0
IL_0001:  ldfld      float64 assembly/Point2D::x
IL_0006:  stloc.0
IL_0007:  ldarga.s   obj
IL_0009:  ldfld      float64 assembly/Point2D::x
IL_000e:  stloc.1
IL_000f:  ldloc.0
IL_0010:  ldloc.1
IL_0011:  ceq
IL_0013:  brfalse.s  IL_0019

IL_0015:  ldc.i4.1
IL_0016:  nop
IL_0017:  br.s       IL_002b

IL_0019:  ldloc.0
IL_001a:  ldloc.0
IL_001b:  beq.s      IL_0027

IL_001d:  ldloc.1
IL_001e:  ldloc.1
IL_001f:  ceq
IL_0021:  ldc.i4.0
IL_0022:  ceq
IL_0024:  nop
IL_0025:  br.s       IL_002b

IL_0027:  ldc.i4.0
IL_0028:  nop
IL_0029:  br.s       IL_002b

IL_002b:  brfalse.s  IL_0052

IL_002d:  ldarg.0
IL_002e:  ldfld      float64 assembly/Point2D::y
IL_0033:  stloc.0
IL_0034:  ldarga.s   obj
IL_0036:  ldfld      float64 assembly/Point2D::y
IL_003b:  stloc.1
IL_003c:  ldloc.0
IL_003d:  ldloc.1
IL_003e:  ceq
IL_0040:  brfalse.s  IL_0044

IL_0042:  ldc.i4.1
IL_0043:  ret

IL_0044:  ldloc.0
IL_0045:  ldloc.0
IL_0046:  beq.s      IL_0050

IL_0048:  ldloc.1
IL_0049:  ldloc.1
IL_004a:  ceq
IL_004c:  ldc.i4.0
IL_004d:  ceq
IL_004f:  ret

IL_0050:  ldc.i4.0
IL_0051:  ret

IL_0052:  ldc.i4.0
IL_0053:  ret
} 

.method public hidebysig virtual final instance bool  Equals(object obj) cil managed
{
.custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  ldarg.1
IL_0001:  isinst     assembly/Point2D
IL_0006:  brfalse.s  IL_0015

IL_0008:  ldarg.0
IL_0009:  ldarg.1
IL_000a:  unbox.any  assembly/Point2D
IL_000f:  call       instance bool assembly/Point2D::Equals(valuetype assembly/Point2D)
IL_0014:  ret

IL_0015:  ldc.i4.0
IL_0016:  ret
} 

.property instance float64 X()
{
.get instance float64 assembly/Point2D::get_X()
} 
.property instance float64 Y()
{
.get instance float64 assembly/Point2D::get_Y()
} 
} 

.method public static int32  fifth() cil managed
{

.maxstack  8
.locals init (valuetype assembly/Point2D V_0,
valuetype assembly/Point2D V_1)
IL_0000:  ldc.r8     10
IL_0009:  ldc.r8     20
IL_0012:  newobj     instance void assembly/Point2D::.ctor(float64,
float64)
IL_0017:  stloc.0
IL_0018:  ldc.r8     30
IL_0021:  ldc.r8     40
IL_002a:  newobj     instance void assembly/Point2D::.ctor(float64,
float64)
IL_002f:  stloc.1
IL_0030:  ldc.i4     0xf4240
IL_0035:  ldloc.0
IL_0036:  ldloc.1
IL_0037:  ldloc.0
IL_0038:  ldloc.1
IL_0039:  ldloc.0
IL_003a:  tail.
IL_003c:  call       int32 assembly::firstCallee@9(int32,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D)
IL_0041:  ret
} 

.method public static int32  main(string[] _argv) cil managed
{
.entrypoint
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = ( 01 00 00 00 ) 

.maxstack  8
IL_0000:  tail.
IL_0002:  call       int32 assembly::fifth()
IL_0007:  ret
} 

.method assembly static int32  firstCallee@9(int32 n,
valuetype assembly/Point2D a,
valuetype assembly/Point2D b,
valuetype assembly/Point2D c,
valuetype assembly/Point2D d,
valuetype assembly/Point2D e) cil managed
{

.maxstack  8
IL_0000:  nop
IL_0001:  ldarga.s   a
IL_0003:  ldfld      float64 assembly/Point2D::x
IL_0008:  ldc.r8     10
IL_0011:  beq.s      IL_0016

IL_0013:  ldc.i4.s   -100
IL_0015:  ret

IL_0016:  nop
IL_0017:  ldarg.0
IL_0018:  brtrue.s   IL_001d

IL_001a:  ldc.i4.s   100
IL_001c:  ret

IL_001d:  nop
IL_001e:  ldarg.0
IL_001f:  ldc.i4.2
IL_0020:  rem
IL_0021:  brtrue.s   IL_0035

IL_0023:  ldarg.0
IL_0024:  ldc.i4.1
IL_0025:  sub
IL_0026:  ldarg.1
IL_0027:  ldarg.2
IL_0028:  ldarg.3
IL_0029:  ldarg.s    d
IL_002b:  ldarg.s    e
IL_002d:  tail.
IL_002f:  call       int32 assembly::secondCallee@14(int32,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D)
IL_0034:  ret

IL_0035:  ldarg.0
IL_0036:  ldc.i4.1
IL_0037:  sub
IL_0038:  ldarg.1
IL_0039:  ldarg.2
IL_003a:  ldarg.3
IL_003b:  ldarg.s    d
IL_003d:  ldarg.s    e
IL_003f:  starg.s    e
IL_0041:  starg.s    d
IL_0043:  starg.s    c
IL_0045:  starg.s    b
IL_0047:  starg.s    a
IL_0049:  starg.s    n
IL_004b:  br.s       IL_0000
} 

.method assembly static int32  secondCallee@14(int32 n,
valuetype assembly/Point2D a,
valuetype assembly/Point2D b,
valuetype assembly/Point2D c,
valuetype assembly/Point2D d,
valuetype assembly/Point2D e) cil managed
{

.maxstack  8
IL_0000:  nop
IL_0001:  ldarg.0
IL_0002:  brtrue.s   IL_0007

IL_0004:  ldc.i4.s   101
IL_0006:  ret

IL_0007:  nop
IL_0008:  ldarg.0
IL_0009:  ldc.i4.2
IL_000a:  rem
IL_000b:  brtrue.s   IL_0025

IL_000d:  ldarg.0
IL_000e:  ldc.i4.1
IL_000f:  sub
IL_0010:  ldarg.1
IL_0011:  ldarg.2
IL_0012:  ldarg.3
IL_0013:  ldarg.s    d
IL_0015:  ldarg.s    e
IL_0017:  starg.s    e
IL_0019:  starg.s    d
IL_001b:  starg.s    c
IL_001d:  starg.s    b
IL_001f:  starg.s    a
IL_0021:  starg.s    n
IL_0023:  br.s       IL_0000

IL_0025:  ldarg.0
IL_0026:  ldc.i4.1
IL_0027:  sub
IL_0028:  ldarg.1
IL_0029:  ldarg.2
IL_002a:  ldarg.3
IL_002b:  ldarg.s    d
IL_002d:  ldarg.s    e
IL_002f:  tail.
IL_0031:  call       int32 assembly::firstCallee@9(int32,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D,
valuetype assembly/Point2D)
IL_0036:  ret
} 

} 

.class private abstract auto ansi sealed '<StartupCode$assembly>'.$assembly
extends [runtime]System.Object
{
}