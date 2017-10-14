# Connect-PnPOnline -Url https://folkuniversitetetsp2016.sharepoint.com/sites/hailian -Credentials dev21
Connect-PnPOnline -Url https://folkuniversitet.sharepoint.com/sites/devsearch -Credentials dev2

$field = Get-PnPField -Identity testchoice

$field.SchemaXml | clip.exe