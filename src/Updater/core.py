from sys import version
from tkinter import *
from tkinter import messagebox
import os
import requests
from Updater import changelogDiag, downloadDiag
from vars import *
from tokenString import *
import json
import time

root = Tk()
root.withdraw()

def downloadNewVersion(versionString, softwareName):
    os.system("taskkill /f /im " + softwareName + ".exe")

    url = "https://github.com/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases/download/" + versionString + "/" + softwareName + ".exe"
    r = requests.get(url, stream=True)

    downDiag = downloadDiag.DownloadDiag(root, "Downloading " + softwareName + " " + versionString)
    while downDiag == None: pass

    with open(softwareName + ".exe", 'wb') as f:
        startTime = time.mktime(time.localtime())
        downloaded = 0
        total = int(r.headers.get('content-length'))

        for chunk in r.iter_content(chunk_size=max(int(total/1000), 1024*1024)):
            if chunk:
                downloaded += len(chunk)
                f.write(chunk)
                cTime = time.mktime(time.localtime())

                downDiag.updateValues(downloaded, total, 0 if cTime == startTime else round(downloaded / (cTime - startTime), 2))
    
    downDiag.destroy()
    
    content = []
    with open(VERSION_PATH, 'r') as f:
        content = json.loads(f.read())
    
    content[softwareName] = versionString
    with open(VERSION_PATH, 'w') as f:
        f.write(json.dumps(content, indent = 4, separators=(',', ': ')))

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

                    changelogRaw = r["body"].split("##")[1].split("\r\n")
                    changelog = []
                    for c in changelogRaw[1:]:
                        if c != "": changelog.append(c)

                    changelogDiag.ChangelogDiag(root, "Changelog", changelog)
        
                    isNewVersion = True
        
                    restartProgram(softwareName)
                    break
    
    root.destroy()
    return isNewVersion