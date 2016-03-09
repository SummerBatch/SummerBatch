<#
 this file is dot-sourced by other power shell scripts...so add only functions here...
#>

Function Format-Error ($thisError) 
{
    [string] $errMsg = $thisError.Exception.Message
	$errMsg += "`r`n`r`n"
    $errMsg += $thisError.InvocationInfo.PositionMessage
    Return $errMsg
}

