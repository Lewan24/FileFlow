![Static Badge](https://img.shields.io/badge/.NET%208-8A2BE2?style=for-the-badge)
![Static Badge](https://img.shields.io/badge/CLI%20Tool-blue?style=for-the-badge)
![Static Badge](https://img.shields.io/badge/License-MIT-yellow?style=for-the-badge)<br><br>

[![.NET](https://github.com/Lewan24/FileFlow/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Lewan24/FileFlow/actions/workflows/dotnet.yml)
<br><br>

![Logo](https://github.com/Lewan24/FileFlow/blob/master/Images/FileFlowLogo.png)

<hr>

# Idea
I would like to create a CLI tool for file versioning, something like "Git".
<hr>

# Licence
The project is based on the MIT license so if you are welcomed, you can easily use and be inspired by my way of software development.
<hr>

> [!IMPORTANT]  
> The FileFlow project is being developed in my spare time, so it is possible that it will not be updated often due to other projects I am working on
<hr>

# Education
The project is being created for educational purposes, to explore how such a system works and what problems developers have to deal with when developing such software.<br><br>

My goal is to learn how the version control system works (backend) and create functional tool that will be fully working
<hr>

# Requirements
All you need to run this tool is installed <strong>.NET 8</strong>
<hr>

# How to run (code)
For now the simpliest way to run the tool:
- Clone the repository
```bash
git clone https://github.com/Lewan24/FileFlow.git
```
- Go inside the repo directory and navigate to: <strong>"src -> FileFlow.Cli"</strong>
- Then run command:
```bash
dotnet run -- [Tool commands]
```
The <strong>[Tool commands]</strong> is actually the commands of the cli tool that you can use. This is how you can run help command:
```bash
dotnet run -- -h
```
<hr>

# How to run (dotnet tool local)
You can also build and install local tool, there are few instructions how to do it:
- Go into "src\FileFlow.Cli"
- Run command to pack the project into nuget
```bash
dotnet pack /p:PackageVersion="{PackcageVersion}"

// you can check the version at "src\FileFlow.Cli\Properties\AssemblyInfo.cs" or
// run application from code and run command 'v' or 'version'
```
- If there is .config directory with temp dotnet tools configuration you can go to next step, otherwise create new tools repository:
```bash
dotnet new tools-manifest
```
- Install FileFlow as dotnet tool (you need to be in directory where is the .config dir)
```bash
dotnet tool install --add-source ./nupkg FileFlow.Cli
```
- You can check if it is for sure in tools list by running command:
```bash
dotnet tool list
```
- Now you can use this tool in this directory and all subdirectories
To use dotnet tool just type:
```bash
dotnet tool run fileflow -- v
```
After tests you can uninstall tool by running command:
```bash
dotnet tool uninstall FileFlow.Cli
```
<hr>

# How to run (dotnet tool install nuget)
You can install tool via nuget for global use, but at this moment it's not available.<br>
It will be available when I publish and deploy nuget into store.

# Additional links
- Milanote board: <strong>[Not created yet]</strong>
- Trello board: <strong>https://trello.com/b/uKlrGHbJ</strong>
