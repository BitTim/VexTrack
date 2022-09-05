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
  Timestamp time;
  String surrender;

  HistoryEntry(
    this.uuid,
    this.desc,
    this.map,
    this.mode,
    this.xp,
    this.score,
    this.enemyScore,
    this.time,
    this.surrender,
  );

  static HistoryEntry fromMap(Map<String, dynamic> map, String uuid) {
    return HistoryEntry(
      uuid,
      map['desc'] as String,
      map['map'] as String,
      map['mode'] as String,
      map['xp'] as int,
      map['score'] as int,
      map['enemyScore'] as int,
      map['time'] as Timestamp,
      map['surrender'] as String,
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

  String getPlacementSuffix(score)
  {
    int lastDigit = score % 10;

    if('$score'.length > 1)
    {
      int secondLastDigit = ((score - lastDigit) / 10) % 10;
      if (secondLastDigit == 1) return "th";
    }

    if(lastDigit == 1) return "st";
    if(lastDigit == 2) return "nd";
    if(lastDigit == 3) return "rd";
    return "th";
  }

  String getResult()
  {
    String scoreFormat = getScoreFormat();

    if(scoreFormat == "default" || scoreFormat == "")
    {
      if((score > enemyScore && surrender != "you") || surrender == "enemy") return "win";
      if((score < enemyScore && surrender != "enemy") || surrender == "you") return "loss";
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
  bool hasSurrendered() { return surrender != "none"; }




  // --------------------------------
  // Formatted getters
  // --------------------------------

  String getFormattedXP()
  {
    return Formatter.formatXP(getXP());
  }

  String getFormattedDesc()
  {
    if (mode == "custom") return desc;

    String modeName = DataService.modes[mode]!.name;
    String scoreFormat = getScoreFormat();
    String formattedDesc;
    if(scoreFormat == "default" || scoreFormat == "")
    {
      formattedDesc = "$modeName $score-$enemyScore";
    }
    else
    {
      formattedDesc = "$modeName $score${getPlacementSuffix(score)}";
    }

    return formattedDesc;
  }

  String getFormattedTime()
  {
    return Formatter.formatTime(getDateTime());
  }
}