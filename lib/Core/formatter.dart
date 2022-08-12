import 'package:intl/intl.dart';
import 'package:universal_io/io.dart';

class Formatter
{
  // --------------------------------
  // DateTime formatting
  // --------------------------------

  static String formatDate(DateTime dt)
  {
    return DateFormat.yMd(Platform.localeName).format(dt);
  }

  static String formatTime(DateTime dt)
  {
    return DateFormat.Hm(Platform.localeName).format(dt);
  }

  static formatDurationDays(Duration d)
  {
    return "${d.inDays} days";
  }




  // --------------------------------
  // Number formatting
  // --------------------------------

  static String formatXP(int xp)
  {
    return "$xp XP";
  }

  static String formatPercentage(double progress)
  {
    return '${(progress * 100).toStringAsFixed(0)}%';
  }

  static String formatLargeNumber(int number)
  {
    return NumberFormat.compact(locale: Platform.localeName).format(number);
  }
}