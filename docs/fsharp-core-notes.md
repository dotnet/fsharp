# Notes and Guidance on FSharp.Core

This technical guide discusses the FSharp.Core library.

Reference documentation for FSharp.Core can be found here: https://fsharp.github.io/fsharp-core-docs/

Much of the guidance below applies to any .NET library respecting binary compatibility.

## FSharp.Core is binary compatible

FSharp.Core is binary compatible across versions of the F# language. For example, this means you can create a newer project with a newer FSharp.Core in an older codebase and things should generally "just work".

**Binary compatibility means that a component built for X can bind to Y at runtime. It doesn't mean that Y behaves 100% the same as X, though.** For example, an older compiler that doesn't know how to understand `inref<'T>` referencing a newer FSharp.Core that has `inref<'T>` defined may not behave correctly if `inref<'T>` is used in source.

## FSharp.Core and F# scripts

F# scripts, executed by F# interactive, execute against the FSharp.Core deployed with the .NET SDK you are using. If you're expecting to use a more modern library feature and find that it's missing, it's likely because you have an older .NET SDK and thus an older F# Interactive. Upgrade your .NET SDK.

## Guidance for package authors

If you are authoring a NuGet package for consumption in the F# and .NET ecosystem, you already have to make a decision about functionality vs. reach by deciding what target framework(s) you support.

As an F# package author, you also need to make this decision with respect to FSharp.Core:

* Targeting an earlier version of FSharp.Core increases your reach because older codebases can use it without issue
* Targeting a newer version of FSharp.Core lets you use and extend newer features

This decision is critical, because it can have a network effect. If you choose a higher FSharp.Core version, then that also becomes a dependency for any other package that may depend on your package.

### Package authors should pin their FSharp.Core reference

The default templates for F# projects carry an implicit reference to FSharp.Core. This is ideal for application developers, since applications almost always want to be referencing the highest FSharp.Core available to them. As you upgrade your .NET SDK, the FSharp.Core package referenced implicitly will also be upgraded over time, since FSharp.Core is also distributed with the .NET SDK.

However, as a package author this means that unless you reference FSharp.Core explicitly, you will default to the latest possible version and thus eliminate any hope of reaching older projects in older environments.

### How to explicitly reference FSharp.Core

It's a simple gesture in your project file that pins to FSharp.Core 4.7.2:

```xml
<ItemGroup>
  <PackageReference Update="FSharp.Core" Version="4.7.2" />
</ItemGroup>
```

Or if you're using Paket:

```
nuget FSharp.Core >= 4.7.2
```

And that's it!

### Compatibility table

The following table can help you decide the minimum language/package version you want to support:

|Minimum F# language version|Minimum FSharp.Core package version|
|------------------------------|------------------------------|
|F# 4.1|4.3.4|
|F# 4.5|4.5.2|
|F# 4.6|4.6.2|
|F# 4.7|4.7.2|
|F# 5.0|5.0.0|

If you want to be compatible with much older projects using an F# 4.0 compiler or earlier, you can still do that but it's not recommended. People using those codebases should upgrade instead.

### Do *not* bundle FSharp.Core directly with a library 

Do _not_ include a copy of FSharp.Core with your library or package, such in the `lib` folder of a package. If you do this, you will create havoc for users of your library.

The decision about which `FSharp.Core` a library binds to is up to the application hosting of the library.

## Guidance for everyone else

If you're not authoring packages for distribution, you have a lot less to worry about.

If you are distributing library code across a private organization as if it were a NuGet package, please see the above guidance, as it likely still applies. Otherwise, the below guidance applies.

### Application authors don't have to explicitly reference FSharp.Core

In general, applications can always just use the latest FSharp.Core bundled in the SDK they are built with.

### C# projects referencing F# projects may need to pin FSharp.Core

You can reference an F# project just fine without needing to be explicit about an FSharp.Core reference when using C# projects based on the .NET SDK. References flow transitively for SDK-style projects, so even if you need to use types directly from FSharp.Core (which you probably shouldn't do anyways) it will pick up the right types from the right assembly.

If you do have an explicit FSharp.Core reference in your C# project that you **need**, you should pin your FSharp.Core reference across your entire codebase. Being in a mixed pinned/non-pinned world is difficult to keep straight over a long period of time.

## Guidance for older projects, compilers, and tools

Modern .NET development, including F#, uses SDK-style projects. You can read about that here: https://docs.microsoft.com/dotnet/core/project-sdk/overview

If you are not using SDK-style projects F# projects and/or have an older toolset, the following guidance applies.

### Consider upgrading

Yes, really. The old project system that manages legacy projects is not that good, the compiler is older and unoptimized for supporting larger codebases, tooling is not as responsive, etc. You will really have a much better life if you upgrade. Try out the `try-convert` tool to do that: https://github.com/dotnet/try-convert

If you cannot upgrade for some reason, the rest of the guidance applies.

### Always deploy FSharp.Core as part of a compiled application

For applications, FSharp.Core is normally part of the application itself (so-called "xcopy deploy" of FSharp.Core).  

For older project files, you may need to use ``<Private>true</Private>`` in your project file. In  Visual Studio this is equivalent to setting the `CopyLocal` property to `true` properties for the `FSharp.Core` reference.

FSharp.Core.dll will normally appear in the `bin` output folder for your application. For example:

```
    Directory of ...\ConsoleApplication3\bin\Debug\net5.0
    
    18/04/2020  13:20             5,632 ConsoleApplication3.exe
    14/10/2020  12:12         1,400,472 FSharp.Core.dll
```

### FSharp.Core and static linking

The ILMerge tool and the F# compiler both allow static linking of assemblies including static linking of FSharp.Core.
This can be useful to build a single standalone file for a tool.

However, these options must be used with caution. 

* Only use this option for applications, not libraries. If it's not a .EXE (or a library that is effectively an application) then don't even try using this option.

Searching on stackoverflow reveals further guidance on this topic.

## Reference: FSharp.Core version and NuGet package numbers

See [the F# version information RFC](https://github.com/fsharp/fslang-design/blob/master/tooling/FST-1004-versioning-plan.md).
