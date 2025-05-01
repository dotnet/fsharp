---
category: Release Notes
categoryindex: 600
index: 1
title: About
---

# About

The release notes for the [F\# language](./Language.html), [FSharp.Core](./FSharp.Core.html) and [FSharp.Compiler.Service](./FSharp.Compiler.Service.html) are based on the [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) format.  
The target audience of these release notes are the respective end-users.

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
* Optionally, include a link to an issue on `dotnet/fsharp` use `Issue #number` before the link to the pull request.  

Example:

```md
* Correctly handle assembly imports with public key token of 0 length. ([Issue #16359](https://github.com/dotnet/fsharp/issues/16359), [PR #16363](https://github.com/dotnet/fsharp/pull/16363))
```

* Optionally, include a link to a language suggestion from  `dotnet/fsharp` use `Language suggestion #number` before the link to the pull request.

Example:

```md
* `while!` ([Language suggestion #1038](https://github.com/fsharp/fslang-suggestions/issues/1038), [PR #14238](https://github.com/dotnet/fsharp/pull/14238))
```

* Choose the right section for your type of change. (`## Added`, `## Changed`, `## Deprecated`, `## Removed`, `## Fixed` or `## Security`).
* Ensure your description makes it clear what the change is about. The reader should be informed on a high level without needing to click through the pull request link and find out in the code what actually changed.
* Maintainers or other contributors might rewrite your changelog entry in the future. This might be done when multiple pull requests can be consolidated under the same umbrella.
* Related pull requests can be listed in the same entry when it makes sense.

Example:

```md
* Miscellaneous fixes to parentheses analysis. ([PR #16262](https://github.com/dotnet/fsharp/pull/16262), [PR #16391](https://github.com/dotnet/fsharp/pull/16391), [PR #16370](https://github.com/dotnet/fsharp/pull/16370))
```

## The release process

### General

How does it work? Different stages/phases?

#### FSharp.Compiler.Service

Perhaps add some specific info if available?