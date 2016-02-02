########################################################################
### BOOTSTRAP. Copy this section into each application setup script  ###
###            to make sure required functions are available.        ###
########################################################################

$pathOfThisScript = Split-Path $MyInvocation.MyCommand.Path -Parent
$parentFolderOfThisScript = $pathOfThisScript | Split-Path -Parent
$scriptsProject = 'Escc.WebApplicationSetupScripts'
$functionsPath = "$pathOfThisScript\..\$scriptsProject\functions.ps1"
if (Test-Path $functionsPath) {
  Write-Host "Checking $scriptsProject is up-to-date"
  Push-Location "$pathOfThisScript\..\$scriptsProject"
  #git pull origin master
  Pop-Location
  Write-Host
  .$functionsPath
} else {
  if ($env:GIT_ORIGIN_URL) {
    $repoUrl = $env:GIT_ORIGIN_URL -f $scriptsProject
    git clone $repoUrl "$pathOfThisScript\..\$scriptsProject"
  } 
  else 
  {
    Write-Warning '$scriptsProject project not found. Please set a GIT_ORIGIN_URL environment variable on your system so that it can be downloaded.
  
Example: C:\>set GIT_ORIGIN_URL=https://example-git-server.com/{0}"
  
{0} will be replaced with the name of the repository to download.'
    Exit
  }
}

########################################################################
### END BOOTSTRAP. #####################################################
########################################################################

$adminProject = "Escc.Umbraco.UserAccessManager"

EnableDotNet40InIIS

Write-Host "Setting up admin website"
CreateApplicationPool $adminProject
CreateWebsite $adminProject "$pathOfThisScript\$adminProject"
CreateHTTPSBinding $adminProject "localhost"
RemoveHTTPBinding $adminProject 80
DisableAnonymousAuthentication $adminProject
EnableWindowsAuthentication $adminProject
#CopyConfig "$adminProject\Web.example.config" "$adminProject\web.config"

Write-Host
Write-Host "Done." -ForegroundColor "Green"
Write-Host
Write-Host "Now replace the sample values in `web.config` with ones appropriate to your setup, and build the solution." -ForegroundColor "Green"