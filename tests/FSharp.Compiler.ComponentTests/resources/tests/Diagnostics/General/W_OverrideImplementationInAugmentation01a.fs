// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1273
// Give warning on use of interface and/or override implementations in augmentations
//
//<Expects id="FS0060" span="(13,22-13,25)" status="warning"></Expects>
module M

type T2b = class
            abstract M : int
            //default a.M = 3
           end
type T2b with
            override x.M = 0    // warning FS0060: Override implementations should be given as part of the initial declaration of a type. While the current language specification allows overrides implementations in augmentations, this is likely to be deprecated in a future revision of the language
        end

