@echo off

rem I need to figure out how to do crossgen again, since it's changed.
rem make sure we have a codegen
rem md %~dp0..\tools\crossgen
rem copy %~dp0..\packages\runtime.win7-x64.Microsoft.NETCore.Runtime.CoreCLR\1.0.2\tools\crossgen.exe %~dp0..\tools\dotnetcli\crossgen.exe
rem copy %~dp0..\packages\runtime.win7-x64.Microsoft.NETCore.Runtime.CoreCLR\1.0.2\runtimes\win7-x64\lib\netstandard1.0\*.* %~dp0..\tools\dotnetcli\*.*

pushd %~dp0..\tools\lkg
    call :crossgen /in fsharp.core.dll  /Trusted_Platform_Assemblies .\..\System.Runtime.dll;.\..\System.Threading.Tasks.dll;.\..\System.Linq.dll;.\..\System.Reflection.dll;.\..\System.Linq.Expressions.dll;.\..\System.Reflection.TypeExtensions.dll;.\..\System.Net.Requests.dll;.\..\System.IO.dll;.\..\System.Reflection.Primitives.dll;.\..\System.Globalization.dll;.\..\System.Collections.dll;.\..\System.Diagnostics.Debug.dll;.\..\System.Resources.ResourceManager.dll;.\..\System.Runtime.Extensions.dll;.\..\System.Threading.dll;.\..\System.Threading.ThreadPool.dll;.\..\System.Console.dll;.\..\System.Reflection.Extensions.dll;.\..\System.Linq.Queryable.dll;.\..\System.Collections.Concurrent.dll;.\..\System.Threading.Thread.dll;.\..\System.Threading.Timer.dll;.\..\System.Text.RegularExpressions.dll;.\..\System.Text.Encoding.dll;.\..\System.Runtime.Numerics.dll;.\..\System.Threading.Tasks.Parallel.dll;
    call :crossgen /in fsharp.compiler.dll  /Trusted_Platform_Assemblies .\..\System.Runtime.dll;.\..\FSharp.Core.dll;.\..\System.Collections.dll;.\..\System.IO.dll;.\..\System.Reflection.dll;.\..\System.Runtime.Numerics.dll;.\..\System.Reflection.Primitives.dll;.\..\System.Reflection.Emit.dll;.\..\System.Reflection.Emit.ILGeneration.dll;.\..\System.Collections.Immutable.dll;.\..\System.Reflection.Metadata.dll;.\..\System.Security.Cryptography.Algorithms.dll;.\..\System.Threading.Tasks.dll;.\..\System.Threading.dll;.\..\System.Resources.ResourceManager.dll;.\..\System.Globalization.dll;.\..\System.Diagnostics.Debug.dll;.\..\System.Reflection.TypeExtensions.dll;.\..\System.Console.dll;.\..\System.Runtime.Extensions.dll;.\..\System.Text.Encoding.dll;.\..\System.Threading.Thread.dll;.\..\System.IO.FileSystem.dll;.\..\System.IO.FileSystem.Primitives.dll;.\..\System.AppContext.dll;.\..\System.Diagnostics.Process.dll;.\..\System.Runtime.Loader.dll;.\..\System.Reflection.Extensions.dll;.\..\System.Collections.Concurrent.dll;.\..\System.Text.Encoding.Extensions.dll;.\..\System.Security.Cryptography.Primitives.dll;.\..\System.Runtime.InteropServices.dll;.\..\System.Threading.ThreadPool.dll;.\..\System.Text.RegularExpressions.dll;.\..\System.Linq.dll;
    call :crossgen /in fsc.dll  /Trusted_Platform_Assemblies .\..\System.Runtime.dll;.\..\FSharp.Core.dll;.\..\FSharp.Compiler.dll;.\..\System.Console.dll;
    call :crossgen /in FSharp.Compiler.Interactive.Settings.dll  /Trusted_Platform_Assemblies .\..\System.Runtime.dll;.\..\FSharp.Core.dll;.\..\System.Threading.dll;.\..\System.Reflection.Primitives.dll;.\..\System.Globalization.dll;.\..\System.Reflection.dll;.\..\System.Resources.ResourceManager.dll;.\..\System.Diagnostics.Debug.dll;.\..\System.Reflection.TypeExtensions.dll;.\..\System.Threading.Thread.dll;.\..\System.Runtime.Extensions.dll;.\..\System.Runtime.Loader.dll;.\..\System.Reflection.Extensions.dll; 
    call :crossgen /in fsi.dll  /Trusted_Platform_Assemblies .\..\System.Runtime.dll;.\..\FSharp.Core.dll;.\..\FSharp.Compiler.dll;.\..\System.Console.dll;.\..\System.Reflection.dll;.\..\System.Collections.dll;.\..\System.Reflection.Primitives.dll;.\..\System.Globalization.dll;.\..\System.Resources.ResourceManager.dll;.\..\System.Diagnostics.Debug.dll;.\..\System.Reflection.TypeExtensions.dll;.\..\System.Runtime.Extensions.dll;.\..\System.IO.dll;.\..\System.Threading.Thread.dll;.\..\FSharp.Compiler.Interactive.Settings.dll;.\..\System.Diagnostics.Process.dll;.\..\System.Reflection.Emit.dll;.\..\System.Reflection.Emit.ILGeneration.dll;.\..\System.Text.Encoding.dll;.\..\System.Threading.dll;.\..\System.IO.FileSystem.dll;.\..\System.AppContext.dll;.\..\System.Runtime.Loader.dll;.\..\System.Reflection.Extensions.dll;
popd
goto :eof

:crossgen
@echo off
"%~dp0..\tools\dotnetcli\crossgen" %* >%2.out
if errorlevel 1 (
    set errorreply=%errorlevel%
    type %1.out
    exit /b %errorreply%
)
goto :eof