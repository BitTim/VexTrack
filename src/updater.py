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
    
    with open(VERSION_PATH, 'w') as f:
        f.write(versionString)

def restartProgram():
    os.startfile(EXE_FILENAME)
    root.destroy()

def checkNewVersion():
    response = requests.get("https://api.github.com/repos/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases/latest")
    latestVersionString = response.json()["tag_name"]

    with open(VERSION_PATH, 'r') as f:
        versionString = f.read() 

    versionNumber = versionString.split("v")[1]
    latestVersionNumber = latestVersionString.split("v")[1]

    if(versionNumber < latestVersionNumber):
        res = messagebox.askquestion("Updater", "A new version is available: " + latestVersionString + "\nDo you want to update?")
        if res == "yes":
            downloadNewVersion(latestVersionString)
            messagebox.showinfo("Updater", "Update complete, program will restart now")
        
            restartProgram()
    root.destroy()

checkNewVersion()