# Contributing to F#

One of the easiest ways to contribute is to participate in discussions on GitHub issues. You can also contribute by submitting pull requests with code changes.

## General feedback and discussions?

Start a [discussion](https://github.com/dotnet/fsharp/discussions) on the [repository issue tracker](https://github.com/dotnet/fsharp/issues).

## Bugs and feature requests?

❗ **IMPORTANT: If you want to report a security-related issue, please see the `Reporting security issues and bugs` section below.**

Before reporting a new issue, try to find an existing issue if one already exists. If it already exists, upvote (👍) it. Also, consider adding a comment with your unique scenarios and requirements related to that issue.  Upvotes and clear details on the issue's impact help us prioritize the most important issues to be worked on sooner rather than later. If you can't find one, that's okay, we'd rather get a duplicate report than none.

If you can't find an existing issue, log a new issue in this GitHub repository.

## Creating Issues

- **DO** use a descriptive title that identifies the issue to be addressed or the requested feature. For example, when describing an issue where the compiler is not behaving as expected, write your bug title in terms of what the compiler should do rather than what it is doing – “F# compiler should report FS1234 when Xyz is used in Abcd.”
- **DO** specify a detailed description of the issue or requested feature.
- **DO** provide the following for bug reports
  - Describe the expected behavior and the actual behavior. If it is not self-evident such as in the case of a crash, provide an explanation for why the expected behavior is expected.
  - Provide an example with source code / projects that reproduce the issue.
  - Specify any relevant exception messages and stack traces.
- **DO** subscribe to notifications for the created issue in case there are any follow up questions.

## Reporting security issues and bugs

Security issues and bugs should be reported privately, via email, to the Microsoft Security Response Center (MSRC)  secure@microsoft.com. You should receive a response within 24 hours. If for some reason you do not, please follow up via email to ensure we received your original message. Further information, including the MSRC PGP key, can be found in the [Security TechCenter](https://technet.microsoft.com/security/ff852094.aspx).

## Writing Code

### Finding an issue to work on

  Over the years we've seen many PRs targeting areas, which we didn't plan to expand further at the time.  In many of these these cases we had to say `no` to those PRs and close them. That, obviously, is not a great outcome for us. And it's especially bad for the contributor, as they've spent a lot of effort preparing the change.
  To resolve this problem, we've decided to separate a bucket of issues, which would be great candidates for community members to contribute to. We mark these issues with the `help wanted` label. [help wanted](https://github.com/dotnet/fsharp/labels/help%20wanted)

  Within that set, we have additionally marked issues that are good candidates for first-time contributors. Here: [Good first issue](https://github.com/dotnet/fsharp/labels/good%20first%20issue)

  If you would like to make a contribution to an area not documented here, first open an issue with a description of the change you would like to make and the problem it solves so it can be discussed before a pull request is submitted.

### The primary customers of the F# repository are users of the dotnet SDK, Visual Studio, Rider and Ionide. At all times their experience is paramount in our mind.

  We are very accepting of community pull requests, there are however, a set of firm considerations that we hold to when reviewing PRs and determining what to merge.  These have been developed over the years to maintain the quality of the product and the experience that F# developers have when installing and upgrading the dotnet SDK and Visual Studio.

- Does the change fix something that needs fixing, is there an issue, does the issue indicate a real problem?
- Does the change improve the readability of something that needs improvement?
- Does the change add a feature that is approved for adding?
- Does the code match or improve of the existing codebase?
- Is the performance improvement measured and can regressions be identified?
- Will our existing customers be able to without effort upgrading the **Major** release of an SDK or VS?
- Will our existing customers be able to without effort upgrading the **Minor** release of an SDK or VS?
- This change is not binary breaking (i.e. does not break VS, Rider or Ionide)?
- Does it have adequate testing?
- Do all existing tests run unmodified?

In general answers to the above should be **Yes**.  A **No** to any of them is not disqualifying of the PR, however a no answer will need an explanation and a discussion.
 
There are additional considerations
- Is the risk of accepting this change High or even Medium, these really refer to how much of the existing user or codebase is impacted. How likely do we feel we are to revert the changes later.
  For an acceptable PR with a high risk, we will definitely need to discuss mitigations for the risk.  A decision to upgrade the SDK or VS needs to be always low risk for our customers, they have businesses to run, they don't want to have to deal with our - risky behavior.  We may defer or delay risky PRs into a later release or abandon it.
- Is the change as small as possible
- Should it be chopped up into smaller, yet independently valuable and releasable to production, chunks
- Is the cost of reviewing the change worth the improvement made by the change
Again, some PR’s are too big or provide too little value to merge.


### Resources to help you get started

Here are some resources to help you get started on how to contribute code or new content.

- [Developers Guide](https://github.com/dotnet/fsharp/blob/main/DEVGUIDE.md) to get started on building the source code on your own.
- [Test Guide](https://github.com/dotnet/fsharp/blob/main/TESTGUIDE.md) how to build run and work with test cases.
- [F# compiler guide](https://github.com/dotnet/fsharp/blob/main/docs/index.md)
- [F# language specification](https://fsharp.org/specs/language-spec/)
- [F# language design](https://github.com/fsharp/fslang-design/)
- [F# language suggestions](https://github.com/fsharp/fslang-suggestions/)
- [help wanted](https://github.com/dotnet/fsharp/labels/help%20wanted) where to start

### Submitting a pull request

You will need to sign a [Contributor License Agreement](https://cla.dotnetfoundation.org/) when submitting your pull request. To complete the Contributor License Agreement (CLA), you will need to follow the instructions provided by the CLA bot when you send the pull request. This needs to only be done once for any .NET Foundation OSS project.

If you don't know what a pull request is read this article: <https://help.github.com/articles/using-pull-requests>. Make sure the repository can build and all tests pass. Familiarize yourself with the project workflow and our coding conventions. 

- **DO** ensure submissions pass all Azure DevOps legs and are merge conflict free.
- **DO** submit language feature requests as issues in the [F# language](https://github.com/fsharp/fslang-suggestions) repos.  Please note: approved in principle does not guarantee acceptance.
- **DO NOT** submit language features as PRs to this repo first, or they will likely be declined.
- **DO** submit issues for other features. This facilitates discussion of a feature separately from its implementation, and increases the acceptance rates for pull requests.
- **DO NOT** submit large code formatting changes without discussing with the team first.

### Reviewing pull requests

Our repository gets a high volume of pull requests and reviewing each of them is a significant time commitment. Our team priorities often force us to focus on reviewing a subset of the active pull requests at a given time.

### Feedback

Contributors will review your pull request and provide feedback.

## Merging pull requests

When your pull request has had the feedback addressed, and it has been signed off by two or more core team reviewers with commit access, and all checks are green, we will commit it.

## Code of conduct

See [CODE-OF-CONDUCT.md](./CODE-OF-CONDUCT.md)
