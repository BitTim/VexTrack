from tkinter import *  
from tkinter import ttk, messagebox

class DownloadDiag(Toplevel):
    def close(self):
        pass

    def __init__(self, parent, title):
        super().__init__(parent)
        self.title(title)
        self.transient()
        self.iconbitmap("Updater.exe")
        self.resizable(False, False)

        ttk.Label(self, text="Download progress", font=('TkDefaultFont', 9,'bold')).grid(padx=8, pady=4, column=0, row=0, columnspan=7, sticky="w")

        self.progressBar = ttk.Progressbar(self, length=64, orient="horizontal", mode="determinate")
        self.progressBar.grid(padx=8, pady=2, columnspan=6, column=0, row=1, sticky="we")
        
        self.percentageLabel = ttk.Label(self, text="0 %")
        self.percentageLabel.grid(padx=8, pady=4, column=6, row=1, sticky="w")

        ttk.Label(self, text="Downloaded:", font=('TkDefaultFont', 9,'bold')).grid(padx=8, pady=4, column=0, row=2, sticky="w")
        ttk.Label(self, text="File Size:", font=('TkDefaultFont', 9,'bold')).grid(padx=8, pady=4, column=2, row=2, sticky="w")
        ttk.Label(self, text="Speed:", font=('TkDefaultFont', 9,'bold')).grid(padx=8, pady=4, column=4, row=2, sticky="w")

        self.downloadedLabel = ttk.Label(self, text="0 B")
        self.downloadedLabel.grid(padx=8, pady=4, column=1, row=2, sticky="w")

        self.fileSizeLabel = ttk.Label(self, text="0 B")
        self.fileSizeLabel.grid(padx=8, pady=4, column=3, row=2, sticky="w")

        self.speedLabel = ttk.Label(self, text="0 b/s")
        self.speedLabel.grid(padx=8, pady=4, column=5, row=2, sticky="w")

        self.protocol("WM_DELETE_WINDOW", self.close)

    def formatSize(self, value, suffix):
        amount = 0
        while value > 1024:
            value /= 1024
            amount += 1
        
        value = round(value, 2)
        valueStr = f'{value:.2f}'

        if amount >= 3: return valueStr + " G" + suffix
        if amount >= 2: return valueStr + " M" + suffix
        if amount >= 1: return valueStr + " K" + suffix
        return valueStr + " " + suffix

    def updateValues(self, downloaded, size, speed):
        progress = round(downloaded / size * 100, 2)
        
        self.progressBar["value"] = progress
        self.percentageLabel["text"] = f'{progress:.2f}' + " %"
        self.downloadedLabel["text"] = self.formatSize(downloaded, "B")
        self.fileSizeLabel["text"] = self.formatSize(size, "B")
        self.speedLabel["text"] = self.formatSize(speed, "b/s")
        self.update()