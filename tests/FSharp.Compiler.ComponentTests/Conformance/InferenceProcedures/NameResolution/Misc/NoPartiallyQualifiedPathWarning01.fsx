// #Regression #Conformance #TypeInference #ReqNOMT 
// Regression test for FSHARP1.0:3274
// Make sure we do not emit any warning, particularly:
// warning FS0191: This declaration opens the namespace 'FSI_0003.S.M' through a partially qualified path. 
//                 Please adjust this code to use the full path of the namespace. A future version of F# will require this. 
//                 This change will make your code more robust as new constructs are added to the F# and .NET libraries.
#load "NoPartiallyQualifiedPathWarning01.fs";;

open S.M;;

exit 0;;
