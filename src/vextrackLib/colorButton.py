from tkinter import *
from tkinter import colorchooser

class ColorButton(Frame):
    def __init__(self, container, command=None, color="#ff0000", *args, **kwargs):
        super().__init__(container, *args, **kwargs)
        self.color = color
        self.command = command
        self.hovering = False
        self.configure(bg=self.color)

        self.configure(width=16, height=16)

        self.bind("<Button-1>", self.onClick)
        self.bind("<Enter>", lambda event: self.setHover(True))
        self.bind("<Leave>", lambda event: self.setHover(False))

    def setHover(self, state):
        self.hovering = state

    def onClick(self, event):
        self.color = colorchooser.askcolor(title="Select Color", color=self.color)[1]
        self.configure(bg=self.color)
        
        if self.command != None: self.command()
    
    def setValues(self, color, command=None):
        if command != None: self.command = command
        self.color = color
        self.configure(bg=self.color)