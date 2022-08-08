import 'package:intl/intl.dart';
import 'package:universal_io/io.dart';

class Formatter
{
  static String formatDate(DateTime dt)
  {
    return DateFormat.yMd(Platform.localeName).format(dt);
  }

  static String formatXP(int xp)
  {
    return "$xp XP";
  }
}