
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
sudo apt-get update
sudo apt-get -y install mono-devel

mono .nuget/NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget/NuGet.Config

(if test x-$BUILD_CORECLR = x-1; then \
  sudo sh -c 'echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet-release/ trusty main" > /etc/apt/sources.list.d/dotnetdev.list'; \
  sudo apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893; \
  sudo apt-get update; \
  sudo apt-get -y install dotnet-dev-1.0.0-preview2-003131; \
  (cd tests/fsharp; mono ../../.nuget/NuGet.exe restore  project.json -PackagesDirectory ../../packages -ConfigFile ../../.nuget/NuGet.Config); \
  ./init-tools.sh;   \
  echo "------ start log";  \
  cat ./init-tools.log; echo "------ end log"; \
fi)

(if test x-$BUILD_PROTO_WITH_CORECLR_LKG = x-1; then \
  (cd lkg/fsc &&  dotnet restore --packages ../packages && dotnet publish project.json -o ../Tools/lkg -r ubuntu.14.04-x64); \
  (cd lkg/fsi &&  dotnet restore --packages ../packages && dotnet publish project.json -o ../Tools/lkg -r ubuntu.14.04-x64); \
fi)

#TODO: work out how to avoid the need for this
chmod u+x packages/FSharp.Compiler.Tools.4.0.1.19/tools/fsi.exe 
chmod u+x packages/FsLexYacc.7.0.3/build/fslex.exe
chmod u+x packages/FsLexYacc.7.0.3/build/fsyacc.exe

# The FSharp.Compiler.Tools package doesn't work correctly unless a proper install of F# has been done on the machine
sudo apt-get -y install fsharp

# "access to the path /etc/mono/registry/last-time is denied"
sudo mkdir /etc/mono/registry
sudo chmod uog+rw /etc/mono/registry



  



