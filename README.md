# dotinfo
A plugin for x64dbg that makes it easy to debug dotnet applications.
# Current features
- marks all jitted functions
- marks all static fields
# How to install
- make sure [dotx64dbg](https://github.com/x64dbg/DotX64Dbg) is installed 
- Create new folder in x64dbg\release\dotplugins\
- Download master branch and copy to this new folder
- Launch x64dbg and read pls #Troubleshooting
# Commands
- dinit - inits clrmd runtime
- nameallmodules - displays all loaded managed assemblies
- analyzemodule [dllname] - marks all jitted functions & static fields from the target assembly
- analyzeallemodules - marks all jitted functions & static fields from all assemblie
# Gif
[example.webm](https://github.com/BadRyuner/DotInfo/assets/54708336/68e5d5f2-e608-4ba1-bd70-5c7ee0a15a2e)
# Troubleshooting
- Error when entered "dinit" command. Solution: open plugin.cs via notepad, add some spaces or comments, save it, wait for hotreload in dotx64dbg, profit???
Unfortunately dotx64dbg does not handle nuget that well.
