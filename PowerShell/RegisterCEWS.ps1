Add-PSSnapIn Microsoft.SharePoint.PowerShell -EV Err -ErrorAction SilentlyContinue


$ssa = Get-SPEnterpriseSearchServiceApplication

$config = New-SPEnterpriseSearchContentEnrichmentConfiguration 

$config.Endpoint = "http://localhost:38164/Service/ContentProcessor.svc"
#$config.DebugMode = $true
$config.FailureMode = "WARNING"
$config.InputProperties = "OriginalPath", "FileExtension"

$config.OutputProperties = "CustomCategory", "CustomExt"



Set-SPEnterpriseSearchContentEnrichmentConfiguration -SearchApplication $ssa -ContentEnrichmentConfiguration $config


Get-Service | ? { $_.DisplayName -match "SharePoint*"} | % {
    Write-Host $_.Status
    if($_.Status -eq "Running")    {
        Restart-Service $_
    }
}