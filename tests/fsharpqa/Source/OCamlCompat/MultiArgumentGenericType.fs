// #Regression #OCaml 


// Regressin Test for FSharp1.0:2098 - OCaml-compat warning for generics written like this: (string, int)Dictionary
//<Expects status="error" span="(11,44)" id="FS0010">Unexpected identifier in expression$</Expects>

open System.Collections.Generic

type CarStats = { Make : string; Weight : float; MaxSpeed : int; }

let cars : List<CarStats> = new (CarStats) List()

let redDevil = { Make = "Ferrari";
                 Weight = 2000.00;
                 MaxSpeed = 300 }

cars.Add(redDevil)
