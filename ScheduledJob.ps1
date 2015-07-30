# Run directly in Scheduler. Requires Anonymous Authentication to be enabled in IIS:
#  Program/Script: PowerShell
#  Arguments: Invoke-WebRequest -Uri https://hostname/api/ExpiringPagesApi




# command: powershell -executionpolicy bypass -File ScheduledJob.ps1

$root = "https://hostname/api/ExpiringPagesApi"
# $user = "username"
# $pass= "password"


# $securepassword = ConvertTo-SecureString $pass -AsPlainText -Force
# $credentials = New-Object System.Management.Automation.PSCredential($user, $securepassword)
# Invoke-WebRequest -Uri $root -Credential $credentials
Invoke-WebRequest -Uri $root