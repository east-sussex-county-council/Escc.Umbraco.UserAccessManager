# Umbraco User Control API

## Page Expiry Email System

A scheduled job that looks for pages that will expire within a set number of days, the number of days is set in `web.config` (`NoOfDaysFrom`).

It is an API call with no user interface. The address is:
 
	https://hostname/api/ExpiringPagesApi/CheckForExpiringNodesByUser/

The API call has username / password protection, using the `apiuser` and `apikey` values. There is a PowerShell script in the Jobs folder of the ESCC.Umbraco.UserAccessManager solution.

### Schedule Setup

#### Action: 
Start a program

#### Program/script: 
Powershell

#### Add arguments:
-File [path to script] \CheckForExpiringNodesByUser.ps1
