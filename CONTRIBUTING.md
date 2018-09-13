## Contribution Guidelines

The Visual F# team is proud to be a contributor to F#, and urge you to join in too. F# users and the F# community are grateful for all contributions to F#.

Besides this overview, we recommend ["A journey into the F~ compiler"](https://skillsmatter.com/skillscasts/11629-a-journey-into-the-f-sharp-compiler/), a talk by Steffen Forkmann. 
For those contributing to the core of the F# compiler, we recommend ["The F# Compiler Technical Overview"](http://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html)

### Getting Started

- Install required software
- Clone the repo \
  `git clone https://github.com/microsoft/visualfsharp.git`
- Read how to build in [DEVGUIDE.md](DEVGUIDE.md)
- Read how to run tests in [TESTGUIDE.md](TESTGUIDE.md)

### What to Contribute?

There are several important ways that you can contribute. We are especially grateful for early feedback on in-development features, bug reports with repro steps, bug fixes with regression test cases, cross-platform expertise and changes, documentation updates, feature tests, suggestions, comments, and ideas. 

We initially solicit contributions for

- compiler optimizations
- compiler performance improvements
- code generation improvements
- bug fixes (see the [issues list](https://github.com/microsoft/visualfsharp/issues))
- library improvements
- F# language and library features

New features are welcome, but be aware that Visual F# is a high-quality programming language with high-quality tools, and we wish to keep it that way. Before embarking on an extensive feature implementation, make a proposal in a GitHub issue or on the [F# Language Suggestions](https://github.com/fsharp/fslang-suggestions) so the community can review and comment on it. 

### Issues

When submitting issues, please use the following guidelines

- Suggestions for the F# Language and Core library should be added and reviewed at the [F# Language Suggestions](https://github.com/fsharp/fslang-suggestions).

- Suggestions for the Visual F# Tools should be added and reviewed at the [Visual Studio F# Tools GitHub](https://github.com/microsoft/visualfsharp).

- New Bug Reports should always give accurate, clear steps for reproducing the bug, and all relevant details about versions, platform, etc.  We suggest the following template:

    Title: &lt;a short, clear title&gt;
    
    Description: &lt;a description of the problem&gt;
    
    Repro Steps: &lt;step by step description&gt;
    
    Expected: &lt;what is expected&gt;
    
    Actual: &lt;what you really get&gt;
    
    Severity: a description on how bad it is and why - is it blocking?
    
    Version: Language, compiler, library, platform version
    
    Link: Give a link to a ZIP, log files or other files if needed
    
    Likely Cause: Link to the place in the code where things are likely going wrong, if possible
    
    Workaround: List any known workarounds

### CLA

Contributors are required to sign a [Contribution License Agreement](https://cla.microsoft.com/) (CLA) before any pull requests will be considered. After submitting a request via the provided form, electronically sign the CLA when you receive the email containing the link to the document. This only needs to be done once for each Microsoft OSS project you contribute to.

### Quality and Testing

Contributions to this repository will be rigorously policed for quality.

All code submissions should be submitted with regression test cases, and will be subject to peer review by the community and Microsoft.  The bar for contributions will be high. This will result in a higher-quality, more stable product.

- We expect contributors to be actively involved in quality assurance.
- Partial, incomplete, or poorly-tested contributions will not be accepted.
- Contributions may be put on hold according to stability, testing, and design-coherence requirements.

#### Mimimum Bar for Code Cleanup Pull Requests

In addition to the above, "Code Cleanup" pull requests have the following minimum requirements:

- There must be no chance of a behavioural change, performance degradation or regression under any reasonable reading of the code in the context of the codebase as a whole.  

- Code cleanup which is unrelated to a bug fix or feature should generally be made separate to other checkins where possible. 
- Code cleanup is much more likely to be accepted towards the start of a release cycle. 

#### Mimimum Bar for Performance Improvement Pull Requests

Performance improvement checkins have the following minimum requirements (in addition to the above)

- Performance tests and figures must be given, either in the PR or in the notes associated with the PR.  PRs without performance figures will be closed with a polite request to please add them.

- The PR must show a reliable, substantive performance improvement that justifies the complexity introduced.  For the compiler, performance improvements of ~1% are of interest.  For the core library, it will depend on the routine in question. For the Visual F# tools, reactivity of the user interface will be of more interest than raw CPU performance.

- Performance improvements should not cause performance degradation in existing code.

#### Mimimum Bar for Bug Fix Pull Requests

Bug fix PRs have the following minimum requirements

- There must be a separate tracking bug entry in the public GitHub issues. A link should be given in the PR. PRs without a matching bug link will be closed with a polite request to please add it.

- The code changes must be reasonably minimal and as low-churn, non-intrusive as possible. Unrelated cleanup should be done in separate PRs (see above), and fixes should be as small as possible. Code cleanup that is part of making a clear and accurate fix is acceptable as part of a bug fix, but care should be taken that it doesn't obscure the fix itself. For example, renaming identifiers to be clearer in a way that would have avoided the original bug is acceptable, but care must still be taken that the actual fix is still apparent and reviewable in the overall diff for the fix.

- Thorough test cases must be included in the PR (unless tests already exist for a failing case). PRs without matching tests will be closed with a polite request to please add the tests.  However, if you need help adding tests, please note this in the description of the change and people will guide you through where to add the tests.

- Bug fix PRs should not cause performance degradation in existing code.

#### Mimimum Bar for Feature Pull Requests

Feature PRs have the following minimum requirements:

- For F# Language and Library features, include a link to the [F# Language Suggestions](https://github.com/fsharp/fslang-suggestions) issue

- For Visual F# Tools features, include a link to the [Visual F# Tools](https://github.com/microsoft/visualfsharp) issue for the feature.

- For F# Library features, if you have made additions to the FSharp.Core library public surface area, update [the SurfaceArea tests](https://github.com/Microsoft/visualfsharp/tree/fsharp4/tests/FSharp.Core.UnitTests).

- For F# Language and Library features, you will be asked to submit a speclet for the feature to the [F# Language Design](https://github.com/fsharp/fslang-design) GitHub repository of speclets.  In some cases you will only need to do this after your feature is accepted, but for more complex features you may be asked to do this during the review of the feature.  

- Language feature implementations must take into account the expectations of typical users about the performance 
  impact of using the feature.  For example, we should avoid the situation where using an optional language feature 
  which appears benign to a typical user has a large negative performance impact on code.

- Language feature implementations should not cause performance degradation in existing code.

### Language Evolution

We are committed to carefully managing the evolution of the F# language.

We actively solicit contributions related to the F# language design, but the process for handling these differs substantially from other kinds of contributions. Significant language and library change should be suggested and reviewed at the [F# Language Suggestions](https://github.com/fsharp/fslang-suggestions) repository.

### Coding guidelines

Although there is currently no strict set of coding or style guidelines, use common sense when contributing code - make an effort to use a similar style to nearby existing code. If you have a passion for helping us develop a set of coding guidelines that we can roll out and apply within this project, get involved and start a discussion issue.
