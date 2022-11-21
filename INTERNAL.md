# Links for internal team members to find build definitions, etc.

Note that usually only the most recent link in each section is interesting.  Older links are included for reference only.

## PR Build Definition

The PR build definition can be found [here](https://dev.azure.com/dnceng/public/_build?definitionId=496) or by
navigating through an existing PR.

There is also a duplicate scouting PR build that is identical to the normal PR build _except_ that it uses a different Windows
machine queue that always has the next preview build of Visual Studio installed.  This is to hopefully get ahead of any breaking
API changes.  That build definition is [here](https://dev.azure.com/dnceng/public/_build?definitionId=961).

## Signed Build Definitions

[VS 16.4 to current](https://dev.azure.com/dnceng/internal/_build?definitionId=499&_a=summary)

[VS 15.7 to 16.3](https://dev.azure.com/devdiv/DevDiv/_build/index?definitionId=8978)

[VS 15.6](https://dev.azure.com/devdiv/DevDiv/_build?definitionId=7239)

[VS 15.0 to 15.5](https://dev.azure.com/devdiv/DevDiv/_build?definitionId=5037)

## Branch auto-merge definitions

Branch auto-merge definitions are specified [here](https://github.com/dotnet/roslyn-tools/blob/master/src/GitHubCreateMergePRs/config.xml).

## VS Insertion Generators

VS 16.4 to current - part of the build definition.  [See below](#vs-insertions-as-part-of-the-build-definition).

The following insertion generators are automatically invoked upon successful completion of a signed build in each of
their respective branches.

[VS 16.3](https://dev.azure.com/devdiv/DevDiv/_release?definitionId=1839&_a=releases)

[VS 16.2](https://dev.azure.com/devdiv/DevDiv/_release?definitionId=1699&_a=releases)

[VS 16.1](https://dev.azure.com/devdiv/DevDiv/_release?definitionId=1669&_a=releases)

VS 16.0 and prior were done manually

## VS Insertions as part of the build definition

Starting with the 16.4 release and moving forwards, the VS insertion is generated as part of the build.  The relevant
bits can be found near the bottom of [`azure-pipelines.yml`](azure-pipelines.yml) under the `VS Insertion` header.  The
interesting parameters are `componentBranchName` and `insertTargetBranch`.  In short, when an internal signed build
completes and the name of the branch built exactly equals the value in the `componentBranchName` parameter, a component
insertion into VS will be created into the `insertTargetBranch` branch.  The link to the insertion PR will be found
near the bottom of the build under the title 'Insert into VS'.  Examine the log for 'Insert VS Payload' and near the
bottom you'll see a line that looks like `Created request #xxxxxx at https://...`.

Insertions generated to any `rel/*` branch will have to be manually verified and merged, and they'll be listed
[here](https://dev.azure.com/devdiv/DevDiv/_git/VS/pullrequests?createdBy=122d5278-3e55-4868-9d40-1e28c2515fc4&_a=active).
Note that insertions for other teams will also be listed.

Insertions to any other VS branch (e.g., `main`) will have the auto-merge flag set and should handle themselves, but
it's a good idea to check the previous link for any old or stalled insertions into VS `main`.

## Preparing for a new VS release branch

### When a VS branch snaps from `main` to `rel/d*` and switches to ask mode:

Update the `insertTargetBranch` value at the bottom of `azure-pipelines.yml` in the appropriate release branch.  E.g., when VS 17.3 snapped and switched to ask mode, [this PR](https://github.com/dotnet/fsharp/pull/13456/files) correctly updates the insertion target so that future builds from that F# branch will get auto-inserted to VS.

### When VS `main` is open for insertions for preview releases of VS:

1. Create a new `release/dev*` branch (e.g., `release/dev17.4`) and initially set its HEAD commit to that of the previous release (e.g., `release/dev17.3` in this case).
2. Set the new branch to receive auto-merges from `main`, and also set the old release branch to flow into the new one.  [This PR](https://github.com/dotnet/roslyn-tools/pull/1245/files) is a good example of what to do when a new `release/dev17.4` branch is created that should receive merges from both `main` and the previous release branch, `release/dev17.3`.
3. Set the packages from the new branch to flow into the correct package feeds via the `darc` tool.  To do this:
   1. Ensure the latest `darc` tool is installed by running `eng/common/darc-init.ps1`.
   2. (only needed once) Run the command `darc authenticate`.  A text file will be opened with instructions on how to populate access tokens.
   3. Check the current package/channel subscriptions by running `darc get-default-channels --source-repo fsharp`.  For this example, notice that the latest subscription shows the F# branch `release/dev17.3` is getting added to the `VS 17.3` channel.
   4. Get the list of `darc` channels and determine the appropriate one to use for the new branch via the command `darc get-channels`.  For this example, notice that a channel named `VS 17.4` is listed.
   5. Add the new F# branch to the appropriate `darc` channel.  In this example, run `darc add-default-channel --channel "VS 17.4" --branch release/dev17.4 --repo https://github.com/dotnet/fsharp`
   6. Ensure the subscription was added by repeating step 3 above.
   7. Note, the help in the `darc` tool is really good.  E.g., you can simply run `darc` to see a list of all commands available, and if you run `darc <some-command>` with no arguments, you'll be given a list of arguments you can use.
   8. Ensure that version numbers are bumped for a new branch.

## Labeling issues on GitHub

Assign appropriate `Area-*` label to bugs, feature improvements and feature requests issues alike. List of `Area` labels with descriptions can be found [here](https://github.com/dotnet/fsharp/labels?q=Area). These areas are laid out to follow the logical organization of the code.

To find all existing open issues without assigned `Area` label, use [this query](https://github.com/dotnet/fsharp/issues?q=is%3Aissue+is%3Aopen+-label%3AArea-AOT+-label%3AArea-Async+-label%3AArea-Build+-label%3AArea-Compiler+-label%3AArea-Compiler-Checking+-label%3AArea-Compiler-CodeGen+-label%3AArea-Compiler-HashCompare+-label%3AArea-Compiler-ImportAndInterop+-label%3AArea-Compiler-Optimization+-label%3AArea-Compiler-Options+-label%3AArea-Compiler-PatternMatching+-label%3AArea-Compiler-Service+-label%3AArea-Compiler-SigFileGen+-label%3AArea-Compiler-SRTP+-label%3AArea-Compiler-StateMachines+-label%3AArea-Compiler-Syntax+-label%3AArea-ComputationExpressions+-label%3AArea-Debug+-label%3AArea-DependencyManager+-label%3AArea-Diagnostics+-label%3AArea-FCS+-label%3AArea-FSC+-label%3AArea-FSI+-label%3AArea-Infrastructure+-label%3AArea-LangService-API+-label%3AArea-LangService-AutoComplete+-label%3AArea-LangService-BlockStructure+-label%3AArea-LangService-CodeLens+-label%3AArea-LangService-Colorization+-label%3AArea-LangService-Diagnostics+-label%3AArea-LangService-FindAllReferences+-label%3AArea-LangService-Navigation+-label%3AArea-LangService-QuickFixes+-label%3AArea-LangService-RenameSymbol+-label%3AArea-LangService-ToolTips+-label%3AArea-LangService-UnusedDeclarations+-label%3AArea-LangService-UnusedOpens+-label%3AArea-Library+-label%3AArea-ProjectsAndBuild+-label%3AArea-Queries+-label%3AArea-Quotations+-label%3AArea-SetupAndDelivery+-label%3AArea-Testing+-label%3AArea-TypeProviders+-label%3AArea-UoM+-label%3AArea-VS+-label%3AArea-VS-Editor+-label%3AArea-VS-FSI+-label%3AArea-XmlDocs)

Since github issue filtering is currently not flexible enough, that query was generated by pasting output of this PowerShell command to the search box (might need to be rerun if new kinds of `Area` labels are added):
```ps1
Invoke-WebRequest -Uri "https://api.github.com/repos/dotnet/fsharp/labels?per_page=100" | ConvertFrom-Json | % { $_.name } | ? { $_.StartsWith("Area-") } | % { Write-Host -NoNewLine ('-label:"' + $_ + '" ') }
```

## Less interesting links

[FSharp.Core (Official NuGet Release)](https://dev.azure.com/dnceng/internal/_release?_a=releases&definitionId=72).
Uploads the final `FSharp.Core` package from the specified build to NuGet.  This should only be run when we know for
certain which build produced the final offical package.

[FSharp.Core (Preview NuGet Release)](https://dev.azure.com/dnceng/internal/_release?_a=releases&definitionId=92).
Uploads the preview `FSharp.Core.*-beta.*` package from the specified build to NuGet.  This should be run every time
a new SDK preview is released.

[FCS (Official NuGet Release)](https://dev.azure.com/dnceng/internal/_release?view=mine&_a=releases&definitionId=99).
Uploads the final `FSharp.Compiler.Service` package from the specified build to NuGet.  Only builds from the `release/fcs`
branch can be selected.  This should only be run when we're fairly certain that the package is complete.

[FCS (Preview NuGet Release)](https://dev.azure.com/dnceng/internal/_release?view=mine&_a=releases&definitionId=98).
Uploads the preview `FSharp.Compiler.Service.*-preview.*` package from the specified build to NuGet.  Only builds from the
`main` branch can be selected.  This can be run whenever we think we're ready to preview a new FCS build.

[Nightly VSIX (main) uploader](https://dev.azure.com/dnceng/internal/_release?_a=releases&definitionId=70).  Uploads
a package from every build of `main` to the [Nightly VSIX feed](README.md#using-nightly-releases-in-visual-studio).

[Nightly VSIX (preview) uploader](https://dev.azure.com/dnceng/internal/_release?_a=releases&definitionId=71).  Uploads
a package from every build of the branch that corresponds to the current Visual Studio preview to the
[Preview VSIX feed](README.md#using-nightly-releases-in-visual-studio).

[Internal source mirror](https://dev.azure.com/dnceng/internal/_git/dotnet-fsharp).
