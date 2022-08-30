import 'package:cloud_firestore/cloud_firestore.dart';

class GameMap
{
  String name;
  bool inRotation;
  String imgURL;

  GameMap(this.name, this.inRotation, this.imgURL);

  static GameMap fromDoc(DocumentSnapshot doc)
  {
    return GameMap(
      doc['name'] as String,
      doc['inRotation'] as bool,
      doc['img'] as String
    );
  }
}