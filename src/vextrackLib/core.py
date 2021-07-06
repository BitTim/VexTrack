from datetime import *
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
    seasonEndDate = datetime.strptime(config["seasonEndDate"], "%d.%m.%Y")
    dateDelta = seasonEndDate - datetime.now()
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
        if date.fromtimestamp(t["time"]) != date.today():
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
    bpActive = config["activeBPLevel"]

    return bpProgress, bpLastUnlocked, bpActive, bpRemaining, bpTotal

def calcLevelValues(config, epilogue):
    levelCollected = config["cXP"]
    if config["activeBPLevel"] - 1 <= vars.NUM_BPLEVELS:
        levelTotal = vars.LEVEL2_OFFSET + vars.NUM_XP_PER_LEVEL * config["activeBPLevel"]
    else:
        levelTotal = vars.NUM_EPLOGUE_XP_PER_LEVEL
    
    levelRemaining = levelTotal - levelCollected
    levelProgress = round(levelCollected / levelTotal * 100)

    return levelProgress, levelCollected, levelRemaining, levelTotal

def calcMiscValues(config, yAxisYou, yAxisIdeal, yAxisDailyIdeal, epilogue):
    seasonEndDate = datetime.strptime(config["seasonEndDate"], "%d.%m.%Y")
    dateDelta = seasonEndDate - datetime.now()
    miscRemainigDays = dateDelta.days

    dailyXP = []
    prevDate = date.fromtimestamp(config["history"][0]["time"])
    index = -1

    for h in config["history"]:
        currDate = date.fromtimestamp(h["time"])
        if currDate == prevDate and index != -1:
            dailyXP[index]["amount"] = dailyXP[index]["amount"] + int(h["amount"])
        else:
            deltaDate = (currDate - prevDate)

            for i in range(1, deltaDate.days):
                dailyXP.append({"date": (prevDate + timedelta(days=i)).strftime("%d.%m.%Y"), "amount": 0})
                index += 1

            dailyXP.append({"date": currDate.strftime("%d.%m.%Y"), "amount": int(h["amount"])})

            prevDate = currDate
            index += 1
    
    miscAverage = 0
    for d in dailyXP:
        miscAverage += d["amount"]

    miscAverage = round(miscAverage / len(dailyXP))

    t = len(yAxisYou) - 1
    dailyXP.sort(key=lambda item: item.get("amount"))

    miscDeviationIdeal = yAxisYou[t] - yAxisIdeal[t]
    miscDeviationDaily = yAxisYou[t] - yAxisDailyIdeal[1]
    miscStrongestDayDate = dailyXP[len(dailyXP) - 1]["date"]
    miscStrongestDayAmount = dailyXP[len(dailyXP) - 1]["amount"]
    miscWeakestDayDate = dailyXP[0]["date"]
    miscWeakestDayAmount = dailyXP[0]["amount"]

    return miscRemainigDays, miscAverage, miscDeviationIdeal, miscDeviationDaily, miscStrongestDayDate, miscStrongestDayAmount, miscWeakestDayDate, miscWeakestDayAmount

def recalcXP(config):
    collectedXP = 0

    for i in range(0, len(config["history"])):
        collectedXP += int(config["history"][i]["amount"])
    
    iteration = 2
    while collectedXP > 0:
        if iteration < vars.NUM_BPLEVELS:
            collectedXP -= vars.LEVEL2_OFFSET + iteration * vars.NUM_XP_PER_LEVEL
        elif iteration < vars.NUM_BPLEVELS + vars.NUM_EPLOGUE_LEVELS:
            collectedXP -= vars.NUM_EPLOGUE_XP_PER_LEVEL
        iteration += 1

    iteration -= 1
    config["activeBPLevel"] = iteration

    if iteration < vars.NUM_BPLEVELS:
        collectedXP += vars.LEVEL2_OFFSET + iteration * vars.NUM_XP_PER_LEVEL
    elif iteration < vars.NUM_BPLEVELS + vars.NUM_EPLOGUE_LEVELS:
        collectedXP += vars.NUM_EPLOGUE_XP_PER_LEVEL
    
    config["cXP"] = collectedXP
    return config

def getScoreTag(desc):
    splitDesc = desc.split()
    scoreCandidates = []

    for s in splitDesc:
        if "-" in s:
            scoreCandidates.append(s)
    
    scores = []
    for c in scoreCandidates:
        scoreComponents = c.split("-")
        if len(scoreComponents) != 2: continue

        noNumeric = False
        for comp in scoreComponents:
            if not comp.isnumeric(): noNumeric = True
        
        if noNumeric: continue

        scores.append(scoreComponents)
    
    if len(scores) != 1: return "none"
    if int(scores[0][0]) > int(scores[0][1]): return "win"
    if int(scores[0][0]) < int(scores[0][1]): return "loss"
    if int(scores[0][0]) == int(scores[0][1]): return "draw"