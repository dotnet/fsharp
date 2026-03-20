// #Regression #Conformance #DeclarationElements #Attributes 
open System.Text.RegularExpressions

[<assembly:System.Reflection.AssemblyVersion("1.2.*")>]
do ()

let asm = System.Reflection.Assembly.GetExecutingAssembly().GetName()

let tspan = System.TimeSpan(System.DateTime.UtcNow.Ticks - System.DateTime(2000,1,1).Ticks)
let defaultBuild = (uint16)tspan.Days % System.UInt16.MaxValue - 1us
let defaultRevision = (uint16)(System.DateTime.UtcNow.TimeOfDay.TotalSeconds / 2.0) % System.UInt16.MaxValue - 1us

printfn $"Version:  {asm.Version.ToString()} defaultBuild: {defaultBuild} defaultRevision: {defaultRevision}"

let success =
    asm.Version.Major = 1 &&
    asm.Version.Minor = 2 &&
    (abs (asm.Version.Build - (int defaultBuild))) <= 1 &&   // default value is days since Jan 1 2000. Allow ±1 for midnight-crossing builds.
    (abs (asm.Version.Revision - (int defaultRevision))) < 120  // default value is seconds in the current day / 2.  Allow 120s tolerance for slow CI.
if success then () else failwith "Failed: 1"
