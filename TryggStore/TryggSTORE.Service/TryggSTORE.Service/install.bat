REM Run as admin
netsh http add urlacl url=http://+:2166/ user="NT AUTHORITY\LocalService"
"%~dp0TryggSTORE.Service.exe" /install
net start TryggSTORE