// #Regression #NoMT #Import 
// Regression for FSHARP1.0:4652
// reference .exe in fsi gives an 84 error

#r @"ReferenceExe.exe"

let x = M.f 1

exit 0
