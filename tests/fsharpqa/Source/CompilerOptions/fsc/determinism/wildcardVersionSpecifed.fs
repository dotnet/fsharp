// #NoMT #CompilerOptions #Determinism 
//<Expects id="FS2025" status="error">this value is a wildcard, and you have requested a deterministic build, these are in conflict.</Expects>
[<assembly: System.Reflection.AssemblyVersion("2.3.4.*")>]
exit 0
