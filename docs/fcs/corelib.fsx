(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Notes on FSharp.Core.dll
=================================================

Versions of FSharp.Core involved in the operation of FSharp.Compiler.Service
---------------------------------------------

There are three versions of FSharp.Core relevant to the operation of FSharp.Compiler.Service:

1. **The FSharp.Compiler.Service.dll static reference to FSharp.Core** - The FCS DLL and nuget have a static minbound dependency on FSharp.Core.

   This is just a normal .NET dependency like any other, it expresses the minimum surface area of FSharp.Core that the implementation of FSharp.Compiler.Service (and any components that depend on it) needs.  It could be a reference to a reference assembly if we supported that.  In theory this could be very low and all is cool - if we could implement FCS in terms of FSharp.Core 2.0.0.0 then that could be the minbound (indeed in theory we could implement FCS pretty almost without any use of FSharp.Core functionality at all, though obviously we don't)

   In practice this is 0-2 versions behind latest FSharp.Core.

2. **The runtime reference to FSharp.Core in a tool, application or test suite that includes FSharp.Compiler.Service** - This is the actual version of FSharp.Core used when, say, fsc.exe or devenv.exe or fsi.exe or fsdocs.exe runs.

   This must be at least as high as (1) and is usually the very latest FSharp.Core available (in or out of repo tree).  This is important to the operation of the FCS-based tool because it is used for execution of scripts, and the default compilation reference for scripts.  If scripts are going to use a particular language feature then this must be sufficient to support the language feature

3. **The FSharp.Core reference in a compilation or analysis being processed by FSharp.Compiler.Service**.

   This can be anything - 2.0.0.0, 4.0.0.0 or 5.0.0 or whatever.  For script compilation and execution is is the same as (2).  It must be sufficient to support language features used in the compilation.

Shipping an FSharp.Core with your application
---------------------------------------------

When building applications or plug-in components which use FSharp.Compiler.Service.dll, you will normally also
include a copy of FSharp.Core.dll as part of your application.

For example, if you build a ``HostedCompiler.exe``, you will normally place an FSharp.Core.dll (say 4.3.1.0) alongside
your ``HostedCompiler.exe``.


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

- the versions of FSharp.Core relevant to the operation of FSharp.Compiler.Service.dll
- which FSharp.Core.dll is used to run your compilation tools
- how  to configure binding redirects for the FSharp.Core.dll used to run your compilation tools
- which FSharp.Core.dll and/or framework assemblies are  referenced during the checking and compilations performed by your tools.

*)
