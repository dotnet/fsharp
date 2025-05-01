// This test checks error ranges for a number of members not allowed in augmentations. It
// also checks we actually get errors for all of these.
module Neg46

type System.Random with
       do printfn "hello"
       static member Factory() = 1

type System.Random with
       static let gen = System.Random()
       static member Factory() = 1

type System.Random with
       static let f x = x
       static member Factory() = 1

type System.Random with
       static let rec f x = g x 
       and g x = f x
       static member Factory() = 1

type System.Random with
       static let rec f x = f x 
       static member Factory() = 1

type System.Random with
       let rec f x = f x 
       static member Factory() = 1

type System.Random with
       let rec f x = g x 
       and g x = f x
       static member Factory() = 1

type System.Random with
       let rec f x = g x 
       static member Factory() = 1

type System.Random with
       let gen = Random()
       static member Factory() = 1

type System.Random with
       abstract M : int -> int
       static member Factory() = 1

type System.Random with
       abstract P : int  with get
       static member Factory() = 1

type System.Random with
       abstract P : int  with get, set
       static member Factory() = 1

type System.Random with
       new () = { }
       static member Factory() = 1

type System.Random with
       val x : int
       static member Factory() = 1

type System.Random with
       static val x : int
       static member Factory() = 1

type System.Random with
       interface System.IComparable with 
          member x.A = 1
       static member Factory() = 1

type System.Random with
       interface System.IComparable 
       static member Factory() = 1

