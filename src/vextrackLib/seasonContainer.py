from tkinter import *
from tkinter import ttk

class SeasonContainer(ttk.LabelFrame):
    def __init__(self, container, name="", *args, **kwargs):
        super().__init__(container, *args, **kwargs)
        self.name = name
        self.remainingDays = 0
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

        remainingContainer = ttk.Frame(self)
        remainingContainer.pack(padx=8, pady=8, side=LEFT)

        ttk.Label(remainingContainer, text="Days remaining:", font=('TkDefaultFont', 9,'bold')).pack(padx=1, pady=0, side=LEFT)

        self.remainingLabel = ttk.Label(remainingContainer, text="90 Days")
        self.remainingLabel.pack(padx=1, pady=0, side=LEFT)

        # ................................
        #  Buttons
        # ................................

        buttonContainer = ttk.Frame(self)
        buttonContainer.pack(padx=8, pady=8, side=RIGHT)

        self.removeBtn = ttk.Button(buttonContainer, text="Remove")
        self.removeBtn.pack(padx=1, pady=0, side=RIGHT)

        self.editBtn = ttk.Button(buttonContainer, text="Edit")
        self.editBtn.pack(padx=1, pady=0, side=RIGHT)
    
    def updateValues(self, progress, remainingDays=None):
        if remainingDays != None: self.remainingDays = remainingDays

        self.bar["value"] = progress
        self.percentageLabel["text"] = str(progress) + "%"
        self.remainingLabel["text"] = str(remainingDays) + " Days"
    
    def setValues(self, name, remainingDays):
        self.name = name
        self.remainingDays = remainingDays
        
        self.configure(text=self.name)