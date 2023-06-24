Container Apps:
- Resource group:
    rg-vg-tm-dev-sa-001
- Key vault:
- App Environment
    App environment: cae-vg-tm-dev-sa-001
    Setup dapr components: 
    Log workspace: log-vg-tm-dev-sa-001
    Enable Dapr
- Container apps:
    TestManager web: ca-vg-tm-tmgrapp-dev-sa-001
    TestManager Api: ca-vg-tm-tmgrapi-dev-sa-001
    TestRunner Web: ca-vg-tm-trunnerapp-dev-sa-001
    TestRunner Api: ca-vg-tm-trunnerapi-dev-sa-001
    Account manager Api: ca-vg-tm-amgrapi-dev-sa-001
    Webhook Api: ca-vg-tm-whapi-dev-sa-001
- Service bus:
- Storage Account:
    st-vg-tm-general-dev-sa-001
    st-vg-tm-sendmailfunc-dev-sa-001
- AZ function app:
    Send mail: func-vg-tm-sm-dev-sa-001
- SSL ? use free of azure

deploymentName='ExampleDeployment'$RANDOM \\
 \\
az deployment group create --name depoy-001 --resource-group rg-vg-tm-dev-sa-001 --template-file F:\\Workspace\\VietGeeksRepo\\TestPlatform\\deployments\\container-app\\main.bicep
az deployment group create --name depoy-00101 --resource-group rg-vg-tm-dev-sa-001 --template-file F:\\Workspace\\VietGeeksRepo\\TestPlatform\\deployments\\container-app-dapr.bicep --parameters F:\\Workspace\\VietGeeksRepo\\TestPlatform\\deployments\\container-app-dapr.bicepparam

az deployment group create --name depoy-test-manager-api-dev-1 --resource-group rg-vg-tm-dev-sa-001 --template-file F:\\Workspace\\VietGeeksRepo\\TestPlatform\\deployments\\container-app-dapr.bicep --parameters F:\\Workspace\\VietGeeksRepo\\TestPlatform\\deployments\\container-app-dapr-test-manager-api-dev.bicepparam

az deployment group create --name depoy-test-manager-api-dev-21 --resource-group rg-vg-tm-dev-sa-001 --template-file F:\\Workspace\\VietGeeksRepo\\TestPlatform\deployments\container-app\roles.bicep

az deployment group create --name depoy-test-manager-api-dev-22 --resource-group rg-vg-tm-dev-sa-001 --template-file F:\\Workspace\\VietGeeksRepo\\TestPlatform\\deployments\container-app\storage.bicep
az containerapp env certificate create -g rg-vg-tm-dev-
sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-manager-api --hostname dev.test-manager-api.testmaster.io --validation-method CNAME
az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-manager --hostname dev.test-manager.testmaster.io --validation-method CNAME
az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-runner-api --hostname dev.test-runner-api.testmaster.io --validation-method CNAME
az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-runner --hostname dev.test-runner.testmaster.io --validation-method CNAME
az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-webhook --hostname dev.webhook.testmaster.io --validation-method CNAME
az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-account-manager --hostname dev.account-manager.testmaster.io --validation-method CNAME

az ad sp create-for-rbac --name "test-master-github" --role contributor --scopes /subscriptions/ec4dddda-f3cb-4c93-a2f1-4f3c705dfc88/resourceGroups/rg-vg-tm-dev-sa-001 --sdk-auth
{
  "clientId": "0623c0f1-3721-4378-b240-8bd95450ba1d",
  "clientSecret": "Y088Q~gHIyuGEgjQNk2x1qB3DiJ6U2rqJNfEKbb.",
  "subscriptionId": "ec4dddda-f3cb-4c93-a2f1-4f3c705dfc88",
  "tenantId": "de0dcb0a-9ef5-4e7f-8037-02fe3a42f876",
  "activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
  "resourceManagerEndpointUrl": "https://management.azure.com/",
  "activeDirectoryGraphResourceId": "https://graph.windows.net/",
  "sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
  "galleryEndpointUrl": "https://gallery.azure.com/",
  "managementEndpointUrl": "https://management.core.windows.net/"
}

---
## Summary
### Step 1
Create resource group:
az group create -l southeastasia -n rg-vg-tm-dev-sa-001
Create SP to allow CD from github, run command, paste result to github action secret ''
az ad sp create-for-rbac --name "test-master-github" --role contributor --scopes /subscriptions/ec4dddda-f3cb-4c93-a2f1-4f3c705dfc88/resourceGroups/rg-vg-tm-dev-sa-001 --sdk-auth
### Step 2
Init environments:
Run bicep 
Custom domain registrations: Could replace by bicep script

az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-manager-api --hostname dev.test-manager-api.testmaster.io --validation-method CNAME

az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-manager --hostname dev.test-manager.testmaster.io --validation-method CNAME

az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-runner-api --hostname dev.test-runner-api.testmaster.io --validation-method CNAME

az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-test-runner --hostname dev.test-runner.testmaster.io --validation-method CNAME

az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-webhook --hostname dev.webhook.testmaster.io --validation-method CNAME

az containerapp env certificate create -g rg-vg-tm-dev-sa-001 --name cae-vg-tm-dev-sa-001 --certificate-name  dev-account-manager --hostname dev.account-manager.testmaster.io --validation-method CNAME
### Step 3

