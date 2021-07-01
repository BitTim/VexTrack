from tkinter import messagebox
import os
import requests
from vars import *

def downloadNewVersion(versionString):
    url = "https://github.com/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases/download/" + versionString + "/" + EXE_FILENAME
    r = requests.get(url)

    with open(EXE_FILENAME, 'wb') as f:
        f.write(r.content) 

def restartProgram():
    os.startfile(EXE_FILENAME)
    exit()

def checkNewVersion():
    response = requests.get("https://api.github.com/repos/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases/latest")
    latestVersionString = response.json()["tag_name"]

    versionNumber = VERSION_STRING.split("v")[1]
    latestVersionNumber = latestVersionString.split("v")[1]

    if(versionNumber < latestVersionNumber):
        res = messagebox.askquestion("Updater", "A new version is available: " + latestVersionString + " Do you want to update?")
        if res == "yes":
            downloadNewVersion(latestVersionString)
            messagebox.showinfo("Updater", "Update complete, program will restart now")
            restartProgram()