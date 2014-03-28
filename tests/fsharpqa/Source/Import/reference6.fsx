// #Regression #NoMT #Import #RequiresPowerPack 
// Regression test for FSharp1.0:5501
// Title: Issues referencing assemblies in F# Interactive

#r "FSharp.PowerPack.Compatibility"

String.split [' '] "this doesn't fail in a project however";;

exit 0
