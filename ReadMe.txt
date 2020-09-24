Default db is pre-created because otherwise when it initializes in azure you will get error "database is locked"
All db migrations must be done on localhost as long as you are using sqlite
Also note that docker container for the host in azure are ephemeral, and reload after a server reboot causing you to loose your db changes

Deployment Steps:
Backup Db on production site
find/replace azuredeploymentusername with your azure kudu username
find/replace abpclient with your angular client hostname
find/replace abphost with your asp.net server hostname
find/replace dockerusername with your docker user name
find/replace sendgridusername with your sendgrid user name
find/replace sendgridpassword with your sendgrid password
Go to ./module-zero-core-template/aspnet-core/build in powershell
run either build-with-ng-prod.ps1 OR build-with-ng.ps1 depending on which environment you want
docker push dockerusername/ng
sync docker image in azure or wherever via reboot of app service

Go to ./module-zero-core-template/aspnet-core/build
run build-host.ps1
cd Mvc
Backup Db on production site (don't forget)
git push to azure using kudu
restore Db
