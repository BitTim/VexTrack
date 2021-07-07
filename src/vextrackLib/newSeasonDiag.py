from tkinter import * 
from tkinter import simpledialog, messagebox, ttk
from vars import *

class NewSeasonDiag(simpledialog.Dialog):
    def __init__(self, parent, title):
        self.remainingDays = None
        self.seasonName = None
        super().__init__(parent, title)

    def body(self, frame):
        self.iconbitmap("VexTrack.exe")

        ttk.Label(frame, text="Season Name").grid(padx=8, pady=4, column=0, row=0, sticky="w")
        self.seasonNameBox = ttk.Entry(frame, width=32)
        self.seasonNameBox.grid(padx=8, pady=2, columnspan=2, column=1, row=0, sticky="w")

        ttk.Label(frame, text="Remaining Days in Season").grid(padx=8, pady=4, column=0, row=1, sticky="w")
        self.remainingDaysBox = ttk.Entry(frame, width=32)
        self.remainingDaysBox.grid(padx=8, pady=2, columnspan=2, column=1, row=1, sticky="w")

        return frame
    
    def ok_pressed(self):
        try:
            self.remainingDays = int(self.remainingDaysBox.get())
        except:
            messagebox.showerror("Invalid Input", "Inputs must be numerical")
            return
        
        if self.remainingDays < 1:
            messagebox.showerror("Invalid Input", "Remaining days cannot be less then 1")
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