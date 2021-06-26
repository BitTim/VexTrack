import tkinter as tk
from tkinter import simpledialog

class AddDiag(simpledialog.Dialog):
    def __init__(self, parent, title):
        self.xpAmount = tk.IntVar()
        self.message = tk.StringVar()
        super().__init__(parent, title)
    
    def body(self, frame):
        self.xpAmountTitle = tk.Label(frame, width=25, text="XP Amount")
        self.xpAmountTitle.pack(padx=16, pady=16, side=tk.LEFT)
        self.xpAmountBox = tk.Entry(frame, width=25, textvariable=self.xpAmount)
        self.xpAmountBox.pack(padx=16, pady=16, side=tk.LEFT)

        self.messageTitle = tk.Label(frame, width=25, text="Message")
        self.messageTitle.pack(padx=16, pady=16, side=tk.LEFT)
        self.messageBox = tk.Entry(frame, width=25, textvariable=self.message)
        self.messageBox.pack(padx=16, pady=16, side = tk.LEFT)

        return frame
    
    def ok_pressed(self):
        self.xpAmount = self.xpAmountBox.get()
        self.message = self.messageBox.get()
        self.destroy()

    def cancel_pressed(self):
        self.destroy()

    def buttonbox(self):
        self.okButton = tk.Button(self, text='OK', width=5, command=self.ok_pressed)
        self.okButton.pack(padx=16, pady=16, side="left")
        cancelButton = tk.Button(self, text='Cancel', width=5, command=self.cancel_pressed)
        cancelButton.pack(padx=16, pady=16, side="right")
        self.bind("<Return>", lambda event: self.ok_pressed())
        self.bind("<Escape>", lambda event: self.cancel_pressed())