import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart';
import 'package:vextrack/Constants/references.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/Goals/goal.dart';
import 'package:vextrack/Models/Goals/contract.dart';
import 'package:vextrack/Models/battlepass_params.dart';
import 'package:vextrack/Models/game_mode.dart';
import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/game_map.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';

class DataService
{
  static BattlepassParams? battlepassParams;
  static Map<String, GameMode> modes = {};
  static Map<String, GameMap> maps = {};
  static Map<String, SeasonMeta> seasonMetas = {};

  static void init() async
  {
    DataService.battlepassParams = await getBattlepassParams();
    DataService.modes = await getModes();
    DataService.maps = await getMaps();
    DataService.seasonMetas = await getSeasonMetas();
  }  

  // ===============================
  //  Parameter data
  // ===============================

  static Future<BattlepassParams> getBattlepassParams() async
  {
    DocumentSnapshot doc = await parametersRef.doc('battlepass').get();
    return BattlepassParams.fromDoc(doc);
  }

  // ===============================
  //  User data
  // ===============================

  static Future<List<Season>> getAllSeasons(String uid) async {
    QuerySnapshot loadedSeasons = await usersRef.doc(uid)
      .collection("seasons")
      .get();

    Map<int, Season> seasonOrder = {};
    for (QueryDocumentSnapshot doc in loadedSeasons.docs)
    {
      List<HistoryEntry> history = await getHistory(uid, doc.id);

      Season s = Season.fromDoc(doc, seasonMetas[doc['id'] as String]!, history);
      int idx = seasonMetas.keys.toList().indexOf(s.id);
      seasonOrder[idx] = s;
    }
    List<int> indecies = seasonOrder.keys.toList();
    indecies.sort();

    List<Season> seasons = [
      for (int i in indecies) seasonOrder[i]!
    ];

    return seasons;
  }

  static Future<Season> getSeason(String uid, String id) async {
    DocumentSnapshot loadedSeason = await usersRef.doc(uid)
      .collection("seasons")
      .doc(id)
      .get();

    List<HistoryEntry> history = await getHistory(uid, id);

    Season season = Season.fromDoc(loadedSeason, seasonMetas[loadedSeason['id'] as String]!, history);
    return season;
  }

  static Future<List<HistoryEntry>> getFullHistory(String uid) async {
    List<HistoryEntry> history = [];
    
    List<Season> seasons = await getAllSeasons(uid);
    for (Season season in seasons) {
      List<HistoryEntry> seasonHistory = await getHistory(uid, season.uuid);
      history.addAll(seasonHistory);
    }

    return history;
  }

  static Future<List<HistoryEntry>> getHistory(String uid, String uuid) async {
    QuerySnapshot loadedHistory = await usersRef.doc(uid)
      .collection("seasons")
      .doc(uuid)
      .collection("history")
      .orderBy("time", descending: true)
      .get();

    List<HistoryEntry> history = loadedHistory.docs.map((doc) => HistoryEntry.fromDoc(doc)).toList();
    return history;
  }

  // ===============================
  //  Season meta data
  // ===============================

  static Future<Map<String, SeasonMeta>> getSeasonMetas() async
  {
    QuerySnapshot loadedSeasonMetas = await seasonsRef.orderBy("end", descending: true).get();
    Map<String, SeasonMeta> seasonMetas = {};

    for(QueryDocumentSnapshot doc in loadedSeasonMetas.docs)
    {
      QuerySnapshot loadedBattlepass = await seasonsRef.doc(doc.id)
        .collection("battlepass")
        .orderBy("level", descending: false)
        .get();
      List<List<String>> battlepass = loadedBattlepass.docs.map((doc) => doc.get("rewards").cast<String>() as List<String>).toList();

      QuerySnapshot loadedEpilogue = await seasonsRef.doc(doc.id)
        .collection("epilogue")
        .orderBy("level", descending: false)
        .get();
      List<List<String>> epilogue = loadedEpilogue.docs.map((doc) => doc.get("rewards").cast<String>() as List<String>).toList();

      SeasonMeta seasonMeta = SeasonMeta.fromDoc(doc, doc.id, battlepass, epilogue);
      seasonMetas[doc.id] = seasonMeta;
    }

    return seasonMetas;
  }

  static String getSeasonName(String id)
  {
    if(DataService.seasonMetas[id] == null) return "";
    return DataService.seasonMetas[id]!.name;
  }

  static String getSeasonFormattedStartDate(String id)
  {
    if(DataService.seasonMetas[id] == null) return "";
    return DataService.seasonMetas[id]!.getFormattedStartDate();
  }

  static String getSeasonFormattedEndDate(String id)
  {
    if(DataService.seasonMetas[id] == null) return "";
    return DataService.seasonMetas[id]!.getFormattedEndDate();
  }

  static String getSeasonFormattedDuration(String id)
  {
    if(DataService.seasonMetas[id] == null) return "";
    return DataService.seasonMetas[id]!.getFormattedDuration();
  }

  static Future<String> getSeasonImgUrl(String id) async
  {
    if(DataService.seasonMetas[id] == null) id = "none";
    String storageURL = DataService.seasonMetas[id]!.imgURL;
    String imgURL = await FirebaseStorage.instance.refFromURL(storageURL).getDownloadURL();
    return imgURL;
  }

  static SeasonMeta? getActiveSeasonMeta()
  {
    int now = DateTime.now().millisecondsSinceEpoch ~/ 1000;

    for(SeasonMeta meta in DataService.seasonMetas.values)
    {
      if(meta.startDate <= now && meta.endDate >= now)
      {
        return meta;
      }
    }

    return null;
  }

  // ===============================
  //  Map data
  // ===============================

  static Future<Map<String, GameMap>> getMaps() async
  {
    QuerySnapshot loadedMaps = await mapsRef.get();
    Map<String, GameMap> maps = {};

    for (QueryDocumentSnapshot doc in loadedMaps.docs)
    {
      GameMap map = GameMap.fromDoc(doc);
      maps[doc.id] = map;
    }

    return maps;
  }

  static String getMapName(String id)
  {
    if(DataService.maps[id] == null) return "";
    return DataService.maps[id]!.name;
  }

  static Future<String> getMapImgUrl(String id) async
  {
    if(DataService.maps[id] == null) id = "none";
    String storageURL = DataService.maps[id]!.imgURL;
    String imgURL = await FirebaseStorage.instance.refFromURL(storageURL).getDownloadURL();
    return imgURL;
  }

  // ===============================
  //  Mode Data
  // ===============================

  static Future<Map<String, GameMode>> getModes() async
  {
    QuerySnapshot loadedModes = await modesRef.get();
    Map<String, GameMode> modes = {};

    for(QueryDocumentSnapshot doc in loadedModes.docs)
    {
      GameMode mode = GameMode.fromDoc(doc);
      modes[doc.id] = mode;
    }

    return modes;
  }

  static String getModeScoreFormat(String id)
  {
    if (DataService.modes[id] == null) return "";
    return DataService.modes[id]!.scoreFormat;
  }

  static int getModeScoreLimit(String id)
  {
    if (DataService.modes[id] == null) return -1;
    return DataService.modes[id]!.scoreLimit;
  }

  // ===============================
  //  Goal Data
  // ===============================

  static Future<List<Contract>> getBattlepassContracts(String uid) async
  {
    List<Contract> progressions = [];
    int levels = DataService.battlepassParams!.levels;
    int epilogue = DataService.battlepassParams!.epilogue;

    // Small Passes here, like aniversary pass, etc.

    SeasonMeta? activeMeta = getActiveSeasonMeta();
    if(activeMeta == null) return progressions;
    
    Season season = await getSeason(uid, activeMeta.id);

    // Battlepass

    Contract bpProgression = Contract(
      "battlepass",
      "Battlepass",
      "",
      "",
      "",
      false
    );

    for(int i = 1; i <= levels; i++)
    {
      int xp = season.activeXP;

      if (i < season.activeLevel) xp = XPCalc.getLevelTotal(i);
      if (i > season.activeLevel || i == 1) xp = 0;

      bpProgression.goals.add(Goal(
        bpProgression,
        "Level $i",
        XPCalc.getLevelTotal(i),
        xp,
        i,
        activeMeta.battlepass[i - 1],
      ));
    }

    progressions.add(bpProgression); //TODO: Keep goals but change progress bar to a similar one to seasons
    // TODO: Create Item object for "reward" for goals and have battlepass levels only have one goal

    // Epilogue

    Contract epProgression = Contract(
      "epilogue",
      "Epilogue",
      "",
      "",
      "",
      false
    );

    for(int i = levels + 1; i <= epilogue; i++)
    {
      int xp = season.activeXP;

      if (i < season.activeLevel) xp = XPCalc.getLevelTotal(i);
      if (i > season.activeLevel) xp = 0;

      epProgression.goals.add(Goal(
        epProgression,
        "Level $i",
        XPCalc.getLevelTotal(i),
        xp,
        i,
        activeMeta.battlepass[i - levels - 1]
      ));
    }

    progressions.add(epProgression);

    return progressions;
  }

  static Future<List<Contract>> getAllContracts(String uid) async
  {
    List<Contract> progressions = [];
    //progressions = await getBattlepassContracts(uid);

    QuerySnapshot loadedProgressions = await usersRef.doc(uid)
      .collection("progressions")
      .get();

    List<Contract> userProgressions = [];
    userProgressions = loadedProgressions.docs.map((doc) => Contract.fromDoc(doc)).toList();

    for (Contract p in userProgressions)
    {
      QuerySnapshot loadedGoals = await usersRef.doc(uid)
        .collection("progressions")
        .doc(p.id)
        .collection("goals")
        .orderBy("order", descending: false)
        .get();
      
      p.goals = loadedGoals.docs.map((doc) => Goal.fromDoc(doc, p)).toList();
    }

    progressions.addAll(userProgressions);
    return progressions;
  }
}