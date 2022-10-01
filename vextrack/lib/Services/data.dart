import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart';
import 'package:uuid/uuid.dart';
import 'package:vextrack/Constants/references.dart';
import 'package:vextrack/Core/util.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/Contracts/goal.dart';
import 'package:vextrack/Models/Contracts/contract.dart';
import 'package:vextrack/Models/History/history_entry.dart';
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
  static Map<String, Contract> contracts = {};
  static bool _loading = false;

  static void init() async
  {
    DataService.battlepassParams = await getBattlepassParams();
    DataService.modes = await getModes();
    DataService.maps = await getMaps();
    DataService.seasonMetas = await pullAllSeasonMetas();
  }

  // ===============================
  //  Parameter data
  // ===============================

  static Future<BattlepassParams> getBattlepassParams() async
  {
    _loading = true;
    DocumentSnapshot doc = await parametersRef.doc('battlepass').get();
    _loading = false;

    return BattlepassParams.fromDoc(doc);
  }

  // ===============================
  //  User data
  // ===============================

  static Future<void> fetchUserData(String uid) async
  {
    _loading = true;
    DocumentSnapshot doc = await usersRef.doc(uid).get();
    userData = UserData.fromDoc(doc);
    _loading = false;
  }

  static Future<List<Season>> getAllSeasons({required String uid, bool activeOnly = false}) async {
    if(userData == null) await fetchUserData(uid);

    _loading = true;
    Map<int, Season> seasonOrder = {};
    for(String id in userData!.seasonIDs.keys)
    {
      bool active = userData!.seasonIDs[id]!.active;

      if(activeOnly && !active) continue;

      int idx = seasonMetas.keys.toList().indexOf(id);
      seasonOrder[idx] = await getSeason(uid, id);
    }
    
    List<int> indecies = seasonOrder.keys.toList();
    indecies.sort();

    List<Season> seasons = [
      for (int i in indecies) seasonOrder[i]!
    ];
    _loading = false;

    return seasons;
  }

  static Future<Season> getSeason(String uid, String id) async {
    if (seasons[id] != null) return seasons[id]!;

    _loading = true;
    DocumentSnapshot loadedSeason = await usersRef.doc(uid)
      .collection("seasons")
      .doc(id)
      .get();

    List<HistoryEntryGroup> history = await getHistory(uid, id);

    Season season = Season.fromDoc(loadedSeason, seasonMetas[id]!, history);
    seasons[id] = season;
    _loading = false;
    return season;
  }



  static Future<List<HistoryEntryGroup>> getHistory(String uid, String seasonID) async {
    if(seasons[seasonID] != null) return seasons[seasonID]!.history;

    _loading = true;
    QuerySnapshot loadedHistory = await usersRef.doc(uid)
      .collection("seasons")
      .doc(seasonID)
      .collection("history")
      .orderBy("day", descending: true)
      .get();

    List<HistoryEntryGroup> history = loadedHistory.docs.map((doc) => HistoryEntryGroup.fromDoc(doc)).toList();
    _loading = false;
    return history;
  }

  static Future<List<HistoryEntryGroup>> pullFullHistory(String uid) async {
    _loading = true;
    List<HistoryEntryGroup> history = [];
    
    List<Season> seasons = await getAllSeasons(uid: uid);
    for (Season season in seasons) {
      List<HistoryEntryGroup> seasonHistory = await getHistory(uid, season.id);
      history.addAll(seasonHistory);
    }

    _loading = false;
    return history;
  }

  // ===============================
  //  Season meta data
  // ===============================

  static Future<Map<String, SeasonMeta>> pullAllSeasonMetas() async
  {
    _loading = true;
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
    _loading = false;

    return seasonMetas;
  }

  static Future<SeasonMeta?> getActiveSeasonMeta() async
  {
    if(DataService.seasonMetas.isEmpty && !_loading) await pullAllSeasonMetas();
    //await Util.waitWhile(() => _loading);

    _loading = true;
    for(SeasonMeta meta in DataService.seasonMetas.values)
    {
      if(meta.isActive())
      {
        _loading = false;
        return meta;
      }
    }
    _loading = false;

    return null;
  }

  static SeasonMeta? getSeasonMetaFromTime(Timestamp time)
  {
    for(SeasonMeta meta in seasonMetas.values)
    {
      if (meta.startDate.compareTo(time) > 0) continue;
      if (meta.endDate.compareTo(time) > 0) return meta;
    }

    return null;
  }

  // ===============================
  //  Map data
  // ===============================

  static Future<Map<String, GameMap>> getMaps() async
  {
    _loading = true;
    QuerySnapshot loadedMaps = await mapsRef.get();
    Map<String, GameMap> maps = {};

    for (QueryDocumentSnapshot doc in loadedMaps.docs)
    {
      GameMap map = GameMap.fromDoc(doc);
      maps[doc.id] = map;
    }
    _loading = false;

    return maps;
  }

  static String getMapName(String id)
  {
    if(DataService.maps[id] == null) return "";
    return DataService.maps[id]!.name;
  }

  static Future<String> getMapImgUrl(String id) async
  {
    _loading = true;
    if(DataService.maps[id] == null) id = "none";
    String storageURL = DataService.maps[id]!.imgURL;
    String imgURL = await FirebaseStorage.instance.refFromURL(storageURL).getDownloadURL();
    _loading = false;
    return imgURL;
  }

  // ===============================
  //  Mode Data
  // ===============================

  static Future<Map<String, GameMode>> getModes() async
  {
    _loading = true;
    QuerySnapshot loadedModes = await modesRef.get();
    Map<String, GameMode> modes = {};

    for(QueryDocumentSnapshot doc in loadedModes.docs)
    {
      GameMode mode = GameMode.fromDoc(doc);
      modes[doc.id] = mode;
    }
    _loading = false;

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
  //  Contract Data
  // ===============================

  static Future<Contract> getContract(String uid, String id) async {
    if (contracts[id] != null) return contracts[id]!;
    if(userData == null && !_loading) await fetchUserData(uid);
    await Util.waitWhile(() => _loading);

    _loading = true;
    DocumentSnapshot loadedContract = await usersRef.doc(uid)
      .collection("contracts")
      .doc(id)
      .get();

    Contract contract = Contract.fromDoc(loadedContract, userData!.contractIDs[id]!.paused);
    QuerySnapshot loadedGoals = await usersRef.doc(uid)
      .collection("contracts")
      .doc(id)
      .collection("goals")
      .orderBy("order", descending: false)
      .get();
      
    contract.goals = loadedGoals.docs.map((doc) => Goal.fromDoc(doc, contract)).toList();
    _loading = false;
    return contract;
  }

  static Future<List<Contract>> getAllContracts({required String uid, bool incompleteOnly = false}) async {
    if(userData == null && !_loading) await fetchUserData(uid);
    List<Contract> loadedContrcats = [];

    for(String id in userData!.contractIDs.keys)
    {
      bool completed = userData!.contractIDs[id]!.completed;
      if(incompleteOnly && completed) continue;

      loadedContrcats.add(await getContract(uid, id));
    }

    return loadedContrcats;
  }

  static refresh()
  {
    DataService.userData = null;
    DataService.seasons = {};
    DataService.contracts = {};
  }







  // ================================
  //  Create data
  // ================================

  static Future<void> updateHistoryEntry(String uid, int idx, Season season, SeasonMeta meta) async
  {
    usersRef.doc(uid)
      .collection("seasons")
      .doc(meta.id)
      .collection("history")
      .doc(season.history[idx].id).set(season.history[idx].toMap(), SetOptions(merge: true));

    usersRef.doc(uid)
      .collection("seasons")
      .doc(meta.id)
      .update(season.toMap());
  }

  static Future<void> updateContract(String uid, Contract contract) async
  {
    Contract c = await getContract(uid, contract.id);

    updateContractMeta(uid: uid, id: c.id, paused: contract.isPaused(), completed: contract.isCompleted());

    usersRef.doc(uid)
      .collection('contracts')
      .doc(contract.id)
      .update(contract.toMap());
    
    for(Goal goal in contract.goals)
    {
      usersRef.doc(uid)
        .collection('contracts')
        .doc(contract.id)
        .collection('goals')
        .doc(goal.id)
        .update(goal.toMap());
    }
  }

  static Future<void> updateContractMeta({required String uid, required String id, bool? paused, bool? completed}) async
  {
    Contract c = await getContract(uid, id);
    paused ??= c.isPaused();
    completed ??= c.isCompleted();

    if(c.isPaused() == paused && c.isCompleted() == completed) return;

    if(userData == null && !_loading) await fetchUserData(uid); 
    userData!.contractIDs[id] = ContractActivityMeta(completed: completed, paused: paused);
    usersRef.doc(uid).update(userData!.toMap());
  }

  static Future<void> addHistoryEntry(String uid, HistoryEntry he) async
  {
    SeasonMeta? meta = getSeasonMetaFromTime(he.time);
    if (meta == null) return;

    Season season = await getSeason(uid, meta.id);
    Timestamp date = Timestamp.fromDate(he.getDate());
    int day = date.toDate().difference(meta.startDate.toDate()).inDays;
    int idx = season.history.indexWhere((element) => element.day == day);

    if (idx == -1)
    {
      HistoryEntryGroup heg = HistoryEntryGroup(const Uuid().v4(), day, 0, date, []);
      season.history.insert(0, heg);
      idx = 0;
    }

    int total = XPCalc.toTotal(season.activeLevel, season.activeXP) + he.xp;
    List<int> levelXP = XPCalc.toLevelXP(total);
    season.activeLevel = levelXP[0];
    season.activeXP = levelXP[1];

    season.history[idx].addEntry(he);
    seasons[meta.id] = season;

    updateHistoryEntry(uid, idx, season, meta);

    for(String id in userData!.contractIDs.keys)
    {
      Contract contract = await getContract(uid, id);
      bool completed = userData!.contractIDs[id]!.completed;
      bool paused = userData!.contractIDs[id]!.paused;

      if (paused || completed) continue;

      contract.addXP(he.xp);
      contracts[id] = contract;
      
      updateContract(uid, contract);
    }
  }
}