from vars import *

def checkLegacy(version):
    if float(version[LEGACY_VERSIONS.index(APP_NAME)].split("v")[1]) <= LEGACY_LAST_APP or float(version[LEGACY_VERSIONS.index("Updater")].split("v")[1]) >= LEGACY_LAST_UPDATER:
        return True
    return False  