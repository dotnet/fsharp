// #Regression #Conformance #DeclarationElements #Attributes 
open System.Text.RegularExpressions

[<assembly:System.Reflection.AssemblyVersion("1.2.*")>]
do 
    ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()

let tspan = System.TimeSpan(System.DateTime.UtcNow.Ticks - System.DateTime(2000,1,1).Ticks)
let defaultBuild = (uint16)tspan.Days % System.UInt16.MaxValue - 1us
let defaultRevision = (uint16)(System.DateTime.UtcNow.TimeOfDay.TotalSeconds / 2.0) % System.UInt16.MaxValue - 1us

printfn "%s" <| asm.Version.ToString()
let success =
    asm.Version.Major = 1 &&
    asm.Version.Minor = 2 &&
    asm.Version.Build = (int defaultBuild) &&   // default value is days since Jan 1 2000.  Should match exactly.
    (abs (asm.Version.Revision - (int defaultRevision))) < 10  // default value is seconds in the current day / 2.  Check if within 10 sec of that.
if success then () else failwith "Failed: 1"
