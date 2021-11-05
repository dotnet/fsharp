(**
---
title: Notes on FSharp.Core
category: Compiler Service API
categoryindex: 300
index: 1100
---
*)
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

Which FSharp.Core and .NET SDK get referenced in compilation?
--------------------------------------

The FSharp.Compiler.Service component can be used to do more or less any sort of F# compilation.
In particular you can reference an explicit FSharp.Core and/or framework
assemblies in the command line arguments (different to the FSharp.Core and a .NET Framework being used to run your tool).

What about if I am using ``FsiEvaluationSession`` or ``GetProjectOptionsFromScript``
-------------------------------------------------------------------

If you do not explicitly reference an FSharp.Core.dll from an SDK location, or if you are processing a script
using ``FsiEvaluationSession`` or ``GetProjectOptionsFromScript``, then an implicit reference to FSharp.Core will be made
by the following choice:

1. The version of FSharp.Core.dll for the SDK implied by global.json

2. The version of FSharp.Core.dll statically referenced by the host assembly returned by ``System.Reflection.Assembly.GetEntryAssembly()``.

3. If there is no static reference to FSharp.Core in the host assembly, then a default is chosen

> NOTE: these may depend on exact flags passed to `GetProjectOptionsFromScript`

*)
