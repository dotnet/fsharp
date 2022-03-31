// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSHARP1.0:3309
// 
// Make sure that argument names are preserved in some/all compiler closures
// 
// Correct IL:
//    .method public strict virtual instance int32 
//            Invoke(class M/C i_want_to_see_this_identifier@7) cil managed			<=== correct!
//    {
//      // Code size       9 (0x9)
//      .maxstack  5
//      IL_0000:  ldarg.1
//      IL_0001:  tail.
//      [...]
//    } // end of method clo@10::Invoke
//
//
// Incorrect IL:
//    .method public strict virtual instance int32 
//            Invoke(class M/C arg0) cil managed						<=== incorrect "arg0" is not what we want
//    {
//      // Code size       9 (0x9)
//      .maxstack  5
//      IL_0000:  ldarg.1
//      IL_0001:  tail.
//      [...]
//    } // end of method clo@10::Invoke
//



#light

module M
    type C =
        member x.F(o:obj) = x.GetHashCode()

    let I (i_want_to_see_this_identifier:C) = i_want_to_see_this_identifier.F()
        
    type T() =
        member x.F = I

 
