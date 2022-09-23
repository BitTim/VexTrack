import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Core/util.dart';
import 'package:vextrack/Models/Contracts/goal.dart';
import 'package:vextrack/themes.dart';

class Contract
{
  String id;
  String name;
  bool timed;
  bool paused;
  String startColor;
  String endColor;
  Timestamp? startTime;
  Timestamp? endTime;
  List<Goal> goals = [];

  Contract(this.id, this.name, this.timed, this.paused, this.startColor, this.endColor, this.startTime, this.endTime);

  static Contract fromDoc(DocumentSnapshot doc, bool paused)
  {
    return Contract(
      doc.id,
      doc['name'] as String,
      doc['timed'] as bool,
      paused,
      doc['startColor'] as String,
      doc['endColor'] as String,
      doc['startTime'] as Timestamp?,
      doc['endTime'] as Timestamp?,
    );
  }

  Map<String, dynamic> toMap()
  {
    Map<String, dynamic> map = {};

    map['name'] = name;
    map['timed'] = timed;
    map['startColor'] = startColor;
    map['endColor'] = endColor;
    map['startTime'] = startTime;
    map['endTime'] = endTime;

    return map;
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
    int remaining = getTotal() - getXP();
    if (remaining < 0) return 0;
    return remaining;
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

    return LinearGradient(colors: [
      AppThemes.getTheme().colorScheme.primary,
      AppThemes.getTheme().colorScheme.primary,
    ]);
  }

  Goal? getNextUnlock()
  {
    for (Goal g in goals)
    {
      if (g.xp >= g.total) continue;
      return g;
    }

    return null;
  }




  // -------------------------------
  // Flags
  // -------------------------------

  bool isCompleted()
  {
    if (getRemaining() <= 0) return true;
    return false;
  }

  bool isPaused()
  {
    if (paused) return true;
    return false;
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




  // --------------------------------
  //  Setters
  // --------------------------------

  void addXP(int xp)
  {
    Goal? active = getNextUnlock();
    if (active == null) return;

    if(active.getRemaining() < xp)
    {
      xp -= active.getRemaining();
      active.xp += active.getRemaining();
      addXP(xp);
      return;
    }

    active.xp += xp;
  }
}