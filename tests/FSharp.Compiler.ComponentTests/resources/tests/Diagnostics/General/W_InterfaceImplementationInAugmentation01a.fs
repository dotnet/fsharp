// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1273
// Give warning on use of interface and/or override implementations in augmentations
//
//<Expects id="FS0069" span="(18,23-18,24)" status="warning"></Expects>



type I = interface
           abstract M : int
         end

type T2b2 = class
            interface I
           end
           
type T2b2 with
            interface I with 
               member a.M = 3
            end
         end
         
