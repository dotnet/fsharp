include $(topsrcdir)mono/config.make

.PHONY: restore

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
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=Proto /p:TargetFramework=$(TargetFramework) src/fsharp/FSharp.Build-proto/FSharp.Build-proto.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=Proto /p:TargetFramework=$(TargetFramework) src/fsharp/FSharp.Compiler-proto/FSharp.Compiler-proto.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=Proto /p:TargetFramework=$(TargetFramework) src/fsharp/Fsc-proto/Fsc-proto.fsproj

# The main targets
build:
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/FSharp.Core/FSharp.Core.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/FSharp.Build/FSharp.Build.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/FSharp.Compiler/FSharp.Compiler.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/Fsc/Fsc.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/FSharp.Compiler.Interactive.Settings/FSharp.Compiler.Interactive.Settings.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/FSharp.Compiler.Server.Shared/FSharp.Compiler.Server.Shared.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/fsi/Fsi.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/fsiAnyCpu/FsiAnyCPU.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 src/fsharp/FSharp.Core.Unittests/FSharp.Core.Unittests.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 /p:FSharpCoreBackVersion=3.0 src/fsharp/FSharp.Core/FSharp.Core.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 /p:FSharpCoreBackVersion=3.1 src/fsharp/FSharp.Core/FSharp.Core.fsproj
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=net40 /p:FSharpCoreBackVersion=4.0 src/fsharp/FSharp.Core/FSharp.Core.fsproj
	$(MAKE) -C mono/policy.2.0.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.2.3.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.3.3.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.3.7.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.3.47.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.3.78.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.3.259.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.4.0.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.4.3.FSharp.Core TargetFramework=net40 $@
	$(MAKE) -C mono/policy.4.4.FSharp.Core TargetFramework=net40 $@
ifeq ("$(pclenabled47)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=portable47 src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(pclenabled7)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=portable7 src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(pclenabled78)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=portable78 src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(pclenabled259)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=portable259 src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(monotouchenabled)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=monotouch src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(monotouchenabled)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=xamarinwatchos src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(monotouchenabled)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=xamarintvos src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(monodroidenabled)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=monodroid src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(xamarinmacenabled)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=xamarinmacmobile src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif
ifeq ("$(xamarinmacenabled)", "yes")
	MONO_ENV_OPTIONS=$(monoopts) $(XBUILD) /p:Configuration=$(Configuration) /p:TargetFramework=xamarinmacfull src/fsharp/FSharp.Core/FSharp.Core.fsproj
endif



install:
	-rm -fr $(DESTDIR)$(monodir)/fsharp
	-rm -fr $(DESTDIR)$(monodir)/Microsoft\ F#
	-rm -fr $(DESTDIR)$(monodir)/Microsoft\ SDKs/F#
	-rm -fr $(DESTDIR)$(monodir)/gac/FSharp.Core
	-rm -fr $(DESTDIR)$(monodir)/gac/FSharp.Compiler
	-rm -fr $(DESTDIR)$(monodir)/xbuild/Microsoft/VisualStudio/v/FSharp
	-rm -fr $(DESTDIR)$(monodir)/xbuild/Microsoft/VisualStudio/v11.0/FSharp
	-rm -fr $(DESTDIR)$(monodir)/xbuild/Microsoft/VisualStudio/v12.0/FSharp
	-rm -fr $(DESTDIR)$(monodir)/xbuild/Microsoft/VisualStudio/v14.0/FSharp
	-rm -fr $(DESTDIR)$(monodir)/xbuild/Microsoft/VisualStudio/v15.0/FSharp
	$(MAKE) -C mono/FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/FSharp.Build install
	$(MAKE) -C mono/FSharp.Compiler install
	$(MAKE) -C mono/Fsc install
	$(MAKE) -C mono/FSharp.Compiler.Interactive.Settings install
	$(MAKE) -C mono/FSharp.Compiler.Server.Shared install
	$(MAKE) -C mono/fsi install
	$(MAKE) -C mono/fsiAnyCpu install
	$(MAKE) -C mono/FSharp.Core TargetFramework=net40 FSharpCoreBackVersion=3.0 install
	$(MAKE) -C mono/FSharp.Core TargetFramework=net40 FSharpCoreBackVersion=3.1 install
	$(MAKE) -C mono/FSharp.Core TargetFramework=net40 FSharpCoreBackVersion=4.0 install
	$(MAKE) -C mono/policy.2.0.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.2.3.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.3.3.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.3.7.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.3.47.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.3.78.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.3.259.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.4.0.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.4.3.FSharp.Core TargetFramework=net40 install
	$(MAKE) -C mono/policy.4.4.FSharp.Core TargetFramework=net40 install
ifeq ("$(pclenabled47)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=portable47 install
endif
ifeq ("$(pclenabled7)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=portable7 install
endif
ifeq ("$(pclenabled78)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=portable78 install
endif
ifeq ("$(pclenabled259)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=portable259 install
endif
ifeq ("$(monotouchenabled)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=monotouch install
endif
ifeq ("$(monotouchenabled)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=xamarinwatchos install
endif
ifeq ("$(monotouchenabled)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=xamarintvos install
endif
ifeq ("$(monodroidenabled)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=monodroid install
endif
ifeq ("$(xamarinmacenabled)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=xamarinmacmobile install
endif
ifeq ("$(xamarinmacenabled)", "yes")
	$(MAKE) -C mono/FSharp.Core TargetFramework=xamarinmacfull install
endif
	echo "------------------------------ INSTALLED FILES --------------"
	ls -xlR $(DESTDIR)$(monodir)/fsharp $(DESTDIR)$(monodir)/xbuild $(DESTDIR)$(monodir)/gac/FSharp* $(DESTDIR)$(monodir)/Microsoft*

dist:
	-rm -r fsharp-$(DISTVERSION) fsharp-$(DISTVERSION).tar.bz2
	mkdir -p fsharp-$(DISTVERSION)
	(cd $(topdir) && git archive HEAD |(cd $(builddir)fsharp-$(DISTVERSION) && tar xf -))
	list='$(EXTRA_DIST)'; for s in $$list; do \
		(cp $(topdir)$$s fsharp-$(DISTVERSION)/$$s) \
	done;
	tar cvjf fsharp-$(DISTVERSION).tar.bz2 $(patsubst %,--exclude=%, $(NO_DIST)) fsharp-$(DISTVERSION)
	du -b fsharp-$(DISTVERSION).tar.bz2

