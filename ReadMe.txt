Deployment Steps:
Backup Db on production site
find/replace abpclient with your angular client hostname
find/replace abphost with your asp.net server hostname
find/replace dockerusername with your docker user name
find/replace sendgridusername with your sendgrid user name
find/replace sendgridpassword with your sendgrid password
Go to ./module-zero-core-template/aspnet-core/build in powershell
run either build-with-ng-prod.ps1 OR build-with-ng.ps1 depending on which environment you want
docker push dockerusername/host
docker push dockerusername/ng
Backup Db on production site (don't forget)
sync docker images in azure or wherever
restore Db
