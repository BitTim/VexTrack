import tkinter as tk
from vextrackLib.initDiag import InitDiag
from vextrackLib.scrollableFrame import ScrollableFrame
from vars import *
import json
import os

from vextrackLib import core, addXPDiag as xpDiag, addGoalDiag as goalDiag, goalContainer, newSeasonDiag, seasonContainer, colorButton
from vextrackLib.settings import *
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

settings = loadSettings()
updatingSettingsUI = False

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
seasonNameVar = StringVar()

# ================================
#  Tabbed View
# ================================

notebook = ttk.Notebook(root)
graphTab = Frame(notebook)
statsTab = Frame(notebook)
historyTab = Frame(notebook)
seasonsTab = Frame(notebook)
settingsTab = Frame(notebook)

notebook.add(graphTab, text="Graph")
notebook.add(statsTab, text="Goals / Stats")
notebook.add(historyTab, text="History")
notebook.add(seasonsTab, text="Seasons")
notebook.add(settingsTab, text="Settings")
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
seasonContainers = []

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
#  Seasons
# ================================

seasonsScrollableContainer = ScrollableFrame(seasonsTab)
seasonsContainer = seasonsScrollableContainer.scrollableFrame
seasonsScrollableContainer.pack(fill="both", expand=True)

# ================================
#  Settings
# ================================

settingsScrollableContainer = ScrollableFrame(settingsTab)
settingsContainer = settingsScrollableContainer.scrollableFrame
settingsScrollableContainer.pack(fill="both", expand=True)

settingsContainer.grid_columnconfigure(0, weight=1)
settingsContainer.grid_columnconfigure(1, weight=1)
settingsContainer.grid_columnconfigure(2, weight=1)
settingsContainer.grid_columnconfigure(3, weight=1)

bufferDaysSettingVar = StringVar()
ttk.Label(settingsContainer, text="Buffer Days:").grid(padx=8, pady=2, columnspan=2, column=0, row=0, sticky="we")
bufferDaysSettingEntry = ttk.Entry(settingsContainer, textvariable=bufferDaysSettingVar)
bufferDaysSettingEntry.grid(padx=8, pady=2, columnspan=2, column=2, row=0, sticky="we")

enableColorsSettingVar = IntVar()
ttk.Label(settingsContainer, text="Enable colors in history:").grid(padx=8, pady=2, columnspan=2, column=0, row=1, sticky="we")
enableColorsSettingCheck = ttk.Checkbutton(settingsContainer, onvalue=1, offvalue=0, variable=enableColorsSettingVar)
enableColorsSettingCheck.grid(padx=8, pady=2, columnspan=2, column=2, row=1, sticky="we")
enableColorsSettingVar.set(settings["useHistoryColors"])

ttk.Label(settingsContainer, text="Win Background:").grid(padx=8, pady=2, column=0, row=2, sticky="we")
winBackgroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["winBackground"])
winBackgroundSettingBtn.grid(padx=8, pady=2, column=1, row=2, sticky="we")

ttk.Label(settingsContainer, text="Win Foreground:").grid(padx=8, pady=2, column=2, row=2, sticky="we")
winForegroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["winForeground"])
winForegroundSettingBtn.grid(padx=8, pady=2, column=3, row=2, sticky="we")

ttk.Label(settingsContainer, text="Loss Background:").grid(padx=8, pady=2, column=0, row=3, sticky="we")
lossBackgroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["lossBackground"])
lossBackgroundSettingBtn.grid(padx=8, pady=2, column=1, row=3, sticky="we")

ttk.Label(settingsContainer, text="Loss Foreground:").grid(padx=8, pady=2, column=2, row=3, sticky="we")
lossForegroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["lossForeground"])
lossForegroundSettingBtn.grid(padx=8, pady=2, column=3, row=3, sticky="we")

ttk.Label(settingsContainer, text="Draw Background:").grid(padx=8, pady=2, column=0, row=4, sticky="we")
drawBackgroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["drawBackground"])
drawBackgroundSettingBtn.grid(padx=8, pady=2, column=1, row=4, sticky="we")

ttk.Label(settingsContainer, text="Draw Foreground:").grid(padx=8, pady=2, column=2, row=4, sticky="we")
drawForegroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["drawForeground"])
drawForegroundSettingBtn.grid(padx=8, pady=2, column=3, row=4, sticky="we")

ttk.Label(settingsContainer, text="None Background:").grid(padx=8, pady=2, column=0, row=5, sticky="we")
noneBackgroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["noneBackground"])
noneBackgroundSettingBtn.grid(padx=8, pady=2, column=1, row=5, sticky="we")

ttk.Label(settingsContainer, text="None Foreground:").grid(padx=8, pady=2, column=2, row=5, sticky="we")
noneForegroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["noneForeground"])
noneForegroundSettingBtn.grid(padx=8, pady=2, column=3, row=5, sticky="we")

ttk.Label(settingsContainer, text="Selected Background:").grid(padx=8, pady=2, column=0, row=6, sticky="we")
selectedBackgroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["selectedBackground"])
selectedBackgroundSettingBtn.grid(padx=8, pady=2, column=1, row=6, sticky="we")

ttk.Label(settingsContainer, text="Selected Foreground:").grid(padx=8, pady=2, column=2, row=6, sticky="we")
selectedForegroundSettingBtn = colorButton.ColorButton(settingsContainer, settings["selectedForeground"])
selectedForegroundSettingBtn.grid(padx=8, pady=2, column=3, row=6, sticky="we")

settingsBtnContainer = tk.Frame(settingsTab)
settingsBtnContainer.pack(fill="x")

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

        if initDiag.activeBPLevel != None or initDiag.cXP != None or initDiag.remainingDays != None or initDiag.seasonName != None:
            initData(initDiag.seasonName, int(initDiag.activeBPLevel), int(initDiag.cXP), int(initDiag.remainingDays))
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
        gcRemove(goalContainers[index].index, index)

def gcEditCallback(index):
    editDiag = goalDiag.AddGoalDiag(root, "Edit Goal", name=goalContainers[index].name, amount=goalContainers[index].amount, color=goalContainers[index].color, edit=True)
    if editDiag.changeStartXP != None and (editDiag.name != goalContainers[index].name or editDiag.xpAmount != goalContainers[index].amount or editDiag.color != goalContainers[index].color or editDiag.changeStartXP == True):
        gcEdit(goalContainers[index].index, index, editDiag.changeStartXP, editDiag.name, int(editDiag.xpAmount), editDiag.color)

def seasonRemoveCallback(index):
    res = messagebox.askquestion("Remove Season", "Are you sure, that you want to remove this season?\nName: " + seasonContainers[index].name)
    if res == "yes":
        seasonRemove(index)

def seasonEditCallback(index):
    data = core.readData()
    names = []
    for s in data["seasons"]:
        names.append(s["name"])

    editDiag = newSeasonDiag.NewSeasonDiag(root, "Edit Season", name=seasonContainers[index].name, forbiddenNames=names, remainingDays=seasonContainers[index].remainingDays, edit=True, ended=1 if seasonContainers[index].remainingDays <= 0 else 0)
    if editDiag.seasonName != seasonContainers[index].name or editDiag.remainingDays != seasonContainers[index].remainingDays:
        seasonEdit(index, editDiag.seasonName, editDiag.remainingDays, editDiag.ended)

def seasonSelectorCallback(choice):
    data = core.readData()
    seasonName = seasonNameVar.get()

    i = 0
    for s in data["seasons"]:
        if s["name"] == seasonName:
            seasonIndex.set(i)
        i += 1

    updateValues()

def defaultSettingsCallback():
    global settings

    res = messagebox.askquestion("Default Settings", "Are you sure you want to revert to default settings?")
    if res == "yes":
        initSettings()
        settings = loadSettings()
        updateValues(True)

def aboutCallback():
    pass

# --------------------------------
#  Init
# --------------------------------

def init():
    runInit = False

    if not os.path.exists(os.path.dirname(DATA_PATH)):
        os.mkdir(os.path.dirname(DATA_PATH))
        runInit = True

    if not os.path.exists(DATA_PATH) or os.stat(DATA_PATH).st_size == 0:
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
    
    if runInit:
        initDiag = InitDiag(root, "Initialization")

        if initDiag.activeBPLevel == None or initDiag.cXP == None or initDiag.remainingDays == None or initDiag.seasonName == None:
            messagebox.showerror("Initialization", "Initial data has to be created")
            exit()
        
        initData(initDiag.seasonName, int(initDiag.activeBPLevel), int(initDiag.cXP), int(initDiag.remainingDays))
    
    data = core.readData()
    seasonIndex.set(len(data["seasons"]) - 1)
    seasonNameVar.set(data["seasons"][seasonIndex.get()]["name"])

    bufferDaysSettingEntry.delete(0, len(str(bufferDaysSettingVar.get())))
    bufferDaysSettingEntry.insert(0, settings["bufferDays"])

# --------------------------------
#  Update Values
# --------------------------------

def initData(seasonName, activeBPLevel, cXP, remainingDays):
    delta = timedelta(int(remainingDays) + 1)
    seasonEndDate = datetime.today() + delta
    seasonEndDateStr = seasonEndDate.strftime("%d.%m.%Y")

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues({"seasons": [{"activeBPLevel": int(activeBPLevel), "cXP": int(cXP)}]}, 0, 0)

    data = {"seasons": [{"name": seasonName, "activeBPLevel": int(activeBPLevel), "cXP": int(cXP), "endDate": seasonEndDateStr, "xpHistory": [{"time": datetime.now().timestamp(), "description": "Initialization", "amount": int(totalXPCollected)}]}]}
    core.writeData(data)

def updateSettings():
    if updatingSettingsUI: return

    bufferDaysSetting = bufferDaysSettingVar.get()
    updateBufferDays = True

    data = core.readData()
    seasonEndDate = datetime.strptime(data["seasons"][seasonIndex.get()]["endDate"], "%d.%m.%Y").date()
    dateDelta = seasonEndDate - datetime.fromtimestamp(data["seasons"][seasonIndex.get()]["xpHistory"][0]["time"]).date()
    duration = dateDelta.days

    try:
        bufferDaysSetting = int(bufferDaysSetting)
        if bufferDaysSetting < 0: bufferDaysSettingEntry.delete(0)
        if bufferDaysSetting >= duration: bufferDaysSettingEntry.delete(len(str(bufferDaysSetting)) - 1)
    except:
        bufferDaysSettingEntry.delete(len(str(bufferDaysSetting)) - 1)

    bufferDaysSetting = bufferDaysSettingVar.get()

    try:
        settings["bufferDays"] = int(bufferDaysSetting)
    except:
        updateBufferDays = False
    
    settings["useHistoryColors"] = enableColorsSettingVar.get()

    settings["winBackground"] = winBackgroundSettingBtn.color
    settings["winForeground"] = winForegroundSettingBtn.color
    settings["lossBackground"] = lossBackgroundSettingBtn.color
    settings["lossForeground"] = lossForegroundSettingBtn.color
    settings["drawBackground"] = drawBackgroundSettingBtn.color
    settings["drawForeground"] = drawForegroundSettingBtn.color
    settings["noneBackground"] = noneBackgroundSettingBtn.color
    settings["noneForeground"] = noneForegroundSettingBtn.color
    settings["selectedBackground"] = selectedBackgroundSettingBtn.color
    settings["selectedForeground"] = selectedForegroundSettingBtn.color

    updateValues(updateBufferDays)
    saveSettings(settings)

def addXP(description, amount):
    data = core.readData()

    data["seasons"][len(data["seasons"]) - 1]["xpHistory"].append({"time": datetime.now().timestamp(), "description": description, "amount": int(amount)})
    data["seasons"][len(data["seasons"]) - 1]["cXP"] += amount

    while data["seasons"][len(data["seasons"]) - 1]["cXP"] >= LEVEL2_OFFSET + NUM_XP_PER_LEVEL * data["seasons"][len(data["seasons"]) - 1]["activeBPLevel"]:
        data["seasons"][len(data["seasons"]) - 1]["cXP"] -= LEVEL2_OFFSET + NUM_XP_PER_LEVEL * data["seasons"][len(data["seasons"]) - 1]["activeBPLevel"]
        data["seasons"][len(data["seasons"]) - 1]["activeBPLevel"] += 1
        messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(data["seasons"][len(data["seasons"]) - 1]["activeBPLevel"] - 1))

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
    incompleteGoals = []
    for i in range(0, len(data["goals"])):
        if data["goals"][i]["remaining"] <= 0: completedGoals.append(i)
        else: incompleteGoals.append(i)
        data["goals"][i]["remaining"] -= amount - prevAmount

    data["seasons"][seasonIndex.get()] = core.recalcXP(data, seasonIndex.get())
    deltaBP = prevBPLevel - data["seasons"][seasonIndex.get()]["activeBPLevel"]
    
    if seasonIndex.get() == len(data["seasons"]) - 1:
        if deltaBP > 0:
            for i in range(0, deltaBP, 1):
                messagebox.showinfo("Sorry", "You have lost Battlepass Level " + str(prevBPLevel - i - 1))
        elif deltaBP < 0:
            for i in range(0, deltaBP, -1):
                messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(-1 * i + prevBPLevel))

    for cg in completedGoals:
        if data["goals"][cg]["remaining"] > 0:
            messagebox.showinfo("Sorry", "You have no longer completed the goal: " + data["goals"][cg]["name"])
    for ig in incompleteGoals:
        if data["goals"][ig]["remaining"] < 0:
            messagebox.showinfo("Congratulations", "Congratulations! You have completed the goal: " + data["goals"][ig]["name"])

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

    if seasonIndex.get() == len(data["seasons"]) - 1:
        if deltaBP > 0:
            for i in range(0, deltaBP, 1):
                messagebox.showinfo("Sorry", "You have lost Battlepass Level " + str(prevBPLevel - i - 1))

    for cg in completedGoals:
        if data["goals"][cg]["remaining"] > 0:
            messagebox.showinfo("Sorry", "You have no longer completed the goal: " + data["goals"][cg]["name"])

    core.writeData(data)
    updateValues()

def gcRemove(dataIndex, index):
    data = core.readData()

    data["goals"].pop(dataIndex)
    goalContainers[index].destroy()
    goalContainers.pop(index)

    core.writeData(data)
    updateValues()

def gcEdit(dataIndex, index, changeStartXP, name, amount, color):
    data = core.readData()
    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, epilogueVar.get(), len(data["seasons"]) - 1)

    completed = False
    if data["goals"][dataIndex]["remaining"] <= 0: completed = True

    data["goals"][dataIndex]["name"] = name
    if changeStartXP: data["goals"][dataIndex]["startXP"] = totalXPCollected
    data["goals"][dataIndex]["remaining"] = amount
    data["goals"][dataIndex]["color"] = color

    goalContainers[index].updateGoal(name, amount, color)

    if data["goals"][dataIndex]["remaining"] > 0 and completed:
        messagebox.showinfo("Sorry", "You have no longer completed the goal: " + data["goals"][dataIndex]["name"])
    elif data["goals"][dataIndex]["remaining"] <= 0 and not completed:
        messagebox.showinfo("Congratulations", "Congratulations! You have completed the goal: " + str(data["goals"][dataIndex]["name"]))

    core.writeData(data)
    updateValues()

def seasonRemove(index):
    data = core.readData()

    data["seasons"].pop(index)
    seasonContainers[index].destroy()
    seasonContainers.pop(index)

    core.writeData(data)
    updateValues()

def seasonEdit(index, name, remaining, ended):
    data = core.readData()
    
    data["seasons"][index]["name"] = name
    if not ended: data["seasons"][index]["endDate"] = datetime.strftime(date.today() + timedelta(days=remaining), "%d.%m.%Y")
    seasonContainers[index].setValues(name, remaining)

    core.writeData(data)
    updateValues()

def updateSeasons(data):
    global seasonContainers

    for i in range(0, len(data["seasons"])):
        if i >= len(seasonContainers):
            sc = seasonContainer.SeasonContainer(seasonsContainer, data["seasons"][i]["name"])
            sc.pack(padx=8, pady=8, fill="x")
            seasonContainers.append(sc)

        totalProgress, _, _, _ = core.calcTotalValues(data, epilogueVar.get(), i)

        seasonEndDate = datetime.strptime(data["seasons"][i]["endDate"], "%d.%m.%Y").date()
        dateDelta = seasonEndDate - date.today()
        remainingDays = dateDelta.days
        if remainingDays < 0: remainingDays = 0

        seasonContainers[i].updateValues(totalProgress, remainingDays)
        seasonContainers[i].removeBtn.configure(command=lambda k=i: seasonRemoveCallback(k))
        seasonContainers[i].editBtn.configure(command=lambda k=i: seasonEditCallback(k))

def updateGoals(data, plot, collectedXP):
    global goalContainers

    if not "goals" in data:
        data["goals"] = []

    if len(data["goals"]) == 0:
        for index in range(0, len(goalContainers)):
            goalContainers[index].destroy()
        goalContainers = []

    j = 0
    for i in range(0, len(data["goals"])):
        alpha = 0.75

        if collectedXP + data["goals"][i]["remaining"] <= 0: continue
        if data["goals"][i]["remaining"] <= 0: alpha = 0.15

        plot.axhline(collectedXP + data["goals"][i]["remaining"], color=data["goals"][i]["color"], alpha=alpha, linestyle="-")
        plot.annotate(data["goals"][i]["name"], (0, collectedXP + data["goals"][i]["remaining"]), xytext=(-3, collectedXP + data["goals"][i]["remaining"] + 5000), color=data["goals"][i]["color"], alpha=alpha)

        if j >= len(goalContainers):
            gc = goalContainer.GoalContainer(statsContainer, i, data["goals"][i]["name"], data["goals"][i]["remaining"], color=data["goals"][i]["color"])
            gc.pack(padx=8, pady=8, fill="x")
            goalContainers.append(gc)
        
        collectedInGoal = collectedXP - data["goals"][i]["startXP"]
        if collectedInGoal < 0:
            data["goals"][i]["startXP"] += collectedInGoal
            collectedInGoal = 0

        totalInGoal = collectedInGoal + data["goals"][i]["remaining"]

        goalProgress = round(collectedInGoal / totalInGoal * 100)
        if goalProgress > 100: goalProgress = 100

        goalContainers[j].setValues(goalProgress, collectedInGoal, data["goals"][i]["remaining"] if data["goals"][i]["remaining"] > 0 else 0, totalInGoal)
        goalContainers[j].removeBtn.configure(command=lambda k=j: gcRemoveCallback(k))
        goalContainers[j].editBtn.configure(command=lambda k=j: gcEditCallback(k))

        j += 1

def updateGraph(data, epilogue, drawEpilogue, plot):
    plot.clear()
    timeAxis = []
    
    seasonEndDate = datetime.strptime(data["seasons"][seasonIndex.get()]["endDate"], "%d.%m.%Y").date()
    dateDelta = seasonEndDate - datetime.fromtimestamp(data["seasons"][seasonIndex.get()]["xpHistory"][0]["time"]).date()
    duration = dateDelta.days
    timeAxis = range(0, duration + 1)

    yAxisIdeal = []

    totalXP = core.cumulativeSum(NUM_BPLEVELS, LEVEL2_OFFSET, NUM_XP_PER_LEVEL) + epilogue * NUM_EPLOGUE_LEVELS * NUM_EPLOGUE_XP_PER_LEVEL
    collectedXP = data["seasons"][seasonIndex.get()]["xpHistory"][0]["amount"]
    
    remainingXP = totalXP - collectedXP
    originalDaily = round(remainingXP / (duration - settings["bufferDays"]))

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

    if date.today() < seasonEndDate: deltaDate = date.today() - prevDate
    else: deltaDate = seasonEndDate - prevDate
    for i in range(0, deltaDate.days): yAxisYou.append(yAxisYou[len(yAxisYou) - 1])

    yAxisDailyIdeal = []

    dateDelta = seasonEndDate - date.today()
    remainingDays = dateDelta.days
    dayDelta = duration - remainingDays

    if date.fromtimestamp(data["seasons"][seasonIndex.get()]["xpHistory"][len(data["seasons"][seasonIndex.get()]["xpHistory"]) - 1]["time"]) == date.today(): offset = 1
    else: offset = 0

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = core.calcTotalValues(data, epilogue, seasonIndex.get())

    if remainingDays >= 0 and remainingDays < duration:
        divisor = remainingDays - settings["bufferDays"] + 1
        if divisor <= 0: divisor = 1
    
        dailyTotal = round((totalXPTotal - yAxisYou[index - offset]) / divisor)
        yAxisDailyIdeal.append(yAxisYou[index - offset])

        for i in range(1, remainingDays + 2):
            yAxisDailyIdeal.append(yAxisDailyIdeal[i - 1] + dailyTotal)
            if yAxisDailyIdeal[i] > totalXP: yAxisDailyIdeal[i] = totalXP

        plot.plot(timeAxis[dayDelta - 1:], yAxisDailyIdeal, color='skyblue', label='Daily Ideal', alpha=1, linestyle="--")
    
    plot.plot(timeAxis[:len(yAxisYou)], yAxisYou, color='red', label='You', linewidth=3)
    if remainingDays >= 0: plot.plot(dayDelta, totalXPCollected, color='darkred', label="Now", marker="o", markersize=5)

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

    plot.add_patch(Rectangle((duration - settings["bufferDays"], 0), settings["bufferDays"], totalXP, edgecolor="red", facecolor="red", alpha=0.1))
    graph.draw()

    return yAxisYou, yAxisIdeal, yAxisDailyIdeal

def updateSettingsUI(updateBufferDays):
    global updatingSettingsUI
    updatingSettingsUI = True

    if updateBufferDays:
        bufferDaysSettingEntry.delete(0, len(str(bufferDaysSettingVar.get())))
        bufferDaysSettingEntry.insert(0, settings["bufferDays"])
    enableColorsSettingVar.set(settings["useHistoryColors"])

    winBackgroundSettingBtn.setValues(color=settings["winBackground"])
    winForegroundSettingBtn.setValues(color=settings["winForeground"])
    lossBackgroundSettingBtn.setValues(color=settings["lossBackground"])
    lossForegroundSettingBtn.setValues(color=settings["lossForeground"])
    drawBackgroundSettingBtn.setValues(color=settings["drawBackground"])
    drawForegroundSettingBtn.setValues(color=settings["drawForeground"])
    noneBackgroundSettingBtn.setValues(color=settings["noneBackground"])
    noneForegroundSettingBtn.setValues(color=settings["noneForeground"])
    selectedBackgroundSettingBtn.setValues(color=settings["selectedBackground"])
    selectedForegroundSettingBtn.setValues(color=settings["selectedForeground"])

    if settings["useHistoryColors"] == 1:
        history.tag_configure("none", background=settings["noneBackground"], foreground=settings["noneForeground"])
        history.tag_configure("win", background=settings["winBackground"], foreground=settings["winForeground"])
        history.tag_configure("loss", background=settings["lossBackground"], foreground=settings["lossForeground"])
        history.tag_configure("draw", background=settings["drawBackground"], foreground=settings["drawForeground"])
    
    history.tag_configure("selected", background=settings["selectedBackground"], foreground=settings["selectedForeground"])
    updatingSettingsUI = False

def updateValues(updateBufferDays=True):
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

    names = []
    for s in data["seasons"]:
        names.append(s["name"])

    dailyProgress, dailyCollected, dailyRemaining, dailyTotal = core.calcDailyValues(data, epilogueVar.get(), seasonIndex.get(), settings)
    if dailyProgress == -1 and dailyCollected == -1 and dailyRemaining == -1 and dailyTotal == -1 and seasonIndex.get() == len(data["seasons"]) - 1:
        seasonDiag = newSeasonDiag.NewSeasonDiag(root, "New season", forbiddenNames=names)
        if seasonDiag.seasonName == None or int(seasonDiag.remainingDays) == None:
            messagebox.showerror("New Season", "New Season has to be created")
            exit()
        
        core.startNewSeason(seasonDiag.seasonName, seasonDiag.remainingDays)
        seasonIndex.set(len(data["seasons"]))
        updateValues()
        return

    dailyProgress, dailyCollected, dailyRemaining, dailyTotal = core.calcDailyValues(data, epilogueVar.get(), len(data["seasons"]) - 1, settings)

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
    
    updateSettingsUI(updateBufferDays)

    history.delete(*history.get_children())
    xpHistoryData = data["seasons"][seasonIndex.get()]["xpHistory"]

    for i in range(len(xpHistoryData) - 1, -1, -1):
        desc = xpHistoryData[i]["description"]
        tag = core.getScoreTag(desc)
        
        history.insert("", "end", values=(desc, str(xpHistoryData[i]["amount"]) + " XP", datetime.fromtimestamp(xpHistoryData[i]["time"]).strftime("%d.%m.%Y %H:%M")), tags=(tag))
    
    seasonNames = [""] + names

    global seasonSelector
    if seasonSelector != None: seasonSelector.destroy()

    seasonSelector = ttk.OptionMenu(root, seasonNameVar, *seasonNames, command=seasonSelectorCallback)
    seasonSelector.pack(padx=8, pady=8, side=tk.LEFT, fill="both", expand=True)

    updateSeasons(data)

# ================================
#  Settings Callbacks
# ================================

bufferDaysSettingVar.trace("w", lambda a, b, c: updateSettings())
enableColorsSettingCheck.configure(command=lambda: updateSettings())

winBackgroundSettingBtn.setValues(command=lambda: updateSettings())
winForegroundSettingBtn.setValues(command=lambda: updateSettings())
lossBackgroundSettingBtn.setValues(command=lambda: updateSettings())
lossForegroundSettingBtn.setValues(command=lambda: updateSettings())
drawBackgroundSettingBtn.setValues(command=lambda: updateSettings())
drawForegroundSettingBtn.setValues(command=lambda: updateSettings())
noneBackgroundSettingBtn.setValues(command=lambda: updateSettings())
noneForegroundSettingBtn.setValues(command=lambda: updateSettings())
selectedBackgroundSettingBtn.setValues(command=lambda: updateSettings())
selectedForegroundSettingBtn.setValues(command=lambda: updateSettings())

# ================================
#  Buttons
# ================================

seasonSelector = None

buttonContainer = ttk.Frame(root)
buttonContainer.pack(padx=8, pady=8, side=tk.RIGHT, fill="both", expand=True)

addXPBtn = ttk.Button(buttonContainer, text="Add XP", command=addXPCallback)
addXPBtn.pack(padx=8, pady=0, side=tk.RIGHT)

resetBtn = ttk.Button(buttonContainer, text="Add Goal", command=addGoalCallback)
resetBtn.pack(padx=8, pady=0, side=tk.RIGHT)

epilogueVar = IntVar()
epilogueCheck = ttk.Checkbutton(buttonContainer, text="Epilogue", onvalue=1, offvalue=0, variable=epilogueVar, command=updateValues)
epilogueCheck.pack(padx=8, pady=0, side=tk.RIGHT)

editBTN = ttk.Button(historyBtnContainer, text="Edit Element", command=editElementCallback)
editBTN.pack(side=tk.RIGHT, fill="both", expand=True)

delBTN = ttk.Button(historyBtnContainer, text="Delete Element", command=deleteElementCallback)
delBTN.pack(side=tk.LEFT, fill="both", expand=True)

resetSettingsBtn = ttk.Button(settingsBtnContainer, text="Default settings", command=defaultSettingsCallback)
resetSettingsBtn.pack(side=tk.LEFT, fill="both", expand=True)

resetDataSettingBtn = ttk.Button(settingsBtnContainer, text="Reset Data", command=resetCallback)
resetDataSettingBtn.pack(side=tk.LEFT, fill="both", expand=True)

aboutSettingBtn = ttk.Button(settingsBtnContainer, text="About", command=aboutCallback)
aboutSettingBtn.pack(side=tk.LEFT, fill="both", expand=True)

# ================================
#  Main Loop
# ================================

os.startfile(UPDATER_PATH)
init()

while dailyBar == None:
    continue

updateValues()
root.mainloop()