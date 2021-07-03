from tkinter import * 
from tkinter import simpledialog, messagebox, ttk

class AddXPDiag(simpledialog.Dialog):
    def __init__(self, parent, title, description=None, amount=None):
        self.xpAmount = amount
        self.description = description
        super().__init__(parent, title)

    def body(self, frame):
        self.iconbitmap("VexTrack.exe")

        ttk.Label(frame, text="Description").grid(padx=8, pady=4, column=0, row=0, sticky="w")
        self.descriptionBox = ttk.Entry(frame, width=32)
        self.descriptionBox.grid(padx=8, pady=2, columnspan=2, column=1, row=0, sticky="w")

        if self.description != None: self.descriptionBox.insert(0, self.description)

        ttk.Label(frame, text="XP Amount").grid(padx=8, pady=4, column=0, row=1, sticky="w")
        self.xpAmountBox = ttk.Entry(frame, width=32)
        self.xpAmountBox.grid(padx=8, pady=2, columnspan=2, column=1, row=1, sticky="w")

        if self.xpAmount != None: self.xpAmountBox.insert(0, self.xpAmount)

        return frame
    
    def ok_pressed(self):
        try:
            self.xpAmount = int(self.xpAmountBox.get())
        except:
            messagebox.showerror("Invalid Input", "Amount must be numerical")
            return
        
        if self.xpAmount == 0:
            messagebox.showerror("Invalid Input", "Amount must be more than 0")
            return
        
        self.description = self.descriptionBox.get()
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