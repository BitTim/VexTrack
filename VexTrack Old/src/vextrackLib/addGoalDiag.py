from tkinter import * 
from tkinter import simpledialog, messagebox, ttk
from vextrackLib import colorButton

class AddGoalDiag(simpledialog.Dialog):
    def __init__(self, parent, title, name=None, amount=None, color="#FF0000", edit=False):
        self.xpAmount = amount
        self.name = name
        self.color = color
        self.changeStartXP = False if edit == True else None
        self.edit = edit
        super().__init__(parent, title)

    def changeColor(self):
        self.color = self.colorBtn.color

    def body(self, frame):
        self.iconbitmap("VexTrack.exe")

        ttk.Label(frame, text="Name").grid(padx=8, pady=4, column=0, row=0, sticky="w")
        self.nameBox = ttk.Entry(frame, width=32)
        self.nameBox.grid(padx=8, pady=2, columnspan=2, column=1, row=0, sticky="w")

        if self.name != None: self.nameBox.insert(0, self.name)

        ttk.Label(frame, text="Remaining XP").grid(padx=8, pady=4, column=0, row=1, sticky="w")
        self.xpAmountBox = ttk.Entry(frame, width=32)
        self.xpAmountBox.grid(padx=8, pady=2, columnspan=2, column=1, row=1, sticky="w")

        if self.xpAmount != None: self.xpAmountBox.insert(0, self.xpAmount)

        ttk.Label(frame, text="Color").grid(padx=8, pady=4, column=0, row=2, sticky="w")

        self.colorBtn = colorButton.ColorButton(frame, color=self.color, command=self.changeColor)
        self.colorBtn.grid(padx=8, pady=4, column=2, row=2, sticky="we")

        self.changeStartXPVar = IntVar()

        if self.edit:
            changeStartXPCheck = ttk.Checkbutton(frame, text="Change Start Point", onvalue=1, offvalue=0, variable=self.changeStartXPVar)
            changeStartXPCheck.grid(padx=8, pady=4, column=2, row=3, sticky="e")

        return frame
    

    def ok_pressed(self):
        try:
            self.xpAmount = int(self.xpAmountBox.get())
        except:
            messagebox.showerror("Invalid Input", "Amount must be numerical")
            return
        
        self.name = self.nameBox.get()
        if self.edit: self.changeStartXP = self.changeStartXPVar.get()
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