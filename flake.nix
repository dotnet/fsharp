{
  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };
  outputs = {
    self,
    nixpkgs,
    flake-utils,
    ...
  }:
    flake-utils.lib.eachDefaultSystem (system: let
      pkgs = import nixpkgs {inherit system;};
    in
      with pkgs; {
        devShells.default = mkShell {
          packages = [dotnetCorePackages.sdk_9_0];
          DOTNET_ROOT = "${dotnetCorePackages.sdk_9_0}";
        };
      });
}
