module Ex1 =
  //simple example
  
  type A = { FA: int }
  type B = { FB: int }
  type AB = { FA: int; FB: int }

  let a = { FA = 1 } 
  let b = { FB = 2 }
  let a' = { a with FA = 2 }
  
module Ex2 =

  //more confusing fields

  type A = { FA: int }
  type B = { FB: int }
  type C = { FC: int }
  type AB = { FA: int; FB: int }
  type AC = { FA: int; FC: int }
  type CB = { FC: int; FB: int }
  type ABC = { FA: int; FB: int; FC: int }

  let a = { FA = 1 }
  let b = { FB = 1 }
  let c = { FC = 1 }
  let ab = { FA = 1; FB = 2 }
  let ac = { FA = 1; FC = 2 }
  let cb = { FC = 1; FB = 2 }
  let abc = { FA = 1; FB = 2; FC = 3 }
  
  let a' = { a with FA = 2 }

module Ex3 =

  //make sure this works with open
  open Ex2
  
  let a = { FA = 1 }
  let b = { FB = 1 }
  let c = { FC = 1 }
  let ab = { FA = 1; FB = 2 }
  let ac = { FA = 1; FC = 2 }
  let cb = { FC = 1; FB = 2 }
  let abc = { FA = 1; FB = 2; FC = 3 }
  
 module Ex4 =
 
  //still choose the last occurring match
  
  type A1 = { FA: int }
  type A2 = { FA: int }
  type C = { FC: int }
  type AB = { FA: int; FB: int }
  
  let a2 = { FA = 1 }
  let r = a2 :> A2 //this produces warnings, but proves that a2 is indeed of type A2.
  
System.IO.File.WriteAllText("test.ok","ok") 
exit 0