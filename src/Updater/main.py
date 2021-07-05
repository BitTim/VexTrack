import os, sys
sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from Updater import core
from vars import *

core.checkNewVersion(APP_NAME)