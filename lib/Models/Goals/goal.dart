import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:vextrack/Models/Goals/progression.dart';

class Goal
{
  Progression parent;
  String name;
  int total;
  int xp;
  int order;

  Goal(this.parent, this.name, this.total, this.xp, this.order);

  static Goal fromDoc(DocumentSnapshot doc, Progression parent)
  {
    return Goal(
      parent,
      doc['name'] as String,
      doc['total'] as int,
      doc['xp'] as int,
      doc['order'] as int,
    );
  }

  double getProgress()
  {
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