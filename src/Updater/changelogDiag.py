from tkinter import * 
from tkinter import simpledialog, messagebox, ttk

class ChangelogDiag(simpledialog.Dialog):
    def __init__(self, parent, title, changelog):
        self.changelog = changelog
        super().__init__(parent, title)

    def body(self, frame):
        self.iconbitmap("Updater.exe")

        ttk.Label(frame, text="Here is what's new:").grid(padx=8, pady=4, column=0, row=0, sticky="w")

        self.changelogBox = Listbox(frame, width=64)
        self.changelogBox.grid(padx=8, pady=2, column=0, row=1, sticky="nswe")
        for i in range(0, len(self.changelog)):
            self.changelogBox.insert(i, self.changelog[i])

        scrollbar = ttk.Scrollbar(frame, orient="vertical", command=self.changelogBox.yview)
        scrollbar.grid(column=1, row=1, sticky="nswe")

        return frame

    def buttonbox(self):
        self.okButton = ttk.Button(self, text='OK', command=lambda: self.destroy())
        self.okButton.pack(padx=8, pady=8, side=RIGHT)
        self.bind("<Return>", lambda event: self.destroy())