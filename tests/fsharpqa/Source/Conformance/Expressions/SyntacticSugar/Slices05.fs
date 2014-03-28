// #Conformance #SyntacticSugar 
// Verify array slicing for 3D arrays

/// Augment the existing Array2D type with Array2D and Array slicing
module Extensions = 
    type ``[,,]``<'T> with 
        // Slice 2D from 3D
        member q.GetSlice(x : int, y1 : int option, y2 : int option, z1 : int option, z2 : int option) = array2D [| [| 1.0 |] |]
        member q.GetSlice(x1 : int option, x2 : int option, y : int, z1 : int option, z2 : int option) = array2D [| [| 1.0 |] |]
        member q.GetSlice(x1 : int option, x2 : int option, y1 : int option, y2 : int option, z: int) = array2D [| [| 1.0 |] |]
        member q.GetSlice(xI : seq<int>, y1 : int option, y2: int option, z : int) = array2D [| [| 1.0 |] |]
        member q.GetSlice(xI :System.DateTime, y1 : int option, y2: int option, z : int) = array2D [| [| 1.0 |] |]

        // Slice 1D from 3D
        member q.GetSlice(x : int, y: int, z1 : int option, z2 : int option) = [| 1.0;2.0 |]
        member q.GetSlice(x1 : int, y1 : int option, y2:int option, z: int) = [| 1.0;2.0 |]
        member q.GetSlice(x1 : int option, x2 : int option, y : int, z: int) =  [| 1.0;2.0 |]

        // Set 2D slice in 3D
        member q.SetSlice(x : int, y1 : int option, y2 : int option, z1 : int option, z2 : int option, vs: 'T[,]) = ()
        member q.SetSlice(x1 : int option, x2 : int option, y : int, z1 : int option, z2 : int option, vs: 'T[,]) = ()
        member q.SetSlice(x1 : int option, x2 : int option, y1 : int option, y2 : int option, z: int, vs: 'T[,]) = ()
        member q.SetSlice(xI : seq<int>, y1 : int option, y2 : int option, z: int, vs: 'T[,]) = ()

        // Set 1D slice in 3D
        member q.SetSlice(x : int, y: int, z1 : int option, z2 : int option, vs: 'T[]) = ()
        member q.SetSlice(x1 : int, y1 : int option, y2:int option, z: int, vs: 'T[]) = ()
        member q.SetSlice(x1 : int option, x2 : int option, y : int, z: int, vs: 'T[]) = ()

        // wacky stuff
        member q.GetSlice(a : float, b : int option, c : System.DateTime option, d : bool option ) = ()
        member q.GetSlice(a : float, b : int option, c : System.DateTime option, d : double option ) = ()

open Extensions

let array3D () = Array3D.zeroCreate 1 1 1 
let x : double [,,] = array3D ()

// fix the first index - getting
let y1 = x.[1,*,1..4]
let y2 = x.[1,1..4,*]
let y3 = x.[1,1..3,*]
let z0 = x.[1,*,1..4]  
let z1 = x.[1,1,1..4]  
let z2 = x.[1,*,1..]  
let z2b = x.[1,1,1..]  
let z3 = x.[1,1,..4]  
let z3b = x.[1,*,..4]  
let z4 : double[] = x.[1,1..,1] 
let z4b : double[,] = x.[1,1..,*] 
let z5 : double[] = x.[1,..4,1] 
let z5b : double[,] = x.[1,..4,*] 
let z6 : double[] = x.[1,1..4,1] 
let z6b : double[,] = x.[1,1..4,*] 
let z7 : double = x.[0,0,0] 

// fix the second index - getting
let qy1 = x.[*,1,1..4]
let qy2 = x.[1..4,1,*]
let qy3 = x.[1..3,1,*]
let qz0 = x.[*,1,1..4]  
let qz1 = x.[1,1,1..4]  
let qz2 = x.[*,1,1..]  
let qz2b = x.[1,1,1..]  
let qz3 = x.[1,1,..4]  
let qz3b = x.[*,1,..4]  
let qz4 : double[] = x.[1..,1,1] 
let qz4b : double[,] = x.[1..,1,*] 
let qz5 : double[] = x.[..4,1,1] 
let qz5b : double[,] = x.[..4,1,*] 
let qz6 : double[] = x.[1..4,1,1] 
let qz6b : double[,] = x.[1..4,1,*] 
let qz7 : double = x.[0,0,0] 

// fix the third index - getting
let zqy1 = x.[*,1..4,1]
let zqy2 = x.[1..4,*,1]
let zqy3 = x.[1..3,*,1]
let zqz0 = x.[*,1..4,1]  
let zqz1 = x.[1,1..4,1]  
let zqz2 = x.[*,1..,1]  
let zqz2b = x.[1,1..,1]  
let zqz3 = x.[1,..4,1]  
let zqz3b = x.[*,..4,1]  
let zqz4 : double[] = x.[1..,1,1] 
let zqz4b : double[,] = x.[1..,*,1] 
let zqz5 : double[] = x.[..4,1,1] 
let zqz5b : double[,] = x.[..4,*,1] 
let zqz6 : double[] = x.[1..4,1,1] 
let zqz6b : double[,] = x.[1..4,*,1] 
let zqz7 : double = x.[0,0,0]
let zqz8 : double[,] = x.[[1;2], *, 1]
let zqz9 = x.[System.DateTime.Now, *, 1]

// fix the first index - setting
x.[1,*,1..4] <- array2D [ [1.0;2.0] ]
x.[1,1..4,*] <- array2D [ [1.0;2.0] ]
x.[1,1..3,*] <- array2D [ [1.0;2.0] ]
x.[1,1,1..4] <-  [| 1.0;2.0 |]
x.[1,1,1..]  <-  [| 1.0;2.0 |]
x.[1,1,..4]  <-  [| 1.0;2.0 |]
x.[1,1..,1] <-  [| 1.0;2.0 |]
x.[1,..4,1] <-  [| 1.0;2.0 |] 
x.[1,1..4,1]  <-  [| 1.0;2.0 |]
x.[0,0,0] <- 1.0 
x.[1,*,..4]  <- array2D [ [1.0;2.0] ]
x.[1,1..,*]   <- array2D [ [1.0;2.0] ]
x.[1,..4,*]   <- array2D [ [1.0;2.0] ]
x.[1,1..4,*]   <- array2D [ [1.0;2.0] ]

// fix the second index - setting
x.[*,1,1..4] <- array2D [ [1.0;2.0] ]
x.[1..4,1,*] <- array2D [ [1.0;2.0] ]
x.[1..3,1,*] <- array2D [ [1.0;2.0] ]
x.[1,1,1..4] <-  [| 1.0;2.0 |]
x.[1,1,1..]  <-  [| 1.0;2.0 |]
x.[1,1,..4]  <-  [| 1.0;2.0 |]
x.[1..,1,1] <-  [| 1.0;2.0 |]
x.[..4,1,1] <-  [| 1.0;2.0 |] 
x.[1..4,1,1]  <-  [| 1.0;2.0 |]
x.[0,0,0] <- 1.0 
x.[*,1,..4]  <- array2D [ [1.0;2.0] ]
x.[1..,1,*]   <- array2D [ [1.0;2.0] ]
x.[..4,1,*]   <- array2D [ [1.0;2.0] ]
x.[1..4,1,*]   <- array2D [ [1.0;2.0] ]

// fix the third index - setting
x.[*,1..4,1] <- array2D [ [1.0;2.0] ]
x.[1..4,*,1] <- array2D [ [1.0;2.0] ]
x.[1..3,*,1] <- array2D [ [1.0;2.0] ]
x.[1,1..4,1] <-  [| 1.0;2.0 |]
x.[1,1..,1]  <-  [| 1.0;2.0 |]
x.[1,..4,1]  <-  [| 1.0;2.0 |]
x.[1..,1,1] <-  [| 1.0;2.0 |]
x.[..4,1,1] <-  [| 1.0;2.0 |] 
x.[1..4,1,1]  <-  [| 1.0;2.0 |]
x.[0,0,0] <- 1.0 
x.[*,..4,1]  <- array2D [ [1.0;2.0] ]
x.[1..,*,1]   <- array2D [ [1.0;2.0] ]
x.[..4,*,1]   <- array2D [ [1.0;2.0] ]
x.[1..4,*,1]   <- array2D [ [1.0;2.0] ]
x.[[3;4],*,1]   <- array2D [ [1.0;2.0] ]

// weird stuff
let w1 = x.[3., *, Some(true)]
let w2 = x.[3., None, ..false]
let w3 = x.[3., 3.. , Option<double>.None]
let w4 = x.[1., ..System.DateTime.Now, Some(3.)]
let w5 = x.[3., Some(1), System.DateTime.Now..true]

exit 0