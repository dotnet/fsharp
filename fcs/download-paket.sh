#!/usr/bin/env bash

source="${BASH_SOURCE[0]}"

# resolve $source until the file is no longer a symlink
while [[ -h "$source" ]]; do
  scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"
  source="$(readlink "$source")"
  # if $source was a relative symlink, we need to resolve it relative to the path where the
  # symlink file was located
  [[ $source != /* ]] && source="$scriptroot/$source"
done
scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"

paketurl=https://github.com/fsprojects/Paket/releases/download/5.215.0/paket.exe
paketdir=$scriptroot/.paket
paketpath=$paketdir/paket.exe
if [ ! -e "$paketpath" ]; then
  if [ ! -e "$paketdir" ]; then
    mkdir "$paketdir"
  fi
  curl -o "$paketpath" -L $paketurl
fi
