import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Core/history_calc.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';
import 'package:vextrack/Services/settings.dart';

class Season
{
  String id;
  int activeLevel;
  int activeXP;
  int inactiveDays;
  SeasonMeta meta;
  List<HistoryEntryGroup> history;

  Season(this.id, this.activeLevel, this.activeXP, this.inactiveDays, this.meta, this.history);

  static Season fromDoc(DocumentSnapshot doc, SeasonMeta meta, List<HistoryEntryGroup> history)
  {
    return Season(
      doc.id,
      doc['activeLevel'] as int,
      doc['activeXP'] as int,
      doc['inactiveDays'] as int,
      meta,
      history,
    );
  }

  Map<String, dynamic> toMap()
  {
    Map<String, dynamic> map = {};

    map['activeLevel'] = activeLevel;
    map['activeXP'] = activeXP;
    map['inactiveDays'] = inactiveDays;

    return map;
  }

  int getTotal()
  {
    return XPCalc.getSeasonTotal();
  }

  int getEpilogueTotal()
  {
    return XPCalc.getEpilogueTotal();
  }

  int getXP()
  {
    return XPCalc.toTotal(activeLevel, activeXP);
  }

  int getRemaining()
  {
    int total = getTotal();
    if (hasCompleted()) total += getEpilogueTotal();

    int remaining = total - getXP();
    if (remaining < 0) remaining = 0;

    return remaining;
  }

  int getDailyAvg()
  {
    int nDays = HistoryCalc.getXPPerDay(history, meta).length - 1;

    int xp = getXP();
    int dailyAvg = (xp / nDays).round();

    return dailyAvg;
  }

  double getProgress()
  {
    int xp = getXP();
    int total = getTotal();

    double maxProgress = XPCalc.getMaxProgress();
    
    double progress = XPCalc.getProgress(xp, total);
    if (progress < 0) progress = 0;
    if (progress > maxProgress) progress = maxProgress;

    return progress;
  }

  int getDuration()
  {
    return meta.getDuration().inDays;
  }

  int getRemainingDays()
  {
    return meta.endDate.toDate().difference(DateTime.now()).inDays + 1;
  }

  int getXPSum()
  {
    List<int> xpPerDay = HistoryCalc.getXPPerDay(history, meta);
    xpPerDay = xpPerDay.sublist(0, xpPerDay.length - 1);

    int xpSum = 0;
    for(int xp in xpPerDay)
    {
      xpSum += xp;
    }

    return xpSum;
  }

  int getUserIdeal({int? offset})
  {
    offset = offset ?? -2;
    offset += SettingsService.bufferDays;

    int remaining = getTotal() - getXPSum();
    if(getRemainingDays() - offset < SettingsService.bufferDays) return remaining;
    return (remaining / (getRemainingDays() - offset)).round();
  }

  int getUserEpilogueIdeal({int? offset})
  {
    offset = offset ?? -2;
    offset += SettingsService.bufferDays;

    int remaining = (getTotal() + getEpilogueTotal()) - getXPSum();
    if(getRemainingDays() - offset < SettingsService.bufferDays) return remaining;
    return (remaining / (getRemainingDays() - offset)).round();
  }

  int getDaysIndexFromDate(DateTime date)
  {
    return date.difference(meta.startDate.toDate()).inDays - 1;
  }

  int getIdeal(int idx, bool epilogue)
  {
    int effectiveDuration = getDuration() - SettingsService.bufferDays;
    int localTotal = epilogue ? getTotal() + getEpilogueTotal() : getTotal();
    double dailyXP = localTotal / effectiveDuration;

    int cumulativeSum = 0;

    for (int i = 0; i < idx + 1; i++)
    {
      int amount = 0;

      if (i < effectiveDuration)
      {
        amount = dailyXP.ceil();
        cumulativeSum += amount;
        if (cumulativeSum > localTotal) cumulativeSum = localTotal;
      }
    }

    return cumulativeSum;
  }

  int getDeviationIdeal(bool epilogue)
  {
    int idx = getDaysIndexFromDate(DateTime.now());

    int ideal = getIdeal(idx, epilogue);
    int xp = getXP();

    return xp - ideal;
  }

  int getDeviationDaily(bool epilogue)
  {
    int idx = getDaysIndexFromDate(DateTime.now());

    int ideal = epilogue ? getUserEpilogueIdeal() : getUserIdeal();
    int xp = HistoryCalc.getXPPerDay(history, meta)[idx];

    return xp - ideal;
  }

  int getCompleteDateDays(bool epilogue)
  {
    int todayIdx = getDaysIndexFromDate(DateTime.now());
    int cumulativeSum = getXP();
    int avg = getDailyAvg();
    int localTotal = epilogue ? getTotal() + getEpilogueTotal() : getTotal();
    int endIdx = 0;

    for(int i = todayIdx; i < getDuration(); i++)
    {
      cumulativeSum += avg;
      if(cumulativeSum > localTotal)
      {
        endIdx = i;
        break;
      }
    }

    return endIdx - todayIdx + 1;
  }

  DateTime getCompleteDate(bool epilogue)
  {
    int days = getCompleteDateDays(epilogue);
    return DateTime.now().add(Duration(days: days));
  }




  // --------------------------------
  // Flags
  // --------------------------------

  bool hasEpilogue() { return getProgress() >= XPCalc.getMaxProgress(); }
  bool hasCompleted() { return getProgress() >= 1.0; }
  bool hasFailed() { return getProgress() < 1.0; }

  bool isActive() { return meta.isActive(); }




  // --------------------------------
  // Formatted getters
  // --------------------------------

  String getFormattedTotal()
  {
    int total = getTotal();
    if (hasCompleted()) total += getEpilogueTotal();

    return Formatter.formatXP(total);
  }

  String getFormattedXP()
  {
    return Formatter.formatXP(getXP());
  }

  String getFormattedRemaining()
  {
    return Formatter.formatXP(getRemaining());
  }

  String getFormattedDailyAvg() {
    return Formatter.formatXP(getDailyAvg());
  }

  String getFormattedProgress()
  {
    return Formatter.formatPercentage(getProgress());
  }

  String getFormattedInactiveDays()
  {
    return Formatter.formatDays(inactiveDays);
  }

  String getFormattedDeviationIdeal(bool epilogue)
  {
    return Formatter.formatXP(getDeviationIdeal(epilogue));
  }

  String getFormattedDeviationDaily(bool epilogue)
  {
    return Formatter.formatXP(getDeviationDaily(epilogue));
  }

  String getFormattedCompleteDate(bool epilogue)
  {
    return "${Formatter.formatDate(getCompleteDate(epilogue))} (${Formatter.formatDays(getCompleteDateDays(epilogue))})";
  }
}