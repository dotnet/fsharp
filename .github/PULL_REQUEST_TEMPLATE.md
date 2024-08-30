## Description

<!-- Please include a summary of the change and which issue is fixed. Please also include relevant motivation and context. List any dependencies that are required for this change. -->

Fixes # (issue, if applicable)

## Checklist

- [ ] Test cases added
- [ ] Performance benchmarks added in case of performance changes
- [ ] Release notes entry updated:
    > Please make sure to add an entry with short succinct description of the change as well as link to this pull request to the respective release notes file, if applicable.
    >
    > Release notes files:
    > - If anything under `src/Compiler` has been changed, please make sure to make an entry in `docs/release-notes/.FSharp.Compiler.Service/<version>.md`, where `<version>` is usually "highest" one, e.g. `42.8.200`
    > - If language feature was added (i.e. `LanguageFeatures.fsi` was changed), please add it to `docs/release-notes/.Language/preview.md`
    > - If a change to `FSharp.Core` was made, please make sure to edit `docs/release-notes/.FSharp.Core/<version>.md` where version is "highest" one, e.g. `8.0.200`.

    > Information about the release notes entries format can be found in the [documentation](https://fsharp.github.io/fsharp-compiler-docs/release-notes/About.html).
    > Example:
    > * More inlines for Result module. ([PR #16106](https://github.com/dotnet/fsharp/pull/16106))
    > * Correctly handle assembly imports with public key token of 0 length. ([Issue #16359](https://github.com/dotnet/fsharp/issues/16359), [PR #16363](https://github.com/dotnet/fsharp/pull/16363))
    > *`while!` ([Language suggestion #1038](https://github.com/fsharp/fslang-suggestions/issues/1038), [PR #14238](https://github.com/dotnet/fsharp/pull/14238))

    > **If you believe that release notes are not necessary for this PR, please add `NO_RELEASE_NOTES` label to the pull request.**