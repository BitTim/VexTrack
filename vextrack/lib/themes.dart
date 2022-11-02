import 'package:flutter/material.dart';
import 'package:flutter/scheduler.dart';
import 'package:vextrack/Services/settings.dart';

class AppThemes {
  static ColorScheme? lightColorScheme;
  static ColorScheme? darkColorScheme;

  static ThemeData getTheme(
      {bool forceDarkMode = false, bool forceLightMode = false}) {
    Brightness brightness = SchedulerBinding.instance.window.platformBrightness;
    bool darkMode = brightness == Brightness.dark;

    if (SettingsService.getSettingsData().theme == "light") {
      forceLightMode = true;
    }
    if (SettingsService.getSettingsData().theme == "dark") forceDarkMode = true;

    if (forceDarkMode) darkMode = true;
    if (forceLightMode) darkMode = false;

    Color seed = (SettingsService.getSettingsData()).accentColor;
    bool overrideSysColor =
        (SettingsService.getSettingsData()).overrideSysColor;
    ColorScheme? checkedScheme;
    ColorScheme scheme;

    if (darkMode) {
      checkedScheme = darkColorScheme;
    } else {
      checkedScheme = lightColorScheme;
    }

    if (overrideSysColor || checkedScheme == null) {
      scheme = ColorScheme.fromSeed(
          seedColor: seed,
          brightness: darkMode ? Brightness.dark : Brightness.light);
    } else {
      scheme = checkedScheme;
    }

    return ThemeData(
      colorScheme: scheme,
      useMaterial3: true,
      visualDensity: VisualDensity.standard,
    );
  }

  static void setThemes(
      ColorScheme? lightColorScheme, ColorScheme? darkColorScheme) {
    AppThemes.lightColorScheme = lightColorScheme;
    AppThemes.darkColorScheme = darkColorScheme;
  }
}
