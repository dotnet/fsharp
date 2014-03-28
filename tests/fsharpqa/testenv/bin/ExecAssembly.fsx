// This script is used for Multitargeting testing.
// The common scenario is:
// 1- Testing against Dev10/NetFx4
// 2- Compile foo.fs targeting NetFx2.0
// 3- Execute foo.exe binding to the NetFx 4.0 
// Since fsi.exe is 4.0 only, feeling this script to fsi would be running in a 4.0 process
// and the 2.0 assembly (foo.exe) would be executed binding to 4.0

// Create a new App Domain
// AppDomainSetup object ensures the AppDomain's current directory is the current test directory
// else we may run into issues with Assembly.Load/ExecuteAssembly not using the same current directory as the test expects
let mutable setup = new System.AppDomainSetup()
setup.ApplicationBase <- System.Environment.CurrentDirectory
let appdomain = System.AppDomain.CreateDomain("F# targeting NetFx 4.0", null, setup, System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted))

// Set the assembly to be loaded and executed. It will be a command line argument...
let assemblyundertest = fsi.CommandLineArgs.[1]

// Execute the assembly
let rv = appdomain.ExecuteAssembly(assemblyundertest);

// Return exit code to the automation harness
exit rv
