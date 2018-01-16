Param(
  [Parameter(HelpMessage="Where are the source files for the application? (required)")]
  [string]$sourceFolder,
	
  [Parameter(Mandatory=$True,HelpMessage="Where is the folder where applications are installed to? (required)")]
  [string]$destinationFolder,
  
  [Parameter(HelpMessage="Where is the folder where applications are backed up before being updated?")]
  [string]$backupFolder,
	
  [Parameter(Mandatory=$True,HelpMessage="Where are the XDT transforms for the *.example.config files? (required)")]
  [string]$transformsFolder,

  [Parameter(Mandatory=$True,HelpMessage="What is the name of the IIS site where the application is being set up? (required)")]
  [string]$websiteName,
	
  [Parameter(Mandatory=$True,HelpMessage="Why is this change being made? (required)")]
  [string]$comment
)

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
  git pull origin master
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

$projectName = "Escc.Umbraco.UserAccessManager" 
$sourceFolder = NormaliseFolderPath $sourceFolder "$PSScriptRoot\$projectName"
$destinationFolder = NormaliseFolderPath $destinationFolder
$backupFolder = NormaliseFolderPath $backupFolder
if (!$backupFolder) { $backupFolder = "$destinationFolder\backups" }
$backupFolder = "$backupFolder\$websiteName"
$destinationFolder = "$destinationFolder\$websiteName"
$transformsFolder = NormaliseFolderPath $transformsFolder

BackupApplication "$destinationFolder/$projectName" $backupFolder $comment

robocopy $sourceFolder "$destinationFolder/$projectName" /MIR /IF *.gif *.png *.eot *.svg *.ttf *.woff *.cshtml *.asax *.aspx *.dll *.less vista_colors.txt *.js *.map *.css csc.* csi.* /XD aspnet_client obj Properties App_Start Controllers IOC Models Services Utility ViewModel

TransformConfig "$sourceFolder\web.example.config" "$destinationFolder\$projectName\web.config" "$transformsFolder\$projectName\web.release.config"
if (Test-Path "$transformsFolder\$projectName\web.$websiteName.config") {
	TransformConfig "$destinationFolder\$projectName\web.config" "$destinationFolder\$projectName\web.temp1.config" "$transformsFolder\$projectName\web.$websiteName.config"
	copy "$destinationFolder\$projectName\web.temp1.config" "$destinationFolder\$projectName\web.config"
	del "$destinationFolder\$projectName\web.temp1.config"
}
copy "$sourceFolder\Views\web.example.config" "$destinationFolder\$projectName\Views\web.config"

EnableDotNet40InIIS
CreateApplicationPool "$projectName-$websiteName"
CheckSiteExistsBeforeAddingApplication $websiteName
CreateVirtualDirectory $websiteName $projectName "$destinationFolder\$projectName" true "$projectName-$websiteName"
DisableAnonymousAuthentication $websiteName $projectName
EnableWindowsAuthentication $websiteName $projectName

# Give application pool account write access so that it can create clientDependency cache files and logs
Write-Host "Granting Modify access to the application pool account"
if (!(Test-Path "$destinationFolder\$projectName\App_Data")) {
	md "$destinationFolder\$projectName\App_Data"
}
$acl = Get-Acl "$destinationFolder/$projectName/App_Data"
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS AppPool\$projectName-$websiteName", "Modify", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($rule)
Set-Acl "$destinationFolder/$projectName/App_Data" $acl

Write-Host
Write-Host "Done." -ForegroundColor "Green"