// #Conformance #ObjectOrientedTypes #TypeExtensions  
//verify that types from a dll can be extended.

namespace NS
  module M = 
    type Lib with
    
    // Extension Methods
          member x.ExtensionMember () = 1
          static member StaticExtensionMember() =1
          
    // Extension Properties
          member x.ExtensionProperty001 = 1
          member x.ExtensionProperty002 with get() = 2
          member x.ExtensionProperty003 with get() = 3
          member x.ExtensionProperty003 with set(i:int) = ()
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set (inp : int) = x.instanceField <- inp             
          member x.ExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()
              

          static member StaticExtensionProperty001 = 11
          static member StaticExtensionProperty002 with get() = 12
          static member StaticExtensionProperty003 with get() = 13
          static member StaticExtensionProperty003 with set(i:int) = ()
// Invalid: can't cross the assembly boundary anymore
//          static member StaticExtensionProperty004 
//            with get () = Lib.staticField
//            and set (inp : int) = Lib.staticField <- inp             
          static member StaticExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()

  module MLong = 
    type NS.Lib with
      member x.ExtensionPropertyLong = 0


  module N =
    type LibGen<'a> with
    // Extension Methods
          member x.ExtensionMember () = Unchecked.defaultof<'a>
          static member StaticExtensionMember() =1
          
    // Extension Properties
          member x.ExtensionProperty001 = 1
          member x.ExtensionProperty002 with get() = 2
          member x.ExtensionProperty003 with get() = 3
          member x.ExtensionProperty003 with set(i:int) = ()
          member x.ExtensionProperty004 
            with get () = x.instanceField
            and set inp = x.instanceField <- inp
          member x.ExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()
              

          static member StaticExtensionProperty001 = 11
          static member StaticExtensionProperty002 with get() = 12
          static member StaticExtensionProperty003 with get() = 13
          static member StaticExtensionProperty003 with set(i:int) = ()
// Invalid: can't cross the assembly boundary anymore
//          static member StaticExtensionProperty004 
//            with get () = Lib.staticField
//            and set inp  = Lib.staticField <- inp
          static member StaticExtensionIndexer001 
            with get (i:int, j:int) = 1
            and set (i:int, j:int, value:int) = ()
  

  
  module F =
    open M
    open MLong
    let mutable res = true
  
    let a = new Lib()
    if not (a.ExtensionProperty001 = 1) then
      printf "Lib.ExtensionProperty001 failed\n"
      res <- false
    
    if not (a.ExtensionProperty002 = 2) then
      printf "Lib.ExtensionProperty002 failed\n"
      res <- false
    
    if not (a.ExtensionProperty003 = 3) then
      printf "Lib.ExtensionProperty003 failed\n"
      res <- false

// Invalid: can't cross the assembly boundary anymore
//  a.ExtensionProperty004 <- 4
//  if not (a.ExtensionProperty004 = 4) then
//    printf "Lib.ExtensionProperty004 failed\n"
//    res <- false
  
    if not (a.ExtensionPropertyLong  = 0) then
      printf "Lib.ExtensionMemberLong failed\n"
      res <- false
  
    if not (Lib.StaticExtensionProperty001 = 11) then
      printf "Lib.StaticExtensionProperty001 failed\n"
      res <- false
    
    if not (Lib.StaticExtensionProperty002 = 12) then
      printf "Lib.StaticExtensionProperty002 failed\n"
      res <- false
    
    if not (Lib.StaticExtensionProperty003 = 13) then
      printf "Lib.StaticExtensionProperty003 failed\n"
      res <- false


// Invalid: can't cross the assembly boundary anymore
//  Lib.StaticExtensionProperty004 <- 5
//  if not (Lib.StaticExtensionProperty004 = 5) then
//    printf "Lib.StaticExtensionProperty004 failed\n"
//    res <- false
    
    
    (if (res) then 0 else 1) |> exit
