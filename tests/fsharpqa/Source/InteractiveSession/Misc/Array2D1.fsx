// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:6348
// FSI should be able to handle this simple code
//<Expects status="success">start</Expects>

type Array2D1<'T> =
  new(a: 'T[,]) =
    printfn "start"
    {
    }

Array2D1 (array2D [[1];[2]])
#q;;
