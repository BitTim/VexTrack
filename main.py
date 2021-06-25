import tkinter as tk
from tkinter import Variable, ttk

import os
import datetime as dt
import json

root = tk.Tk()
root.title("Valorant XP Calculator")
root.geometry("640x480")
root.resizable(False, False)

# ================================
#  Daily XP Container
# ================================

dailyXPContainer = ttk.LabelFrame(root, text="Daily XP")
dailyXPContainer.pack(padx=8, pady=8)

# --------------------------------
#  Progress
# --------------------------------

progressContainer = ttk.Frame(dailyXPContainer)
progressContainer.pack(padx=8, pady=0)

bar = ttk.Progressbar(progressContainer, orient="horizontal", length=500, mode="determinate")
bar.pack(padx=8, pady=8, side=tk.LEFT)

bar["value"] = 35

percentageLabel = ttk.Label(progressContainer, text="35%")
percentageLabel.pack(padx=8, pady=8, side=tk.LEFT)

# --------------------------------
#  Collecetd
# --------------------------------

dailyCollectedContainer = ttk.Frame(dailyXPContainer)
dailyCollectedContainer.pack(padx=8, pady=0, side=tk.LEFT)

dailyCollectedTitle = ttk.Label(dailyCollectedContainer, text="Collected:")
dailyCollectedTitle.pack(padx=8, pady=8, side=tk.LEFT)

dailyCollectedLabel = ttk.Label(dailyCollectedContainer, text="99999")
dailyCollectedLabel.pack(padx=8, pady=8, side=tk.LEFT)

# --------------------------------
#  Remaining
# --------------------------------

dailyRemainingContainer = ttk.Frame(dailyXPContainer)
dailyRemainingContainer.pack(padx=8, pady=0, side=tk.LEFT)

dailyRemainingTitle = ttk.Label(dailyRemainingContainer, text="Remaining:")
dailyRemainingTitle.pack(padx=8, pady=8, side=tk.LEFT)

dailyRemainingLabel = ttk.Label(dailyRemainingContainer, text="99999")
dailyRemainingLabel.pack(padx=8, pady=8, side=tk.LEFT)

# ================================
#  Main Loop
# ================================

# root.mainloop()


NUM_BPLEVELS = 50
NUM_XP_PER_LEVEL = 750
LEVEL2_OFFSET = 500
NUM_XP_WEEK1 = 15600
WEEKLY_INCREMENT = 1

CONFIG_PATH = "config.json"

def cumulativeSum(index, offset, amount):
    value = 0
    for i in range(2, index + 1):
        value += amount * i + offset
    return value

def readConfig(path):
    with open(path, "rt") as f:
        config = json.loads(f.read())
    return config

def writeConfig(path, config):
    with open(path, "wt") as f:
        f.write(json.dumps(config, indent = 4, separators=(',', ': ')))

def init(reset=False):
    runInit = reset

    if not os.path.isfile(CONFIG_PATH):
        f = open(CONFIG_PATH, "x")
        f.close()
        runInit = True

    if not runInit and os.stat(CONFIG_PATH).st_size == 0:
        runInit = True

    if runInit:
        print("----------------[  Init  ]----------------")

        activeBPLevel = int(input("Current Battlepass Level: "))
        cXP = int(input("Current Level XP: "))
        remainingDays = int(input("Remaining Days: "))

        delta = dt.timedelta(remainingDays + 1)
        seasonEndDate = dt.datetime.today() + delta
        seasonEndDateStr = seasonEndDate.strftime("%d.%m.%Y")

        config = {"activeBPLevel": activeBPLevel, "cXP": cXP, "seasonEndDate": seasonEndDateStr, "history": []}
        writeConfig(CONFIG_PATH, config)
    else:
        config = readConfig(CONFIG_PATH)
    
    return config

def displayStats(config):
    seasonEndDate = dt.datetime.strptime(config["seasonEndDate"], "%d.%m.%Y")
    dateDelta = seasonEndDate - dt.datetime.now()
    remainingDays = dateDelta.days

    cLevelRemainingXP = LEVEL2_OFFSET + NUM_XP_PER_LEVEL * config["activeBPLevel"] - config["cXP"]
    totalXP = cumulativeSum(NUM_BPLEVELS, LEVEL2_OFFSET, NUM_XP_PER_LEVEL)
    collectedXP = cumulativeSum(config["activeBPLevel"] - 1, LEVEL2_OFFSET, NUM_XP_PER_LEVEL) + config["cXP"]
    remainingXP = totalXP - collectedXP

    dailyXP = round(remainingXP / remainingDays)

    xpToday = 0
    for t in config["history"]:
        if dt.date.fromtimestamp(t["time"]) != dt.date.today():
            continue
        xpToday += t["amount"]

    dailyXPRemaining = dailyXP - xpToday
    dailyProgress = round(xpToday / dailyXP * 100)

    print("----------------[ General XP ]----------------")
    print("Total XP:", totalXP)
    print("Collected XP:", collectedXP)
    print("Remaining XP:", remainingXP)
    print("Daily XP:", dailyXP)
    print("----------------[  Daily XP  ]----------------")
    print("XP Today:", xpToday)
    print("Daily XP Remaining:", dailyXPRemaining)
    print("Daily Progress:", str(dailyProgress) + "%")
    print("----------------[ Battlepass ]----------------")
    print("Last unlocked Battlepass Level:", config["activeBPLevel"] - 1)
    print("Active Battlepass Level:", config["activeBPLevel"])
    print("Remaining XP to next Level:", cLevelRemainingXP)
    print("Remaining Days:", remainingDays)
    print("----------------[  Tracking  ]----------------")
    
    for t in config["history"]:
        print("[" + dt.datetime.fromtimestamp(t["time"]).strftime("%H:%M %d.%m.%Y") + "]:", t["amount"], "XP")
    
    if len(config["history"]) == 0: print("Wow. Such empty. :)")

def trackXP(amount, config):
    config["history"].append({"time": dt.datetime.now().timestamp(), "amount": amount})
    config["cXP"] += amount
    print("Added ", amount, " XP")

    while config["cXP"] >= LEVEL2_OFFSET + NUM_XP_PER_LEVEL * config["activeBPLevel"]:
        config["cXP"] -= LEVEL2_OFFSET + NUM_XP_PER_LEVEL * config["activeBPLevel"]
        config["activeBPLevel"] += 1
        print("Congratulations! You unlocked Level", config["activeBPLevel"] - 1)
    
    writeConfig(CONFIG_PATH, config)

def printHelp():
    print("help           - Displays this page")
    print("stats          - Displays XP stats")
    print("track [amount] - Add [amount] XP to history")
    print("end            - Ends the application")

def main():
    config = init()

    while True:
        cmd = input("> ")
        args = cmd.split()

        if   args[0] == "help": printHelp()
        elif args[0] == "stats": displayStats(config)
        elif args[0] == "track": trackXP(int(args[1]), config)
        elif args[0] == "reset": config = init(True)
        elif args[0] == "end": break
        else: print("Command \"" + args[0] + "\" not found")

        print("")


main()