import 'package:vextrack/Core/calc.dart';
import 'package:vextrack/Services/data.dart';

class XPCalc
{
  static int getSeasonTotal()
  {
    int lvl2Offset = DataService.battlepassParams!.lvl2Offset;
    int levels = DataService.battlepassParams!.levels;
    int xpStep = DataService.battlepassParams!.xpStep;

    int total = 0;
    total = Calc.cumulativeSum(levels, lvl2Offset, xpStep);

    return total;
  }

  static int getEpilogueTotal()
  {
    int epilogue = DataService.battlepassParams!.epilogue;
    int xpEpilogueStep = DataService.battlepassParams!.xpEpilogueStep;

    int total = 0;
    total = epilogue * xpEpilogueStep;

    return total;
  }

  static int toTotal(int level, int lxp)
  {
    int lvl2Offset = DataService.battlepassParams!.lvl2Offset;
    int levels = DataService.battlepassParams!.levels;
    int xpStep = DataService.battlepassParams!.xpStep;

    int epilogue = DataService.battlepassParams!.epilogue;
    int xpEpilogueStep = DataService.battlepassParams!.xpEpilogueStep;

    int xp = 0;
    level = level - 1; // Since last level is the currently active one, represented by lxp

    int extraLevels = 0;
    if (level > levels)
    {
      extraLevels = level - levels;
      level = levels;
    }

    if (extraLevels > epilogue) extraLevels = epilogue;

    xp = Calc.cumulativeSum(level, lvl2Offset, xpStep);
    xp += extraLevels * xpEpilogueStep;
    xp += lxp;

    return xp;
  }

  static List<int> toLevelXP(int total)
  {
    int activeLevel = 2;
    int activeXP = total;

    for(int i = 2; i < DataService.battlepassParams!.levels + DataService.battlepassParams!.epilogue; i++)
    {
      int lvlXP = getLevelTotal(i + 1);
      activeXP -= lvlXP;

      if (activeXP < 0)
      {
        activeLevel = i;
        activeXP += lvlXP;
        break;
      }
    }

    return [activeLevel, activeXP];
  }




  static double getMaxProgress()
  {
    double seasonTotal = getSeasonTotal().toDouble();
    return (seasonTotal + getEpilogueTotal().toDouble()) / seasonTotal;
  }

  static double getProgress(int xp, int total)
  {
    return xp.toDouble() / total.toDouble();
  }




  static getLevelTotal(int level)
  {
    int lvl2Offset = DataService.battlepassParams!.lvl2Offset;
    int levels = DataService.battlepassParams!.levels;
    int xpStep = DataService.battlepassParams!.xpStep;
    int xpEpilogueStep = DataService.battlepassParams!.xpEpilogueStep;

    int xp = 0;
    if(level == 1) return xp;    

    if (level <= levels)
    {
      xp = lvl2Offset + (level - 1) * xpStep;
    }
    else
    {
      xp = xpEpilogueStep;
    }

    return xp;
  }
}