import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';

class Season
{
  String uuid;
  String id;
  int activeLevel;
  int activeXP;
  SeasonMeta meta;

  Season(this.uuid, this.id, this.activeLevel, this.activeXP, this.meta);

  static Season fromDoc(DocumentSnapshot doc, SeasonMeta meta)
  {
    return Season(
      doc['uuid'] as String,
      doc['id'] as String,
      doc['activeLevel'] as int,
      doc['activeXP'] as int,
      meta,
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

  String getPrecentage()
  {
    double progress = getProgress();
    return '${(progress * 100).toStringAsFixed(0)}%';
  }

  bool hasEpilogue() { return getProgress() >= XPCalc.getMaxProgress(); }
  bool hasCompleted() { return getProgress() >= 1.0; }
  bool hasFailed() { return getProgress() < 1.0; }

  bool isActive() { return (DateTime.now().millisecondsSinceEpoch / 1000) < meta.endDate; }

  String getFormattedTotal() {
    int total = getTotal();
    if (hasCompleted()) total += getEpilogueTotal();

    return '$total XP';
  }

  String getFormattedXP() {
    int xp = getXP();
    return '$xp XP';
  }

  String getFormattedRemaining() {
    int remaining = getRemaining();
    return '$remaining XP';
  }
}