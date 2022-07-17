import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:intl/intl.dart';
import 'package:universal_io/io.dart';

class SeasonMeta
{
  String name;
  int startDate;
  int endDate;
  String imgURL;

  SeasonMeta(this.name, this.startDate, this.endDate, this.imgURL);

  static SeasonMeta fromDoc(DocumentSnapshot doc)
  {
    return SeasonMeta(
      doc['name'] as String,
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
}