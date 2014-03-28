// #Regression #Conformance #ObjectOrientedTypes #TypeInference 
#light
// Verify type kind inference based on type members
module TypeInference 

// val binding / explicit field
type TK_C_003 = 
 val a : obj

// object constructor
type TK_C_004 = 
 new() = {}

// explicit object constructor + implicit constructor
type TK_C_005() = 
 new(s:string) = new TK_C_005()

//  implicit constructor
type TK_C_006() = class end
 
// let binding
type TK_C_007(o:obj) =
 let m = 0

// do binding
type TK_C_008(o:obj) =
 do System.Console.WriteLine()


// static let binding
type TK_C_009(o:obj) =
 static let m = 0

// do binding
type TK_C_010(o:obj) =
 static do System.Console.WriteLine()
 
// non-abstract instance member 
type TK_C_011 =
 member x.m(b:bool) = 0

// non-abstract static member
type TK_C_012 =
 static member m(c:char) = 0

// non-abstract override member 
type TK_C_013 = 
 override m.ToString()=  "TK_C_006"
 
// inherits decl 
type TK_C_014() = 
  inherit TK_C_006()

// inherit interface + concrete element // 2594 & 1978
type TK_I_003 = 
  abstract M: unit -> string

type TK_C_015() =
  interface TK_I_003 with
    member x.M() = "M"

  
let mutable a = false
try 

  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_003")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_004")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_005")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_006")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_007")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_008")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_009")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_010")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_011")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_012")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_013")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_014")).IsClass
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_015")).IsClass
with 
  | _ as e -> 
    printfn "%A" e
    a <- false
    
(if (a) then 0 else 1) |> exit
