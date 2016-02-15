<#
Never use Write-Host as it writes out to Host UI screen. There is no easy way to redirect it, short of writing our own host.
#>

# Enable Verbose messaging...
$VerbosePreference = "Continue"

# Enable Debug messaging...
$DebugPreference = "Continue"

# if you need to trace at statement level...
#Set-PSDebug -Trace 2

# Let user know script file being executed...
Write-Output "Start Executing Script File: $(([System.IO.FileInfo]$scriptFileInfo).FullName)"

Write-Output "Loading Assembly: " 
[System.Reflection.Assembly]::LoadFrom("..\..\bin\Debug\Summer.Batch.Core.dll")
$ExitStatus = [Summer.Batch.Core.ExitStatus]

[Summer.Batch.Core.ExitStatus]$ScriptExitStatus = $ExitStatus::Unknown
Write-Output "Initial ScriptExitStatus is $ScriptExitStatus"

#$ExitStatus = New-Object Summer.Batch.Core.ExitStatus
#Write-Output "Initial ExitStatus is Summer.Batch.Core.ExitStatus.Failed"
try
{
	#Make all errors terminating
	$ErrorActionPreference = "Stop"; 

	# cd to working directory for this script...
	Set-Location -Path .\TestData\PowerShell
	Write-Output "Working Directory: $pwd"

	#dot-source helper functions
	. ./HelperFunctions.ps1

	#=> this was passed  via Variables, i.e. List<FileInfo> $filesToCompare
	Write-Output "Loop over List<FileInfo> filesToCompare. List was passed via PowerShellTasklet.Variables property"
	$i = 1
	foreach($file in $filesToCompare)
	{
		$relPath = resolve-path ([System.IO.FileInfo]$file).FullName -relative 
		Write-Output "$i. Relative Path to file: $relPath"
		$i++
	}

	#get relative path to file
	$relPath = resolve-path ([System.IO.FileInfo]$fileToCompare).FullName -relative 
	Write-Output "Relative Path to compare file: $relPath"

	#get file name
	$fileName = ([System.IO.FileInfo]$fileToCompare).Name
	Write-Output "Compare File Name: $fileName"

	#Copy file to compare...
	Write-Output "Copying file $relPath to current Working Directory"
	Copy-Item ([System.IO.FileInfo]$fileToCompare).FullName . -Force

	#Compare files...
	fc.exe /b $fileName $fileName > $null
	if ($LastExitCode -eq 0) {
		Write-Output "The files are equal"
	}
	else {
		Write-Output "The files are NOT equal"
	}
	#Delete file if exists...
	If (Test-Path $fileName)
	{
		Write-Output "Deleting file $fileName from current Working Directory"
		Remove-Item $fileName
	}

	$ScriptExitStatus = $ExitStatus::Completed
	Write-Output "On Exit ScriptExitStatus is $ScriptExitStatus"

} catch {
	
	#we are done...let PowerShellTasklet know something failed...
	$ErrorMessage = $_.Exception.Message
    $FailedItem = $_.Exception.ItemName
	
	[string] $errString = Format-Error($_)
	Write-Error $errString -ErrorAction Continue
	#Format-List -Property PositionMessage -InputObject $_.InvocationInfo  -Expand Both | Out-String -Width 512 | Write-Error -ErrorAction Continue

	$ScriptExitStatus = $ExitStatus::Failed

	#=> Exit <> 0 will set ExitStatus of the step to Failed
    Exit 1

}finally{

   $ErrorActionPreference = "Continue"; #Reset the error action pref to default

}

#Trace Message...
Write-Output "End Executing Script File: $(([System.IO.FileInfo]$scriptFileInfo).FullName)"

# Write-Error will add to $Error Array an ErrorRecord element
Write-Error  "Test"

# must specify exit or return status...used by PowerShellExitCodeMapper to set ExitStatus...
Exit 0
