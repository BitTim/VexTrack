import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/cupertino.dart';
import 'package:vextrack/Core/util.dart';
import 'package:vextrack/Models/Goals/goal.dart';

class Progression
{
  String id;
  String name;
  String startColor;
  String endColor;
  String dependency;
  bool paused;
  List<Goal> goals = [];

  Progression(this.id, this.name, this.startColor, this.endColor, this.dependency, this.paused);

  static Progression fromDoc(DocumentSnapshot doc)
  {
    return Progression(
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

  double getProgress()
  {
    int total = getTotal();
    int xp = getXP();
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
      stops.add(stops.last + (g.total.toDouble() / getTotal().toDouble()));
    }

    return stops;
  }

  String getPrecentage()
  {
    double progress = getProgress();
    return '${(progress * 100).toStringAsFixed(0)}%';
  }

  LinearGradient getGradient()
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

  String getNextUnlockName()
  {
    return getNextUnlock()?.name ?? "None";
  }

  String getNextUnlockPercentage()
  {
    return getNextUnlock()?.getPrecentage() ?? "0%";
  }
}