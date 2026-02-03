// #FSI #Regression
// Regression for 6348, these used to fail on VS2008 in FSI

/// A type of operations
type IOps<'T> =
    abstract Add : 'T * 'T -> 'T
    abstract Zero : 'T;;

/// Create an instance of an F77Array and capture its operation set
type Matrix<'T> internal (ops: IOps<'T>, arr: 'T[,]) =
    member internal x.Ops = ops
    member internal x.Data = arr;;

type Matrix =
    /// A function to capture operations 
    static member inline private captureOps() = 
        { new IOps<_> with 
            member x.Add(a,b) = a + b
            member x.Zero = LanguagePrimitives.GenericZero<_> }

    /// Create an instance of an F77Array and capture its operation set
    static member inline Create nRows nCols =  Matrix<'T>(Matrix.captureOps(), Array2D.zeroCreate nRows nCols);;

Matrix.Create 10 10;;

type Array2D1<'T> =
  new(a: 'T[,]) =
    printfn "start"
    {
    };;

Array2D1 (array2D [[1];[2]]) |> ignore;;

#q;;