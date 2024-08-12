param (
    [string] $ResourceGroupName,
    [string] $StorageAccountName,
    [string] $IndexDocument
)

$ErrorActionPreference = 'Stop'

$storageContainerName = 'data-protection'
$storageAccount = Get-AzStorageAccount -ResourceGroupName $ResourceGroupName -AccountName $StorageAccountName
$ctx = $storageAccount.Context

 if(Get-AzStorageContainer -Name $storageContainerName -Context $ctx -ErrorAction SilentlyContinue)  
    {  
        Write-Host -ForegroundColor Magenta $storageContainerName "- container already exists."  
    }  
    else  
    {  
        New-AzStorageContainer -Context $ctx -Name $storageContainerName -Permission Off 
        New-Item $IndexDocument -Force
        #Set-Content $IndexDocument ''
        Set-AzStorageBlobContent -Context $ctx -Container $storageContainerName -File $IndexDocument -Blob $IndexDocument -Properties @{'ContentType' = 'text/plain'}  
    }      

 