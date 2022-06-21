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

  static HistoryEntry fromJSON(Map<String, dynamic> json) {
    return HistoryEntry(
      json['uuid'] as String,
      json['desc'] as String,
      json['map'] as String,
      json['mode'] as String,
      json['xp'] as int,
      json['score'] as int,
      json['enemyScore'] as int,
      json['time'] as int,
      json['surrenderedWin'] as bool,
      json['surrenderedLoss'] as bool
    );
  }
}