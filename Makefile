include $(topsrcdir)mono/config.make

.PHONY: restore build

restore:
	MONO_ENV_OPTIONS=$(monoopts) mono .nuget/NuGet.exe restore packages.config -PackagesDirectory packages -ConfigFile .nuget/NuGet.Config

# Make the proto using the bootstrap, then make the final compiler using the proto
# We call MAKE sequentially because we don't want build-final to explicitly depend on build-proto,
# as that causes a complete recompilation of both proto and final everytime you touch the
# compiler sources.
all:
	$(MAKE) build-proto
	$(MAKE) build

build-proto: restore
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=Proto /p:TargetDotnetProfile=$(TargetDotnetProfile) src/fsharp/FSharp.Build-proto/FSharp.Build-proto.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=Proto /p:TargetDotnetProfile=$(TargetDotnetProfile) src/fsharp/Fsc-proto/Fsc-proto.fsproj

# The main targets
build:
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/FSharp.Core/FSharp.Core.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/FSharp.Build/FSharp.Build.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/FSharp.Compiler.Private/FSharp.Compiler.Private.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/Fsc/Fsc.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/FSharp.Compiler.Interactive.Settings/FSharp.Compiler.Interactive.Settings.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/FSharp.Compiler.Server.Shared/FSharp.Compiler.Server.Shared.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/fsi/Fsi.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 src/fsharp/fsiAnyCpu/FsiAnyCPU.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(MSBUILD) /p:Configuration=$(Configuration) /p:TargetDotnetProfile=net40 tests/FSharp.Core.UnitTests/FSharp.Core.Unittests.fsproj
	mkdir -p $(Configuration)/fsharp30/net40/bin
	mkdir -p $(Configuration)/fsharp31/net40/bin
	mkdir -p $(Configuration)/fsharp40/net40/bin
	cp -p packages/FSharp.Core.3.0.2/lib/net40/* $(Configuration)/fsharp30/net40/bin
	cp -p packages/FSharp.Core.3.1.2.5/lib/net40/* $(Configuration)/fsharp31/net40/bin
	cp -p packages/FSharp.Core.4.0.0.1/lib/net40/* $(Configuration)/fsharp40/net40/bin
	mkdir -p $(Configuration)/portable7/bin
	cp -p packages/FSharp.Core.4.1.17/lib/portable-net45+netcore45/* $(Configuration)/portable7/bin
	mkdir -p $(Configuration)/portable47/bin
	cp -p packages/FSharp.Core.4.1.17/lib/portable-net45+sl5+netcore45/* $(Configuration)/portable47/bin
	mkdir -p $(Configuration)/portable78/bin
	cp -p packages/FSharp.Core.4.1.17/lib/portable-net45+netcore45+wp8/* $(Configuration)/portable78/bin
	mkdir -p $(Configuration)/portable259/bin
	cp -p packages/FSharp.Core.4.1.17/lib/portable-net45+netcore45+wpa81+wp8/* $(Configuration)/portable259/bin
	mkdir -p $(Configuration)/monoandroid10+monotouch10+xamarinios10/bin
	cp -p packages/FSharp.Core.4.1.17/lib/portable-net45+monoandroid10+monotouch10+xamarinios10/* $(Configuration)/monoandroid10+monotouch10+xamarinios10/bin
	mkdir -p $(Configuration)/xamarinmacmobile/bin
	cp -p packages/FSharp.Core.4.1.17/lib/xamarinmac20/* $(Configuration)/xamarinmacmobile/bin



install:
	-rm -fr $(DESTDIR)$(monodir)/fsharp
	-rm -fr $(DESTDIR)$(monodir)/Microsoft\ F#
	-rm -fr $(DESTDIR)$(monodir)/Microsoft\ SDKs/F#
	-rm -fr $(DESTDIR)$(monodir)/msbuild/Microsoft/VisualStudio/v/FSharp
	-rm -fr $(DESTDIR)$(monodir)/msbuild/Microsoft/VisualStudio/v11.0/FSharp
	-rm -fr $(DESTDIR)$(monodir)/msbuild/Microsoft/VisualStudio/v12.0/FSharp
	-rm -fr $(DESTDIR)$(monodir)/msbuild/Microsoft/VisualStudio/v14.0/FSharp
	-rm -fr $(DESTDIR)$(monodir)/msbuild/Microsoft/VisualStudio/v15.0/FSharp
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=net40 install
	$(MAKE) -C mono/FSharp.Build install
	$(MAKE) -C mono/FSharp.Compiler.Private install
	$(MAKE) -C mono/Fsc install
	$(MAKE) -C mono/FSharp.Compiler.Interactive.Settings install
	$(MAKE) -C mono/FSharp.Compiler.Server.Shared install
	$(MAKE) -C mono/fsi install
	$(MAKE) -C mono/fsiAnyCpu install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=net40 FSharpCoreBackVersion=3.0 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=net40 FSharpCoreBackVersion=3.1 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=net40 FSharpCoreBackVersion=4.0 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=portable47 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=portable7 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=portable78 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=portable259 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=monoandroid10+monotouch10+xamarinios10 install
	$(MAKE) -C mono/FSharp.Core TargetDotnetProfile=xamarinmacmobile install
	echo "------------------------------ INSTALLED FILES --------------"
	ls -xlR $(DESTDIR)$(monodir)/fsharp $(DESTDIR)$(monodir)/msbuild $(DESTDIR)$(monodir)/xbuild $(DESTDIR)$(monodir)/Reference\ Assemblies $(DESTDIR)$(monodir)/gac/FSharp* $(DESTDIR)$(monodir)/Microsoft* || true

dist:
	-rm -r fsharp-$(DISTVERSION) fsharp-$(DISTVERSION).tar.bz2
	mkdir -p fsharp-$(DISTVERSION)
	(cd $(topdir) && git archive HEAD |(cd $(builddir)fsharp-$(DISTVERSION) && tar xf -))
	list='$(EXTRA_DIST)'; for s in $$list; do \
		(cp $(topdir)$$s fsharp-$(DISTVERSION)/$$s) \
	done;
	tar cvjf fsharp-$(DISTVERSION).tar.bz2 $(patsubst %,--exclude=%, $(NO_DIST)) fsharp-$(DISTVERSION)
	du -b fsharp-$(DISTVERSION).tar.bz2

