<#
Never use Write-Host as it writes out to Host UI screen. There is no easy way to redirect it, short of writing our own host.

PowerShellTasklet expects that $global:ScriptExitStatus is set in the script on exit, see examples below
$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Unknown
$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Executing 
$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Completed
$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Noop
$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Failed
$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Stopped

If $global:ScriptExitStatus is set to Executing, Unknown, or $null then PowerShellTasklet will use 
PowerShellExitCodeMapper provided by user to determine step exit status. 
PowerShellExitCodeMapper method GetExitStatus takes 1 parameters $LastExitStatus which is set
by PowerShell runtime on exit from system commands or by Return and Exit.
#>

# Enable Verbose messaging...
$VerbosePreference = "Continue"

# Enable Debug messaging...
$DebugPreference = "Continue"

# if you need to trace at statement level...
#Set-PSDebug -Trace 2

# set script file being executed...
$scriptFile = $MyInvocation.MyCommand.Definition

# Let user know script file being executed...
Write-Output "Start Executing Script File: $scriptFile"

# $ScriptExitStatus is pre-set from PowerShellTasklet in a GLOBAL scope...
Write-Output "Initial ScriptExitStatus => $($global:ScriptExitStatus.ExitCode)"

try
{
	#Make all errors terminating
	$ErrorActionPreference = "Stop"; 

	# make sure to set ScriptExitStatus in a global scope...
	#$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Completed

	#user defined ExitStatus...[Summer.Batch.Core.ExitStatus] must be used to strong type $userExitStatus 
	[Summer.Batch.Core.ExitStatus]$userExitStatus = New-Object Summer.Batch.Core.ExitStatus -ArgumentList "UserExitCode","Description"

	# set ScriptExitStatus to User defined ExitStatus...
	$global:ScriptExitStatus = $userExitStatus
	
	Write-Output "On Exit ScriptExitStatus => $($global:ScriptExitStatus.ExitCode)"

} catch {
	
	# let PowerShellTasklet know something failed...
	[string] $errString = Format-Error($_)
	Write-Error $errString -ErrorAction Continue
	#Format-List -Property PositionMessage -InputObject $_.InvocationInfo | Out-String -Width 512 | Write-Error -ErrorAction Continue

	# using built-in ExitStatus...if set will be used by PowerShellTasklet...
	$global:ScriptExitStatus = [Summer.Batch.Core.ExitStatus]::Failed

	#=> Exit <> 0 will set ExitStatus of the step to Failed, if $global:ScriptExitStatus is set to Executing, Unknown, or $null
    Exit 1

}finally{

   $ErrorActionPreference = "Continue"; #Reset the error action pref to default

}

#Trace Message...
Write-Output "End Executing Script File: $scriptFile"

# must specify exit or return status...
Exit 0
