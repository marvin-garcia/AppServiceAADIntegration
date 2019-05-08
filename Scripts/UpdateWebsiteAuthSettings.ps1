$resourceGroupName = ''
$resourceName = '<site-name>/authsettings'
$resourceType = 'Microsoft.Web/sites/config'
$backendApiId = ''
$apiVersion = '2018-02-01'

# Get website auth settings
$resource = Invoke-AzureRmResourceAction -ResourceGroupName $resourceGroupName -ResourceType $resourceType -ResourceName $resourceName -Action list -ApiVersion $apiVersion -Force

# Add backend API token to login parameters
$resource.properties.additionalLoginParams = @{
    "response_type" = "code id_token"
    "resource" = $backendApiId
}

# Update website
New-AzureRmResource -PropertyObject $resource.properties -ResourceGroupName $resourceGroupName -ResourceType $resourceType -ResourceName $resourceName -ApiVersion $apiVersion -Force