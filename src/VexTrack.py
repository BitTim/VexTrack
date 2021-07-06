import tkinter as tk
from vextrackLib.initDiag import InitDiag
from vextrackLib.scrollableFrame import ScrollableFrame
import vars
import json
import os

from vextrackLib import core, addXPDiag as xpDiag, addGoalDiag as goalDiag, goalContainer
from updaterLib import core as uCore

from tkinter import *
from tkinter import ttk, messagebox
from datetime import *

import matplotlib
matplotlib.use("TkAgg")
from matplotlib.figure import Figure
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from matplotlib.patches import Rectangle

windowSize = vars.WINDOW_GEOMETRY.split("x")
newUpdaterVersion = uCore.checkNewVersion("Updater")

with open(vars.VERSION_PATH, 'r') as f:
    versionString = json.loads(f.read())[vars.APP_NAME]

root = tk.Tk()
root.title(vars.APP_NAME + " " + versionString)
root.iconbitmap("VexTrack.exe")
root.geometry(vars.WINDOW_GEOMETRY)
root.minsize(int(windowSize[0]), int(windowSize[1]))

if newUpdaterVersion == True:
    root.iconify()
    root.update()
    root.deiconify()

# ================================
#  Tabbed View
# ================================

notebook = ttk.Notebook(root)
graphTab = Frame(notebook)
statsTab = Frame(notebook)
historyTab = Frame(notebook)

notebook.add(graphTab, text="Graph")
notebook.add(statsTab, text="Stats")
notebook.add(historyTab, text="History")
notebook.pack(padx=8, pady=8, fill="both", expand=True)

# --------------------------------
#  Graph
# --------------------------------

figure = Figure(figsize=(5, 4), dpi=100, tight_layout=True)
graphPlot = figure.add_subplot(1, 1, 1)

graphPlot.set_frame_on(False)
graphPlot.margins(x=0, y=0)

graph = FigureCanvasTkAgg(figure, graphTab)
graph.get_tk_widget().pack(fill="both", expand=True)

graphPlot.tick_params(left = False, right = False, labelleft = False, labelbottom = False, bottom = False)

# --------------------------------
#  Stats
# --------------------------------    

statsScrollableContainer = ScrollableFrame(statsTab)
statsContainer = statsScrollableContainer.scrollableFrame
statsScrollableContainer.pack(fill="both", expand=True)

miscStatsContainer = ttk.LabelFrame(statsContainer, text="Miscellaneous")
miscStatsContainer.pack(padx=8, pady=8, fill="x")

miscContentContainer = ttk.Frame(miscStatsContainer)
miscContentContainer.pack(padx=8, pady=8, fill="x")

ttk.Label(miscContentContainer, text="Remaining Days:", font=('TkDefaultFont', 9,'bold')).grid(column=0, row=0, sticky="w", padx=8)

miscRemainingDaysLabel = ttk.Label(miscContentContainer, text="90")
miscRemainingDaysLabel.grid(column=1, row=0, sticky="w", padx=8)

ttk.Label(miscContentContainer, text="Average XP per Day:", font=('TkDefaultFont', 9,'bold')).grid(column=3, row=0, sticky="w", padx=8)

miscAverageLabel = ttk.Label(miscContentContainer, text="99999 XP")
miscAverageLabel.grid(column=4, row=0, sticky="w", padx=8)

ttk.Label(miscContentContainer, text="Deviation from ideal:", font=('TkDefaultFont', 9,'bold')).grid(column=0, row=1, sticky="w", padx=8)

miscIdealDeviationLabel = ttk.Label(miscContentContainer, text="99999 XP")
miscIdealDeviationLabel.grid(column=1, row=1, sticky="w", padx=8)

ttk.Label(miscContentContainer, text="Deviation from daily ideal:", font=('TkDefaultFont', 9,'bold')).grid(column=3, row=1, sticky="w", padx=8)

miscDailyDeviationLabel = ttk.Label(miscContentContainer, text="99999 XP")
miscDailyDeviationLabel.grid(column=4, row=1, sticky="w", padx=8)

ttk.Label(miscContentContainer, text="Strongest day: ", font=('TkDefaultFont', 9,'bold')).grid(column=0, row=2, sticky="w", padx=8)

miscStrongestDayDateLabel = ttk.Label(miscContentContainer, text="01.01.1970")
miscStrongestDayDateLabel.grid(column=1, row=2, sticky="w", padx=8)

miscStrongestDayAmountLabel = ttk.Label(miscContentContainer, text="99999 XP")
miscStrongestDayAmountLabel.grid(column=2, row=2, sticky="w", padx=8)

ttk.Label(miscContentContainer, text="Weakest day:", font=('TkDefaultFont', 9,'bold')).grid(column=3, row=2, sticky="w", padx=8)

miscWeakestDayDateLabel = ttk.Label(miscContentContainer, text="01.01.1970")
miscWeakestDayDateLabel.grid(column=4, row=2, sticky="w", padx=8)

miscWeakestDayAmountLabel = ttk.Label(miscContentContainer, text="99999 XP")
miscWeakestDayAmountLabel.grid(column=5, row=2, sticky="w", padx=8)

# --------------------------------
#  Total XP
# --------------------------------

totalXPContainer = ttk.LabelFrame(statsContainer, text="Total XP")
totalXPContainer.pack(padx=8, pady=8, fill="x")

# ................................
#  Progress
# ................................

totalProgressContainer = ttk.Frame(totalXPContainer)
totalProgressContainer.pack(padx=8, pady=0)

totalXPPercentageLabel = ttk.Label(totalProgressContainer)
totalXPPercentageLabel.pack(padx=8, pady=8, side=tk.RIGHT, fill="x", expand=True)

totalXPBar = ttk.Progressbar(totalProgressContainer, orient="horizontal", length=10000, mode="determinate")
totalXPBar.pack(padx=1, pady=8, side=tk.RIGHT, fill="x")

totalXPBar["value"] = 35
totalXPPercentageLabel["text"] = str(totalXPBar["value"]) + "%"


# ................................
#  Collecetd
# ................................

totalCollectedContainer = ttk.Frame(totalXPContainer)
totalCollectedContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(totalCollectedContainer, text="Collected:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

totalCollectedLabel = ttk.Label(totalCollectedContainer, text="9999999 XP")
totalCollectedLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Remaining
# ................................

totalRemainingContainer = ttk.Frame(totalXPContainer)
totalRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(totalRemainingContainer, text="Remaining:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

totalRemainingLabel = ttk.Label(totalRemainingContainer, text="9999999 XP")
totalRemainingLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Total
# ................................

totalTotalContainer = ttk.Frame(totalXPContainer)
totalTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(totalTotalContainer, text="Total:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

totalTotalLabel = ttk.Label(totalTotalContainer, text="9999999 XP")
totalTotalLabel.pack(padx=1, pady=0, side=tk.LEFT)

# --------------------------------
#  Battlepass
# --------------------------------

bpContainer = ttk.LabelFrame(statsContainer, text="Battlepass")
bpContainer.pack(padx=8, pady=8, fill="x")

# ................................
#  Progress
# ................................

bpProgressContainer = ttk.Frame(bpContainer)
bpProgressContainer.pack(padx=8, pady=0)

bpPercentageLabel = ttk.Label(bpProgressContainer)
bpPercentageLabel.pack(padx=8, pady=8, side=tk.RIGHT, fill="x", expand=True)

bpBar = ttk.Progressbar(bpProgressContainer, orient="horizontal", length=10000, mode="determinate")
bpBar.pack(padx=8, pady=8, side=tk.RIGHT, fill="x")

bpBar["value"] = 35
bpPercentageLabel["text"] = str(bpBar["value"]) + "%"

# ................................
#  Collecetd
# ................................

bpPreviousUnlockContainer = ttk.Frame(bpContainer)
bpPreviousUnlockContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(bpPreviousUnlockContainer, text="Previous Unlock:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

bpPreviousUnlockLabel = ttk.Label(bpPreviousUnlockContainer, text="54")
bpPreviousUnlockLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Active
# ................................

bpActiveContainer = ttk.Frame(bpContainer)
bpActiveContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(bpActiveContainer, text="Active:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

bpActiveLabel = ttk.Label(bpActiveContainer, text="55")
bpActiveLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Remaining
# ................................

bpRemainingContainer = ttk.Frame(bpContainer)
bpRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(bpRemainingContainer, text="Remaining:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

bpRemainingLabel = ttk.Label(bpRemainingContainer, text="54")
bpRemainingLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Total
# ................................

bpTotalContainer = ttk.Frame(bpContainer)
bpTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(bpTotalContainer, text="Total:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

bpTotalLabel = ttk.Label(bpTotalContainer, text="55")
bpTotalLabel.pack(padx=1, pady=0, side=tk.LEFT)

# --------------------------------
#  Level
# --------------------------------

levelContainer = ttk.LabelFrame(statsContainer, text="Active Level")
levelContainer.pack(padx=8, pady=8, fill="x")

# ................................
#  Progress
# ................................

levelProgressContainer = ttk.Frame(levelContainer)
levelProgressContainer.pack(padx=8, pady=0)

levelPercentageLabel = ttk.Label(levelProgressContainer)
levelPercentageLabel.pack(padx=8, pady=8, side=tk.RIGHT, fill="x", expand=True)

levelBar = ttk.Progressbar(levelProgressContainer, orient="horizontal", length=10000, mode="determinate")
levelBar.pack(padx=8, pady=8, side=tk.RIGHT, fill="x")

levelBar["value"] = 35
levelPercentageLabel["text"] = str(levelBar["value"]) + "%"


# ................................
#  Collecetd
# ................................

levelCollectedContainer = ttk.Frame(levelContainer)
levelCollectedContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(levelCollectedContainer, text="Collected:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

levelCollectedLabel = ttk.Label(levelCollectedContainer, text="37999")
levelCollectedLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Remaining
# ................................

levelRemainingContainer = ttk.Frame(levelContainer)
levelRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(levelRemainingContainer, text="Remaining:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

levelRemainingLabel = ttk.Label(levelRemainingContainer, text="38000")
levelRemainingLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Total
# ................................

levelTotalContainer = ttk.Frame(levelContainer)
levelTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(levelTotalContainer, text="Total:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

levelTotalLabel = ttk.Label(levelTotalContainer, text="38000")
levelTotalLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ................................
#  Goals
# ................................

goalContainers = []

# --------------------------------
#  History
# --------------------------------

historyListContainer = tk.Frame(historyTab)
historyListContainer.pack(fill="both", expand=True)

historyStyle = ttk.Style()
historyStyle.configure("history.Treeview", highlightthickness=0, bd=0, font=('TkDefaultFont', 10))
historyStyle.configure("history.Treeview.Heading", font=('TkDefaultFont', 10,'bold'))
historyStyle.layout("history.Treeview", [('history.Treeview.treearea', {'sticky': 'nswe'})])

history = ttk.Treeview(historyListContainer, columns=(1, 2, 3), show="headings", height="16", style="history.Treeview")
history.pack(side=tk.LEFT, fill="both", expand=True)

history.heading(1, text="Description", anchor="w")
history.heading(2, text="Amount", anchor="e")
history.heading(3, text="Timestamp", anchor="e")

history.column(1, anchor="w")
history.column(2, anchor="e")
history.column(3, anchor="e")

# Create history Scrollbar
historyScrollbar = Scrollbar(historyListContainer, orient=VERTICAL, command=history.yview)
historyScrollbar.pack(side=tk.LEFT, fill="y")

history.configure(yscrollcommand=historyScrollbar.set)

historyBtnContainer = tk.Frame(historyTab)
historyBtnContainer.pack(fill="x")

# ================================
#  Daily XP Container
# ================================

dailyXPContainer = ttk.LabelFrame(root, text="Daily XP")
dailyXPContainer.pack(padx=8, pady=8, fill="x")

# --------------------------------
#  Progress
# --------------------------------

dailyProgressContainer = ttk.Frame(dailyXPContainer)
dailyProgressContainer.pack(padx=8, pady=0)

dailyPercentageLabel = ttk.Label(dailyProgressContainer, text="0%")
dailyPercentageLabel.pack(padx=8, pady=8, side=tk.RIGHT, fill="x", expand=True)

dailyBar = ttk.Progressbar(dailyProgressContainer, orient="horizontal", length=10000, mode="determinate", value=0)
dailyBar.pack(padx=8, pady=8, side=tk.RIGHT, fill="x")


# --------------------------------
#  Collecetd
# --------------------------------

dailyCollectedContainer = ttk.Frame(dailyXPContainer)
dailyCollectedContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(dailyCollectedContainer, text="Collected:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

dailyCollectedLabel = ttk.Label(dailyCollectedContainer, text="99999")
dailyCollectedLabel.pack(padx=1, pady=0, side=tk.LEFT)

# --------------------------------
#  Remaining
# --------------------------------

dailyRemainingContainer = ttk.Frame(dailyXPContainer)
dailyRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(dailyRemainingContainer, text="Remaining:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

dailyRemainingLabel = ttk.Label(dailyRemainingContainer, text="99999")
dailyRemainingLabel.pack(padx=1, pady=0, side=tk.LEFT)

# --------------------------------
#  Total
# --------------------------------

dailyTotalContainer = ttk.Frame(dailyXPContainer)
dailyTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(dailyTotalContainer, text="Total:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=tk.LEFT)

dailyTotalLabel = ttk.Label(dailyTotalContainer, text="99999")
dailyTotalLabel.pack(padx=1, pady=0, side=tk.LEFT)

# ================================
#  Functions
# ================================

# --------------------------------
#  Callbacks
# --------------------------------

def addXPCallback():
    addXPDiag = xpDiag.AddXPDiag(root, "Add XP")

    if addXPDiag.description != None and addXPDiag.xpAmount != None:
        addXP(addXPDiag.description, int(addXPDiag.xpAmount))
    
    updateValues()

def addGoalCallback():
    addGoalDiag = goalDiag.AddGoalDiag(root, "Add Goal")

    if addGoalDiag.name != None and addGoalDiag.xpAmount != None and addGoalDiag.color != None:
        addGoal(addGoalDiag.name, int(addGoalDiag.xpAmount), addGoalDiag.color)
    
    updateValues()

def resetCallback():
    res = messagebox.askquestion("Reset config", "Are you sure you want to reset your config? This will remove all progress")
    if res == "yes":
        initDiag = InitDiag(root, "Reset Config")

        if initDiag.activeBPLevel != None and initDiag.cXP != None and initDiag.remainingDays != None:
            initConfig(int(initDiag.activeBPLevel), int(initDiag.cXP), int(initDiag.remainingDays))
        else:
            messagebox.showinfo("Reset Config", "Cancelled, previous config has not been removed")

        updateValues()
        return
    return

def editElementCallback():
    currElementFocus = history.focus()
    currElement = history.item(currElementFocus)
    currElementID = history.index(currElementFocus)

    if len(currElement["values"]) == 0:
        return

    editDiag = xpDiag.AddXPDiag(root, "Edit Element", currElement["values"][0], currElement["values"][1].strip(" XP"))
    if editDiag.description != currElement["values"][0] or editDiag.xpAmount != currElement["values"][1].strip(" XP"):
        editElement(currElementID, editDiag.description, int(editDiag.xpAmount))

def deleteElementCallback():
    currElementFocus = history.focus()
    currElement = history.item(currElementFocus)
    currElementID = history.index(currElementFocus)

    if len(currElement["values"]) == 0:
        return

    res = messagebox.askquestion("Delete Element", "Are you sure, that you want to delete the selected element?\nDescription: " + currElement["values"][0] + "\nAmount: " + currElement["values"][1])
    if res == "yes":
        deleteElement(currElementID)

def gcRemoveCallback(index):
    res = messagebox.askquestion("Remove Goal", "Are you sure, that you want to remove this goal?\nName: " + goalContainers[index].name)
    if res == "yes":
        gcRemove(index)

def gcEditCallback(index):
    editDiag = goalDiag.AddGoalDiag(root, "Edit Goal", name=goalContainers[index].name, amount=goalContainers[index].amount, color=goalContainers[index].color, edit=True)
    if editDiag.changeStartXP != None and (editDiag.name != goalContainers[index].name or editDiag.xpAmount != goalContainers[index].amount or editDiag.color != goalContainers[index].color or editDiag.changeStartXP == True):
        gcEdit(index, editDiag.changeStartXP, editDiag.name, int(editDiag.xpAmount), editDiag.color)

# --------------------------------
#  Init
# --------------------------------

def init():
    runInit = False

    if not os.path.exists(os.path.dirname(vars.CONFIG_PATH)):
        os.mkdir(os.path.dirname(vars.CONFIG_PATH))
        runInit = True

    if not os.path.exists(vars.CONFIG_PATH):
        f = open(vars.CONFIG_PATH, "x")
        f.close()
        runInit = True

    if not runInit and os.stat(vars.CONFIG_PATH).st_size == 0:
        runInit = True
    
    if runInit:
        initDiag = InitDiag(root, "Initialization")

        if initDiag.activeBPLevel== None and initDiag.cXP == None and initDiag.remainingDays == None:
            messagebox.showerror("Initialization", "Initial config has to be created")
            exit()
        
        initConfig(int(initDiag.activeBPLevel), int(initDiag.cXP), int(initDiag.remainingDays))

# --------------------------------
#  Update Values
# --------------------------------

def initConfig(activeBPLevel, cXP, remainingDays):
    delta = timedelta(int(remainingDays) + 1)
    seasonEndDate = datetime.today() + delta
    seasonEndDateStr = seasonEndDate.strftime("%d.%m.%Y")

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues({"activeBPLevel": int(activeBPLevel), "cXP": int(cXP)}, 0)

    config = {"activeBPLevel": int(activeBPLevel), "cXP": int(cXP), "seasonEndDate": seasonEndDateStr, "history": [{"time": datetime.now().timestamp(), "description": "Initialization", "amount": int(totalXPCollected)}]}
    core.writeConfig(vars.CONFIG_PATH, config)

def addXP(description, amount):
    config = core.readConfig(vars.CONFIG_PATH)
    config["history"].append({"time": datetime.now().timestamp(), "description": description, "amount": int(amount)})
    config["cXP"] += amount

    while config["cXP"] >= vars.LEVEL2_OFFSET + vars.NUM_XP_PER_LEVEL * config["activeBPLevel"]:
        config["cXP"] -= vars.LEVEL2_OFFSET + vars.NUM_XP_PER_LEVEL * config["activeBPLevel"]
        config["activeBPLevel"] += 1
        messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(config["activeBPLevel"] - 1))

    if not "goals" in config: config["goals"] = []

    for i in range(0, len(config["goals"])):
        completed = False
        if config["goals"][i]["remaining"] <= 0: completed = True

        config["goals"][i]["remaining"] -= amount

        if config["goals"][i]["remaining"] <= 0 and not completed:
            messagebox.showinfo("Congratulations", "Congratulations! You have completed the goal: " + str(config["goals"][i]["name"]))
        
    core.writeConfig(vars.CONFIG_PATH, config)

def addGoal(name, amount, color):
    config = core.readConfig(vars.CONFIG_PATH)

    if not "goals" in config:
        config["goals"] = []

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(config, epilogueVar.get())
    config["goals"].append({"name": name, "remaining": amount, "startXP": totalXPCollected,"color": color})
    core.writeConfig(vars.CONFIG_PATH, config)

def editElement(index, description, amount):
    config = core.readConfig(vars.CONFIG_PATH)
    historyLen = len(config["history"])

    prevBPLevel = config["activeBPLevel"]

    index = historyLen - 1 - index
    prevAmount = config["history"][index]["amount"]
    config["history"][index]["description"] = description
    config["history"][index]["amount"] = amount

    if not "goals" in config: config["goals"] = []
    completedGoals = []
    for i in range(0, len(config["goals"])):
        if config["goals"][i]["remaining"] <= 0: completedGoals.append(i)
        config["goals"][i]["remaining"] -= amount - prevAmount

    config = core.recalcXP(config)
    deltaBP = prevBPLevel - config["activeBPLevel"]
    
    if deltaBP > 0:
        for i in range(0, deltaBP, 1):
            messagebox.showinfo("Sorry", "You have lost Battlepass Level " + str(prevBPLevel - i - 1))
    elif deltaBP < 0:
        for i in range(0, deltaBP, -1):
            messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(-1 * i + prevBPLevel))

    for cg in completedGoals:
        if config["goals"][cg]["remaining"] > 0:
            messagebox.showinfo("Sorry", "You have no longer completed the goal: " + config["goals"][cg]["name"])


    core.writeConfig(vars.CONFIG_PATH, config)
    updateValues()

def deleteElement(index):
    config = core.readConfig(vars.CONFIG_PATH)
    historyLen = len(config["history"])

    prevBPLevel = config["activeBPLevel"]

    index = historyLen - 1 - index
    amount = config["history"][index]["amount"]
    config["history"].pop(index)

    if not "goals" in config: config["goals"] = []
    completedGoals = []
    for i in range(0, len(config["goals"])):
        if config["goals"][i]["remaining"] <= 0: completedGoals.append(i)
        config["goals"][i]["remaining"] += amount

    config = core.recalcXP(config)
    deltaBP = prevBPLevel - config["activeBPLevel"]

    if deltaBP > 0:
        for i in range(0, deltaBP, 1):
            messagebox.showinfo("Sorry", "You have lost Battlepass Level " + str(prevBPLevel - i - 1))
    elif deltaBP < 0:
        for i in range(0, deltaBP, -1):
            messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(-1 * i + prevBPLevel))

    for cg in completedGoals:
        if config["goals"][cg]["remaining"] > 0:
            messagebox.showinfo("Sorry", "You have no longer completed the goal: " + config["goals"][cg]["name"])

    core.writeConfig(vars.CONFIG_PATH, config)
    updateValues()

def gcRemove(index):
    config = core.readConfig(vars.CONFIG_PATH)

    config["goals"].pop(index)
    goalContainers[index].destroy()
    goalContainers.pop(index)

    core.writeConfig(vars.CONFIG_PATH, config)
    updateValues()

def gcEdit(index, changeStartXP, name, amount, color):
    config = core.readConfig(vars.CONFIG_PATH)
    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(config, epilogueVar.get())

    completed = False
    if config["goals"][index]["remaining"] <= 0: completed = True

    config["goals"][index]["name"] = name
    if changeStartXP: config["goals"][index]["startXP"] = totalXPCollected
    config["goals"][index]["remaining"] = amount
    config["goals"][index]["color"] = color

    goalContainers[index].updateGoal(name, amount, color)

    if config["goals"][index]["remaining"] > 0 and completed:
        messagebox.showinfo("Sorry", "You have no longer completed the goal: " + config["goals"][index]["name"])
    elif config["goals"][index]["remaining"] <= 0 and not completed:
        messagebox.showinfo("Congratulations", "Congratulations! You have completed the goal: " + str(config["goals"][index]["name"]))

    core.writeConfig(vars.CONFIG_PATH, config)
    updateValues()

def updateGoals(config, plot, collectedXP):
    if not "goals" in config:
        config["goals"] = []

    for i in range(0, len(config["goals"])):
        alpha = 0.75

        if collectedXP + config["goals"][i]["remaining"] <= 0: continue
        if config["goals"][i]["remaining"] <= 0: alpha = 0.15

        plot.axhline(collectedXP + config["goals"][i]["remaining"], color=config["goals"][i]["color"], alpha=alpha, linestyle="-")
        plot.annotate(config["goals"][i]["name"], (0, collectedXP + config["goals"][i]["remaining"]), xytext=(-3, collectedXP + config["goals"][i]["remaining"] + 5000), color=config["goals"][i]["color"], alpha=alpha)

        if i >= len(goalContainers):
            gc = goalContainer.GoalContainer(statsContainer, config["goals"][i]["name"], config["goals"][i]["remaining"], color=config["goals"][i]["color"])
            gc.pack(padx=8, pady=8, fill="x")
            goalContainers.append(gc)
        
        collectedInGoal = collectedXP - config["goals"][i]["startXP"]
        if collectedInGoal < 0:
            config["goals"][i]["startXP"] += collectedInGoal
            collectedInGoal = 0

        totalInGoal = collectedInGoal + config["goals"][i]["remaining"]

        goalProgress = round(collectedInGoal / totalInGoal * 100)
        if goalProgress > 100: goalProgress = 100

        goalContainers[i].setValues(goalProgress, collectedInGoal, config["goals"][i]["remaining"] if config["goals"][i]["remaining"] > 0 else 0, totalInGoal)
        goalContainers[i].removeBtn.configure(command=lambda j=i: gcRemoveCallback(j))
        goalContainers[i].editBtn.configure(command=lambda j=i: gcEditCallback(j))

def updateGraph(config, epilogue, plot):
    plot.clear()
    timeAxis = []
    
    seasonEndDate = datetime.strptime(config["seasonEndDate"], "%d.%m.%Y")
    dateDelta = seasonEndDate - datetime.fromtimestamp(config["history"][0]["time"])
    duration = dateDelta.days
    timeAxis = range(0, duration + 1)

    yAxisIdeal = []

    totalXP = core.cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + epilogue * vars.NUM_EPLOGUE_LEVELS * vars.NUM_EPLOGUE_XP_PER_LEVEL
    collectedXP = config["history"][0]["amount"]
    
    remainingXP = totalXP - collectedXP
    originalDaily = round(remainingXP / (duration - vars.BUFFER_DAYS))

    yAxisIdeal.append(collectedXP)

    for i in range(1, duration + 1):
        yAxisIdeal.append(yAxisIdeal[i - 1] + originalDaily)
        if yAxisIdeal[i] > totalXP: yAxisIdeal[i] = totalXP

    plot.plot(timeAxis, yAxisIdeal, color='gray', label='Ideal', linestyle="-")

    yAxisYou = []
    prevDate = date.fromtimestamp(config["history"][0]["time"])
    index = -1

    for h in config["history"]:
        currDate = date.fromtimestamp(h["time"])
        if currDate == prevDate and index != -1:
            yAxisYou[index] = yAxisYou[index] + int(h["amount"])
        else:
            deltaDate = (currDate - prevDate)
            prevDate = currDate

            if index != -1: prevValue = yAxisYou[index]
            else: prevValue = 0

            for i in range(1, deltaDate.days):
                yAxisYou.append(prevValue)
                index += 1

            yAxisYou.append(int(h["amount"]) + prevValue)
            index += 1

    deltaDate = date.today() - prevDate
    for i in range(0, deltaDate.days): yAxisYou.append(yAxisYou[len(yAxisYou) - 1])

    yAxisDailyIdeal = []

    dateDelta = seasonEndDate - datetime.now()
    remainingDays = dateDelta.days
    dayDelta = duration - remainingDays

    if date.fromtimestamp(config["history"][len(config["history"]) - 1]["time"]) == date.today(): offset = 1
    else: offset = 0

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(config, epilogue)
    dailyTotal = round((totalXPTotal - yAxisYou[index - offset]) / (remainingDays - vars.BUFFER_DAYS + 1))

    yAxisDailyIdeal.append(yAxisYou[index - offset])

    for i in range(1, remainingDays + 2):
        yAxisDailyIdeal.append(yAxisDailyIdeal[i - 1] + dailyTotal)
        if yAxisDailyIdeal[i] > totalXP: yAxisDailyIdeal[i] = totalXP

    plot.plot(timeAxis[dayDelta - 1:], yAxisDailyIdeal, color='skyblue', label='Daily Ideal', alpha=1, linestyle="--")
    plot.plot(timeAxis[:len(yAxisYou)], yAxisYou, color='red', label='You', linewidth=3)
    plot.plot(dayDelta, totalXPCollected, color='darkred', label="Now", marker="o", markersize=5)

    plot.set_xticks(range(0, duration + 1, 5), minor=True)
    plot.tick_params(which="both", top=False, bottom=False, left=False, right=False, labelleft=False)
    plot.grid(axis="x", color="lightgray", which="both", linestyle=":")

    # --------------------------------
    # Draw lines for significant unlocks
    # --------------------------------

    for i in range(0, vars.NUM_BPLEVELS + 1, 1):
        plot.axhline(core.cumulativeSum(i, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL), color="gray", alpha=0.05, linestyle="-")

    for i in range(0, vars.NUM_BPLEVELS + 1, 5):
        plot.axhline(core.cumulativeSum(i, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL), color="green", alpha=0.15, linestyle="-")
    
    if epilogue:
        for i in range(1, vars.NUM_EPLOGUE_LEVELS + 1, 1):
            plot.axhline(core.cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + i * vars.NUM_EPLOGUE_XP_PER_LEVEL, color="green", alpha=0.15, linestyle="-")
    
    updateGoals(config, plot, totalXPCollected)

    # --------------------------------
    # Draw Buffer Zone
    # --------------------------------

    plot.add_patch(Rectangle((duration - vars.BUFFER_DAYS, 0), vars.BUFFER_DAYS, totalXP, edgecolor="red", facecolor="red", alpha=0.1))
    graph.draw()

    return yAxisYou, yAxisIdeal, yAxisDailyIdeal

def updateValues():
    config = core.readConfig(vars.CONFIG_PATH)

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(config, 0)

    if not "goals" in config: config["goals"] = []

    largestGoalRemaining = 0
    for g in config["goals"]:
        if g["remaining"] + totalXPCollected > largestGoalRemaining: largestGoalRemaining = g["remaining"] + totalXPCollected

    if config["activeBPLevel"] > 50 or largestGoalRemaining > totalXPTotal:
        epilogueCheck.config(state=DISABLED)
        epilogueVar.set(1)
    else:
        epilogueCheck.config(state=NORMAL)

    dailyProgress, dailyCollected, dailyRemaining, dailyTotal = core.calcDailyValues(config, epilogueVar.get())
    if(dailyProgress > 100): dailyProgress = 100
    if(dailyRemaining < 0): dailyRemaining = 0

    yAxisYou, yAxisIdeal, yAxisDailyIdeal = updateGraph(config, epilogueVar.get(), graphPlot)

    dailyBar["value"] = dailyProgress
    dailyPercentageLabel["text"] = str(dailyProgress) + "%"
    dailyCollectedLabel["text"] = str(dailyCollected) + " XP"
    dailyRemainingLabel["text"] = str(dailyRemaining) + " XP"
    dailyTotalLabel["text"] = str(dailyTotal) + " XP"

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(config, epilogueVar.get())
    totalXPBar["value"] = totalXPProgress
    totalXPPercentageLabel["text"] = str(totalXPProgress) + "%"
    totalCollectedLabel["text"] = str(totalXPCollected) + " XP"
    totalRemainingLabel["text"] = str(totalXPRemaining) + " XP"
    totalTotalLabel["text"] = str(totalXPTotal) + " XP"

    bpProgress, bpCollected, bpActive, bpRemaining, bpTotal = core.calcBattlepassValues(config, epilogueVar.get())
    bpBar["value"] = bpProgress
    bpPercentageLabel["text"] = str(bpProgress) + "%"
    bpPreviousUnlockLabel["text"] = str(bpCollected) + " Levels"
    bpActiveLabel["text"] = "Level " + str(bpActive)
    bpRemainingLabel["text"] = str(bpRemaining) + " Levels"
    bpTotalLabel["text"] = str(bpTotal) + " Levels"

    levelProgress, levelCollected, levelRemaining, levelTotal = core.calcLevelValues(config, epilogueVar.get())
    levelBar["value"] = levelProgress
    levelPercentageLabel["text"] = str(levelProgress) + "%"
    levelCollectedLabel["text"] = str(levelCollected) + " XP"
    levelRemainingLabel["text"] = str(levelRemaining) + " XP"
    levelTotalLabel["text"] = str(levelTotal) + " XP"

    miscRemainigDays, miscAverage, miscDeviationIdeal, miscDeviationDaily, miscStrongestDayDate, miscStrongestDayAmount, miscWeakestDayDate, miscWeakestDayAmount = core.calcMiscValues(config, yAxisYou, yAxisIdeal, yAxisDailyIdeal, epilogueVar.get())
    miscRemainingDaysLabel["text"] = str(miscRemainigDays) + " Days"
    miscAverageLabel["text"] = str(miscAverage) + " XP"
    miscIdealDeviationLabel["text"] = str(miscDeviationIdeal) + " XP"
    miscDailyDeviationLabel["text"] = str(miscDeviationDaily) + " XP"
    miscStrongestDayDateLabel["text"] = str(miscStrongestDayDate)
    miscStrongestDayAmountLabel["text"] = str(miscStrongestDayAmount) + " XP"
    miscWeakestDayDateLabel["text"] = str(miscWeakestDayDate)
    miscWeakestDayAmountLabel["text"] = str(miscWeakestDayAmount) + " XP"

    history.delete(*history.get_children())
    for i in range(len(config["history"]) - 1, -1, -1):
        history.insert("", "end", values=(config["history"][i]["description"], str(config["history"][i]["amount"]) + " XP", datetime.fromtimestamp(config["history"][i]["time"]).strftime("%d.%m.%Y %H:%M")))
    
# ================================
#  Buttons
# ================================

buttonContainer = ttk.Frame(root)
buttonContainer.pack(padx=8, pady=8, side=tk.RIGHT, fill="both", expand=True)

addXPBtn = ttk.Button(buttonContainer, text="Add XP", command=addXPCallback)
addXPBtn.pack(padx=8, pady=0, side=tk.RIGHT)

resetBtn = ttk.Button(buttonContainer, text="Add Goal", command=addGoalCallback)
resetBtn.pack(padx=8, pady=0, side=tk.RIGHT)

resetBtn = ttk.Button(buttonContainer, text="Reset", command=resetCallback)
resetBtn.pack(padx=8, pady=0, side=tk.RIGHT)

epilogueVar = IntVar()
epilogueCheck = ttk.Checkbutton(buttonContainer, text="Epilogue", onvalue=1, offvalue=0, variable=epilogueVar, command=updateValues)
epilogueCheck.pack(padx=8, pady=0, side=tk.RIGHT)

editBTN = ttk.Button(historyBtnContainer, text="Edit Element", command=editElementCallback)
editBTN.pack(side=tk.RIGHT, fill="both", expand=True)

delBTN = ttk.Button(historyBtnContainer, text="Delete Element", command=deleteElementCallback)
delBTN.pack(side=tk.LEFT, fill="both", expand=True)

# ================================
#  Main Loop
# ================================

os.startfile(vars.UPDATER_PATH)
init()

while dailyBar == None:
    continue

updateValues()
root.mainloop()