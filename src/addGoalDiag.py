from tkinter import * 
from tkinter import simpledialog, messagebox, ttk, colorchooser

class AddGoalDiag(simpledialog.Dialog):
    def __init__(self, parent, title, name=None, amount=None, color="#FF0000"):
        self.xpAmount = amount
        self.name = name
        self.color = color
        super().__init__(parent, title)

    def changeColor(self):
        self.color = colorchooser.askcolor(title="Choose Color", color=self.color)[1]
        self.colorPreview.configure(bg=self.color)

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
        self.colorPreview = Frame(frame, width=16, height=16, bg=self.color)
        self.colorPreview.grid(padx=8, pady=4, column=1, row=2, sticky="w")

        colorBtn = ttk.Button(frame, text="Change Color", command=self.changeColor)
        colorBtn.grid(padx=8, pady=4, column=2, row=2, sticky="e")

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
        
        self.name = self.nameBox.get()
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