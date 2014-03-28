// #Regression #OCaml 


// Regressin Test for FSharp1.0:2098 - OCaml-compat warning for generics written like this: (string, int)Dictionary
//<Expects status="error" span="(11,33)" id="FS0035">This construct is deprecated: The use of the type syntax 'int C' and 'C  <int>' is not permitted here\. Consider adjusting this type to be written in the form 'C<int>'$</Expects>

open System.Collections.Generic

type CarStats = { Make : string; Weight : float; MaxSpeed : int; }

let cars : List<CarStats> = new (CarStats) List()

let redDevil = { Make = "Ferrari";
                 Weight = 2000.00;
                 MaxSpeed = 300 }

cars.Add(redDevil)
