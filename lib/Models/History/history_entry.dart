import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Services/data.dart';

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

  int getXP() { return xp; }
  DateTime getDateTime() { return DateTime.fromMillisecondsSinceEpoch(time * 1000).toLocal(); }
  
  DateTime getDate()
  {
    DateTime dt = getDateTime();
    return DateTime(dt.year, dt.month, dt.day);
  }

  String getScoreFormat()
  {
    return DataService.getModeScoreFormat(mode.toLowerCase().replaceAll(RegExp(' +'), '-'));
  }

  String getPlacementSuffix()
  {
    if(score == 1) return "st";
    if(score == 2) return "nd";
    if(score == 3) return "rd";
    return "th";
  }

  String getResult()
  {
    String scoreFormat = getScoreFormat();

    if(scoreFormat == "default" || scoreFormat == "")
    {
      if(score > enemyScore || surrenderedWin) return "win";
      if(score < enemyScore || surrenderedLoss) return "loss";
    }
    else if(scoreFormat == "placement")
    {
      if(score < 4) return "win";
      if(score > DataService.getModeScoreLimit(mode.toLowerCase()) - 3) return "loss";
    }

    return "draw";
  }




  // --------------------------------
  // Flags
  // --------------------------------

  bool hasWon() { return getResult() == "win"; }
  bool hasLost() { return getResult() == "loss"; }
  bool isDraw() { return getResult() == "draw"; }
  bool hasSurrendered() { return surrenderedWin || surrenderedLoss; }




  // --------------------------------
  // Formatted getters
  // --------------------------------

  String getFormattedXP()
  {
    return Formatter.formatXP(getXP());
  }

  String getFormattedDesc()
  {
    if (desc != "") return desc;

    String scoreFormat = getScoreFormat();
    String formattedDesc;
    if(scoreFormat == "default" || scoreFormat == "")
    {
      formattedDesc = "$mode $score-$enemyScore";
    }
    else
    {
      formattedDesc = "$mode $score${getPlacementSuffix()}";
    }

    return formattedDesc;
  }

  String getFormattedTime()
  {
    return Formatter.formatTime(getDateTime());
  }
}