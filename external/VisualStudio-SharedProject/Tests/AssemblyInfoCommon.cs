// <copyright>
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// The following assembly information is common to all VisualStudioTools Test assemblies.
// If you get compiler errors CS0579, "Duplicate '<attributename>' attribute", check your 
// Properties\AssemblyInfo.cs file and remove any lines duplicating the ones below.
[assembly: AssemblyCompany("Microsoft")]
[assembly: AssemblyProduct("Tools for Visual Studio")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AssemblyVersion(AssemblyVersionInfo.StableVersion)]
[assembly: AssemblyFileVersion(AssemblyVersionInfo.Version)]

class AssemblyVersionInfo {
    // This version string (and the comments for StableVersion and Version)
    // should be updated manually between major releases.
    // Servicing branches should retain the value
    public const string ReleaseVersion = "1.0";
    // This version string (and the comment for StableVersion) should be
    // updated manually between minor releases.
    // Servicing branches should retain the value
    public const string MinorVersion = "0";

    public const string BuildNumber = "0.00";

#if DEV10
    public const string VSMajorVersion = "10";
    const string VSVersionSuffix = "2010";
#elif DEV11
    public const string VSMajorVersion = "11";
    const string VSVersionSuffix = "2012";
#elif DEV12
    public const string VSMajorVersion = "12";
    const string VSVersionSuffix = "2013";
#elif DEV14
    public const string VSMajorVersion = "14";
    const string VSVersionSuffix = "2014";
#else
#error Unrecognized VS Version.
#endif

    public const string VSVersion = VSMajorVersion + ".0";

    // Defaults to "1.0.0.(2010|2012|2013)"
    public const string StableVersion = ReleaseVersion + "." + MinorVersion + "." + VSVersionSuffix;

    // Defaults to "1.0.0.00"
    public const string Version = ReleaseVersion + "." + BuildNumber;
}
