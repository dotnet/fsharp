// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:2439
// fsi.CommandLineArgs
// scenario: no arguments - first arg (index 0) is fsi.exe / fsiAnyCPU.exe
let x = System.IO.Path.GetFileName(fsi.CommandLineArgs.[0])
let y1 = compare "fsi.exe" (x.ToLower())           // we accept both fsi.exe and fsi 
let y2 = compare "fsi" (x.ToLower())
let y1' = compare "fsianycpu.exe" (x.ToLower())    // we accept both fsiAnyCPU.exe and fsiAnyCPU 
let y2' = compare "fsianycpu" (x.ToLower())

if y1<>0 && y2<>0 && y1'<>0 && y2'<>0 then exit 1
exit 0;;

