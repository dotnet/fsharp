// #Regression #NoMT #FSI 
// Public fields did not print.
//<Expects status="success">val it: PublicField = FSI_0002+PublicField \{X = 2;
//                                             Y = 1;\}</Expects>
[<Struct>]
type PublicField = 
    val X : int
    val mutable Y : int
    new (x) = { X = x ; Y = 1 }

let t2 = PublicField(2);;
t2;;
#q;;
