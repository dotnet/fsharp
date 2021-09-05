// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:5891
// Covering the blit between zero-based and non-zero based Array2D
// The rest of the tests are Unittests.
//
//<Expects status="success">val it: bool = true</Expects>

//val a: string[,] = [bound1=1
//                    bound2=2
//                    ["a12"; "a13"]
//                    ["a22"; "a23"]
//                    ["a32"; "a33"]]
//
//val b: string[,] = [["b00"; "b01"]
//                    ["b10"; "b11"]
//                    ["b20"; "b21"]]
                      
let a = Array2D.initBased 1 2 3 2 (fun a b -> "a" + string a + string b)

let b = Array2D.initBased 0 0 3 2 (fun a b -> "b" + string a + string b)

Array2D.blit a 1 2 b 0 0 2 2;;

b.[0,0] = "a12";;

exit 0;;


