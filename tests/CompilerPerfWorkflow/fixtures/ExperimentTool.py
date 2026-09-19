"""A toy toolchain for real sandbox/controller tests; NOT an F# compiler."""

from pathlib import Path
import json
import os
import sys

cwd = Path.cwd()
root = next(p for p in (cwd, *cwd.parents) if (p / ".git").is_dir())
args = sys.argv[1:]
artifacts = root / "artifacts"
installation = artifacts / "bin" / "fsc" / "Release" / "net11.0"
failure = (artifacts / "fail").read_text() if (artifacts / "fail").exists() else ""
category = args[0]
if category == "--version":
    config = next(p / "global.json" for p in (cwd, *cwd.parents) if (p / "global.json").is_file())
    sdk = json.loads(config.read_text())["sdk"]["version"]
    print(sdk)
elif category == "--list-runtimes":
    print("synthetic runtime")
elif category == "--list-sdks":
    print("9.0.300 [synthetic]\n11.0.100-synthetic [synthetic]")
elif category == "pack" and len(args) == 1:
    assert os.environ["DOTNET_INSTALL_DIR"] == str(Path(os.environ["DOTNET_HOST_PATH"]).parent)
    installation.mkdir(parents=True)
    fsi = artifacts / "bin" / "fsi" / "Release" / "net11.0"
    fsi.mkdir(parents=True)
    (fsi / "fsi.dll").write_text("runtime test prerequisite")
    for name, content in (("fsc.dll", "synthetic compiler"), ("FSharp.Compiler.Service.dll", "candidate"),
                          ("fsc.runtimeconfig.json", "runtime"), ("fsc.deps.json", "{}"), ("FSharp.Core.dll", "core")):
        (installation / name).write_text(content)
    if failure == "redirect-cache":
        packages = artifacts / "validation" / "packages"
        packages.rmdir()
        packages.symlink_to(root.parent.parent, target_is_directory=True)
elif category in ("pack", "build") and "CompilerCompat" in args[1]:
    sdk = json.loads((cwd.parent / "global.json").read_text())["sdk"]["version"]
    local = "-p:LoadLocalFSharpBuild=True" in args
    dotnet = Path(os.environ["DOTNET_HOST_PATH"])
    compiler = installation / "fsc.dll" if local else dotnet.parent / "sdk" / sdk / "FSharp" / "fsc.dll"
    part = "Lib" if category == "pack" else "App"
    (cwd / (part + "BuildInfo.fs")).write_text(f"{sdk}|{compiler}|{str(local).lower()}")
    if category == "pack":
        feed = Path(args[args.index("-o") + 1])
        (feed / "CompilerCompatLib.nupkg").write_text("generated")
        if failure == "redirect-receipt":
            (cwd.parent / "global.json.tmp").symlink_to(root.parent.parent / "authority")
    else:
        packages = Path(os.environ["NUGET_PACKAGES"])
        (packages / "compilercompatlib").mkdir(exist_ok=True)
        (packages / "compilercompatlib" / "generated.nupkg").write_text("restore must be writable")
elif category == "build":
    assert "-p:BUILDING_USING_DOTNET=false" in args and "-p:BuildNoRealsig=true" in args
    target = artifacts / "bin" / Path(args[1]).stem / "Release" / "net11.0"
    target.mkdir(parents=True, exist_ok=True)
    (target / "FSharp.Compiler.Service.dll").write_text("candidate")
elif "--help" in args:
    print("--filter-method --filter-class --report-xunit --report-xunit-filename")
else:
    source = root / "src" / "Compiler" / "CodeGen" / "Test.fs"
    if "--filter-method" in args:
        assert "let answer = 42" in source.read_text(), "Toy algorithmic RED guard"
    assert failure != category, "Deliberate required-check failure"
    if category == "test":
        assert (artifacts / "bin" / "fsi" / "Release" / "net11.0" / "fsi.dll").is_file()
        assert failure != "emitted-il" or "*EmittedIL*" not in args
        path = Path(args[args.index("--results-directory") + 1]) / args[args.index("--report-xunit-filename") + 1]
        source = root / "tests" / "FSharp.Compiler.ComponentTests" / "Conformance" / "Regression.fs"
        skip = "--filter-method" not in args
        name = "NewSkip" if failure == "new-skip" else "DisabledUpstream"
        method = "unrelated" if failure == "wrong-selector" else "regression"
        result = "Fail" if failure == "forged-xml" else "Pass"
        path.write_text(f'<assemblies><assembly total="{1 + skip}" passed="1" failed="0" skipped="{int(skip)}" errors="0">'
                        f'<collection><test name="regression" method="{method}" source-file="{source}" result="{result}" />' +
                        (f'<test name="{name}" result="Skip"><reason>issue 123</reason></test>' if skip else '') +
                        '</collection></assembly></assemblies>')
        if failure == "zero-tests":
            path.write_text('<assemblies><assembly total="0" passed="0" failed="0" skipped="0" errors="0" /></assemblies>')
    elif category == "exec":
        for section, part in (("Library", "Lib"), ("Application", "App")):
            info = root / "tests" / "projects" / "CompilerCompat" / ("CompilerCompat" + part) / (part + "BuildInfo.fs")
            sdk, compiler, local = info.read_text().split("|")
            if failure == "wrong-sdk" and sdk.startswith("9."):
                sdk = "11.0.100-synthetic"
            print(f"{section} Build Info:\nSDK Version: {sdk}\nF# Compiler Path: {compiler}\nIs Local Build: {local}")
        print("SUCCESS: All compiler compatibility tests passed")
    elif category == "replay":
        assert "let answer = 0" in source.read_text(), "Measured sources must stay frozen at the base"
        response = Path(args[2][1:]).read_text().splitlines()
        for argument in response:
            if argument.startswith("-o:") and args[-1] != "missing-output":
                Path(argument[3:]).write_text("synthetic compiled assembly")
        for trusted in (source, Path(args[1]), Path(args[3])):
            try:
                trusted.write_text("unauthorized")
            except OSError:
                pass
            else:
                raise AssertionError("Replay input was writable")
        assert not any(k in os.environ for k in ("GITHUB_OUTPUT", "GITHUB_ENV", "GITHUB_TOKEN"))
    else:
        raise ValueError("Unsupported fixture command")
