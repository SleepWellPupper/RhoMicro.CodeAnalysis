param(
	[string]$Configuration = "Debug"
)

$props_path = Join-Path $psscriptroot "../Directory.Build.props"
$props_text = Get-Content $props_path
[xml] $props_root = $props_text
$revision_node = $props_root.SelectSingleNode("/Project/PropertyGroup/AutoIncrementedAssemblyRevision");
$revision = [int]$revision_node.Innertext
$new_revision = $revision + 1
$revision_node.InnerText = $new_revision
$props_root.Save($props_path)

Write-Host -ForegroundColor Green "Incremented assembly revision from $revision to $new_revision."

Write-Host -ForegroundColor Yellow "Building UtilityGenerators.csproj"
dotnet build -c $Configuration -v q -p:SolutionName="RhoMicro.CodeAnalysis"
if ($LASTEXITCODE -ne 0) {
	Write-Host -ForegroundColor Red 'Error while building UtilityGenerators.csproj'
	return
}
else {
	Write-Host -ForegroundColor Green 'Built UtilityGenerators.csproj'
}

Write-Host -ForegroundColor Yellow "Building UtilityGenerators.Dev.csproj"
dotnet build ../UtilityGenerators.Dev/UtilityGenerators.Dev.csproj -c $Configuration -v q -p:SolutionName="RhoMicro.CodeAnalysis"
if ($LASTEXITCODE -ne 0) {
	Write-Host -ForegroundColor Red 'Error while building UtilityGenerators.Dev.csproj'
	return
}
else {
	Write-Host -ForegroundColor Green 'Built UtilityGenerators.Dev.csproj'
}

Write-Host -ForegroundColor Yellow "Restoring UtilityGenerators.csproj"
dotnet restore --force
if ($LASTEXITCODE -ne 0) {
	Write-Host -ForegroundColor Red 'Error while restoring UtilityGenerators.csproj'
	return
}
else {
	Write-Host -ForegroundColor Green 'Restored UtilityGenerators.csproj'
}
