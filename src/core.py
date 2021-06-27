import datetime as dt
import json
import vars

def cumulativeSum(index, offset, amount):
    value = 0
    for i in range(2, index + 1):
        value += amount * i + offset
    return value

def readConfig(path):
    with open(path, "rt") as f:
        config = json.loads(f.read())
    return config

def writeConfig(path, config):
    with open(path, "wt") as f:
        f.write(json.dumps(config, indent = 4, separators=(',', ': ')))

# ================================
#  Calculate Values
# ================================

def calcDailyValues(config, epilogue):
    seasonEndDate = dt.datetime.strptime(config["seasonEndDate"], "%d.%m.%Y")
    dateDelta = seasonEndDate - dt.datetime.now()
    remainingDays = dateDelta.days

    totalXP = cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + epilogue * vars.NUM_EPLOGUE_LEVELS * vars.NUM_EPLOGUE_XP_PER_LEVEL
    
    if config["activeBPLevel"] - 1 <= vars.NUM_BPLEVELS:
        collectedXP = cumulativeSum(config["activeBPLevel"] - 1, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + config["cXP"]
    else:
        collectedXP = cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + config["cXP"] + (config["activeBPLevel"] - vars.NUM_BPLEVELS - 1) * vars.NUM_EPLOGUE_XP_PER_LEVEL
    
    remainingXP = totalXP - collectedXP
    dailyXP = round(remainingXP / (remainingDays - vars.BUFFER_DAYS))

    xpToday = 0
    for t in config["history"]:
        if dt.date.fromtimestamp(t["time"]) != dt.date.today():
            continue
        xpToday += int(t["amount"])

    dailyXPRemaining = dailyXP - xpToday
    dailyProgress = round(xpToday / dailyXP * 100)

    return dailyProgress, xpToday, dailyXPRemaining, dailyXP

def calcTotalValues(config, epilogue):
    totalXP = cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + epilogue * vars.NUM_EPLOGUE_LEVELS * vars.NUM_EPLOGUE_XP_PER_LEVEL
    
    if config["activeBPLevel"] - 1 <= vars.NUM_BPLEVELS:
        collectedXP = cumulativeSum(config["activeBPLevel"] - 1, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + config["cXP"]
    else:
        collectedXP = cumulativeSum(vars.NUM_BPLEVELS, vars.LEVEL2_OFFSET, vars.NUM_XP_PER_LEVEL) + config["cXP"] + (config["activeBPLevel"] - vars.NUM_BPLEVELS - 1) * vars.NUM_EPLOGUE_XP_PER_LEVEL
    
    remainingXP = totalXP - collectedXP
    totalProgress = round(collectedXP / totalXP * 100)

    return totalProgress, collectedXP, remainingXP, totalXP

def calcBattlepassValues(config, epilogue):
    bpLastUnlocked = config["activeBPLevel"] - 1
    bpTotal = vars.NUM_BPLEVELS + epilogue * vars.NUM_EPLOGUE_LEVELS
    bpRemaining = bpTotal - bpLastUnlocked
    bpProgress = round(bpLastUnlocked / bpTotal * 100)

    return bpProgress, bpLastUnlocked, bpRemaining, bpTotal

def calcLevelValues(config, epilogue):
    levelCollected = config["cXP"]
    if config["activeBPLevel"] - 1 <= vars.NUM_BPLEVELS:
        levelTotal = vars.LEVEL2_OFFSET + vars.NUM_XP_PER_LEVEL * config["activeBPLevel"]
    else:
        levelTotal = vars.NUM_EPLOGUE_XP_PER_LEVEL
    
    levelRemaining = levelTotal - levelCollected
    levelProgress = round(levelCollected / levelTotal * 100)

    return levelProgress, levelCollected, levelRemaining, levelTotal