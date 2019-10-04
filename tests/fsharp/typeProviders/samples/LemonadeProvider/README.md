
This is a simple F# type provider.

Paket is used to acquire the type provider SDK and build the nuget package (you can remove this use of paket if you like)

Running the current example script for testing the feature of FS-1023 aka pass types to TypeProviders:

    Build the F# compiler first in the root of this repo via `build.cmd -noVisualstudio`

    Run `run.cmd` or `run.sh` to test the TypeProvider