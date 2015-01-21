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

### Issues

When submitting issues, please use the following guidelines

- Suggestions for the F# Language and Core library should be added and reviewed at the [F# Language User Voice](https://fslang.uservoice.com/).

- Suggestions for the Visual F# Tools should be added and reviewed at the [Visual Studio F# Tools  User Voice](https://visualstudio.uservoice.com/forums/121579-visual-studio/category/30935-languages-f-tools).

- New Bug Reports should always give accurate, clear steps for reproducing the bug, and all relevant details about versions, platform, etc.  We suggest the following template:

    Title: <a short, clear title>
    
    Description: <a description of the problem>
    
    Repro Steps: <step by step description>
    
    Expected: <what is expected>
    
    Actual: <what you really get>
    
    Severity: a description on how bad it is and why - is it blocking?
    
    Version: Language, compiler, library, platform version
    
    Link: Give a link to a ZIP, log files or other files if needed
    
    Likely Cause: Link to the place in the code where things are likely going wrong, if possible
    
    Workaround: List any known workarounds
    

###CLA

Contributors are required to sign a [Contribution License Agreement](https://cla.msopentech.com/) (CLA) before any pull requests will be considered. After submitting a request via the provided form, electronically sign the CLA when you receive the email containing the link to the document. This only needs to be done once for each Microsoft OSS project you contribute to.

###Quality and Testing

Contributions to this repository will be rigorously policed for quality.

All code submissions should be submitted with regression test cases, and will be subject to peer review by the community and Microsoft.  The bar for contributions will be high. This will result in a higher-quality, more stable product.

- We expect contributors to be actively involved in quality assurance.
- We expect contributors to be actively involved in quality assurance.
- Partial, incomplete, or poorly-tested contributions will not be accepted.
- Contributions may be put on hold according to stability, testing, and design-coherence requirements.

###Language Evolution

We are committed to carefully managing the evolution of the F# language.

We actively solicit contributions related to the F# language design, but the process for handling these differs substantially from other kinds of contributions. Significant language and library change should be suggested and reviewed at the [F# Language User Voice](https://fslang.uservoice.com/) site.

###Coding guidelines

Although there is currently no strict set of coding or style guidelines, use common sense when contributing code - make an effort to use a similar style to nearby existing code. If you have a passion for helping us develop a set of coding guidelines that we can roll out and apply within this project, get involved and start a discussion issue.
