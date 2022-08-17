import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Services/data.dart';

class HistoryEntry {
  String desc;
  String map;
  String mode;
  int xp;
  int score;
  int enemyScore;
  Timestamp time;
  bool surrenderedWin;
  bool surrenderedLoss;

  HistoryEntry(
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

  static HistoryEntry fromMap(Map<String, dynamic> map) {
    return HistoryEntry(
      map['desc'] as String,
      map['map'] as String,
      map['mode'] as String,
      map['xp'] as int,
      map['score'] as int,
      map['enemyScore'] as int,
      map['time'] as Timestamp,
      map['surrenderWin'] as bool,
      map['surrenderLoss'] as bool
    );
  }

  int getXP() { return xp; }
  DateTime getDateTime() { return time.toDate().toLocal(); }
  
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