import 'dart:ui';

import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Core/util.dart';

class SettingsData {
  double bufferDays;
  bool ignoreInactiveDays;
  bool hideInit;
  bool singleSeasonHistory;

  String theme;
  bool overrideSysColor;
  Color accentColor;

  SettingsData({
    required this.bufferDays,
    required this.ignoreInactiveDays,
    required this.hideInit,
    required this.singleSeasonHistory,
    required this.theme,
    required this.overrideSysColor,
    required this.accentColor,
  });

  static SettingsData getDefault() {
    return SettingsData(
      bufferDays: 7,
      ignoreInactiveDays: false,
      hideInit: true,
      singleSeasonHistory: false,
      theme: "auto",
      overrideSysColor: true,
      accentColor: AppColors.defaultAccent,
    );
  }

  static SettingsData? fromMap(Map<String, dynamic>? map) {
    if (map == null || map.isEmpty) return null;

    return SettingsData(
        bufferDays: map['bufferDays'],
        ignoreInactiveDays: map['ignoreInactiveDays'],
        hideInit: map['ignoreInit'],
        singleSeasonHistory: map['singleSeasonHistory'],
        theme: map['theme'],
        overrideSysColor: map['overrideSysColor'],
        accentColor: (map['accentColor'] as String).toColor());
  }

  Map<String, dynamic> toMap() {
    Map<String, dynamic> map = {};

    map['bufferDays'] = bufferDays;
    map['ignoreInactiveDays'] = ignoreInactiveDays;
    map['ignoreInit'] = hideInit;
    map['singleSeasonHistory'] = singleSeasonHistory;
    map['theme'] = theme;
    map['overrideSysColor'] = overrideSysColor;
    map['accentColor'] = accentColor.value.toRadixString(16);

    return map;
  }
}
