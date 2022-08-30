import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Models/Goals/contract.dart';

class Goal
{
  Contract parent;
  String name;
  int total;
  int xp;
  int order;
  List<String> rewards;

  Goal(this.parent, this.name, this.total, this.xp, this.order, this.rewards);

  static Goal fromDoc(DocumentSnapshot doc, Contract parent)
  {
    return Goal(
      parent,
      doc['name'] as String,
      doc['total'] as int,
      doc['xp'] as int,
      doc['order'] as int,
      doc['rewards'].cast<String>(),
    );
  }

  int getTotal()
  {
    return total;
  }

  int getXP()
  {
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

  LinearGradient getGradient()
  {
    return parent.getGradient();
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
}