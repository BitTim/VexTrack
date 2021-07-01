import tkinter as tk
from initDiag import InitDiag
import vars
import os

from core import *
from tkinter import *
from tkinter import ttk, messagebox
from addDiag import AddDiag

from datetime import *

import matplotlib
matplotlib.use("TkAgg")
from matplotlib.figure import Figure
from matplotlib.backends.backend_tkagg import FigureCanvasTkAgg
from matplotlib.patches import Rectangle

windowSize = vars.WINDOW_GEOMETRY.split("x")

with open(vars.VERSION_PATH, 'r') as f:
        versionString = f.read()

root = tk.Tk()
root.title(vars.WINDOW_TITLE + " " + versionString)
root.iconbitmap("VexTrack.exe")
root.geometry(vars.WINDOW_GEOMETRY)
root.minsize(int(windowSize[0]), int(windowSize[1]))

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

# --------------------------------
#  Total XP
# --------------------------------

totalXPContainer = ttk.LabelFrame(statsTab, text="Total XP")
totalXPContainer.pack(padx=8, pady=8, fill="x")

# ................................
#  Progress
# ................................

totalProgressContainer = ttk.Frame(totalXPContainer)
totalProgressContainer.pack(padx=8, pady=0)

totalXPPercentageLabel = ttk.Label(totalProgressContainer)
totalXPPercentageLabel.pack(padx=8, pady=8, side=tk.RIGHT, fill="x", expand=True)

totalXPBar = ttk.Progressbar(totalProgressContainer, orient="horizontal", length=10000, mode="determinate")
totalXPBar.pack(padx=8, pady=8, side=tk.RIGHT, fill="x")

totalXPBar["value"] = 35
totalXPPercentageLabel["text"] = str(totalXPBar["value"]) + "%"


# ................................
#  Collecetd
# ................................

totalCollectedContainer = ttk.Frame(totalXPContainer)
totalCollectedContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(totalCollectedContainer, text="Collected:").pack(padx=8, pady=0, side=tk.LEFT)

totalCollectedLabel = ttk.Label(totalCollectedContainer, text="9999999 XP")
totalCollectedLabel.pack(padx=8, pady=0, side=tk.LEFT)

# ................................
#  Remaining
# ................................

totalRemainingContainer = ttk.Frame(totalXPContainer)
totalRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(totalRemainingContainer, text="Remaining:").pack(padx=8, pady=0, side=tk.LEFT)

totalRemainingLabel = ttk.Label(totalRemainingContainer, text="9999999 XP")
totalRemainingLabel.pack(padx=8, pady=0, side=tk.LEFT)

# ................................
#  Total
# ................................

totalTotalContainer = ttk.Frame(totalXPContainer)
totalTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(totalTotalContainer, text="Total:").pack(padx=8, pady=0, side=tk.LEFT)

totalTotalLabel = ttk.Label(totalTotalContainer, text="9999999 XP")
totalTotalLabel.pack(padx=8, pady=0, side=tk.LEFT)

# --------------------------------
#  Battlepass
# --------------------------------

bpContainer = ttk.LabelFrame(statsTab, text="Battlepass")
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

ttk.Label(bpPreviousUnlockContainer, text="Previous Unlock:").pack(padx=8, pady=0, side=tk.LEFT)

bpPreviousUnlockLabel = ttk.Label(bpPreviousUnlockContainer, text="54")
bpPreviousUnlockLabel.pack(padx=8, pady=0, side=tk.LEFT)

# ................................
#  Remaining
# ................................

bpRemainingContainer = ttk.Frame(bpContainer)
bpRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(bpRemainingContainer, text="Remaining:").pack(padx=8, pady=0, side=tk.LEFT)

bpRemainingLabel = ttk.Label(bpRemainingContainer, text="54")
bpRemainingLabel.pack(padx=8, pady=0, side=tk.LEFT)

# ................................
#  Total
# ................................

bpTotalContainer = ttk.Frame(bpContainer)
bpTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(bpTotalContainer, text="Total:").pack(padx=8, pady=0, side=tk.LEFT)

bpTotalLabel = ttk.Label(bpTotalContainer, text="55")
bpTotalLabel.pack(padx=8, pady=0, side=tk.LEFT)

# --------------------------------
#  Battlepass
# --------------------------------

levelContainer = ttk.LabelFrame(statsTab, text="Active Level")
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

ttk.Label(levelCollectedContainer, text="Collected:").pack(padx=8, pady=0, side=tk.LEFT)

levelCollectedLabel = ttk.Label(levelCollectedContainer, text="37999")
levelCollectedLabel.pack(padx=8, pady=0, side=tk.LEFT)

# ................................
#  Remaining
# ................................

levelRemainingContainer = ttk.Frame(levelContainer)
levelRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(levelRemainingContainer, text="Remaining:").pack(padx=8, pady=0, side=tk.LEFT)

levelRemainingLabel = ttk.Label(levelRemainingContainer, text="38000")
levelRemainingLabel.pack(padx=8, pady=0, side=tk.LEFT)

# ................................
#  Total
# ................................

levelTotalContainer = ttk.Frame(levelContainer)
levelTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(levelTotalContainer, text="Total:").pack(padx=8, pady=0, side=tk.LEFT)

levelTotalLabel = ttk.Label(levelTotalContainer, text="38000")
levelTotalLabel.pack(padx=8, pady=0, side=tk.LEFT)

# --------------------------------
#  History
# --------------------------------

historyStyle = ttk.Style()
historyStyle.configure("history.Treeview", highlightthickness=0, bd=0, font=('TkDefaultFont', 10))
historyStyle.configure("history.Treeview.Heading", font=('TkDefaultFont', 10,'bold'))
historyStyle.layout("history.Treeview", [('history.Treeview.treearea', {'sticky': 'nswe'})])

history = ttk.Treeview(historyTab, columns=(1, 2, 3), show="headings", height="16", style="history.Treeview")
history.pack(side=tk.LEFT, fill="both", expand=True)

history.heading(1, text="Description", anchor="w")
history.heading(2, text="Amount", anchor="e")
history.heading(3, text="Timestamp", anchor="e")

history.column(1, anchor="w")
history.column(2, anchor="e")
history.column(3, anchor="e")

# Create history Scrollbar
historyScrollbar = Scrollbar(historyTab, orient=VERTICAL, command=history.yview)
historyScrollbar.pack(side=tk.LEFT, fill="y")

history.configure(yscrollcommand=historyScrollbar.set)

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

ttk.Label(dailyCollectedContainer, text="Collected:").pack(padx=8, pady=0, side=tk.LEFT)

dailyCollectedLabel = ttk.Label(dailyCollectedContainer, text="99999")
dailyCollectedLabel.pack(padx=8, pady=0, side=tk.LEFT)

# --------------------------------
#  Remaining
# --------------------------------

dailyRemainingContainer = ttk.Frame(dailyXPContainer)
dailyRemainingContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(dailyRemainingContainer, text="Remaining:").pack(padx=8, pady=0, side=tk.LEFT)

dailyRemainingLabel = ttk.Label(dailyRemainingContainer, text="99999")
dailyRemainingLabel.pack(padx=8, pady=0, side=tk.LEFT)

# --------------------------------
#  Total
# --------------------------------

dailyTotalContainer = ttk.Frame(dailyXPContainer)
dailyTotalContainer.pack(padx=8, pady=8, side=tk.LEFT)

ttk.Label(dailyTotalContainer, text="Total:").pack(padx=8, pady=0, side=tk.LEFT)

dailyTotalLabel = ttk.Label(dailyTotalContainer, text="99999")
dailyTotalLabel.pack(padx=8, pady=0, side=tk.LEFT)

# ================================
#  Functions
# ================================

# --------------------------------
#  Callbacks
# --------------------------------

def addXPCallback():
    addDiag = AddDiag(root, "Add XP")

    if addDiag.description != None and addDiag.xpAmount != None:
        addXP(addDiag.description, int(addDiag.xpAmount))
    
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

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = calcTotalValues({"activeBPLevel": int(activeBPLevel), "cXP": int(cXP)}, 0)

    config = {"activeBPLevel": int(activeBPLevel), "cXP": int(cXP), "seasonEndDate": seasonEndDateStr, "history": [{"time": datetime.now().timestamp(), "description": "Initialization", "amount": int(totalXPCollected)}]}
    writeConfig(vars.CONFIG_PATH, config)

def addXP(description, amount):
    config = readConfig(vars.CONFIG_PATH)
    config["history"].append({"time": datetime.now().timestamp(), "description": description, "amount": int(amount)})
    config["cXP"] += amount

    while config["cXP"] >= vars.LEVEL2_OFFSET + vars.NUM_XP_PER_LEVEL * config["activeBPLevel"]:
        config["cXP"] -= vars.LEVEL2_OFFSET + vars.NUM_XP_PER_LEVEL * config["activeBPLevel"]
        config["activeBPLevel"] += 1
        messagebox.showinfo("Congratulations", "Congratulations! You have unlocked Battlepass Level " + str(config["activeBPLevel"] - 1))

    writeConfig(vars.CONFIG_PATH, config)

def updateGraph(config, epilogue, plot):
    plot.clear()
    timeAxis = []
    
    seasonEndDate = dt.datetime.strptime(config["seasonEndDate"], "%d.%m.%Y")
    dateDelta = seasonEndDate - dt.datetime.fromtimestamp(config["history"][0]["time"])
    duration = dateDelta.days
    timeAxis = range(0, duration + 1)

    yAxisIdeal = []

    totalXP = cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + epilogue * vars.NUM_EPLOGUE_LEVELS * vars.NUM_EPLOGUE_XP_PER_LEVEL
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

    if prevDate != date.today(): yAxisYou.append(yAxisYou[len(yAxisYou) - 1])
    plot.plot(timeAxis[:len(yAxisYou)], yAxisYou, color='red', label='You', linewidth=3)

    yAxisDailyIdeal = []

    dateDelta = seasonEndDate - dt.datetime.now()
    remainingDays = dateDelta.days
    dayDelta = duration - remainingDays

    dailyProgress, dailyCollected, dailyRemaining, dailyTotal = calcDailyValues(config, epilogue)
    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = calcTotalValues(config, epilogue)

    yAxisDailyIdeal.append(totalXPCollected)

    for i in range(1, remainingDays + 1):
        yAxisDailyIdeal.append(yAxisDailyIdeal[i - 1] + dailyTotal)
        if yAxisDailyIdeal[i] > totalXP: yAxisDailyIdeal[i] = totalXP

    plot.plot(timeAxis[dayDelta:], yAxisDailyIdeal, color='skyblue', label='Daily Ideal', alpha=1, linestyle="-.")
    plot.plot(dayDelta, totalXPCollected, color='darkred', label="Now", marker="o", markersize=5)

    plot.tick_params(top=False, bottom=False, left=False, right=False, labelleft=False)

    # --------------------------------
    # Draw lines for significant unlocks
    # --------------------------------

    for i in range(0, vars.NUM_BPLEVELS + 1, 1):
        plot.axhline(cumulativeSum(i, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL), color="gray", alpha=0.05, linestyle="--")

    for i in range(0, vars.NUM_BPLEVELS + 1, 5):
        plot.axhline(cumulativeSum(i, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL), color="green", alpha=0.15, linestyle="--")
    
    if epilogue:
        for i in range(1, vars.NUM_EPLOGUE_LEVELS + 1, 1):
            plot.axhline(cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + i * vars.NUM_EPLOGUE_XP_PER_LEVEL, color="green", alpha=0.15, linestyle="--")

    # --------------------------------
    # Draw Buffer Zone
    # --------------------------------

    plot.add_patch(Rectangle((duration - vars.BUFFER_DAYS, 0), vars.BUFFER_DAYS, totalXP, edgecolor="red", facecolor="red", alpha=0.1))

    graph.draw()

def updateValues():
    config = readConfig(vars.CONFIG_PATH)

    if config["activeBPLevel"] > 50:
        epilogueCheck.config(state=DISABLED)
        epilogueVar.set(1)
    else:
        epilogueCheck.config(state=NORMAL)

    dailyProgress, dailyCollected, dailyRemaining, dailyTotal = calcDailyValues(config, epilogueVar.get())
    if(dailyProgress > 100): dailyProgress = 100
    if(dailyRemaining < 0): dailyRemaining = 0

    dailyBar["value"] = dailyProgress
    dailyPercentageLabel["text"] = str(dailyProgress) + "%"
    dailyCollectedLabel["text"] = str(dailyCollected) + " XP"
    dailyRemainingLabel["text"] = str(dailyRemaining) + " XP"
    dailyTotalLabel["text"] = str(dailyTotal) + " XP"

    totalXPProgress, totalXPCollected, totalXPRemaining, totalXPTotal = calcTotalValues(config, epilogueVar.get())
    totalXPBar["value"] = totalXPProgress
    totalXPPercentageLabel["text"] = str(totalXPProgress) + "%"
    totalCollectedLabel["text"] = str(totalXPCollected) + " XP"
    totalRemainingLabel["text"] = str(totalXPRemaining) + " XP"
    totalTotalLabel["text"] = str(totalXPTotal) + " XP"

    bpProgress, bpCollected, bpRemaining, bpTotal = calcBattlepassValues(config, epilogueVar.get())
    bpBar["value"] = bpProgress
    bpPercentageLabel["text"] = str(bpProgress) + "%"
    bpPreviousUnlockLabel["text"] = str(bpCollected) + " Levels"
    bpRemainingLabel["text"] = str(bpRemaining) + " Levels"
    bpTotalLabel["text"] = str(bpTotal) + " Levels"

    levelProgress, levelCollected, levelRemaining, levelTotal = calcLevelValues(config, epilogueVar.get())
    levelBar["value"] = levelProgress
    levelPercentageLabel["text"] = str(levelProgress) + "%"
    levelCollectedLabel["text"] = str(levelCollected) + " XP"
    levelRemainingLabel["text"] = str(levelRemaining) + " XP"
    levelTotalLabel["text"] = str(levelTotal) + " XP"

    history.delete(*history.get_children())
    for i in range(len(config["history"]) - 1, -1, -1):
        history.insert("", "end", values=(config["history"][i]["description"], str(config["history"][i]["amount"]) + " XP", datetime.fromtimestamp(config["history"][i]["time"]).strftime("%d.%m.%Y %H:%M")))
    
    updateGraph(config, epilogueVar.get(), graphPlot)

# ================================
#  Buttons
# ================================

buttonContainer = ttk.Frame(root)
buttonContainer.pack(padx=8, pady=8, side=tk.RIGHT, fill="both", expand=True)

addXPBtn = ttk.Button(buttonContainer, text="Add XP", command=addXPCallback)
addXPBtn.pack(padx=8, pady=0, side=tk.RIGHT)

resetBtn = ttk.Button(buttonContainer, text="Reset", command=resetCallback)
resetBtn.pack(padx=8, pady=0, side=tk.RIGHT)

epilogueVar = IntVar()
epilogueCheck = ttk.Checkbutton(buttonContainer, text="Epilogue", onvalue=1, offvalue=0, variable=epilogueVar, command=updateValues)
epilogueCheck.pack(padx=8, pady=0, side=tk.RIGHT)

# ================================
#  Main Loop
# ================================

os.startfile(vars.UPDATER_PATH)

init()

while dailyBar == None:
    continue

updateValues()
root.mainloop()