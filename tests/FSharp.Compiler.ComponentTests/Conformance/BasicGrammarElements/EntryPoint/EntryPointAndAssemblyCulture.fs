// #NoMT #EntryPoint #Regression
// Regression for 6333 - we used to not allow AssemblyCultureAttribute on anything other than a library and not with an empty value, unlike C#
module AssemblyInfo
 
    open System.Reflection; 
    open System.Runtime.CompilerServices; 
    open System.Runtime.InteropServices; 
      
    // General Information about an assembly is controlled through the following  
    // set of attributes. Change these attribute values to modify the information 
    // associated with an assembly. 
    [<assembly: AssemblyTitle("FSharpApplication")>] 
    [<assembly: AssemblyDescription("")>] 
    [<assembly: AssemblyConfiguration("")>] 
    [<assembly: AssemblyCompany("Microsoft")>] 
    [<assembly: AssemblyProduct("FSharpApplication")>] 
    [<assembly: AssemblyCopyright("Copyright � Microsoft 2009")>] 
    [<assembly: AssemblyTrademark("")>] 
    [<assembly: AssemblyCulture("")>] 
      
     // Setting ComVisible to false makes the types in this assembly not visible  
     // to COM components.  If you need to access a type in this assembly from  
     // COM, set the ComVisible attribute to true on that type. 
     [<assembly: ComVisible(false)>] 
      
     // The following GUID is for the ID of the typelib if this project is exposed to COM 
     [<assembly: Guid("3629FB54-C5C8-4FA1-8A3C-887C2CE58BDD")>]
      
     // Version information for an assembly consists of the following four values: 
     // 
     //      Major Version 
     //      Minor Version  
     //      Build Number 
     //      Revision 
     // 
     // You can specify all the values or you can default the Build and Revision Numbers  
     // by using the '*' as shown below: 
     // [assembly: AssemblyVersion("1.0.*")] 
     [<assembly: AssemblyVersion("1.0.0.0")>] 
     [<assembly: AssemblyFileVersion("1.0.0.0")>] 
     do ()
 
    [<EntryPoint>]
    let _main(args : string[]) =
        exit 0