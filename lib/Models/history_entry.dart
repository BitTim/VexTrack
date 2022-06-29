import 'package:cloud_firestore/cloud_firestore.dart';

class HistoryEntry {
  String uuid;
  String desc;
  String map;
  String mode;
  int xp;
  int score;
  int enemyScore;
  int time;
  bool surrenderedWin;
  bool surrenderedLoss;

  HistoryEntry(
    this.uuid,
    this.desc,
    this.map,
    this.mode,
    this.xp,
    this.score,
    this.enemyScore,
    this.time,
    this.surrenderedWin,
    this.surrenderedLoss
  );

  static HistoryEntry fromDoc(DocumentSnapshot doc) {
    return HistoryEntry(
      doc['uuid'] as String,
      doc['desc'] as String,
      (doc['map'] as String).toLowerCase(),
      doc['mode'] as String,
      doc['xp'] as int,
      doc['score'] as int,
      doc['enemyScore'] as int,
      doc['time'] as int,
      doc['surrenderedWin'] as bool,
      doc['surrenderedLoss'] as bool
    );
  }

  String getParsedDesc() {
    if (desc != "") return desc;
    return "$mode $score-$enemyScore";
  }

  String getParsedTime() {
    return DateTime.fromMillisecondsSinceEpoch(time * 1000).toLocal().toString();
  }

  bool hasWon() { return score > enemyScore || surrenderedWin; }
  bool hasLost() { return score < enemyScore || surrenderedLoss; }
  bool isDraw() { return score == enemyScore; }
  bool hasSurrendered() { return surrenderedWin || surrenderedLoss; }
}