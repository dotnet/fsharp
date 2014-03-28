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


