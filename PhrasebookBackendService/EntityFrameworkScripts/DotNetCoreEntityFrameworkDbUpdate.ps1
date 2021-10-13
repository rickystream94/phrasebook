[CmdletBinding()]
param
(
    [Parameter(Mandatory=$true)]
    [ValidateSet("Development","Production")]
    [string] $Environment
)

$originalLocation = Get-Location

# Choose target environment
$env:ASPNETCORE_ENVIRONMENT = $Environment

# Change working directory: we have to be in the context of the startup project
Set-Location "$PSScriptRoot\..\PhrasebookBackendService"

dotnet ef database update --verbose

Set-Location $originalLocation