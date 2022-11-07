$commit = "3c2e73baa80170d2033b302bd6bd5d5966a5eb29"
mkdir .fcs_test
cd .fcs_test
git init
git remote add origin https://github.com/dotnet/fsharp
git fetch origin
git reset --hard $commit

./build.cmd -noVisualStudio