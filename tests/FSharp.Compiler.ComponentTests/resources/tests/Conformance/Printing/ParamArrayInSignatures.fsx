// #Regression #NoMT #Printing 
// Regression test for https://github.com/Microsoft/visualfsharp/issues/109

// pretty printing signatures with params arguments
//<Expects status="success">type Heterogeneous =</Expects>
//<Expects status="success">  static member Echo: \[<System.ParamArray>\] args: obj\[\] -> obj\[\]</Expects>

type Heterogeneous =
    static member Echo([<System.ParamArray>] args: obj[]) = args

