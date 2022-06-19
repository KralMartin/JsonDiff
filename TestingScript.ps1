param ($action = "", $id = "powershell", $uri = "http://jsondiff-prod.eu-west-1.elasticbeanstalk.com", $body='"eyJpbnB1dCI6InRlc3RWYWx1ZSJ9"')
		   
if($action -eq "" -or $action -eq "diff"){
	$uri = "$($uri)/v1/diff/$($id)"	
	Invoke-WebRequest -Uri $uri -Method GET 
}
elseif($action -eq "left"){
	$dic =@{"accept"="*/*"; "Content-Type"="application/custom"}
	$uri = "$($uri)/v1/diff/$($id)/left"	
	Invoke-WebRequest -Uri $uri -Method POST -Headers $dic -body $body
}
elseif($action -eq "right"){
	$dic =@{"accept"="*/*"; "Content-Type"="application/custom"}
	$uri = "$($uri)/v1/diff/$($id)/right"	
	Invoke-WebRequest -Uri $uri -Method POST -Headers $dic -body $body
}
else{
	write-error "invalid action"
}