import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';

class Season
{
  String uuid;
  String id;
  int activeLevel;
  int activeXP;
  SeasonMeta meta;
  List<HistoryEntry> history;

  Season(this.uuid, this.id, this.activeLevel, this.activeXP, this.meta, this.history);

  static Season fromDoc(DocumentSnapshot doc, SeasonMeta meta, List<HistoryEntry> history)
  {
    return Season(
      doc['uuid'] as String,
      doc['id'] as String,
      doc['activeLevel'] as int,
      doc['activeXP'] as int,
      meta,
      history,
    );
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

  bool isActive() { return (DateTime.now().millisecondsSinceEpoch / 1000) < meta.endDate; }




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
    int xp = getXP();
    int dailyAvg = xp ~/ meta.getDateTime(meta.endDate).difference(meta.getDateTime(meta.startDate)).inDays;
    
    return Formatter.formatXP(dailyAvg);
  }

  String getFormattedProgress()
  {
    return Formatter.formatPercentage(getProgress());
  }
}