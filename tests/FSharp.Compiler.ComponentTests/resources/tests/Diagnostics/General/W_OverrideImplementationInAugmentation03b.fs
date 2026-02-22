// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1273
// Give warning on use of interface and/or override implementations in augmentations
//
//<Expects id="FS0060" span="(20,22-20,25)" status="warning">Override implementations in augmentations are now deprecated.</Expects>
module M

[<Measure>] type kg

[<AbstractClassAttribute>]
type T4() = class
             abstract M : float<kg>
             //default a.M = ...
            end
type T4b() = class
               inherit T4()
             end

type T4b with
            override x.M = 11.1<kg>
         end
