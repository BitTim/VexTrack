import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Core/history_calc.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';

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
    int nDays = HistoryCalc.getXPPerDay(history, meta).length;

    int xp = getXP();
    int dailyAvg = xp ~/ nDays;

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
}