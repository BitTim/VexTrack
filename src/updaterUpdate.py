from tkinter import *
from tkinter import messagebox
import os
import requests
from vars import *

root = Tk()
root.withdraw()

def downloadNewUpdaterVersion(versionString):
    os.system("taskkill /f /im " + "Updater.exe")

    url = "https://github.com/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases/download/U" + versionString + "/" + "Updater.exe"
    r = requests.get(url)

    with open("Updater.exe", 'wb') as f:
        f.write(r.content)
    
    content = []
    with open(VERSION_PATH, 'r') as f:
        content = f.readlines()

    content[1] = versionString
    with open(VERSION_PATH, 'w') as f:
        f.writelines(content)

def checkNewUpdaterVersion():
    content = []
    with open(VERSION_PATH, 'r') as f:
        content = f.readlines()
    
    if len(content) < 2:
        content.append("v1.0")

        for i in range(0, len(content)):
            if i != len(content) - 1: content[i] += "\n"

        with open(VERSION_PATH, 'w') as f:
            f.writelines(content)
    
    versionString = content[1]

    versionNumber = versionString.split("v")[1]
    response = requests.get("https://api.github.com/repos/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases")
    releases = response.json()

    for r in releases:
        tokenized = r["name"].split()
        if tokenized[0] == "Updater":
            latestVersionString = tokenized[1]
            latestVersionNumber = latestVersionString.split("v")[1]        

            if versionNumber > latestVersionNumber:
                break

            if versionNumber < latestVersionNumber:
                messagebox.showinfo("Updater", "Updater is updating from version " + versionString + " to " + latestVersionString)
                downloadNewUpdaterVersion(latestVersionString)
                break
    
    root.destroy()