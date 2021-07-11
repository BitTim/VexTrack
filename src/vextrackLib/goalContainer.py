from tkinter import *
from tkinter import ttk

class GoalContainer(ttk.LabelFrame):
    def __init__(self, container, index, name="", amount=0, color="#ff0000", *args, **kwargs):
        super().__init__(container, *args, **kwargs)
        self.name = name
        self.index = index
        self.amount = amount
        self.color = color
        self.configure(text=self.name)

        # ................................
        #  Progress
        # ................................

        progressContainer = ttk.Frame(self)
        progressContainer.pack(padx=8, pady=0)

        self.percentageLabel = ttk.Label(progressContainer)
        self.percentageLabel.pack(padx=8, pady=8, side=RIGHT, fill="x", expand=True)

        self.bar = ttk.Progressbar(progressContainer, orient="horizontal", length=10000, mode="determinate")
        self.bar.pack(padx=1, pady=8, side=RIGHT, fill="x")

        self.bar["value"] = 35
        self.percentageLabel["text"] = str(self.bar["value"]) + "%"


        # ................................
        #  Collecetd
        # ................................

        collectedContainer = ttk.Frame(self)
        collectedContainer.pack(padx=8, pady=8, side=LEFT)

        ttk.Label(collectedContainer, text="Collected:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=LEFT)

        self.collectedLabel = ttk.Label(collectedContainer, text="9999999 XP")
        self.collectedLabel.pack(padx=1, pady=0, side=LEFT)

        # ................................
        #  Remaining
        # ................................

        remainingContainer = ttk.Frame(self)
        remainingContainer.pack(padx=8, pady=8, side=LEFT)

        ttk.Label(remainingContainer, text="Remaining:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=LEFT)

        self.remainingLabel = ttk.Label(remainingContainer, text="9999999 XP")
        self.remainingLabel.pack(padx=1, pady=0, side=LEFT)

        # ................................
        #  Total
        # ................................

        totalContainer = ttk.Frame(self)
        totalContainer.pack(padx=8, pady=8, side=LEFT)

        ttk.Label(totalContainer, text="Total:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=LEFT)

        self.totalLabel = ttk.Label(totalContainer, text="9999999 XP")
        self.totalLabel.pack(padx=1, pady=0, side=LEFT)

        # ................................
        #  Color
        # ................................

        colorContainer = ttk.Frame(self)
        colorContainer.pack(padx=8, pady=8, side=LEFT)

        ttk.Label(colorContainer, text="Color:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=LEFT)

        self.colorFrame = Frame(colorContainer, bg=self.color, width=16, height=16)
        self.colorFrame.pack(padx=1, pady=0, side=LEFT)

        # ................................
        #  Buttons
        # ................................

        buttonContainer = ttk.Frame(self)
        buttonContainer.pack(padx=8, pady=8, side=RIGHT)

        self.removeBtn = ttk.Button(buttonContainer, text="Remove")
        self.removeBtn.pack(padx=1, pady=0, side=RIGHT)

        self.editBtn = ttk.Button(buttonContainer, text="Edit")
        self.editBtn.pack(padx=1, pady=0, side=RIGHT)
    
    def setValues(self, progress, collected, remaining, total, color=None):
        if color != None: self.color = color
        self.amount = remaining

        self.bar["value"] = progress
        self.percentageLabel["text"] = str(progress) + "%"
        self.collectedLabel["text"] = str(collected) + " XP"
        self.remainingLabel["text"] = str(remaining) + " XP"
        self.totalLabel["text"] = str(total) + " XP"
        self.colorFrame.configure(bg=self.color)
    
    def updateGoal(self, name, amount, color):
        self.name = name
        self.amount = amount
        self.color = color
        
        self.configure(text=self.name)
        self.colorFrame.configure(bg=self.color)