from tkinter import * 
from tkinter import simpledialog, messagebox, ttk
from vars import *

class NewSeasonDiag(simpledialog.Dialog):
    def __init__(self, parent, title, name=None, forbiddenNames=[], remainingDays=None, edit=False, ended=0):
        self.remainingDays = remainingDays
        self.seasonName = name
        self.edit = edit
        self.ended = ended
        self.forbiddenNames = forbiddenNames
        self.endedSeasonVar = IntVar()
        self.endedSeasonVar.set(self.ended)
        super().__init__(parent, title)

    def toggleEndedSeason(self):
        if self.endedSeasonVar.get() == 1: self.remainingDaysBox.configure(state=DISABLED)
        else: self.remainingDaysBox.configure(state=NORMAL)
        self.ended = self.endedSeasonVar.get()

    def body(self, frame):
        self.iconbitmap("VexTrack.exe")

        ttk.Label(frame, text="Season Name").grid(padx=8, pady=4, column=0, row=0, sticky="w")
        self.seasonNameBox = ttk.Entry(frame, width=32)
        self.seasonNameBox.grid(padx=8, pady=2, columnspan=2, column=1, row=0, sticky="w")

        if self.seasonName != None: self.seasonNameBox.insert(0, self.seasonName)

        ttk.Label(frame, text="Remaining Days in Season").grid(padx=8, pady=4, column=0, row=1, sticky="w")
        self.remainingDaysBox = ttk.Entry(frame, width=32)
        self.remainingDaysBox.grid(padx=8, pady=2, columnspan=2, column=1, row=1, sticky="w")

        if self.remainingDays != None: self.remainingDaysBox.insert(0, self.remainingDays)
        if self.ended == 1: self.remainingDaysBox.configure(state=DISABLED)

        if self.edit:
            endedSeason = ttk.Checkbutton(frame, text="Ended Season", onvalue=1, offvalue=0, variable=self.endedSeasonVar, command=self.toggleEndedSeason)
            endedSeason.grid(padx=8, pady=4, column=1, row=2, sticky="e")

        return frame
    
    def ok_pressed(self):
        try:
            self.remainingDays = int(self.remainingDaysBox.get())
        except:
            messagebox.showerror("Invalid Input", "Inputs must be numerical")
            return
        
        if self.remainingDays < 1 and self.ended == 0:
            messagebox.showerror("Invalid Input", "Remaining days cannot be less then 1")
            return
        
        if self.seasonNameBox.get() in self.forbiddenNames:
            messagebox.showerror("Invalid Input", "A season with the same name already exists")
            return

        self.seasonName = self.seasonNameBox.get()
        if self.seasonName == None:
            messagebox.showerror("Invalid Input", "Season Name cannot be empty")
            return

        self.destroy()

    def buttonbox(self):
        self.okButton = ttk.Button(self, text='OK', command=self.ok_pressed)
        self.okButton.pack(padx=8, pady=8, side=RIGHT)
        self.bind("<Return>", lambda event: self.ok_pressed())

        if self.edit:
            cancelButton = ttk.Button(self, text='Cancel', command=lambda: self.destroy())
            cancelButton.pack(padx=8, pady=8, side=RIGHT)
            self.bind("<Escape>", lambda event: self.destroy())