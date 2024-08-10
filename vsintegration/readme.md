This folder contains projects and tests related to Visual Studio tooling.

# src/FSharp.Editor

Top-level project for the Visual F# IDE tools.  This project contains the following

* Interfaces which implement Roslyn workspace APIs
* Top-level interactions with the F# Compiler Service
* Logic for handling data in the F# Compiler Service
* Helpers for interacting with Roslyn APIs
* Various utilities

In general, if something is implemented here and the logic becomes rather lengthy, it may be a good idea to push that logic down into the F# Compiler Service so that other editors can benefit.

# src/FSharp.VS.FSI

F# Interactive implementation.

# src/FSharp.UIResources

GUI controls and resources for Visual F# tooling.

# src/FSharp.LanguageService

Legacy bindings to the F# Compiler Service.  Most of the code paths here are dead, and exist mainly to allow our test suite to extensively test the F# Compiler Service.

# src/FSharp.LanguageService.Base

Legacy bits for a base-level, editor-agnostic language service.  This code isn't used anywhere else, though.

# src/FSharp.ProjectSystem.Base

Legacy bindings to the legacy (and unsupported) MPF project system type in Visual Studio.  This code will eventually be deprecated.

# src/FSharp.ProjectSystem.FSharp

Legacy project system to handle F# projects targeting the .NET Framework, with some code paths also handling .NET Core.  The latter will eventually be factored out, as this code will eventually be deprecated.

# src/FSharp.ProjectSystem.PropertyPages

GUI for F# project properties.

# tests/Salsa

Legacy tooling for IDE unit tests.  Used extensively in tests, hence it still exists.

# tests/unittests

IDE unit tests.  Some code paths go through bits which aren't ever executed when _using_ F# in Visual Studio, but they do extensively test the F# Compiler Service.

# utils/LanguageServiceProfiling

A skeleton command line tool which exercises the F# Compiler Service.

# ItemTemplates

Visual Studio item templates for F# projects.

# ProjectTemplates

Visual Studio project templates for .NET Framework projects.
