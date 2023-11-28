Add-Type -Path "../TryggLogin.Server.dll"
[ESLogin.Background.ESLoginBackgroundPlugin]::GetMGUID();
[Console]::ReadKey();