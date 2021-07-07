import tkinter as tk
from vextrackLib.initDiag import InitDiag
from vextrackLib.scrollableFrame import ScrollableFrame
from vars import *
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

windowSize = WINDOW_GEOMETRY.split("x")
newUpdaterVersion = uCore.checkNewVersion("Updater")
versionString, _, _ = uCore.getVersionString(APP_NAME)

root = tk.Tk()
root.title(APP_NAME + " " + versionString)
root.iconbitmap("VexTrack.exe")
root.geometry(WINDOW_GEOMETRY)
root.minsize(int(windowSize[0]), int(windowSize[1]))

if newUpdaterVersion == True:
    root.iconify()
    root.update()
    root.deiconify()

seasonIndex = IntVar()

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

def historySelect(event):
    elements = history.get_children()

    for e in elements:
        tags = history.item(e, "tags")

        if "selected" in tags:
            tag = core.getScoreTag(history.item(e, "values")[0])
            history.item(e, tags=(tag))
            break

    currElementFocus = history.focus()
    history.item(currElementFocus, tags=("selected"))

def _fixed_map(style, style_name, option):
    return [elm for elm in style.map(style_name, query_opt=option) if elm[:2] != ('!disabled', '!selected')]

historyListContainer = tk.Frame(historyTab)
historyListContainer.pack(fill="both", expand=True)

historyStyle = ttk.Style()
historyStyle.map("history.Treeview", foreground=_fixed_map(historyStyle, "history.Treeview", "foreground"), background=_fixed_map(historyStyle, "history.Treeview", "background"))
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

history.tag_configure("none", background=NONE_BG_COLOR, foreground=NONE_FG_COLOR)
history.tag_configure("win", background=WIN_BG_COLOR, foreground=WIN_FG_COLOR)
history.tag_configure("loss", background=LOSS_BG_COLOR, foreground=LOSS_FG_COLOR)
history.tag_configure("draw", background=DRAW_BG_COLOR, foreground=DRAW_FG_COLOR)
history.tag_configure("selected", background=SELECTED_BG_COLOR, foreground=SELECTED_FG_COLOR)

# Create history Scrollbar
historyScrollbar = Scrollbar(historyListContainer, orient=VERTICAL, command=history.yview)
historyScrollbar.pack(side=tk.LEFT, fill="y")

history.configure(yscrollcommand=historyScrollbar.set)
history.bind("<<TreeviewSelect>>", historySelect)

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
    res = messagebox.askquestion("Reset data", "Are you sure you want to reset your data? This will remove all progress")
    if res == "yes":
        initDiag = InitDiag(root, "Reset Config")

        if initDiag.activeBPLevel != None and initDiag.cXP != None and initDiag.remainingDays != None:
            initData(int(initDiag.activeBPLevel), int(initDiag.cXP), int(initDiag.remainingDays))
        else:
            messagebox.showinfo("Reset data", "Cancelled, previous data has not been removed")

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

    if not os.path.exists(os.path.dirname(DATA_PATH)):
        os.mkdir(os.path.dirname(DATA_PATH))
        runInit = True

    if not os.path.exists(DATA_PATH):
        if os.path.exists(OLD_DATA_PATH):
            data = None
            with open(OLD_DATA_PATH, "r") as f:
                data = f.read()
            with open(DATA_PATH + ".bak", "w") as f:
                f.write(data)
            
            newData = core.convertDataFormat(json.loads(data))
            core.writeData(newData)
            
            os.remove(OLD_DATA_PATH)
        else:
            f = open(DATA_PATH, "x")
            f.close()
            runInit = True

    # TODO: Update init dialog

    if not runInit and os.stat(DATA_PATH).st_size == 0:
        runInit = True
    
    if runInit:
        initDiag = InitDiag(root, "Initialization")

        if initDiag.activeBPLevel== None and initDiag.cXP == None and initDiag.remainingDays == None:
            messagebox.showerror("Initialization", "Initial data has to be created")
            exit()
        
        initData(int(initDiag.activeBPLevel), int(initDiag.cXP), int(initDiag.remainingDays))
    
    data = core.readData()
    seasonIndex.set(len(data["seasons"]) - 1)

# --------------------------------
#  Update Values
# --------------------------------

# TODO: Update this
def initData(activeBPLevel, cXP, remainingDays):
    delta = timedelta(int(remainingDays) + 1)
    seasonEndDate = datetime.today() + delta
    seasonEndDateStr = seasonEndDate.strftime("%d.%m.%Y")

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues({"activeBPLevel": int(activeBPLevel), "cXP": int(cXP)}, 0)

    data = {"activeBPLevel": int(activeBPLevel), "cXP": int(cXP), "endDate": seasonEndDateStr, "xpHistory": [{"time": datetime.now().timestamp(), "description": "Initialization", "amount": int(totalXPCollected)}]}
    core.writeData(data)

def addXP(description, amount):
    data = core.readData()

    data["seasons"][seasonIndex.get()]["xpHistory"].append({"time": datetime.now().timestamp(), "description": description, "amount": int(amount)})
    data["seasons"][seasonIndex.get()]["cXP"] += amount

    while data["seasons"][seasonIndex.get()]["cXP"] >= LEVEL2_OFFSET + NUM_XP_PER_LEVEL * data["seasons"][seasonIndex.get()]["activeBPLevel"]:
        data["seasons"][seasonIndex.get()]["cXP"] -= LEVEL2_OFFSET + NUM_XP_PER_LEVEL * data["seasons"][seasonIndex.get()]["activeBPLevel"]
        data["seasons"][seasonIndex.get()]["activeBPLevel"] += 1
        messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(data["seasons"][seasonIndex.get()]["activeBPLevel"] - 1))

    if not "goals" in data: data["goals"] = []

    for i in range(0, len(data["goals"])):
        completed = False
        if data["goals"][i]["remaining"] <= 0: completed = True

        data["goals"][i]["remaining"] -= amount

        if data["goals"][i]["remaining"] <= 0 and not completed:
            messagebox.showinfo("Congratulations", "Congratulations! You have completed the goal: " + str(data["goals"][i]["name"]))
        
    core.writeData(data)

def addGoal(name, amount, color):
    data = core.readData()

    if not "goals" in data:
        data["goals"] = []

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, epilogueVar.get(), len(data["seasons"]) - 1)
    data["goals"].append({"name": name, "remaining": amount, "startXP": totalXPCollected,"color": color})
    core.writeData(data)

def editElement(index, description, amount):
    data = core.readData()
    historyLen = len(data["seasons"][seasonIndex.get()]["xpHistory"])

    prevBPLevel = data["seasons"][seasonIndex.get()]["activeBPLevel"]

    index = historyLen - 1 - index
    prevAmount = data["seasons"][seasonIndex.get()]["xpHistory"][index]["amount"]
    data["seasons"][seasonIndex.get()]["xpHistory"][index]["description"] = description
    data["seasons"][seasonIndex.get()]["xpHistory"][index]["amount"] = amount

    if not "goals" in data: data["goals"] = []
    completedGoals = []
    for i in range(0, len(data["goals"])):
        if data["goals"][i]["remaining"] <= 0: completedGoals.append(i)
        data["goals"][i]["remaining"] -= amount - prevAmount

    data["seasons"][seasonIndex.get()] = core.recalcXP(data, seasonIndex.get())
    deltaBP = prevBPLevel - data["seasons"][seasonIndex.get()]["activeBPLevel"]
    
    if deltaBP > 0:
        for i in range(0, deltaBP, 1):
            messagebox.showinfo("Sorry", "You have lost Battlepass Level " + str(prevBPLevel - i - 1))
    elif deltaBP < 0:
        for i in range(0, deltaBP, -1):
            messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(-1 * i + prevBPLevel))

    for cg in completedGoals:
        if data["goals"][cg]["remaining"] > 0:
            messagebox.showinfo("Sorry", "You have no longer completed the goal: " + data["goals"][cg]["name"])


    core.writeData(data)
    updateValues()

def deleteElement(index):
    data = core.readData()
    historyLen = len(data["seasons"][seasonIndex.get()]["xpHistory"])

    prevBPLevel = data["seasons"][seasonIndex.get()]["activeBPLevel"]

    index = historyLen - 1 - index
    amount = data["seasons"][seasonIndex.get()]["xpHistory"][index]["amount"]
    data["seasons"][seasonIndex.get()]["xpHistory"].pop(index)

    if not "goals" in data: data["goals"] = []
    completedGoals = []
    for i in range(0, len(data["goals"])):
        if data["goals"][i]["remaining"] <= 0: completedGoals.append(i)
        data["goals"][i]["remaining"] += amount

    data["seasons"][seasonIndex.get()] = core.recalcXP(data, seasonIndex.get())
    deltaBP = prevBPLevel - data["seasons"][seasonIndex.get()]["activeBPLevel"]

    if deltaBP > 0:
        for i in range(0, deltaBP, 1):
            messagebox.showinfo("Sorry", "You have lost Battlepass Level " + str(prevBPLevel - i - 1))
    elif deltaBP < 0:
        for i in range(0, deltaBP, -1):
            messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(-1 * i + prevBPLevel))

    for cg in completedGoals:
        if data["goals"][cg]["remaining"] > 0:
            messagebox.showinfo("Sorry", "You have no longer completed the goal: " + data["goals"][cg]["name"])

    core.writeData(data)
    updateValues()

def gcRemove(index):
    data = core.readData()

    data["goals"].pop(index)
    goalContainers[index].destroy()
    goalContainers.pop(index)

    core.writeData(data)
    updateValues()

def gcEdit(index, changeStartXP, name, amount, color):
    data = core.readData()
    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, epilogueVar.get(), len(data["seasons"]) - 1)

    completed = False
    if data["goals"][index]["remaining"] <= 0: completed = True

    data["goals"][index]["name"] = name
    if changeStartXP: data["goals"][index]["startXP"] = totalXPCollected
    data["goals"][index]["remaining"] = amount
    data["goals"][index]["color"] = color

    goalContainers[index].updateGoal(name, amount, color)

    if data["goals"][index]["remaining"] > 0 and completed:
        messagebox.showinfo("Sorry", "You have no longer completed the goal: " + data["goals"][index]["name"])
    elif data["goals"][index]["remaining"] <= 0 and not completed:
        messagebox.showinfo("Congratulations", "Congratulations! You have completed the goal: " + str(data["goals"][index]["name"]))

    core.writeData(data)
    updateValues()

def updateGoals(data, plot, collectedXP):
    if not "goals" in data:
        data["goals"] = []

    for i in range(0, len(data["goals"])):
        alpha = 0.75

        if collectedXP + data["goals"][i]["remaining"] <= 0: continue
        if data["goals"][i]["remaining"] <= 0: alpha = 0.15

        plot.axhline(collectedXP + data["goals"][i]["remaining"], color=data["goals"][i]["color"], alpha=alpha, linestyle="-")
        plot.annotate(data["goals"][i]["name"], (0, collectedXP + data["goals"][i]["remaining"]), xytext=(-3, collectedXP + data["goals"][i]["remaining"] + 5000), color=data["goals"][i]["color"], alpha=alpha)

        if i >= len(goalContainers):
            gc = goalContainer.GoalContainer(statsContainer, data["goals"][i]["name"], data["goals"][i]["remaining"], color=data["goals"][i]["color"])
            gc.pack(padx=8, pady=8, fill="x")
            goalContainers.append(gc)
        
        collectedInGoal = collectedXP - data["goals"][i]["startXP"]
        if collectedInGoal < 0:
            data["goals"][i]["startXP"] += collectedInGoal
            collectedInGoal = 0

        totalInGoal = collectedInGoal + data["goals"][i]["remaining"]

        goalProgress = round(collectedInGoal / totalInGoal * 100)
        if goalProgress > 100: goalProgress = 100

        goalContainers[i].setValues(goalProgress, collectedInGoal, data["goals"][i]["remaining"] if data["goals"][i]["remaining"] > 0 else 0, totalInGoal)
        goalContainers[i].removeBtn.configure(command=lambda j=i: gcRemoveCallback(j))
        goalContainers[i].editBtn.configure(command=lambda j=i: gcEditCallback(j))

def updateGraph(data, epilogue, drawEpilogue, plot):
    plot.clear()
    timeAxis = []
    
    seasonEndDate = datetime.strptime(data["seasons"][seasonIndex.get()]["endDate"], "%d.%m.%Y")
    dateDelta = seasonEndDate - datetime.fromtimestamp(data["seasons"][seasonIndex.get()]["xpHistory"][0]["time"])
    duration = dateDelta.days
    timeAxis = range(0, duration + 1)

    yAxisIdeal = []

    totalXP = core.cumulativeSum(NUM_BPLEVELS, LEVEL2_OFFSET, NUM_XP_PER_LEVEL) + epilogue * NUM_EPLOGUE_LEVELS * NUM_EPLOGUE_XP_PER_LEVEL
    collectedXP = data["seasons"][seasonIndex.get()]["xpHistory"][0]["amount"]
    
    remainingXP = totalXP - collectedXP
    originalDaily = round(remainingXP / (duration - BUFFER_DAYS))

    yAxisIdeal.append(collectedXP)

    for i in range(1, duration + 1):
        yAxisIdeal.append(yAxisIdeal[i - 1] + originalDaily)
        if yAxisIdeal[i] > totalXP: yAxisIdeal[i] = totalXP

    plot.plot(timeAxis, yAxisIdeal, color='gray', label='Ideal', linestyle="-")

    yAxisYou = []
    prevDate = date.fromtimestamp(data["seasons"][seasonIndex.get()]["xpHistory"][0]["time"])
    index = -1

    for h in data["seasons"][seasonIndex.get()]["xpHistory"]:
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

    if date.fromtimestamp(data["seasons"][seasonIndex.get()]["xpHistory"][len(data["seasons"][seasonIndex.get()]["xpHistory"]) - 1]["time"]) == date.today(): offset = 1
    else: offset = 0

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, epilogue, seasonIndex.get())
    dailyTotal = round((totalXPTotal - yAxisYou[index - offset]) / (remainingDays - BUFFER_DAYS + 1))

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
    # Draw lines for battlepass unlocks
    # --------------------------------

    for i in range(0, NUM_BPLEVELS + 1, 1):
        alpha = 0.5
        neededXP = core.cumulativeSum(i, LEVEL2_OFFSET, NUM_XP_PER_LEVEL)

        if totalXPCollected >= neededXP: alpha = 0.05
        plot.axhline(neededXP, color="lightgray", alpha=alpha, linestyle="-")

    for i in range(0, NUM_BPLEVELS + 1, 5):
        alpha = 0.5
        neededXP = core.cumulativeSum(i, LEVEL2_OFFSET, NUM_XP_PER_LEVEL)

        if totalXPCollected >= neededXP: alpha = 0.05
        plot.axhline(neededXP, color="limegreen", alpha=alpha, linestyle="-")
    
    if epilogue or drawEpilogue:
        for i in range(1, NUM_EPLOGUE_LEVELS + 1, 1):
            alpha = 0.5
            neededXP = core.cumulativeSum(NUM_BPLEVELS, LEVEL2_OFFSET, NUM_XP_PER_LEVEL) + i * NUM_EPLOGUE_XP_PER_LEVEL

            if totalXPCollected >= neededXP: alpha = 0.05

            plot.axhline(neededXP, color="orange", alpha=alpha, linestyle="-")
    
    updateGoals(data, plot, totalXPCollected)

    # --------------------------------
    # Draw Buffer Zone
    # --------------------------------

    plot.add_patch(Rectangle((duration - BUFFER_DAYS, 0), BUFFER_DAYS, totalXP, edgecolor="red", facecolor="red", alpha=0.1))
    graph.draw()

    return yAxisYou, yAxisIdeal, yAxisDailyIdeal

def updateValues():
    data = core.readData()
    drawEpilogue = False

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, 0, seasonIndex.get())

    if not "goals" in data: data["goals"] = []

    largestGoalRemaining = 0
    for g in data["goals"]:
        if g["remaining"] + totalXPCollected > largestGoalRemaining: largestGoalRemaining = g["remaining"] + totalXPCollected

    if data["seasons"][seasonIndex.get()]["activeBPLevel"] > 50:
        epilogueCheck.configure(state=DISABLED)
        epilogueVar.set(1)
    else:
        epilogueCheck.configure(state=NORMAL)
    
    if largestGoalRemaining > totalXPTotal:
        drawEpilogue = True

    dailyProgress, dailyCollected, dailyRemaining, dailyTotal = core.calcDailyValues(data, epilogueVar.get(), seasonIndex.get())
    if(dailyProgress > 100): dailyProgress = 100
    if(dailyRemaining < 0): dailyRemaining = 0

    yAxisYou, yAxisIdeal, yAxisDailyIdeal = updateGraph(data,epilogueVar.get(), drawEpilogue, graphPlot)

    dailyBar["value"] = dailyProgress
    dailyPercentageLabel["text"] = str(dailyProgress) + "%"
    dailyCollectedLabel["text"] = str(dailyCollected) + " XP"
    dailyRemainingLabel["text"] = str(dailyRemaining) + " XP"
    dailyTotalLabel["text"] = str(dailyTotal) + " XP"

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, epilogueVar.get(), seasonIndex.get())
    totalXPBar["value"] = totalXPProgress
    totalXPPercentageLabel["text"] = str(totalXPProgress) + "%"
    totalCollectedLabel["text"] = str(totalXPCollected) + " XP"
    totalRemainingLabel["text"] = str(totalXPRemaining) + " XP"
    totalTotalLabel["text"] = str(totalXPTotal) + " XP"

    bpProgress, bpCollected, bpActive, bpRemaining, bpTotal = core.calcBattlepassValues(data, epilogueVar.get(), seasonIndex.get())
    bpBar["value"] = bpProgress
    bpPercentageLabel["text"] = str(bpProgress) + "%"
    bpPreviousUnlockLabel["text"] = str(bpCollected) + " Levels"
    bpActiveLabel["text"] = "Level " + str(bpActive)
    bpRemainingLabel["text"] = str(bpRemaining) + " Levels"
    bpTotalLabel["text"] = str(bpTotal) + " Levels"

    levelProgress, levelCollected, levelRemaining, levelTotal = core.calcLevelValues(data, epilogueVar.get(), seasonIndex.get())
    levelBar["value"] = levelProgress
    levelPercentageLabel["text"] = str(levelProgress) + "%"
    levelCollectedLabel["text"] = str(levelCollected) + " XP"
    levelRemainingLabel["text"] = str(levelRemaining) + " XP"
    levelTotalLabel["text"] = str(levelTotal) + " XP"

    miscRemainigDays, miscAverage, miscDeviationIdeal, miscDeviationDaily, miscStrongestDayDate, miscStrongestDayAmount, miscWeakestDayDate, miscWeakestDayAmount = core.calcMiscValues(data, yAxisYou, yAxisIdeal, yAxisDailyIdeal, epilogueVar.get(), seasonIndex.get())
    miscRemainingDaysLabel["text"] = str(miscRemainigDays) + " Days"
    miscAverageLabel["text"] = str(miscAverage) + " XP"
    miscIdealDeviationLabel["text"] = str(miscDeviationIdeal) + " XP"
    miscDailyDeviationLabel["text"] = str(miscDeviationDaily) + " XP"
    miscStrongestDayDateLabel["text"] = str(miscStrongestDayDate)
    miscStrongestDayAmountLabel["text"] = str(miscStrongestDayAmount) + " XP"
    miscWeakestDayDateLabel["text"] = str(miscWeakestDayDate)
    miscWeakestDayAmountLabel["text"] = str(miscWeakestDayAmount) + " XP"

    history.delete(*history.get_children())
    xpHistoryData = data["seasons"][seasonIndex.get()]["xpHistory"]

    for i in range(len(xpHistoryData) - 1, -1, -1):
        desc = xpHistoryData[i]["description"]
        tag = core.getScoreTag(desc)
        
        history.insert("", "end", values=(desc, str(xpHistoryData[i]["amount"]) + " XP", datetime.fromtimestamp(xpHistoryData[i]["time"]).strftime("%d.%m.%Y %H:%M")), tags=(tag))
    
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

os.startfile(UPDATER_PATH)
init()

while dailyBar == None:
    continue

updateValues()
root.mainloop()