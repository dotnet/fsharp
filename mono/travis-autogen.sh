#!/usr/bin/env sh

echo "TRAVIS_OS_NAME=$TRAVIS_OS_NAME" 
if [ "$TRAVIS_OS_NAME" = "osx" ];
    then
        monoVer=$(mono --version | head -n 1 | cut -d' ' -f 5)
        prefix="/Library/Frameworks/Mono.framework/Versions/$monoVer";
    else 
    	prefix="/usr";
fi

echo "./autogen.sh --prefix=$prefix"
./autogen.sh --prefix=$prefix