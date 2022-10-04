// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1980
//<Expects status="success"></Expects>
#light

type X = struct
          val f : decimal
          new (arg) = 
            for i in [1;2] do ()
            X(1)
         end     
