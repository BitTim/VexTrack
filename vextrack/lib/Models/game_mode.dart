import 'package:cloud_firestore/cloud_firestore.dart';

class GameMode {
  String name;
  String scoreFormat;
  int scoreLimit;
  bool ranked;
  bool challenges;
  bool canSurrender;

  GameMode(this.name, this.scoreFormat, this.scoreLimit, this.ranked, this.challenges, this.canSurrender);

  static GameMode fromDoc(DocumentSnapshot doc) {
    return GameMode(
      doc['name'] as String,
      doc['scoreFormat'] as String,
      doc['scoreLimit'] as int,
      doc['ranked'] as bool,
      doc['challenges'] as bool,
      doc['canSurrender'] as bool,
    );
  }
}