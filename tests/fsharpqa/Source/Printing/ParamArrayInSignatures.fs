// #Regression #NoMT #Printing 
#light

// pretty printing signatures with params arguments
//<Expects status=success>type Heterogeneous =</Expects>
//<Expects status=success>  class</Expects>
//<Expects status=success>    static member Echo : \[<System.ParamArray>\] obj \[\] -> obj \[\]</Expects>
//<Expects status=success>  end</Expects>

type Heterogeneous =
    static member Echo([<System.ParamArray>] args: obj[]) = args