
pushd bin
call "%~dp0cg.bat" /in FSharp.Compiler.Interactive.Settings.dll  /Trusted_Platform_Assemblies  .\System.Runtime.dll;.\FSharp.Core.dll;.\System.Threading.dll;.\System.Reflection.Primitives.dll;.\System.Globalization.dll;.\System.Reflection.dll;.\System.Resources.ResourceManager.dll;.\System.Diagnostics.Debug.dll;.\System.Reflection.TypeExtensions.dll;.\System.Threading.Thread.dll;.\System.Runtime.Extensions.dll;.\System.Runtime.Loader.dll;.\System.Reflection.Extensions.dll; 
popd
