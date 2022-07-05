import 'package:cloud_firestore/cloud_firestore.dart';

class BattlepassParams
{
  int epilogue;
  int levels;
  int lvl2Offset;
  int xpEpilogueStep;
  int xpStep;

  BattlepassParams(this.epilogue, this.levels, this.lvl2Offset, this.xpEpilogueStep, this.xpStep);

  static BattlepassParams fromDoc(DocumentSnapshot doc)
  {
    return BattlepassParams(
      doc['epilogue'] as int,
      doc['levels'] as int,
      doc['lvl2Offset'] as int,
      doc['xpEpilogueStep'] as int,
      doc['xpStep'] as int
    );
  }
}