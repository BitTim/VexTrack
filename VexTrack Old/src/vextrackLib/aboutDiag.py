from tkinter import *  
from tkinter import ttk
from vars import *

class AboutDiag(Toplevel):
    def __init__(self, parent, title, versionString, updaterVersionString):
        super().__init__(parent)
        self.title(title)
        self.transient()
        self.iconbitmap("VexTrack.exe")
        self.resizable(False, False)

        self.img = PhotoImage(file="VexTrack.png")
        self.img = self.img.subsample(4, 4)
        Label(self, image=self.img).grid(padx=8, pady=8, row=0, sticky="nswe", columnspan=2)

        ttk.Label(self, text="VexTrack", font=('TkDefaultFont', 16,'bold')).grid(columnspan=2, row=1)

        ttk.Label(self, text="Version:").grid(column=0, row=2)
        ttk.Label(self, text=versionString).grid(column=1, row=2)
        ttk.Label(self, text="Updater Version:").grid(column=0, row=3)
        ttk.Label(self, text=updaterVersionString).grid(column=1, row=3)

        ttk.Label(self, text="").grid(columnspan=2, row=4)
        ttk.Label(self, text="Created by BitTim").grid(columnspan=2, row=5)
        ttk.Label(self, text="https://github.com/BitTim/VexTrack").grid(columnspan=2, row=6)
        ttk.Label(self, text="").grid(columnspan=2, row=7)