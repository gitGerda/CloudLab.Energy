Write-Host "
						   ____  _                    _  _         _                                                                           
						  / ___|| |  ___   _   _   __| || |  __ _ | |__                                                                        
						 | |    | | / _ \ | | | | / _` || | / _` || '_ \                                                                       
						 | |___ | || (_) || |_| || (_| || || (_| || |_) |                                                                      
						  \____||_| \___/  \__,_| \__,_||_| \__,_||_.__/                                                                       
" -ForegroundColor DarkMagenta;
Write-Host "
		  _____          _           _____        _  _              _    _                 _____                     _            
		 |  __ \        | |         / ____|      | || |            | |  (_)               / ____|                   (_)           
		 | |  | |  __ _ | |_  __ _ | |      ___  | || |  ___   ___ | |_  _   ___   _ __  | (___    ___  _ __ __   __ _   ___  ___ 
		 | |  | | / _` || __|/ _` || |     / _ \ | || | / _ \ / __|| __|| | / _ \ | '_ \  \___ \  / _ \| '__|\ \ / /| | / __|/ _ \
		 | |__| || (_| || |_| (_| || |____| (_) || || ||  __/| (__ | |_ | || (_) || | | | ____) ||  __/| |    \ V / | || (__|  __/
		 |_____/  \__,_| \__|\__,_| \_____|\___/ |_||_| \___| \___| \__||_| \___/ |_| |_||_____/  \___||_|     \_/  |_| \___|\___|

												     cloudlab.energy.datacollectionservice

" -ForegroundColor Magenta;
Write-Host "robot:> " -ForegroundColor Yellow -NoNewline
Write-Host "Enter path to DataCollectionService.exe including executable file. (Example: C:\Program Files\Cloudlab\DataCollectionService.exe)" -ForegroundColor white;
Write-Host "user:> " -ForegroundColor Yellow -NoNewline
$path = Read-Host "path";
sc.exe create CloudLabDataCollectionService start=auto binpath=$path;
if($LastExitCode -ne 0){
Write-Host "robot:> " -ForegroundColor Yellow -NoNewline
Write-Host "Some errors occured" -ForegroundColor DarkRed;
}
else{
Write-Host "robot:> " -ForegroundColor Yellow -NoNewline
Write-Host "CloudLabDataCollectionService successfully installed!" -ForegroundColor green;
sc description CloudLabDataCollectionService "Electric Power Reading Service";
Write-Host "robot:> " -ForegroundColor Yellow -NoNewline
Write-Host "CloudLabDataCollectionService starting..." -ForegroundColor green;
sc start CloudLabDataCollectionService;
if($LastExitCode -ne 0)
{
Write-Host "robot:> " -ForegroundColor Yellow -NoNewline
Write-Host "Some errors occured" -ForegroundColor DarkRed;
}
else{
Write-Host "robot:> " -ForegroundColor Yellow -NoNewline
Write-Host "CloudLabDataCollectionService successfully started!" -ForegroundColor green;
Write-Host "robot:> " -ForegroundColor Yellow -NoNewline
Write-Host "Good Bye!" -ForegroundColor green;
services.msc;
}
}

