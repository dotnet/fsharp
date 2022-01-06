// #Regression #OCaml 
// Regression Test for FSharp1.0:2098 - OCaml-compat warning for generics written like this: (string, int)Dictionary
//<Expects status="warning" span="(10,19)" id="FS0062">This construct is for ML compatibility\. The syntax '\(typ,\.\.\.,typ\) ident' is not used in F# code\. Consider using 'ident<typ,\.\.\.,typ>' instead\. You can disable this warning by using '--mlcompatibility' or '--nowarn:62'\.$</Expects>
module TestModule

open System.Collections.Generic

type CarStats = { Make : string; Weight : float; MaxSpeed : int; }

let raceResults : (string, CarStats) Dictionary = new Dictionary<string, CarStats>()

let MadMike  = { Make = "Ferrari";
                 Weight = 2000.00;
                 MaxSpeed = 300 }

let SmartJoe  = { Make = "BMW";
                 Weight = 1890.50;
                 MaxSpeed = 287 }


raceResults.Add("Gold", SmartJoe)
raceResults.Add("Silver", MadMike)

()
