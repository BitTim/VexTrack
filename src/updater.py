from tkinter import *
from tkinter import messagebox
import os
import requests
from vars import *

root = Tk()
root.withdraw()

def downloadNewVersion(versionString):
    os.system("taskkill /f /im " + EXE_FILENAME)

    url = "https://github.com/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases/download/" + versionString + "/" + EXE_FILENAME
    r = requests.get(url)

    with open(EXE_FILENAME, 'wb') as f:
        f.write(r.content)
    
    content = []
    with open(VERSION_PATH, 'r') as f:
        content = f.readlines()
    
    content[0] = versionString
    for i in range(0, len(content)):
        if i != len(content) - 1: content[i] += "\n"
    
    with open(VERSION_PATH, 'w') as f:
        f.writelines(content)

def restartProgram():
    os.startfile(EXE_FILENAME)

def checkNewVersion():
    with open(VERSION_PATH, 'r') as f:
        versionString = f.readlines()[0] 

    versionNumber = versionString.split("v")[1]
    
    response = requests.get("https://api.github.com/repos/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases")
    releases = response.json()

    for r in releases:
        tokenized = r["name"].split()
        if tokenized[0] == GITHUB_REPO:
            latestVersionString = tokenized[1]
            latestVersionNumber = latestVersionString.split("v")[1]

            if versionNumber > latestVersionNumber:
                break

            if versionNumber < latestVersionNumber:
                res = messagebox.askquestion("Updater", "A new version is available: " + latestVersionString + "\nDo you want to update?")
                if res == "yes":
                    downloadNewVersion(latestVersionString)
                    messagebox.showinfo("Updater", "Update complete, program will restart now")
        
                    restartProgram()
                    break
    root.destroy()

checkNewVersion()