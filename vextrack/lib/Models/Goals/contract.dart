import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/cupertino.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Core/util.dart';
import 'package:vextrack/Models/Goals/goal.dart';

class Contract
{
  String id;
  String name;
  String startColor;
  String endColor;
  String dependency;
  bool paused;
  List<Goal> goals = [];

  Contract(this.id, this.name, this.startColor, this.endColor, this.dependency, this.paused);

  static Contract fromDoc(DocumentSnapshot doc)
  {
    return Contract(
      doc.id,
      doc['name'] as String,
      doc['startColor'] as String,
      doc['endColor'] as String,
      doc['dependency'] as String,
      doc['paused'] as bool,
    );
  }

  int getTotal()
  {
    int total = 0;

    for (Goal goal in goals)
    {
      total += goal.total;
    }

    return total;
  }

  int getXP()
  {
    int xp = 0;

    for (Goal goal in goals)
    {
      xp += goal.xp;
    }

    return xp;
  }

  int getRemaining()
  {
    return getTotal() - getXP();
  }

  double getProgress()
  {
    int total = getTotal();
    int xp = getXP();

    if(total == 0) return 1.0;
    return xp.toDouble() / total.toDouble();
  }

  int getSegmentCount()
  {
    return goals.length;
  }

  List<double> getSegmentStops()
  {
    List<double> stops = [0.0];

    for (Goal g in goals)
    {
      double offset = 0.0;
      if(getTotal() == 0) { offset = 0.0; }
      else { offset = (g.total.toDouble() / getTotal().toDouble()); }

      stops.add(stops.last + offset);
    }

    return stops;
  }

  LinearGradient getGradient()
  {
    if (startColor != "" && endColor != "")
    {
      return LinearGradient(
        colors: [
          startColor.toColor(),
          endColor.toColor(),
        ],
        begin: Alignment.topLeft,
        end: Alignment.bottomRight,
      );
    }

    return AppColors.accentGradient;
  }

  Goal? getNextUnlock()
  {
    for (Goal g in goals)
    {
      if (g.xp > 0 && g.xp < g.total)
      {
        return g;
      }
    }

    return null;
  }




  // -------------------------------
  // Flags
  // -------------------------------

  bool isActive()
  {
    if (paused) return false;
    if (getRemaining() <= 0) return false;

    return true;
  }




  // --------------------------------
  // Formatted getters
  // --------------------------------

  String getFormattedTotal()
  {
    return Formatter.formatXP(getTotal());
  }

  String getFormattedXP()
  {
    return Formatter.formatXP(getXP());
  }

  String getFormattedRemaining()
  {
    return Formatter.formatXP(getRemaining());
  }

  String getFormattedProgress()
  {
    return Formatter.formatPercentage(getProgress());
  }

  String getNextUnlockName()
  {
    return getNextUnlock()?.name ?? "None";
  }

  String getNextUnlockFormattedProgress()
  {
    return getNextUnlock()?.getFormattedProgress() ?? "0%";
  }
}