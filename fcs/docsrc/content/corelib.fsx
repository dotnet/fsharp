(*** hide ***)
#I "../../../artifacts/bin/fcs/net461"
(**
Compiler Services: Notes on FSharp.Core.dll
=================================================

Shipping an FSharp.Core with your application
---------------------------------------------

When building applications or plug-in components which use FSharp.Compiler.Service.dll, you will normally also
include a copy of FSharp.Core.dll as part of your application.

For example, if you build a ``HostedCompiler.exe``, you will normally place an FSharp.Core.dll (say 4.3.1.0) alongside
your ``HostedCompiler.exe``.

Binding redirects for your application
--------------------------------------

The FSharp.Compiler.Service.dll component depends on FSharp.Core 4.4.0.0.  Normally your application will target
a later version of FSharp.Core, and you may need a [binding redirect](https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/redirect-assembly-versions) to ensure
that other versions of FSharp.Core forward to the final version of FSharp.Core.dll your application uses.
Binding redirect files are normally generated automatically by build tools. If not, you can use one like this
(if your tool is called ``HostedCompiler.exe``, the binding redirect file is called ``HostedCompiler.exe.config``)

Some other dependencies may also need to be reconciled and forwarded.

    <?xml version="1.0" encoding="utf-8" ?>
    <configuration>
        <runtime>
          <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
            <dependentAssembly>
              <assemblyIdentity name="FSharp.Core" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
              <bindingRedirect oldVersion="2.0.0.0-4.4.0.0" newVersion="4.4.1.0"/>
            </dependentAssembly>
            <dependentAssembly>
              <assemblyIdentity name="System.Collections.Immutable" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
              <bindingRedirect oldVersion="1.0.0.0-1.2.0.0" newVersion="1.2.1.0" />
            </dependentAssembly>
          </assemblyBinding>
        </runtime>
    </configuration>

Which FSharp.Core and .NET Framework gets referenced in compilation?
--------------------------------------

The FSharp.Compiler.Service component can be used to do more or less any sort of F# compilation.
In particular you can reference an explicit FSharp.Core and/or framework
assemblies in the command line arguments (different to the FSharp.Core and a .NET Framework being used to run your tool).

To target a specific FSharp.Core and/or .NET Framework assemblies, use the ``--noframework`` argument
and the appropriate command-line arguments:

    [<Literal>]
    let fsharpCorePath =
        @"C:\Program Files (x86)\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.1.0\FSharp.Core.dll"
    let errors2, exitCode2 =
      scs.Compile(
        [| "fsc.exe"; "--noframework";
           "-r"; fsharpCorePath;
           "-r"; @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll";
           "-o"; fn3;
           "-a"; fn2 |])

You will need to determine the location of these assemblies.  The easiest way to locate these DLLs in a cross-platform way and
convert them to command-line arguments is to [crack an F# project file](https://fsharp.github.io/FSharp.Compiler.Service/project.html).
Alternatively you can compute SDK paths yourself, and some helpers to do this are in [the tests for FSharp.Compiler.Service.dll](https://github.com/fsharp/FSharp.Compiler.Service/blob/8a943dd3b545648690cb3bed652a469bdb6dd869/tests/service/Common.fs#L54).


What about if I am processing a script or using ``GetCheckOptionsFromScriptRoot``
-------------------------------------------------------------------------

If you do _not_ explicitly reference an FSharp.Core.dll from an SDK location, or if you are processing a script
using ``FsiEvaluationSession`` or ``GetCheckOptionsFromScriptRoot``, then an implicit reference to FSharp.Core will be made
by the following choice:

1. The version of FSharp.Core.dll statically referenced by the host assembly returned by ``System.Reflection.Assembly.GetEntryAssembly()``.

2. If there is no static reference to FSharp.Core in the host assembly, then

   - For FSharp.Compiler.Service 1.4.0.x above (F# 4.0 series), a reference to FSharp.Core version 4.4.0.0 is added

Do I need to include FSharp.Core.optdata and FSharp.Core.sigdata?
--------------------------------------

No, unless you are doing something with very old FSharp.Core.dll.

Summary
-------

In this design note we have discussed three things:

- which FSharp.Core.dll is used to run your compilation tools
- how  to configure binding redirects for the FSharp.Core.dll used to run your compilation tools
- which FSharp.Core.dll and/or framework assemblies are  referenced during the checking and compilations performed by your tools.

*)
