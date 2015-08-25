# powershell.exe
# Set the Uri to suit the environment
# ===================================
$postParams = @{apiuser='username';apikey='password'}
Invoke-RestMethod -Uri https://hostname/api/ExpiringPagesApi/CheckForExpiringNodesByUser/ -Method POST -Body $postParams
