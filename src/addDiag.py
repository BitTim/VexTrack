from tkinter import * 
from tkinter import simpledialog, messagebox

class AddDiag(simpledialog.Dialog):
    def __init__(self, parent, title):
        self.xpAmount = None
        self.description = None
        super().__init__(parent, title)

    def body(self, frame):
        self.descriptionContainer = Frame(frame)
        self.descriptionContainer.pack(padx=8, pady=8)

        Label(self.descriptionContainer, text="Description").pack(padx=8, pady=0, side=LEFT)
        self.descriptionBox = Entry(self.descriptionContainer, width=32)
        self.descriptionBox.pack(padx=8, pady=0, side=LEFT)

        self.xpAmountContainer = Frame(frame)
        self.xpAmountContainer.pack(padx=8, pady=8)

        Label(self.xpAmountContainer, text="XP Amount").pack(padx=8, pady=0, side=LEFT)
        self.xpAmountBox = Entry(self.xpAmountContainer, width=32)
        self.xpAmountBox.pack(padx=8, pady=0, side=LEFT)

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
        self.okButton = Button(self, text='OK', width=5, command=self.ok_pressed)
        self.okButton.pack(padx=8, pady=8, side=RIGHT)
        cancelButton = Button(self, text='Cancel', width=5, command=self.cancel_pressed)
        cancelButton.pack(padx=8, pady=8, side=RIGHT)
        self.bind("<Return>", lambda event: self.ok_pressed())
        self.bind("<Escape>", lambda event: self.cancel_pressed())