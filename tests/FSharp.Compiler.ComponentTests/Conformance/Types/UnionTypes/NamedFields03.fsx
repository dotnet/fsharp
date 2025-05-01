// #Conformance #TypesAndModules #Unions 
// Potentially ambiguous cases where we need to decide if user is specifying named argument or passing result of equality comparison to boolean/generic field
//<Expects status="success"></Expects>

let checkType (o : obj) ty =
    if o.GetType() = ty then ()
    else
        printfn "Actual type %A" (o.GetType())
        failwith "Failed: 1"

let Value = 10
let o = Some(Value = 1) // name matches, assume it is named field syntax
checkType o (typeof<Option<int>>)

let oo = Some((Value = 1))  // enclosing in parens indicates result of equality operation
checkType oo (typeof<Option<bool>>)

let ooo = Some(Value = (Value = 1))
checkType ooo (typeof<Option<bool>>)

let test = 10
let o2 = Some(test = 1)  // name does not match, take this to mean the result of equality comparison
checkType o2 (typeof<Option<bool>>)


type MyDU<'a> = 
    | Case1 of V1 : int * V2 : int * V3 : bool * V4 : 'a

let o3 = Case1(2, 3, test = 1, V4 = "")   // V3 name doesn't match, use equality operation result.  V4 name does match, use named field
checkType o3 (typeof<MyDU<string>>)

let (Case1(V3 = x; V4 = z; V1 = y)) = o3
let (Case1(yy, _, xx, zz)) = o3

let o4 = Case1(2, 3, test = 1, test = 2)  // neither name matches, assume both are equality operation results
checkType o4 (typeof<MyDU<bool>>)

let V3 = 12
let V4 = ""
let o5 = Case1(2, 3, V3 = true, V4 = "")  // V3 name matches, assume named field.  Same with V4
checkType o5 (typeof<MyDU<string>>)

let o6 = Case1(2, 3, (V3 = 12), "")
checkType o6 (typeof<MyDU<string>>)

let o7 = Case1(2, 3, true, (V4 = ""))
checkType o7 (typeof<MyDU<bool>>)
