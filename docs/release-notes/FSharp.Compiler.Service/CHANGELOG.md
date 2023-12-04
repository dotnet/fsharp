---
category: Release Notes
categoryindex: 600
index: 1
title: FSharp.Compiler.Service
---

# FSharp.Compiler.Service

These release notes aim to follow the principles of [KeepAChangelog](https://keepachangelog.com/).  
Each entry takes an end-user centric perspective.

## Unreleased

### Fixes

* Miscellaneous fixes to parentheses analysis. ([PR #16262](https://github.com/dotnet/fsharp/pull/16262))
* Correctly handle assembly imports with public key token of 0 length. ([Issue #16359](https://github.com/dotnet/fsharp/issues/16359), [PR #16363](https://github.com/dotnet/fsharp/pull/16363))

## 43.8.100 - 2023-11-14

### Fixed

* Include the `get,set` keywords in the range of `SynMemberDefn.AutoProperty`. ([PR #15835](https://github.com/dotnet/fsharp/pull/15835))