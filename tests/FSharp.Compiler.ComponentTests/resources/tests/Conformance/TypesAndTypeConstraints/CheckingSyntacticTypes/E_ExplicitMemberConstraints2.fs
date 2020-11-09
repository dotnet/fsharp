// #Conformance #TypeConstraints
//<Expects id="FS0001" status="error">None of the types 'bool, int, string' support the operator 'get_M'</Expects>

let inline g< ^t, ^u, ^v when (^t or ^u or ^v) : (static member M : string)>() = 0

let _ = g<bool, int, string>()
