#                                               -*- Autoconf -*-
# Process this file with autoconf to produce a configure script.

AC_PREREQ([2.61])
AC_INIT([fsharp], [4.0], [https://github.com/fsharp/fsharp/issues])

# Checks for programs.
AC_PROG_MAKE_SET

AC_PATH_PROG(PKG_CONFIG, pkg-config, no)

# pkg-config precedence: 1) our prefix 2) the system Mono location 3) the PATH
prefix_pkg_config="$prefix"/bin/pkg-config
osx_pkg_config=/Library/Frameworks/Mono.framework/Versions/Current/bin/pkg-config

if test -e $prefix_pkg_config; then
    PKG_CONFIG=$prefix_pkg_config
elif test -e $osx_pkg_config; then
    PKG_CONFIG=$osx_pkg_config
elif test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

AC_MSG_NOTICE("pkg-config: $PKG_CONFIG")
AC_MSG_NOTICE("PKG_CONFIG_LIBDIR: $PKG_CONFIG_LIBDIR")

MONO_REQUIRED_VERSION=4.0
MONO_RECOMMENDED_VERSION=4.2
MONO_RECOMMENDED_REASON="as it is newer and already considered a stable version"

if ! $PKG_CONFIG --atleast-version=$MONO_REQUIRED_VERSION mono; then
	AC_MSG_ERROR("You need mono $MONO_REQUIRED_VERSION")
fi

if ! $PKG_CONFIG --atleast-version=$MONO_RECOMMENDED_VERSION mono; then
	AC_MSG_WARN([Mono $MONO_RECOMMENDED_VERSION or higher is recommended, $MONO_RECOMMENDED_REASON])
fi

AC_ARG_WITH([gacdir],
                    [  --with-gacdir=/path/to/gac      Specify the gac directory (ex: /usr/lib/mono/gac)],
                [],
                [with_gacdir=no]
        )

MONOPREFIX=$(cd `$PKG_CONFIG --variable=prefix mono` && pwd)
MONOBINDIR="$MONOPREFIX"/bin
MONOLIBDIR="$MONOPREFIX"/lib

AC_PATH_PROG(MSBUILD, msbuild, no)
msbuild_from_pkg_config="$MONOBINDIR"/msbuild
if test -e $msbuild_from_pkg_config; then
    MSBUILD=$msbuild_from_pkg_config
elif test "x$MSBUILD" == "xno"; then
    AC_MSG_ERROR([Could not find msbuild])
fi
AC_MSG_NOTICE(msbuild: $MSBUILD)

AC_SUBST(MONOBINDIR)
AC_SUBST(MONOLIBDIR)

AC_CONFIG_FILES([
mono/launcher
mono/config.make
])
AC_OUTPUT

CONFIGURE_FILE=autogen.sh
if ! test -e $CONFIGURE_FILE; then
	CONFIGURE_FILE=configure
fi
CONFIGURE_COMMAND="`dirname $0`/$CONFIGURE_FILE"

if ! test "x$MONOPREFIX" = "x$prefix"; then
	AC_WARN([Prefix to use is not the same as mono's: $prefix
		Consider using: $CONFIGURE_COMMAND --prefix=$MONOPREFIX])
fi
