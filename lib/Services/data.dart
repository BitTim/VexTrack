import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart';
import 'package:vextrack/Constants/references.dart';
import 'package:vextrack/Models/Goals/goal.dart';
import 'package:vextrack/Models/Goals/contract.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
import 'package:vextrack/Models/battlepass_params.dart';
import 'package:vextrack/Models/game_mode.dart';
import 'package:vextrack/Models/game_map.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';
import 'package:vextrack/Models/user_data.dart';

class DataService
{
  static UserData? userData;
  static BattlepassParams? battlepassParams;
  static Map<String, GameMode> modes = {};
  static Map<String, GameMap> maps = {};
  static Map<String, SeasonMeta> seasonMetas = {};
  static Map<String, Season> seasons = {};

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

  static Future<void> fetchUserData(String uid) async
  {
    DocumentSnapshot doc = await usersRef.doc(uid).get();
    userData = UserData.fromDoc(doc);
  }

  static Future<List<Season>> getAllSeasons({required String uid, bool activeOnly = false}) async {
    if(userData == null) await fetchUserData(uid);

    Map<int, Season> seasonOrder = {};
    for(Map<String, dynamic> s in userData!.seasonIDs)
    {
      bool active = s['active'];
      String id = s['id'];

      if(activeOnly && !active) continue;

      int idx = seasonMetas.keys.toList().indexOf(id);
      seasonOrder[idx] = await getSeason(uid, id);
    }
    
    List<int> indecies = seasonOrder.keys.toList();
    indecies.sort();

    List<Season> seasons = [
      for (int i in indecies) seasonOrder[i]!
    ];

    return seasons;
  }

  static Future<Season> getSeason(String uid, String id) async {
    if (seasons[id] != null) return seasons[id]!;

    DocumentSnapshot loadedSeason = await usersRef.doc(uid)
      .collection("seasons")
      .doc(id)
      .get();

    List<HistoryEntryGroup> history = await getHistory(uid, id);

    Season season = Season.fromDoc(loadedSeason, seasonMetas[id]!, history);
    return season;
  }



  static Future<List<HistoryEntryGroup>> getHistory(String uid, String seasonID) async {
    if(seasons[seasonID] != null) return seasons[seasonID]!.history;

    QuerySnapshot loadedHistory = await usersRef.doc(uid)
      .collection("seasons")
      .doc(seasonID)
      .collection("history")
      .orderBy("day", descending: true)
      .get();

    List<HistoryEntryGroup> history = loadedHistory.docs.map((doc) => HistoryEntryGroup.fromDoc(doc)).toList();
    return history;
  }

  static Future<List<HistoryEntryGroup>> getFullHistory(String uid) async {
    List<HistoryEntryGroup> history = [];
    
    List<Season> seasons = await getAllSeasons(uid: uid);
    for (Season season in seasons) {
      List<HistoryEntryGroup> seasonHistory = await getHistory(uid, season.id);
      history.addAll(seasonHistory);
    }

    return history;
  }

  // ===============================
  //  Season meta data
  // ===============================

  static Future<Map<String, SeasonMeta>> getSeasonMetas() async
  {
    QuerySnapshot loadedSeasonMetas = await seasonsRef.orderBy("start", descending: true).get();
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

  static SeasonMeta? getActiveSeasonMeta()
  {
    for(SeasonMeta meta in DataService.seasonMetas.values)
    {
      if(meta.isActive()) return meta;
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

  static Future<List<Contract>> getAllContracts(String uid) async
  {
    List<Contract> progressions = [];

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

  static refresh() async
  {
    DataService.userData = null;
    DataService.seasons = {};
  }
}