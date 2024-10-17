---
title: Running the documentation locally
category: Compiler Internals
categoryindex: 200
index: 999
---
# Running the documentation locally

The source of this documentation website is hosted on https://github.com/fsharp/fsharp-compiler-docs.  
You can follow this guide to see the results of your document changes rendered in the browser.

## Setup

`fsharp/fsharp-compiler-docs` will clone the `dotnet/fsharp` repository first to generate the documentation.  
You can however, easily run the documentation locally and modify the `docs` from `dotnet/fsharp`.

* Clone `fsharp/fsharp-compiler-docs` at the same level as your local `dotnet/fsharp` repository:


    git clone https://github.com/fsharp/fsharp-compiler-docs.git


* Restore the `FSharp.Compiler.Service` project in `fsharp-compiler-docs`:


    cd fsharp-compiler-docs/FSharp.Compiler.Service
    dotnet restore


* Restore the local tools in `fsharp-compiler-docs`:


    cd ..
    dotnet tool restore


* Run the documentation tool using your `dotnet/fsharp` fork as input.


    dotnet fsdocs watch --eval --sourcefolder ../fsharp/ --input ../fsharp/docs/


## Release notes caveat

The release notes pages from `docs/release-notes` are composed from the MarkDown files in subfolders.  
Changing any of these files, won't regenerate the served webpage. Only the changes to the `.fsx` will trigger the tool. This is a known limitation.
