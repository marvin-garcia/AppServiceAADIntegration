$resourceGroupName = ''
$appServicePlanName = ''
$appServicePlanSku = '' # Allowed values: B1, B2, B3, D1, F1, FREE, P1, P1V2, P2, P2V2, P3, P3V2, PC2, PC3, PC4, S1, S2, S3, SHARED. Default: B1.
$backendSiteName = ''
$frontendSiteName = ''
$location = ''

az group create --name $resourceGroupName --location $location

az appservice plan create --name $appServicePlanName --resource-group $resourceGroupName --sku $appServicePlanSku

az webapp create --resource-group $resourceGroupName --plan $appServicePlanName --name $frontendSiteName
az webapp create --resource-group $resourceGroupName --plan $appServicePlanName --name $backendSiteName