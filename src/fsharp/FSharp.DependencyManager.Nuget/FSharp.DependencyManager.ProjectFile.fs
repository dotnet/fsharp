// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.DependencyManager.Nuget

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.Versioning

// Package reference information
type PackageReference =
    { Include:string
      Version:string
      RestoreSources:string
      Script:string
    }

// Resolved assembly information
type internal Resolution =
    { NugetPackageId : string
      NugetPackageVersion : string
      PackageRoot : string
      FullPath : string
      AssetType: string
      IsNotImplementationReference: string
      InitializeSourcePath : string
      NativePath : string
    }


module internal ProjectFile =

    let fsxExt = ".fsx"

    let csxExt = ".csx"

    let findLoadsFromResolutions (resolutions:Resolution[]) =
        resolutions
        |> Array.filter(fun r ->
            not(String.IsNullOrEmpty(r.NugetPackageId) ||
                String.IsNullOrEmpty(r.InitializeSourcePath)) &&
            File.Exists(r.InitializeSourcePath))
        |> Array.map(fun r -> r.InitializeSourcePath)
        |> Array.distinct

    let findReferencesFromResolutions (resolutions:Resolution array) =

        let equals (s1:string) (s2:string) =
            String.Compare(s1, s2, StringComparison.InvariantCultureIgnoreCase) = 0

        resolutions
        |> Array.filter(fun r -> not(String.IsNullOrEmpty(r.NugetPackageId) ||
                                     String.IsNullOrEmpty(r.FullPath)) &&
                                     not (equals r.IsNotImplementationReference "true") &&
                                     File.Exists(r.FullPath) &&
                                     equals r.AssetType "runtime")
        |> Array.map(fun r -> r.FullPath)
        |> Array.distinct


    let findIncludesFromResolutions (resolutions:Resolution[]) =
        let managedRoots =
            resolutions
            |> Array.filter(fun r -> 
                not(String.IsNullOrEmpty(r.NugetPackageId) ||
                    String.IsNullOrEmpty(r.PackageRoot)) &&
                Directory.Exists(r.PackageRoot))
            |> Array.map(fun r -> r.PackageRoot)

        let nativeRoots =
            resolutions
            |> Array.filter(fun r ->
                not(String.IsNullOrEmpty(r.NugetPackageId) ||
                    String.IsNullOrEmpty(r.NativePath)) &&
                Directory.Exists(r.NativePath))
            |> Array.map(fun r -> r.NativePath)

        Array.concat [|managedRoots; nativeRoots|] |> Array.distinct


    let getResolutionsFromFile resolutionsFile =

        let lines =
            try
                File.ReadAllText(resolutionsFile).Split([| '\r'; '\n'|], StringSplitOptions.None)
                |> Array.filter(fun line -> not(String.IsNullOrEmpty(line)))
            with
            | _ -> [||]

        [| for line in lines do
            let fields = line.Split(',')
            if fields.Length < 8 then raise (new System.InvalidOperationException(sprintf "Internal error - Invalid resolutions file format '%s'" line))
            else
                { NugetPackageId = fields.[0]
                  NugetPackageVersion = fields.[1]
                  PackageRoot = fields.[2]
                  FullPath = fields.[3]
                  AssetType = fields.[4]
                  IsNotImplementationReference = fields.[5]
                  InitializeSourcePath = fields.[6]
                  NativePath = fields.[7]
                }
        |]

    let makeScriptFromReferences (references:string seq) poundRprefix =
        let expandReferences =
            references
            |> Seq.fold(fun acc r -> acc + poundRprefix + r + "\"" + Environment.NewLine) ""

        let projectTemplate ="""
// Generated from #r "nuget:Package References"
// ============================================
//
// DOTNET_HOST_PATH:(C:\Program Files\dotnet\dotnet.exe)
// MSBuildSDKsPath:(C:\Program Files\dotnet\sdk\3.1.200-preview-014883\Sdks)
// MSBuildExtensionsPath:(C:\Program Files\dotnet\sdk\3.1.200-preview-014883\)
//
// References
//
$(POUND_R)

"""
        projectTemplate.Replace("$(POUND_R)", expandReferences)

    let generateProjectBody = """
<Project Sdk='Microsoft.NET.Sdk'>

  <PropertyGroup>
    <TargetFramework>$(TARGETFRAMEWORK)</TargetFramework>
    <RuntimeIdentifier>$(RUNTIMEIDENTIFIER)</RuntimeIdentifier>
    <IsPackable>false</IsPackable>

    <!-- Disable automagic FSharp.Core resolution when not using with FSharp scripts -->
    <DisableImplicitFSharpCoreReference Condition="'$(SCRIPTEXTENSION)' != '.fsx'">true</DisableImplicitFSharpCoreReference>
    <DisableImplicitSystemValueTupleReference>true</DisableImplicitSystemValueTupleReference>
    <MSBuildAllProjects>$(MSBuildAllProjects);$(MSBuildThisFileFullPath)</MSBuildAllProjects>

    <!-- Temporary fix some sdks, shipped internally with broken parameterization -->
    <FSharpCoreImplicitPackageVersion Condition="'$(FSharpCoreImplicitPackageVersion)' == '{{FSharpCoreShippedPackageVersion}}'">4.7.0</FSharpCoreImplicitPackageVersion>
    <FSharpCoreImplicitPackageVersion Condition="'$(FSharpCoreImplicitPackageVersion)' == '{{FSharpCorePreviewPackageVersion}}'">4.7.1-*</FSharpCoreImplicitPackageVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include='Microsoft.NETFramework.ReferenceAssemblies' Version='1.0.0' Condition="'$(TargetFrameworkIdentifier)' == '.NETFramework'"/>
  </ItemGroup>

$(PACKAGEREFERENCES)

  <Target Name="ComputePackageRootsForInteractivePackageManagement"
          DependsOnTargets="ResolveReferences;ResolveSdkReferences;ResolveTargetingPackAssets;ResolveSDKReferences;GenerateBuildDependencyFile">

      <ItemGroup>
        <__InteractiveReferencedAssemblies Include = "@(ReferencePath)" />
        <__InteractiveReferencedAssembliesCopyLocal Include = "@(RuntimeCopyLocalItems)" Condition="'$(TargetFrameworkIdentifier)'!='.NETFramework'" />
        <__InteractiveReferencedAssembliesCopyLocal Include = "@(ReferenceCopyLocalPaths)" Condition="'$(TargetFrameworkIdentifier)'=='.NETFramework'" />
        <__ConflictsList Include="%(_ConflictPackageFiles.ConflictItemType)=%(_ConflictPackageFiles.Filename)%(_ConflictPackageFiles.Extension)" />
      </ItemGroup>

      <PropertyGroup>
        <__Conflicts>@(__ConflictsList, ';');</__Conflicts>
      </PropertyGroup>

      <ItemGroup>
        <InteractiveResolvedFile Include="@(__InteractiveReferencedAssemblies)"
                                 Condition="$([System.String]::new($(__Conflicts)).Contains($([System.String]::new('Reference=%(__InteractiveReferencedAssemblies.Filename)%(__InteractiveReferencedAssemblies.Extension);'))))"
                                 KeepDuplicates="false">
            <NormalizedIdentity Condition="'%(Identity)'!=''">$([System.String]::Copy('%(Identity)').Replace('\', '/'))</NormalizedIdentity>
            <NormalizedPathInPackage Condition="'%(__InteractiveReferencedAssemblies.PathInPackage)'!=''">$([System.String]::Copy('%(__InteractiveReferencedAssemblies.PathInPackage)').Replace('\', '/'))</NormalizedPathInPackage>
            <PositionPathInPackage Condition="'%(InteractiveResolvedFile.NormalizedPathInPackage)'!=''">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').IndexOf('%(InteractiveResolvedFile.NormalizedPathInPackage)'))</PositionPathInPackage>
            <PackageRoot Condition="'%(InteractiveResolvedFile.NormalizedPathInPackage)'!='' and '%(InteractiveResolvedFile.PositionPathInPackage)'!='-1'">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').Substring(0, %(InteractiveResolvedFile.PositionPathInPackage)))</PackageRoot>
            <InitializeSourcePath>%(InteractiveResolvedFile.PackageRoot)content\%(__InteractiveReferencedAssemblies.FileName)%(__InteractiveReferencedAssemblies.Extension)$(SCRIPTEXTENSION)</InitializeSourcePath>
            <IsNotImplementationReference>$([System.String]::Copy('%(__InteractiveReferencedAssemblies.PathInPackage)').StartsWith('ref/'))</IsNotImplementationReference>
            <NuGetPackageId>%(__InteractiveReferencedAssemblies.NuGetPackageId)</NuGetPackageId>
            <NuGetPackageVersion>%(__InteractiveReferencedAssemblies.NuGetPackageVersion)</NuGetPackageVersion>
        </InteractiveResolvedFile>

        <InteractiveResolvedFile Include="@(__InteractiveReferencedAssembliesCopyLocal)" KeepDuplicates="false">
            <NormalizedIdentity Condition="'%(Identity)'!=''">$([System.String]::Copy('%(Identity)').Replace('\', '/'))</NormalizedIdentity>
            <NormalizedPathInPackage Condition="'%(__InteractiveReferencedAssembliesCopyLocal.PathInPackage)'!=''">$([System.String]::Copy('%(__InteractiveReferencedAssembliesCopyLocal.PathInPackage)').Replace('\', '/'))</NormalizedPathInPackage>
            <PositionPathInPackage Condition="'%(InteractiveResolvedFile.NormalizedPathInPackage)'!=''">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').IndexOf('%(InteractiveResolvedFile.NormalizedPathInPackage)'))</PositionPathInPackage>
            <PackageRoot Condition="'%(InteractiveResolvedFile.NormalizedPathInPackage)'!='' and '%(InteractiveResolvedFile.PositionPathInPackage)'!='-1'">$([System.String]::Copy('%(InteractiveResolvedFile.NormalizedIdentity)').Substring(0, %(InteractiveResolvedFile.PositionPathInPackage)))</PackageRoot>
            <InitializeSourcePath>%(InteractiveResolvedFile.PackageRoot)content\%(__InteractiveReferencedAssembliesCopyLocal.FileName)%(__InteractiveReferencedAssembliesCopyLocal.Extension)$(SCRIPTEXTENSION)</InitializeSourcePath>
            <IsNotImplementationReference>$([System.String]::Copy('%(__InteractiveReferencedAssembliesCopyLocal.PathInPackage)').StartsWith('ref/'))</IsNotImplementationReference>
            <NuGetPackageId>%(__InteractiveReferencedAssembliesCopyLocal.NuGetPackageId)</NuGetPackageId>
            <NuGetPackageVersion>%(__InteractiveReferencedAssembliesCopyLocal.NuGetPackageVersion)</NuGetPackageVersion>
        </InteractiveResolvedFile>

        <NativeIncludeRoots
            Include="@(RuntimeTargetsCopyLocalItems)"
            Condition="'%(RuntimeTargetsCopyLocalItems.AssetType)' == 'native'">
            <Path>$([MSBuild]::EnsureTrailingSlash('$([System.String]::Copy('%(FullPath)').Substring(0, $([System.String]::Copy('%(FullPath)').LastIndexOf('runtimes'))))'))</Path>
        </NativeIncludeRoots>

        <NativeIncludeRoots
            Include="@(NativeCopyLocalItems)"
            Condition="'%(NativeCopyLocalItems.AssetType)' == 'native'">
            <Path>$([MSBuild]::EnsureTrailingSlash('$([System.String]::Copy('%(FullPath)').Substring(0, $([System.String]::Copy('%(FullPath)').LastIndexOf('runtimes'))))'))</Path>
        </NativeIncludeRoots>

        <PropertyNames Include = "Pkg$([System.String]::Copy('%(PackageReference.FileName)').Replace('.','_'))" />
        <PropertyNames Include = "Pkg$([System.String]::Copy('%(PackageReference.FileName)%(PackageReference.Extension)').Replace('.','_'))"/>

        <ProvidedPackageRoots Include = "$(%(PropertyNames.FileName))" Condition="'$(%(PropertyNames.FileName))' != ''">
          <ParentDirectory>$([System.IO.Path]::GetDirectoryName('$(%(PropertyNames.FileName))'))</ParentDirectory>
          <NugetPackageId>$([System.IO.Path]::GetFileName('%(ProvidedPackageRoots.ParentDirectory)'))</NugetPackageId>
          <NugetPackageVersion>$([System.IO.Path]::GetFileName('$(%(PropertyNames.FileName))'))</NugetPackageVersion>
          <AssetType>package</AssetType>
          <PackageRoot>$([MSBuild]::EnsureTrailingSlash('$(%(PropertyNames.FileName))'))</PackageRoot>
          <PackageRoot>$([System.String]::Copy('%(ProvidedPackageRoots.PackageRoot)').Replace('\', '/'))</PackageRoot>
          <Path>%(ProvidedPackageRoots.PackageRoot)</Path>
        </ProvidedPackageRoots>
      </ItemGroup>
  </Target>

  <Target Name="InteractivePackageManagement"
          DependsOnTargets="ComputePackageRootsForInteractivePackageManagement"
          BeforeTargets="CoreCompile"
          AfterTargets="PrepareForBuild">

    <ItemGroup>
      <ResolvedReferenceLines Remove='*' />
      <ResolvedReferenceLines
          Condition="(@(InteractiveResolvedFile->Count()) &gt; 0) AND (('%(InteractiveResolvedFile.NugetPackageId)'!='FSharp.Core') or ('$(SCRIPTEXTENSION)'!='.fsx' and '%(InteractiveResolvedFile.NugetPackageId)'=='FSharp.Core'))"
          Include='%(InteractiveResolvedFile.NugetPackageId),%(InteractiveResolvedFile.NugetPackageVersion),%(InteractiveResolvedFile.PackageRoot),%(InteractiveResolvedFile.FullPath),%(InteractiveResolvedFile.AssetType),%(InteractiveResolvedFile.IsNotImplementationReference),%(InteractiveResolvedFile.InitializeSourcePath),'
          KeepDuplicates="false" />
      <ResolvedReferenceLines
          Condition="(@(NativeIncludeRoots->Count()) &gt; 0) AND (('%(NativeIncludeRoots.NugetPackageId)'!='FSharp.Core') or ('$(SCRIPTEXTENSION)'!='.fsx' and '%(NativeIncludeRoots.NugetPackageId)'=='FSharp.Core'))"
          Include='%(NativeIncludeRoots.NugetPackageId),%(NativeIncludeRoots.NugetPackageVersion),%(NativeIncludeRoots.PackageRoot),,%(NativeIncludeRoots.AssetType),,,%(NativeIncludeRoots.Path)'
          KeepDuplicates="false" />
      <ResolvedReferenceLines
          Condition="(@(ProvidedPackageRoots->Count()) &gt; 0) AND (('%(ProvidedPackageRoots.NugetPackageId)'!='FSharp.Core') or ('$(SCRIPTEXTENSION)'!='.fsx' and '%(ProvidedPackageRoots.NugetPackageId)'=='FSharp.Core'))"
          Include='%(ProvidedPackageRoots.NugetPackageId),%(ProvidedPackageRoots.NugetPackageVersion),%(ProvidedPackageRoots.PackageRoot),,%(ProvidedPackageRoots.AssetType),,,%(ProvidedPackageRoots.Path)'
          KeepDuplicates="false" />
    </ItemGroup>

    <WriteLinesToFile Lines='@(ResolvedReferenceLines)' 
                      File='$(MSBuildProjectFullPath).resolvedReferences.paths' 
                      Overwrite='True' WriteOnlyWhenDifferent='True' />
  </Target>

</Project>"""
