import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/xp_calc.dart';

class Season
{
  String uuid;
  String id;
  int activeLevel;
  int activeXP;

  Season(this.uuid, this.id, this.activeLevel, this.activeXP);

  static Season fromDoc(DocumentSnapshot doc)
  {
    return Season(
      doc['uuid'] as String,
      doc['id'] as String,
      doc['activeLevel'] as int,
      doc['activeXP'] as int,
    );
  }

  double getProgress()
  {
    int xp = XPCalc.toTotal(activeLevel, activeXP);
    int total = XPCalc.getSeasonTotal();

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
}