# VexTrack

![Status badge](https://img.shields.io/badge/Status-Stable-green?style=for-the-badge "Development Status") [![Language badge](https://img.shields.io/badge/Language-Python_3.9.6-inactive?logo=python&logoColor=ffffff&style=for-the-badge)](https://python.org "Language") 
![GitHub License](https://img.shields.io/github/license/BitTim/ValorantXPCalc?logo=github&style=for-the-badge "License")
![GitHub repo size](https://img.shields.io/github/repo-size/BitTim/ValorantXPCalc?logo=github&style=for-the-badge) ![GitHub contributors](https://img.shields.io/github/contributors/BitTim/ValorantXPCalc?logo=github&style=for-the-badge "Contributors") ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/BitTim/ValorantXPCalc?logo=github&style=for-the-badge "Last commit")

## Installation
Download zip from ![here](https://github.com/BitTim/VexTrack/releases) and extract all contents in a folder. After opening VexTrack for the first time, a new folder "dat" will be created in the same folder the exe is in automatically.

## I updated and the program is crashing, what to do?
It may take me a while to fix the update, but once I do, you simply open the "version" file and replace the upper version number with the version number of the previous release (e.g. for v1.4 it would be v1.3). After that, start "Updater.exe" and let it update. Since you have no way of knowing if i fixed the update or not, retry that step until it works.

## Compilation
First of all, download and install the latest version of python from ![here](https://python.org).

To compile the binaries, you need the following packages:

    pip install tkinter
    pip install matplotlib
    pip install pyinstaller

When you have everything installed, create a new file called "tokenString.py" in src/ which should contain the following:

    TOKEN=<YOUR_GITHUB_PERSONAL_ACCESS_TOKEN>

Where <YOUR_GITHUB_PERSONAL_ACCESS_TOKEN> should be replaced by your personal access token, which can be generated ![here](https://github.com/settings/tokens). The only needed permissions are `repo:status` and `public_repo`.
After you have created tokenString.py, run these two commands to compile the binaries:

    pyinstaller --onefile --windowed --icon=VexTrack.ico --name=VexTrack --paths "./src" src/VexTrack.py
    pyinstaller --onefile --windowed --icon=VexTrack.ico --name=Updater --paths "./src" src/Updater.py

The compiled .exe files are in a new folder "dist". Move them to the desired location and move the file "version" for versions v1.1 - v1.4 or "version.json" for versions v1.5+ into the same folder as the .exe files
