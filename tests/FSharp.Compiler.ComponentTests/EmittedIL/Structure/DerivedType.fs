module B

open A

type V() =
  inherit U()
  override x.TestBase() = base.TestBase() + 1
  override x.TetSecuritySafeCritical() = base.TetSecuritySafeCritical() + 1

[<EntryPoint>]
let main (args : string[]) =
    let x = V()
#if BASE_CALL
    x.TestBase()
#endif
#if CAS
    x.TestCAS()
#endif
#if SSC
    x.TetSecuritySafeCritical()
#endif