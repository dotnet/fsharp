// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1273
// Give warning on use of interface and/or override implementations in augmentations
//<Expects id="FS0069" span="(27,23-27,25)" status="warning">Interface implementations in augmentations are now deprecated\. Interface implementations should be given on the initial declaration of a type\.$</Expects>

type I = interface
         end
         
type I2 = interface
           inherit I
          end

type I3 = interface
           inherit I2
          end
          
type I4 = interface
           inherit I3
           abstract M : int
          end

type T2b2 = class
             interface I4
            end
           
type T2b2 with
            interface I4 with 
               member a.M = 3
            end
          end
