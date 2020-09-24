# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs"
$webMvcFolder = Join-Path $slnFolder "src/AbpCompanyName.AbpProjectName.Web.Host"

## CLEAR ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## RESTORE NUGET PACKAGES #####################################################

Set-Location $slnFolder
dotnet restore

## PUBLISH WEB MVC PROJECT ###################################################

Set-Location $webMvcFolder
dotnet publish -c Release --output (Join-Path $outputFolder "Mvc")

## CREATE DOCKER IMAGES #######################################################

# Mvc
Set-Location (Join-Path $outputFolder "Mvc")

docker rmi dockerusername/mvc -f
docker build -t dockerusername/mvc .

## DOCKER COMPOSE FILES #######################################################

Copy-Item (Join-Path $slnFolder "docker/mvc/*.*") $outputFolder

## FINALIZE ###################################################################

Set-Location $outputFolder

cd Mvc
#init -- comment out after first run
git init
git config --global credential.helper 'store --file ~/.my-git-credentials'
git remote add azure 'https://azuredeploymentusername@abphost.scm.azurewebsites.net:443/abphost.git'

#not init
git add .
git commit -m "deployment commit message"
git push azure master --force
