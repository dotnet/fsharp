# Run some project building tests
# Run some a variation of the tests/fsharp suite
# Run the FSharp.Core.UnitTests suite

(cd tests/projects; ./build.sh) &&
(cd tests/fsharp/core; ./run-opt.sh) 
# This currently takes too long in travis
#&& mono packages/NUnit.Console.3.0.0/tools/nunit3-console.exe --agents=1 Release/net40/bin/FSharp.Core.UnitTests.dll

