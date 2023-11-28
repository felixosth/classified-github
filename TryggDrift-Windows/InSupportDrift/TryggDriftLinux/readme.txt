https://swimburger.net/blog/dotnet/how-to-run-a-dotnet-core-console-app-as-a-service-using-systemd-on-linux

Change the values in appsettings.json

Modify some values in TryggDrift.service

sudo cp TryggDrift.service /etc/systemd/system/TryggDrift.service
sudo systemctl daemon-reload
sudo systemctl enable TryggDrift

sudo systemctl start TryggDrift



sudo journalctl -u TryggDrift


chmod +x /tryggdrift/TryggDriftLinux
chmod 777 /tryggdrift/
chown yourusername -R /tryggdrift

sudo apt-get install lsb-core