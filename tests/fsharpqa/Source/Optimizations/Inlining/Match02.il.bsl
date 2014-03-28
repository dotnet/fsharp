  .method public static int32  testmethod() cil managed
  {
    // Code size       66 (0x42)
    .maxstack  5
    .locals init (int32 V_0,
             class [mscorlib]System.Tuple`2<float64,string> V_1,
             int32 V_2,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<class [FSharp.Core]Microsoft.FSharp.Core.Unit,string> V_3)
    IL_0000:  ldc.r8     1.
    IL_0009:  ldstr      ""
    IL_000e:  newobj     instance void class [mscorlib]System.Tuple`2<float64,string>::.ctor(!0,
                                                                                             !1)
    IL_0013:  stloc.1
    IL_0014:  ldc.i4.2
    IL_0015:  stloc.0
    IL_0016:  newobj     instance void Match02/f@18::.ctor()
    IL_001b:  stloc.3
    IL_001c:  ldc.i4.2
    IL_001d:  stloc.2
    IL_001e:  ldc.i4.2
    IL_001f:  ldloc.0
    IL_0020:  add
    IL_0021:  ldloc.2
    IL_0022:  add
    IL_0023:  ldc.r8     2.
    IL_002c:  ldstr      ""
    IL_0031:  newobj     instance void class [mscorlib]System.Tuple`2<float64,string>::.ctor(!0,
                                                                                             !1)
    IL_0036:  stloc.1
    IL_0037:  ldc.i4.3
    IL_0038:  add
    IL_0039:  newobj     instance void Match02/testmethod@20::.ctor()
    IL_003e:  stloc.3
    IL_003f:  ldc.i4.3
    IL_0040:  add
    IL_0041:  ret
  } // end of method Match02::testmethod
