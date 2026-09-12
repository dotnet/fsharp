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

`fsharp/fsharp-compiler-docs` is driven by a [Fun.Build](https://github.com/slaveOftime/Fun.Build) script, `build.fsx`.
By default it clones `dotnet/fsharp` into a git-ignored `fsharp` folder, builds `FSharp.Compiler.Service` and
generates the site from that clone. To work on the `docs` of your own `dotnet/fsharp` checkout instead, point the
`FSHARP_REPO` environment variable at it: the clone is skipped and your checkout is built and read in place.

* Clone `fsharp/fsharp-compiler-docs`:


    git clone https://github.com/fsharp/fsharp-compiler-docs.git
    cd fsharp-compiler-docs


* Run the `Build` pipeline once. It builds `FSharp.Compiler.Service.slnx` with the SDK your checkout pins (using
  the `eng/common/dotnet.sh` bootstrap, so that SDK does not need to be on your `PATH`), restores the `fsdocs` tool
  and generates the site into `output/`:


    FSHARP_REPO=/path/to/your/fsharp dotnet fsi build.fsx


* Then serve the docs with live reload:


    FSHARP_REPO=/path/to/your/fsharp dotnet fsi build.fsx -- -p Watch


Anything after the pipeline name is passed on to `fsdocs watch`, for example `-- -p Watch --nolaunch --port 8080`.
The `Watch` pipeline does not rebuild `FSharp.Compiler.Service`. Rerun `Build` after changing signature files or
XML documentation in `src/Compiler`.

## How watch works

`fsdocs watch` is a lazy development server: a page is built the first time it is requested and cached until a
file that influences it changes. Diagnostics, including which `FSharp.Compiler.Service.dll` is being documented,
are at `/.fsdocs/doctor` on the served site.

A page is rebuilt when the content of its own `.md` or `.fsx` file changes. Files a script pulls in through
`#load`, and files it reads while evaluating, are not tracked
([FSharp.Formatting#1309](https://github.com/fsprojects/FSharp.Formatting/issues/1309)). This affects the release
notes pages in `docs/release-notes`, which are composed from the MarkDown files in the hidden subfolders through
`.aux/Common.fsx`: editing those does not regenerate the served page. Make any edit to the `.fsx` of the page
(touching the file is not enough, the content has to change), or restart the watch.

Restart the watch as well after changing the package versions referenced from `.aux/Common.fsx`. The running F#
Interactive session keeps the assemblies it already loaded.
