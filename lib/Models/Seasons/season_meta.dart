import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:intl/intl.dart';
import 'package:universal_io/io.dart';

class SeasonMeta
{
  String id;
  String name;
  List<List<String>> battlepass;
  List<List<String>> epilogue;
  int startDate;
  int endDate;
  String imgURL;

  SeasonMeta(this.id, this.name, this.battlepass, this.epilogue, this.startDate, this.endDate, this.imgURL);

  static SeasonMeta fromDoc(DocumentSnapshot doc, String id, List<List<String>> battlepass, List<List<String>> epilogue)
  {
    return SeasonMeta(
      id,
      doc['name'] as String,
      battlepass,
      epilogue,
      doc['start'] as int,
      doc['end'] as int,
      doc['img'] as String
    );
  }

  DateTime getDateTime(int timestamp) { return DateTime.fromMillisecondsSinceEpoch(timestamp * 1000).toLocal(); }

  String getFormattedStartDate()
  {
    return DateFormat.yMd(Platform.localeName).format(getDateTime(startDate));
  }

  String getFormattedEndDate()
  {
    return DateFormat.yMd(Platform.localeName).format(getDateTime(endDate));
  }

  String getFormattedDuration() {
    var duration = getDateTime(endDate).difference(getDateTime(startDate));
    return '${duration.inDays} days';
  }
}