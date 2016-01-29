<# Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information. #>

<# Download and install the dotnet cli #>

$clipath = $args[0]
$BackUpPath = [System.IO.Path]::Combine($args[1], "dotnet-latest.zip")
$Destination = [System.IO.Path]::Combine($args[1], "dotnet")

Invoke-WebRequest -UseBasicParsing $clipath -OutFile $BackUpPath
Add-Type -assembly "system.io.compression.filesystem"
[io.compression.zipfile]::ExtractToDirectory($BackUpPath, $destination)
