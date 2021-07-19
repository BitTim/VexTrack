import tkinter as tk
from tkinter import ttk

class ScrollableFrame(ttk.Frame):
    def onScroll(self, event):
        _, _, _, height, = self.canvas.bbox("all")
        if height > self.winfo_height() and self.hovering:
            self.canvas.yview_scroll(round(-1 * (event.delta / 120)), "units")

    def setHover(self, state):
        self.hovering = state

    def __init__(self, container, *args, **kwargs):
        super().__init__(container, *args, **kwargs)
        self.hovering = False
        self.canvas = tk.Canvas(self)
        self.scrollbar = ttk.Scrollbar(self, orient="vertical", command=self.canvas.yview)
        self.scrollableFrame = ttk.Frame(self.canvas)

        self.scrollableFrame.bind("<Configure>", lambda e: self.canvas.configure(scrollregion=self.canvas.bbox("all")))
        self.windowID = self.canvas.create_window((0, 0), window=self.scrollableFrame, anchor="nw")
        self.canvas.configure(yscrollcommand=self.scrollbar.set)

        self.canvas.pack(side="left", fill="both", expand=True)
        self.scrollbar.pack(side="right", fill="y")
    
        self.canvas.bind('<Configure>', lambda event: self.canvas.itemconfigure(self.windowID, width=event.width))
        self.canvas.bind_all("<MouseWheel>", lambda event: self.onScroll(event), add=True)
        self.canvas.bind("<Enter>", lambda event: self.setHover(True))
        self.canvas.bind("<Leave>", lambda event: self.setHover(False))