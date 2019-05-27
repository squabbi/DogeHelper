#!/bin/bash
HOST='dogetower'
PORT='69'
USER='root'
PASSWD='gamefreak'
FILE='dogehelper.tar'

echo "Current directory: " && pwd
echo "Commencing clean build, targeting .NETCore 2.1..."
echo "Cleaning build..."
dotnet clean -c Release

echo "Building project..."
echo
dotnet build -c Release -f netcoreapp2.1
echo

echo "Publishing build..."
echo
dotnet publish -c Release -f netcoreapp2.1
echo

echo "Packaging publish folder..."
echo "Removing old .tar"
rm bin/Release/netcoreapp2.1/publish/dogehelper.tar

echo
echo "Creating tar..."
cd bin/Release/netcoreapp2.1/publish/
tar -cf dogehelper.tar *.*

sshpass -p "gamefreak" scp -P 69 dogehelper.tar root@dogetower:/tmp/dogehelper.tar

read -n 1 -s -r -p "Press any key to continue..."