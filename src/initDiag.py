from tkinter import * 
from tkinter import simpledialog, messagebox
from vars import *

from vars import NUM_BPLEVELS, NUM_EPLOGUE_LEVELS

class InitDiag(simpledialog.Dialog):
    def __init__(self, parent, title):
        self.activeBPLevel = None
        self.cXP = None
        self.remainingDays = None
        super().__init__(parent, title)

    def body(self, frame):
        self.activeBPContainer = Frame(frame)
        self.activeBPContainer.pack(padx=8, pady=8, fill="x")

        Label(self.activeBPContainer, text="Active Battlepass Level", anchor="w").pack(padx=8, pady=0, side=LEFT)
        self.activeBPLevelBox = Entry(self.activeBPContainer, width=32)
        self.activeBPLevelBox.pack(padx=8, pady=0, side=RIGHT)

        self.cXPContainer = Frame(frame)
        self.cXPContainer.pack(padx=8, pady=8, fill="x")

        Label(self.cXPContainer, text="Battlepass Level Progress", anchor="w").pack(padx=8, pady=0, side=LEFT)
        self.cXPBox = Entry(self.cXPContainer, width=32)
        self.cXPBox.pack(padx=8, pady=0, side=RIGHT)

        self.remainingDaysContainer = Frame(frame)
        self.remainingDaysContainer.pack(padx=8, pady=8, fill="x")

        Label(self.remainingDaysContainer, text="Remaining Days in Season").pack(padx=8, pady=0, side=LEFT)
        self.remainingDaysBox = Entry(self.remainingDaysContainer, width=32)
        self.remainingDaysBox.pack(padx=8, pady=0, side=RIGHT)

        return frame
    
    def ok_pressed(self):
        try:
            self.activeBPLevel = int(self.activeBPLevelBox.get())
            self.cXP = int(self.cXPBox.get())
            self.remainingDays = int(self.remainingDaysBox.get())
        except:
            messagebox.showerror("Invalid Input", "Inputs must be numerical")
            return

        if self.activeBPLevel > NUM_BPLEVELS + NUM_EPLOGUE_LEVELS or self.activeBPLevel < 2:
            messagebox.showerror("Invalid Input", "Active level must be between 2 and " + str(NUM_BPLEVELS + NUM_EPLOGUE_LEVELS))
            return
        
        if self.cXP > self.activeBPLevel * NUM_XP_PER_LEVEL + LEVEL2_OFFSET - 1 or self.cXP < 0:
            messagebox.showerror("Invalid Input", "Level progress must be between 0 and " + str(self.activeBPLevel * NUM_XP_PER_LEVEL + LEVEL2_OFFSET - 1) + " for this active Battlepass level")
            return
        
        if self.remainingDays < 1:
            messagebox.showerror("Invalid Input", "Remaining days cannot be less then 1")
            return

        self.destroy()

    def cancel_pressed(self):
        self.destroy()

    def buttonbox(self):
        self.okButton = Button(self, text='OK', width=5, command=self.ok_pressed)
        self.okButton.pack(padx=8, pady=8, side=RIGHT)
        cancelButton = Button(self, text='Cancel', width=5, command=self.cancel_pressed)
        cancelButton.pack(padx=8, pady=8, side=RIGHT)
        self.bind("<Return>", lambda event: self.ok_pressed())
        self.bind("<Escape>", lambda event: self.cancel_pressed())