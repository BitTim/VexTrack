from tkinter import * 
from tkinter import simpledialog, messagebox, ttk

class WarningDiag(simpledialog.Dialog):
    def __init__(self, parent, title, warnings):
        self.warnings = warnings
        self.response = "no"
        super().__init__(parent, title)

    def body(self, frame):
        self.iconbitmap("Updater.exe")

        ttk.Label(frame, text="This update has the following warnings:").grid(padx=8, pady=4, column=0, row=0, sticky="w")

        self.warningsBox = Listbox(frame, width=64)
        self.warningsBox.grid(padx=8, pady=2, column=0, row=1, sticky="nswe")
        for i in range(0, len(self.warnings)):
            self.warningsBox.insert(i, self.warnings[i])

        ttk.Label(frame, text="Are you sure you want to update?").grid(padx=8, pady=4, column=0, row=2, sticky="w")

        scrollbar = ttk.Scrollbar(frame, orient="vertical", command=self.warningsBox.yview)
        scrollbar.grid(column=1, row=1, sticky="nswe")

        return frame

    def giveResponse(self, response):
        self.response = response
        self.destroy()

    def buttonbox(self):
        self.noButton = ttk.Button(self, text='No', command=lambda: self.giveResponse("no"))
        self.noButton.pack(padx=8, pady=8, side=RIGHT)
        self.yesButton = ttk.Button(self, text='Yes', command=lambda: self.giveResponse("yes"))
        self.yesButton.pack(padx=8, pady=8, side=RIGHT)
        self.bind("<Return>", lambda event: self.giveResponse("yes"))
        self.bind("<Escape>", lambda event: self.giveResponse("no"))