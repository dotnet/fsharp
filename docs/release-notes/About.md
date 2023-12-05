---
category: Release Notes
categoryindex: 600
index: 1
title: About
---

# About

The release notes for the [F\# language](./Language.md), [FSharp.Core](./FSharp.Core.md) and [FSharp.Compiler.Service](./FSharp.Compiler.Service.md) are based on the [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) format.  
The target audience these release notes are the respective end-users.

## Writing a changelog entry

In order to keep the change logs consistent the following format was proposed for each entry:

```md
* <Informative description>. ([PR #16106](https://github.com/dotnet/fsharp/pull/16106))
```

Some tips:

* Use valid [Markdown](https://www.markdownguide.org/).
* Use `*` as bullet point symbol. We don't want to mix `*` and `-`.
* Start your description with a capital and end the sentence with a dot.
* **Always** include a link to your pull request before the closing `)`, `([PR #16106](https://github.com/dotnet/fsharp/pull/16106))`.
* If possible, include a link to an issue on `dotnet/fsharp` use `[Issue #16359](https://github.com/dotnet/fsharp/issues/16359)` before the link to the pull request.
* If possible, include a link to a language suggestion from  `dotnet/fsharp` use `[Language suggestion #721](https://github.com/fsharp/fslang-suggestions/issues/721)` before the link to the pull request.
* Choose the right section for your type of change. (`## Added`, `## Changed`, `## Deprecated`, `## Removed`, `## Fixed` or `## Security`).
* Ensure you description makes it clear what the change is about. The reader should be informed on a high level without needing to click through the pull request link and find out in the code what actually changed.

## The release process

### General

How does it work? Different stages/phases?

#### FSharp.Compiler.Service

Perhaps add some specific info if available?