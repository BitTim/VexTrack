from tkinter import *
from tkinter import messagebox
import os
import requests
from vars import *
from tokenString import *
import json

root = Tk()
root.withdraw()

def downloadNewVersion(versionString, softwareName):
    os.system("taskkill /f /im " + softwareName + ".exe")

    url = "https://github.com/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases/download/" + versionString + "/" + softwareName + ".exe"
    r = requests.get(url)

    with open(softwareName + ".exe", 'wb') as f:
        f.write(r.content)
    
    content = []
    with open(VERSION_PATH, 'r') as f:
        content = f.readlines()
    
    content[0] = versionString
    for i in range(0, len(content)):
        if i != len(content) - 1: content[i] += "\n"
    
    with open(VERSION_PATH, 'w') as f:
        f.writelines(content)

def restartProgram(softwareName):
    os.startfile(softwareName + ".exe")

def checkNewVersion(softwareName):
    isNewVersion = False

    # Update old version file
    if not os.path.exists(VERSION_PATH):
        if os.path.exists(OLD_VERSION_PATH):
            version = []
            with open(OLD_VERSION_PATH, 'r') as f:
                version = f.readlines()
            
            for i in range(0, len(version)): version[i] = version[i].strip()

            newVersion = {GITHUB_REPO: version[0], "Updater": version[1]}
            os.remove(OLD_VERSION_PATH)
        else:
            newVersion = {GITHUB_REPO: "v1.0", "Updater": "v1.0"}
        
        with open(VERSION_PATH, 'w') as f:
            f.write(json.dumps(newVersion, indent = 4, separators=(',', ': ')))

    with open(VERSION_PATH, 'r') as f:
        versionString = json.loads(f.read())[softwareName]

    versionNumber = versionString.split("v")[1]
    
    response = requests.get("https://api.github.com/repos/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases", headers={"Authorization": TOKEN})
    releases = response.json()

    for r in releases:
        tokenized = r["name"].split()
        if tokenized[0] == softwareName:
            latestVersionString = tokenized[1]
            latestVersionNumber = latestVersionString.split("v")[1]

            if versionNumber > latestVersionNumber:
                break

            if versionNumber < latestVersionNumber:
                res = messagebox.askquestion("Updater", "A new version of " + softwareName + " is available: " + latestVersionString + "\nDo you want to update?")
                if res == "yes":
                    downloadNewVersion(latestVersionString, softwareName)
                    messagebox.showinfo("Updater", "Update complete, program will restart now")
        
                    isNewVersion = True
        
                    restartProgram(softwareName)
                    break
    
    root.destroy()
    return isNewVersion