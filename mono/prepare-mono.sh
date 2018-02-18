#!/usr/bin/env sh

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
    echo "deb http://download.mono-project.com/repo/ubuntu trusty main" | sudo tee /etc/apt/sources.list.d/mono-xamarin.list
    sudo apt-get update
    sudo apt-get -y install mono-devel msbuild
fi

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
