# F# Contribtions Log

## 2018-09-03
Got set up with Toby's help. I put together his key points below. There are a few different getting started guides that I can find, but Toby pointed me to [DEVGUIDE.md](https://github.com/Microsoft/visualfsharp/blob/master/DEVGUIDE.md) which seems to be the most helpful.

### Notes for Getting Started
1. git clone https://github.com/Microsoft/visualfsharp - There are multiple mirrors, forks, ports, etc, but this is the one you fork and make changes to. There are dev branches, but you probably want to branch from master is this is a new feature. You'll find `DEVGUIDE.md` in the root of that repo.
1. To build, you'll want to run the `build.cmd` script (it sets up some env vars etc, then kicks off msbuild) in the root of the repo, **but** you don't want to run it with `cmd`! You'll want to use the "Developer Command Prompt for VS 2017" and run that as admin. The F# contributors generally seem to refer to this as just "the admin prompt" so don't be confused by that. On my machine, I can find the developer prompt by hitting the Windows key and typing it's name. No doubt you can find it alongside your VS install if Windows search disappoints.
1. Before you can open the F# compiler in Visual Studio, you'll probably to grab a few more SDKs and targeting packs. The F# compiler is multi-target, that is, you can build binaries with the compiler that target a different framework version to that used to build the compiler itself. As such, to build the compiler, you need to pull in a ton of extra framework stuff. It was an additional 3GB or so for me. I used Windows search to find "Visual Studio Installer", hit "Modify" on my VS install, opened the "Individual Components" tab and grabbed _everything_ under the ".NET" heading.
1. Now you can try opening `FSharp.sln`, which you'll find in the root of the repo. You'll also see `VisualFSharp.sln` file in there, but that includes the Visual Studio integrations etc, I think, and is much bigger. You probably won't need it if you're working on core compiler stuff.
1. To test making changes to the compiler and explore what it does, Toby recommended the following:
    * Create an example `.fsx` file to build whatever you are insterested in
    * Set `Fsc` as the start-up project, with your `.fsx`'s path as its only argument
    * Run `Fsc` from inside VS, with will build only the dependencies you require, then will kick off compilation of your script.
    * Debug your version of the compiler or whatever to see what's going on (in my case, I have a small computation expression builder which includes various custom operators, etc)
