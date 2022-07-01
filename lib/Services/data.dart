import 'package:cached_network_image/cached_network_image.dart';
import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_storage/firebase_storage.dart';
import 'package:flutter/cupertino.dart';
import 'package:vextrack/Constants/references.dart';
import 'package:vextrack/Models/history_entry.dart';
import 'package:vextrack/Models/game_map.dart';
import 'package:vextrack/Models/season.dart';

class DataService
{
  // ===============================
  //  User data
  // ===============================

 

  // ===============================
  //  Season data
  // ===============================

  static Future<List<Season>> getAllSeasons(String uid) async {
    QuerySnapshot loadedSeasons = await usersRef.doc(uid)
      .collection("seasons")
      .orderBy("endDate", descending: true)
      .get();

    List<Season> seasons = loadedSeasons.docs.map((doc) => Season.fromDoc(doc)).toList();
    return seasons;
  }

  static Future<Season> getSeason(String uid, String uuid) async {
    DocumentSnapshot loadedSeason = await usersRef.doc(uid)
      .collection("seasons")
      .doc(uuid)
      .get();

    Season season = Season.fromDoc(loadedSeason);
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
  //  Map data
  // ===============================

  static Future<GameMap> getMap(String id) async
  {
    final loadedMap = await mapsRef.doc(id).get();

    GameMap map = GameMap.fromDoc(loadedMap);
    return map;
  }

  static Future<String> getMapImgUrl(String id) async
  {
    String storageURL = await getMap(id).then((map) => map.imgURL);
    String imgURL = await FirebaseStorage.instance.refFromURL(storageURL).getDownloadURL();
    return imgURL;
  }
}