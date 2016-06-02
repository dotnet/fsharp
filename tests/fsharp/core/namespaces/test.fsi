// #Conformance #Namespaces #Modules

namespace Hello.Goodbye

type A = A | B | C

module Utils = begin
  val failures : bool ref
  val report_failure : unit -> unit
  val test : string -> bool -> unit
end

module X = begin
  val x : int
end


namespace Hello.Beatles

type Song = HeyJude | Yesterday

module X = begin
  val x : int
end



// Check recursive name resolution
namespace rec CheckRecursiveNameResolution1

    module Test =

      module N = 
          val x : Test.M.C

      module M = 
          type C


// Check recursive name resolution
namespace rec CheckRecursiveNameResolution2

    module Test =

      module N = 
          val x : M.C

      module M = 
          type C


// Check recursive name resolution
namespace rec CheckRecursiveNameResolution3

    module Test =

      open M

      module N = 
          val x : C

      module M = 
          type C

// Check recursive name resolution
namespace rec CheckRecursiveNameResolution4

    module Test =

      open Test.M

      module N = 
          val x : C

      module M = 
          type C


// Check recursive name resolution
namespace rec CheckRecursiveNameResolution5

    module Test =

      open Test.M

      module N = 
          val x : C

      module M = 
          type C

// Check recursive name resolution
namespace rec global

    open Test.M
    module Test =

      open Test.M
      open M

      module N = 
          val x : Test.M.C
          val x2 : M.C
          val x3 : C

      module M = 
          type C

