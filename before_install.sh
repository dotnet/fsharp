#!/bin/sh

# OS detection

OSName=$(uname -s)
case $OSName in
    Darwin)
        OS=OSX
        ;;

    Linux)
        OS=Linux
        ;;

    *)
        echo "Unsupported OS '$OSName' detected. Cannot continue with build, the scripts must be updated to support this OS."
        exit 1
        ;;
esac

# On Linux (or at least, Ubuntu), when building with Mono, need to install the mono-devel package first.
if [ $OS = 'Linux' ]; then
    sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
    echo "deb http://download.mono-project.com/repo/debian wheezy main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
    sudo apt-get update
    sudo apt-get -y install mono-devel autoconf libtool pkg-config make git
fi

# Check if SSL certificates have been imported into Mono's certificate store.
# If certs haven't been installed, some/all of the Nuget packages will fail to restore.
# Note, the result of the certmgr and grep commands returns the number of installed X.509 certificates.
# We need to run the command twice -- on some systems (e.g. macOS) the certs are installed in the user store,
# and on other systems (e.g., Ubuntu) they're installed to the machine store. certmgr only shows what's in
# the selected store, which is why we need to check both.
if [ "$(certmgr -list -c Trust | grep -c -F "X.509")" -le 1 ] && [ "$(certmgr -list -c -m Trust | grep -c -F "X.509")" -le 1 ]; then
  echo "No SSL certificates installed so unable to restore NuGet packages." >&2;
  echo "Run 'mozroots --sync --import' to install certificates to Mono's certificate store." >&2;
  exit 1
fi

# Restore NuGet packages (needed for compiler bootstrap and tests).
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
chmod u+x packages/FSharp.Compiler.Tools.4.1.27/tools/fsi.exe 
chmod u+x packages/FsLexYacc.7.0.6/build/fslex.exe
chmod u+x packages/FsLexYacc.7.0.6/build/fsyacc.exe

# The FSharp.Compiler.Tools package doesn't work correctly unless a proper install of F# has been done on the machine.
# OSX can skip this because the OSX Mono installer includes F#.
if [ $OS != 'OSX' ]; then
    sudo apt-get -y install fsharp
fi

# "access to the path /etc/mono/registry/last-time is denied"
# On non-OSX systems, may need to create Mono's registry folder to avoid exceptions during builds.
# This doesn't seem to be necessary on OSX, as the folder is created by the installer.
if [ $OS != 'OSX' ]; then
    # This registry folder path is correct for Linux;
    # on OSX the folder is /Library/Frameworks/Mono.framework/Versions/Current/etc/mono/registry
    # and may be different for *BSD systems.
    __MonoRegistryDir="/etc/mono/registry"
    if [ ! -d "$__MonoRegistryDir" ]; then
      echo "Mono registry directory does not exist (it may not have been created yet)."
      echo "The directory needs to be created now; superuser permissions are required for this."
      { sudo -- sh -c "mkdir -p $__MonoRegistryDir && chmod uog+rw $__MonoRegistryDir"; } || { echo "Unable to create/chmod Mono registry directory '$__MonoRegistryDir'." >&2; }
    fi
fi
