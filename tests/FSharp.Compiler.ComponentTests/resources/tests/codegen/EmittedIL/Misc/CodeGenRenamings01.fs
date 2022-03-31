// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSharp1.0:5645
// Title: Action proposed changes from review of compiled names

let alist = [ 1 .. 10 ]
let array = [| 1; 2; 3 |]
let aseq = seq { 1 .. 10 }
let list1 = [ 1, 1; 2, 2 ]
let seq1 = seq { yield (1, 1); yield (2, 2) }
let array1 = [| (1, 1); (2, 2) |]
let a3 = Array2D.create 2 2 0
let array3D = Array3D.create 3 3 3 0
let array4D = Array4D.create 4 4 4 4 0

// List Renamings
let _ = List.ofArray array
let _ = List.ofSeq aseq

// Map Renamings
let _ = Map.ofArray array1
let _ = Map.ofList list1
let _ = Map.ofSeq seq1

// Array Renamings
let a1 = Array.ofList alist
let a2 = Array.ofSeq  aseq
Array.get a1 0 |> Array.set a2 0

// Array2D Renamings
(Array2D.length1 a3, Array2D.length2 a3, Array2D.base1 a3, Array2D.base2 a3) |> ignore
Array2D.get a3 0 0 |> Array2D.set a3 0 0

// Array3D Renamings
(Array3D.length1 array3D, Array3D.length2 array3D, Array3D.length3 array3D) |> ignore
Array3D.get array3D 0 0 0 |> Array3D.set array3D 0 0 0

// Array4D Renamings
(Array4D.length1 array4D, Array4D.length2 array4D, Array4D.length3 array4D, Array4D.length4 array4D) |> ignore
Array4D.get array4D 0 0 0 0 |> Array4D.set array4D 0 0 0 0
