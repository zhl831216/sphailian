Set-PNPTraceLog -On -Level Debug # turn on for extra info


Connect-PnPOnline -Url https://folkuniversitetetsp2016.sharepoint.com/sites/hailian -Credentials dev21

# Connect-PnPOnline -Url https://folkuniversitet.sharepoint.com/sites/devsearch -Credentials dev2


$web = Get-PnPWeb

Apply-PnPProvisioningTemplate -Path ./NewsitesList .xml -Web $web


