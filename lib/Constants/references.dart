import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_auth/firebase_auth.dart';

final _auth = FirebaseAuth.instance;
final _firestore = FirebaseFirestore.instance;

final agentsRef = _firestore.collection('agents');
final mapsRef = _firestore.collection('maps');
final modesRef = _firestore.collection('modes');
final parametersRef = _firestore.collection('parameters');
final seasonsRef = _firestore.collection('seasons');
final usersRef = _firestore.collection('users');