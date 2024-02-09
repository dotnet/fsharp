// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSHARP1.0:5155
// Stepping through if/then/else which is then stored into a local mistakenly steps into "else" branch

let F(y) =
  let x1 = if System.DateTime.Now.Year > 2000 then 1 else 2
  let y1 = if System.DateTime.Now.Year > 2000 then 1 else 2
  let x2 = if System.DateTime.Now.Year < 2000 then 1 else 2
  let y2 = if System.DateTime.Now.Year < 2000 then 1 else 2
  x1,y1,x2,y2

F(1) |> ignore  
