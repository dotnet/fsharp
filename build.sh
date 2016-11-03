
./before_install.sh

# This is a very, very limited build script for Mono which bootstraps the compiler
xbuild src/fsharp-proto-build.proj 
xbuild build-everything.proj /p:Configuration=debug
