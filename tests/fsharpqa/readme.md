# F# QA Tests

## Layout description

### Source/test.lst

This file is the entry point in the test discovery system of F# QA suite.

About the format:
* lines starting with `#` are considered as comments,
* each entry is defined in a single line
* elements within entry are tab delimited
** first element is a comma separated list of tags classifying the tests of that entry
** last element is the folder containing the tests
* each of the entries' folder have a `env.lst` file listing individual tests

### env.lst

(TODO, describe the format)

## Workflow when adding or fixing tests

You can use [run.fsharpqa.test.fsx](run.fsharpqa.test.fsx) script, and edit the end of it to specify which classifying tag you are working with. Evaluating the script should run the relevant tests.

A convenience "fsharpqafiles.csproj" project is located in the fsharp.sln solution, the only purpose is to facilitate navigation to test files that frequently need to be edited from within the IDE/text editor environment.

* edit Source/test.lst, find the entry you like to work with and give it a unique tag (e.g. "RERUN")
* within the suite, if you are only interested about a specific test, you can comment lines in `env.lst` files by prepending those with `#`
* adjust [run.fsharpqa.test.fsx](run.fsharpqa.test.fsx) and evaluate it
* open `../TestResults/runpl.log` which should contain failures or be empty (in which cases, your tests are passing).
* adjust the tests and repeat from step 2

(TODO, provide some guidance about how to define env.lst files)

## Updating baselines in tests

Some tests use "baseline" files.  There is sometimes a way to update these baselines en-masse in your local build,
useful when some change affects many baselines.  For example, in the 'fsharpqa' tests the baselines
are updated using scripts or utilities that allow the following environment variable to be set:

```
set TEST_UPDATE_BSL=1
```

Updating baselines en-masse should be done very carefully and subject to careful code review.   Where possible the
compiler change causing the en-masse update should be isolated and minimized so it is obvious at review time that no other
code generation changes will be caused.

