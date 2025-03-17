param
(
    [Parameter (Mandatory = $false)]
    [object] $WebhookData
)
 
# Login using Azure assigned managed identity.
Connect-AzAccount -Identity
 
$currentDate = Get-Date
Write-Output $currentDate
 
if ($WebhookData.RequestBody) {
    Write-Output $WebhookData.RequestBody
    $inputData = (ConvertFrom-Json -InputObject $WebhookData.RequestBody)
    Write-Output $inputData
    $acrName = $inputData.request.host.Replace('.azurecr.io', '')
    $repositoryName = $inputData.target.repository
    $tagName = $inputData.target.tag
 
    Update-AzContainerRegistryTag -RegistryName $acrName -RepositoryName $repositoryName -Name $tagName -WriteEnabled $false
}
else {
    Write-Output "Request body is absent."
}