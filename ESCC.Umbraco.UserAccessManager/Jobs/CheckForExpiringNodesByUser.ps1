# powershell.exe 
# Before use, set the Uri, username and password to suit the environment
# ===================================
# Written to support PowerShell v2
# ===================================
$URL="http://hostname/api/ExpiringPagesApi/CheckForExpiringNodesByUser/"

$NVC = New-Object System.Collections.Specialized.NameValueCollection
$NVC.Add('apiuser', 'username');
$NVC.Add('apikey', 'password');

$WC = New-Object System.Net.WebClient
$WC.UseDefaultCredentials = $true
$Result = $WC.UploadValues($URL,"post", $NVC);

[System.Text.Encoding]::UTF8.GetString($Result)
$WC.Dispose();
