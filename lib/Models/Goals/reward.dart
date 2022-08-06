import 'package:cloud_firestore/cloud_firestore.dart';

class Reward
{
  String name;
  String imgURL;

  Reward(this.name, this.imgURL);

  static Reward fromDoc(DocumentSnapshot doc)
  {
    return Reward(
      doc['name'] as String,
      doc['img'] as String
    );
  }
}