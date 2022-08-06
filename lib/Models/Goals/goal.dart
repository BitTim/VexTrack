import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
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

  double getProgress()
  {
    if(total == 0) return 1.0;
    return xp.toDouble() / total.toDouble();
  }

  String getPrecentage()
  {
    double progress = getProgress();
    return '${(progress * 100).toStringAsFixed(0)}%';
  }

  LinearGradient getGradient()
  {
    return parent.getGradient();
  }

  int getRemaining()
  {
    return total - xp;
  }
}