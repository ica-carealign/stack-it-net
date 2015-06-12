param([string]$path = $(throw "-path is required."))

pwd
ls

$absPath = Resolve-Path $path
$xml = [xml](Get-Content $absPath)
$authNode = $xml.configuration.appSettings.add | where {$_.key -Eq "AuthenticationProvider"}
$authNode.value = "Crowd"
$xml.Save($path)