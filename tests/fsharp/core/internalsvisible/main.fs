
// F# internalsVisibleTo checks
printf "publicF   2          = %2d\n" (Library.M.publicF 2)
printf "internalF 2          = %2d\n" (Library.M.internalF 2)
printf "signatureInternalF 2 = %2d\n" (Library.M.signatureInternalF 2)

// C# PublicClass
printf   "APublicClass.InternalProperty   = %2d\n" LibraryCS.APublicClass.InternalProperty
//printf "APublicClass.PrivateProperty    = %2d\n" LibraryCS.APublicClass.PrivateProperty     // private members are not visible via InternalsVisibleTo

// C# InternalClass
printf   "AInternalClass.InternalProperty = %2d\n" LibraryCS.AInternalClass.InternalProperty
//printf "AInternalClass.PrivateProperty  = %2d\n" LibraryCS.AInternalClass.PrivateProperty   // private members are not visible via InternalsVisibleTo

// C# PrivateClass (is just an internal class)
printf   "APrivateClass.InternalProperty  = %2d\n" LibraryCS.APrivateClass.InternalProperty   // for types, private *IS* visible (private is internal)
//printf "APrivateClass.PrivateProperty   = %2d\n" LibraryCS.APrivateClass.PrivateProperty    // private members are not visible via InternalsVisibleTo

//printf "privateF  2 = %d\n"   (Library.M.privateF  2) // inaccessible

module internal Repro1332 = 
  let c = LibraryCS.Class1()
  //c.Protected |> ignore
  c.Internal |> ignore
  c.ProtectedInternal |> ignore
  LibraryCS.Class1.InternalStatic |> ignore
  LibraryCS.Class1.ProtectedInternalStatic |> ignore

type internal Class2() = 
  inherit LibraryCS.Class1()
  member c.M() = 
      c.Internal |> ignore
      c.ProtectedInternal |> ignore
      c.Protected |> ignore
      LibraryCS.Class1.InternalStatic |> ignore
      LibraryCS.Class1.ProtectedInternalStatic |> ignore
      LibraryCS.Class1.ProtectedStatic |> ignore


(* Check that internalVisibleTo items can be used in internal items *)
module internal Repro3737 =
  let internal internalModuleInternalVal_uses_csInternalType (x : LibraryCS.AInternalClass) = 123
  let internal internalModuleInternalVal_uses_fsInternalType (x : Library.P.InternalClass)  = 123
  let internal internalModuleInternalVal_uses_fsInternalObject = Library.P.InternalObject

  let internalModuleNormalVal_uses_csInternalType (x : LibraryCS.AInternalClass) = 123
  let internalModuleNormalVal_uses_fsInternalType (x : Library.P.InternalClass)  = 123
  let internalModuleNormalVal_uses_fsInternalObject = Library.P.InternalObject

// https://github.com/Microsoft/visualfsharp/issues/2401
module ReproFS =
  let c = new Repro.C( fun x -> "" )