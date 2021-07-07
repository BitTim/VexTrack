from sys import version
from tkinter import *
from tkinter import messagebox
import os
import requests
from . import changelogDiag, downloadDiag, legacy
from vars import *
from tokenString import *
import json
import time

root = Tk()
root.withdraw()

def downloadNewVersion(versionString, softwareName, legacyMode):
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
    
    if not legacyMode:
        content = []
        with open(VERSION_PATH, 'r') as f:
            content = json.loads(f.read())
        
        content[softwareName] = versionString
        with open(VERSION_PATH, 'w') as f:
            f.write(json.dumps(content, indent = 4, separators=(',', ': ')))
    else:
        content = []
        with open(OLD_VERSION_PATH, 'r') as f:
            content = f.readlines()
        
        content[LEGACY_VERSIONS.index(softwareName)] = versionString
        for i in range(0, len(content)):
            if i != len(content) - 1: content[i] += "\n"

        with open(OLD_VERSION_PATH, 'w') as f:
            f.writelines(content)

def restartProgram(softwareName):
    os.startfile(softwareName + ".exe")

def getVersionString(softwareName):
    legacyMode = False
    newVersion = {}
    ignoredVersions = []

    if not os.path.exists(VERSION_PATH):
        if os.path.exists(OLD_VERSION_PATH):
            version = []
            with open(OLD_VERSION_PATH, 'r') as f:
                version = f.readlines()
            
            for i in range(0, len(version)): version[i] = version[i].strip()
            legacyMode = legacy.checkLegacy(version)

            if not legacyMode:
                newVersion = {GITHUB_REPO: version[0], "Updater": version[1]}
                os.remove(OLD_VERSION_PATH)
        else:
            newVersion = {GITHUB_REPO: "v1.0", "Updater": "v1.0"}
        
        print(legacyMode, newVersion)

        if not legacyMode:
            with open(VERSION_PATH, 'w') as f:
                f.write(json.dumps(newVersion, indent = 4, separators=(',', ': ')))

    if legacyMode:
        with open(OLD_VERSION_PATH, 'r') as f:
            versionString = f.readlines()[LEGACY_VERSIONS.index(softwareName)]
    else:
        with open(VERSION_PATH, 'r') as f:
            versionFile = json.loads(f.read())
            versionString = versionFile[softwareName]
            
            if "ignored" in versionFile: ignoredVersions = versionFile["ignored"][softwareName]
    
    return versionString, legacyMode, ignoredVersions

def ignoreVersion(versionString, softwareName, legacyMode):
    if legacyMode: return

    versionFile = None
    with open(VERSION_PATH, 'r') as f:
        versionFile = json.loads(f.read())
    
    if not "ignored" in versionFile:
        versionFile["ignored"] = {APP_NAME: [], "Updater": []}
    
    versionFile["ignored"][softwareName].append(versionString)

    with open(VERSION_PATH, 'w') as f:
        f.write(json.dumps(versionFile, indent = 4, separators=(',', ': ')))

def checkNewVersion(softwareName):
    isNewVersion = False
    legacyMode = False

    versionString, legacyMode, ignoredVersions = getVersionString(softwareName)
    versionNumber = versionString.split("v")[1]
    
    response = requests.get("https://api.github.com/repos/" + GITHUB_USER + "/" + GITHUB_REPO + "/releases", headers={"Authorization": TOKEN})
    releases = response.json()

    if "message" in releases:
        messagebox.showerror("Ratelimit reached", "Updater could not fetch new version of " + softwareName + ":\nYou have reached your rate limit. Try again later")
        root.destroy()
        return False

    for r in releases:
        tokenized = r["name"].split()
        if tokenized[0] == softwareName:
            latestVersionString = tokenized[1]
            latestVersionNumber = latestVersionString.split("v")[1]

            if latestVersionString in ignoredVersions: break
            if versionNumber > latestVersionNumber: break

            if versionNumber < latestVersionNumber:
                res = messagebox.askquestion("Updater", "A new version of " + softwareName + " is available: " + latestVersionString + "\nDo you want to update?")
                if res == "yes":
                    downloadNewVersion(latestVersionString, softwareName, legacyMode)

                    changelogRaw = r["body"].split("##")[1].split("\r\n")
                    changelog = []
                    for c in changelogRaw[1:]:
                        if c != "": changelog.append(c)

                    changelogDiag.ChangelogDiag(root, "Changelog", changelog)
        
                    isNewVersion = True
        
                    restartProgram(softwareName)
                else:
                    ignoreVersion(latestVersionString, softwareName, legacyMode)
                break
    
    root.destroy()
    return isNewVersion