# VexTrack

![Status badge](https://img.shields.io/badge/Status-Stable-green?style=for-the-badge "Development Status") [![Language badge](https://img.shields.io/badge/Language-Python_3.8.2-inactive?logo=python&logoColor=ffffff&style=for-the-badge)](https://python.org "Language") 
![GitHub License](https://img.shields.io/github/license/BitTim/ValorantXPCalc?logo=github&style=for-the-badge "License")
![GitHub repo size](https://img.shields.io/github/repo-size/BitTim/ValorantXPCalc?logo=github&style=for-the-badge) ![GitHub contributors](https://img.shields.io/github/contributors/BitTim/ValorantXPCalc?logo=github&style=for-the-badge "Contributors") ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/BitTim/ValorantXPCalc?logo=github&style=for-the-badge "Last commit")

## Installation
Download zip from ![here](https://github.com/BitTim/VexTrack/releases) and extract all contents in a folder. After opening VexTrack for the first time, a new folder "dat" will be created in the same folder the exe is in automatically.

## Compilation
First of all, download and install the latest version of python from ![here](https://python.org).

Then install pyinstaller via this command: `pip install pyinstaller`

When you have both installed, create a new file called "tokenString.py" in src/ which should contain the following:

    TOKEN=<YOUR_GITHUB_PERSONAL_ACCESS_TOKEN>

Where <YOUR_GITHUB_PERSONAL_ACCESS_TOKEN> should be replaced by your personal access token, which can be generated ![here](https://github.com/settings/tokens). The only needed permissions are `repo:status` and `public_repo`.
After you have created tokenString.py, run these two commands to compile the binaries:

    pyinstaller --onefile --windowed --icon=VexTrack.ico src/VexTrack.py
    pyinstaller --onefile --windowed --icon=VexTrack.ico src/Updater.py

The compiled .exe files are in a new folder "dist". Move them to the desired location and move the file "version" into the same folder as the .exe files
