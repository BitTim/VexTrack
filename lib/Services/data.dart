import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/cupertino.dart';
import 'package:vextrack/Models/season.dart';
import 'package:vextrack/Services/auth.dart';

class DataService with ChangeNotifier
{
  final FirebaseFirestore _db;
  final AuthService _auth;
  DataService(this._db, this._auth);

  Stream<List<Season>> get seasons => _db.collection("users")
    .doc(_auth.currentUID)
    .collection("seasons")
    .snapshots()
    .map((snapshot) => snapshot.docs.map((doc) => Season.fromJSON(doc.data())).toList());
}