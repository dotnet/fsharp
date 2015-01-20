##Contribution Guidelines

The Visual F# team is proud to be a contributor to F#, and urge you to join in too. F# users and the F# community are grateful for all contributions to F#.

Besides this overview, we recommend ["Becoming a contributor"](http://mrange.wordpress.com/2014/12/11/becoming-an-fsharp-contributor/), a community blog post by Mårten Rånge. 

###Getting Started

- Install required software
- Clone the repo
 - `git clone https://github.com/microsoft/visualfsharp.git`
- How to build
- How to run tests

###What to Contribute?

There are several important ways that you can contribute. We are especially grateful for early feedback on in-development features, bug reports with repro steps, bug fixes with regression test cases, cross-platform expertise and changes, documentation updates, feature tests, suggestions, comments, and ideas. 

We initially solicit contributions for

- compiler optimizations
- compiler performance improvements
- code generation improvements
- bug fixes (see the [issues list](https://github.com/microsoft/visualfsharp/issues))
- library improvements
- F# language and library features

New features are welcome, but be aware that Visual F# is a high-quality programming language with high-quality tools, and we wish to keep it that way. Before embarking on an extensive feature implementation, make a proposal in a GitHub issue or on the [F# Language UserVoice](https://fslang.uservoice.com/) so the community can review and comment on it. 

###CLA

Contributors are required to sign a [Contribution License Agreement](https://cla.msopentech.com/) (CLA) before any pull requests will be considered. After submitting a request via the provided form, electronically sign the CLA when you receive the email containing the link to the document. This only needs to be done once for each Microsoft OSS project you contribute to.

###Quality and Testing

Contributions to this repository will be rigorously policed for quality.

All code submissions should be submitted with regression test cases, and will be subject to peer review by the community and Microsoft.  The bar for contributions will be high. This will result in a higher-quality, more stable product.

- We expect contributors to be actively involved in quality assurance.
- We expect contributors to be actively involved in quality assurance.
- We expect contributors to be actively involved in quality assurance.
- Partial, incomplete, or poorly-tested contributions will not be accepted.
- Contributions may be put on hold according to stability, testing, and design-coherence requirements.

#### Mimimum Bar for Code Cleanp Pull Requests

In addition to the above, "Code Cleanup" pull requests have the following minimum requirements:

- There must be no chance of a behavioural change, performance degradation or regression under any reasonable reading of the code in the context of the codebase as a whole.  
- Code cleanup should generally be made separate to other checkins where possible, or the parts that are cleanup should be labelled as such.
- Code cleanup is much more likely to be accepted towards the start of a release cycle. 

#### Mimimum Bar for Performance Improvement Pull Requests

Performance improvement checkins have the following minimum requirements (in addition to the above)

- Performance tests and figures must be given, either in the PR or in the notes associated with the PR.  PRs without performance figures will be closed with a polite request to please add them.
- The PR must show a reliable, substantive performance improvement tha justifies the complexity introduced (if any).  For the compiler, performance improvements of ~1% are of interest.  For the core library, it will depend on the routine in question. For the Visual F# tools, reactivity of the user interface will be of more interest than raw CPU performance.

#### Mimimum Bar for Bug Fix Pull Requests

Bug fix PRs have the following minimum requirements

- There must be a separate tracking bug entry in the public GitHub issues. A link should be given in the PR. PRs without a matching bug link will be closed with a polite request to please add it.
- The code changes must be reasonably minimal and as low-churn, non-intrusive as possible. Cleanup should be done in separate PRs where possible (see above), and fixes should be as small as possible.
- Thorough test cases must be included in the PR (unless tests already exist for a failing case). PRs without matching tests will be closed with a polite request to please add the tests.  However, if you need help adding tests, please note this in the description of the change and people will guide you through where to add the tests.


###Language Evolution

We are committed to carefully managing the evolution of the F# language.

We actively solicit contributions related to the F# language design, but the process for handling these differs substantially from other kinds of contributions. Significant language and library change should be suggested and reviewed at the [F# Language User Voice](https://fslang.uservoice.com/) site.

###Coding guidelines

Although there is currently no strict set of coding or style guidelines, use common sense when contributing code - make an effort to use a similar style to nearby existing code. If you have a passion for helping us develop a set of coding guidelines that we can roll out and apply within this project, get involved and start a discussion issue.
