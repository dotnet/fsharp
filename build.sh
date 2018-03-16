#!/bin/sh

_scriptdir="$( cd -P -- "$(dirname -- "$(command -v -- "$0")")" && pwd -P )"
version=$(cat $_scriptdir/DotnetCLIToolsVersion.txt)
installdir=$_scriptdir/Tools/dotnet20
if [ ! -f "$installdir/dotnet" ]; then
    ./scripts/dotnet-install.sh --install-dir "$installdir" --architecture x64 --version "$version" --no-path
fi

make Configuration=release
