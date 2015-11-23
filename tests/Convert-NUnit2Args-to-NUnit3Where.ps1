<#
.SYNOPSIS

Convert nunit-console.exe (v2) --include and --exclude arguments to --where syntax of nunit3-console.exe

.PARAMETER IncludeCategories

Categories to include.

.PARAMETER ExcludeCategories

Categories to exclude.

.EXAMPLE

Convert-NUnit2Args-to-NUnit3Where -IncludeCategories LongRunning,RequireInternet -ExcludeCategories WIP,KnownFailure
#>
param(
    [string[]] $IncludeCategories = $null,
    [string[]] $ExcludeCategories = $null
)

Set-StrictMode -Version 2
$ErrorActionPreference = "Stop"

if ($IncludeCategories -eq $null) {
    $IncludeCategories = @()
}

if ($ExcludeCategories -eq $null) {
    $ExcludeCategories = @()
}

function Add-Parens
{
    param( [string]$expr )
    
    if ([string]::IsNullOrEmpty($expr)) {
        $expr
    } else {
        "( " + $expr + " )"
    }
}

$includes = Add-Parens ( @( $IncludeCategories | ForEach-Object { "( cat == {0} )" -f $_ } ) -join ' or ' )

$excludes = @( $ExcludeCategories | ForEach-Object { "( cat != {0} )" -f $_ } ) -join ' and '

$all = $includes, $excludes | Where {$_}

$whereString = $all -join ' and '

echo $whereString
