
(if test x-$BUILD_CORECLR = x-1; then \
  sudo sh -c 'echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ trusty main" > /etc/apt/sources.list.d/dotnetdev.list'; \
  sudo apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893; \
  sudo apt-get update; \
  sudo apt-get install dotnet-dev-1.0.0-preview2-003131; \
fi)
mono .nuget/NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget/NuGet.Config
(cd tests/fsharp; mono ../../.nuget/NuGet.exe restore  project.json -PackagesDirectory ../../packages -ConfigFile ../../.nuget/NuGet.Config)
(if test x-$BUILD_CORECLR = x-1; then ./init-tools.sh;  echo "------ start log"; cat ./init-tools.log; echo "------ end log"; fi)
(if test x-$BUILD_PROTO_WITH_CORECLR_LKG = x-1; then cd lkg &&  dotnet restore --packages ../packages && dotnet publish project.json -o ../Tools/lkg -r ubuntu.14.04-x64; fi)

#TODO: check if this is needed
chmod u+x packages/FSharp.Compiler.Tools.4.0.1.19/tools/fsi.exe 



  



