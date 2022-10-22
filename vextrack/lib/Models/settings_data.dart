import 'package:vextrack/Constants/colors.dart';

class SettingsData
{
  double bufferDays;
  bool ignoreInactiveDays;
  bool ingoreInit;
  bool singleSeasonHistory;

  String theme;
  bool useMaterialYou;
  String accentColor;

  SettingsData({
    required this.bufferDays,
    required this.ignoreInactiveDays,
    required this.ingoreInit,
    required this.singleSeasonHistory,

    required this.theme,
    required this.useMaterialYou,
    required this.accentColor,
  });

  static SettingsData getDefault()
  {
    return SettingsData(
      bufferDays: 7,
      ignoreInactiveDays: false,
      ingoreInit: true,
      singleSeasonHistory: false,
      theme: "auto",
      useMaterialYou: true,
      accentColor: AppColors.defaultAccent.toString(),
    );
  }
}