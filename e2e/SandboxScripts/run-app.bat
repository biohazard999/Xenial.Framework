REM DOWNLOAD dotnet install

curl -L "https://dot.net/v1/dotnet-install.ps1" --output C:\users\WDAGUtilityAccount\Desktop\dotnet-install.ps1

echo > C:\users\WDAGUtilityAccount\Desktop\set-ExecutionPolicy.txt
powershell -Command "Set-ExecutionPolicy Unrestricted"
echo > C:\users\WDAGUtilityAccount\Desktop\setted-ExecutionPolicy.txt
echo > C:\users\WDAGUtilityAccount\Desktop\installing-dotnet.txt
REM powershell -File "C:\users\WDAGUtilityAccount\Desktop\dotnet-install.ps1" --version 5.0.102
setx PATH %PATH%;C:\Users\WDAGUtilityAccount\AppData\Local\Microsoft\dotnet; /M
setx PATH %PATH%;C:\Users\WDAGUtilityAccount\AppData\Local\Microsoft\dotnet;
set PATH=%PATH%;C:\Users\WDAGUtilityAccount\AppData\Local\Microsoft\dotnet;
echo > C:\users\WDAGUtilityAccount\Desktop\installed-dotnet.txt

echo > C:\users\WDAGUtilityAccount\Desktop\ready.txt
CD C:\users\WDAGUtilityAccount\Desktop\artifacts
mkdir Xenial.FeatureCenter.Win
tar -xf Xenial.FeatureCenter.Win.v0.0.45-alpha.0.9.AnyCPU.zip -C ./Xenial.FeatureCenter.Win
Xenial.FeatureCenter.Win\Xenial.FeatureCenter.Win.exe