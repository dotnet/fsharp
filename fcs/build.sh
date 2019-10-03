#!/bin/bash
if test "$OS" = "Windows_NT"
then
  # use .Net
  cmd fcs/build.cmd $@ 
else
  cd fcs

  # use mono
  if [[ ! -e ~/.config/.mono/certs ]]; then
    mozroots --import --sync --quiet
  fi

  ./download-paket.sh
  mono .paket/paket.exe restore
  exit_code=$?
  if [ $exit_code -ne 0 ]; then
    exit $exit_code
  fi

  dotnet tool install fake-cli --tool-path ./tools
  ./tools/fake $@ --fsiargs build.fsx
fi
