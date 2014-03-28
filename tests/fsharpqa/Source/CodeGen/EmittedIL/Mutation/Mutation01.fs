// #Regression #NoMono #NoMT #CodeGen #EmittedIL #NETFX20Only #NETFX40Only 
// Regression test for FSHARP1.0:1206
// A long long time ago (before the fix, e.g. in release 1.9.2.9 Sept 2007)
// we used to emit IL like this:
//     .method public instance void  setV(int32 v) cil managed
//    {
//      // Code size       9 (0x9)
//      .maxstack  4
//      IL_0000:  ldarga.s   0			<=============================== (apparently this is problematic)
//      IL_0002:  ldarg.1
//      IL_0003:  call       instance void A/Test::set_v(int32)
//      IL_0008:  ret
//    } // end of method Test::setV

#light
type Test = struct
              val mutable v: int
              member t.setV v = t.v <- 0
            end
