from tkinter import * 
from tkinter import simpledialog, messagebox, ttk
from vars import *

from vars import NUM_BPLEVELS, NUM_EPLOGUE_LEVELS

class InitDiag(simpledialog.Dialog):
    def __init__(self, parent, title):
        self.activeBPLevel = None
        self.cXP = None
        self.remainingDays = None
        super().__init__(parent, title)

    def body(self, frame):
        self.iconbitmap("VexTrack.exe")

        ttk.Label(frame, text="Active Battlepass Level").grid(padx=8, pady=4, column=0, row=0, sticky="w")
        self.activeBPLevelBox = ttk.Entry(frame, width=32)
        self.activeBPLevelBox.grid(padx=8, pady=2, columnspan=2, column=1, row=0, sticky="w")

        ttk.Label(frame, text="Battlepass Level Progress").grid(padx=8, pady=4, column=0, row=1, sticky="w")
        self.cXPBox = ttk.Entry(frame, width=32)
        self.cXPBox.grid(padx=8, pady=2, columnspan=2, column=1, row=1, sticky="w")

        ttk.Label(frame, text="Remaining Days in Season").grid(padx=8, pady=4, column=0, row=2, sticky="w")
        self.remainingDaysBox = ttk.Entry(frame, width=32)
        self.remainingDaysBox.grid(padx=8, pady=2, columnspan=2, column=1, row=2, sticky="w")

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
        self.okButton = ttk.Button(self, text='OK', command=self.ok_pressed)
        self.okButton.pack(padx=8, pady=8, side=RIGHT)
        cancelButton = ttk.Button(self, text='Cancel', command=self.cancel_pressed)
        cancelButton.pack(padx=8, pady=8, side=RIGHT)
        self.bind("<Return>", lambda event: self.ok_pressed())
        self.bind("<Escape>", lambda event: self.cancel_pressed())