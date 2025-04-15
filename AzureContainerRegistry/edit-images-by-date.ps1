param
(
  [Parameter (Mandatory = $true)]
  [DateTime] $DateTo
)

$currentDate = Get-Date
Write-Output $currentDate

az login --identity
# Define Azure Container Registry name
$acrName = "acrshoppingapp1"  # Replace with your ACR name
#$dateThreshold = [datetime]::Parse("2025-03-10T18:00:00Z")
$dateThreshold = $DateTo

# Get all repositories in ACR
$repositories = az acr repository list --name $acrName --output json | ConvertFrom-Json

# Loop through each repository and fetch manifests with creation date
$allImages = @()
foreach ($repo in $repositories) {
  #az acr repository show-manifests --name $acrName --repository $repo --output json
  $manifests = az acr manifest list-metadata --name $repo --registry $acrName `
    --query "[].{Repository: '$repo', Digest: digest, Timestamp: createdTime, Tags: tags}" --output json | ConvertFrom-Json
    
  # WARNING: This command has been deprecated and will be removed in a future release. Use 'acr manifest list-metadata' instead. 
  # $manifests = az acr repository show-manifests --name $acrName --repository $repo `
  #   --query "[].{Repository: '$repo', Digest: digest, Timestamp: timestamp}" --output json | ConvertFrom-Json
    
  $allImages += $manifests
}

# Display results
Write-Output "All Images Count: $($allImages.Count)"
$oldImages = $allImages | Where-Object { [datetime]::Parse($_.Timestamp) -lt $dateThreshold }
Write-Output "Old Images Count: $($oldImages.Count)"

foreach ($oldImage in $oldImages) {
  Write-Output $oldImage
  if ($oldImage.Tags -and $oldImage.Tags.Count -gt 0) {
    az acr repository update --name $acrName --image "$($oldImage.Repository):$($oldImage.Tags[0])" --write-enabled true `
      --query ["createdTime, digest"]
    Write-Output "Modified: $($oldImage.Repository):$($oldImage.Tags[0])"
  }
  else {
    az acr repository update --name $acrName --image "$($oldImage.Repository)@$($oldImage.Digest)" --write-enabled true `
      --query ["createdTime, digest"]
    Write-Output "Modified: $($oldImage.Repository)@$($oldImage.Digest)"
  }
}