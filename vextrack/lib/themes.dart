import 'package:flutter/material.dart';
import 'package:flutter/scheduler.dart';
import 'package:vextrack/Services/settings.dart';

class AppThemes
{
  static ColorScheme? lightColorScheme;
  static ColorScheme? darkColorScheme;

  static ThemeData getTheme({bool forceDarkMode = false, bool forceLightMode = false})
  {
    Brightness brightness = SchedulerBinding.instance.window.platformBrightness;
    bool darkMode = brightness == Brightness.dark;

    if (forceDarkMode) darkMode = true;
    if (forceLightMode) darkMode = false;

    return ThemeData(
      colorScheme: (darkMode ? darkColorScheme : lightColorScheme) ?? ColorScheme.fromSeed(
        seedColor: SettingsService.accent,
        brightness: darkMode ? Brightness.dark : Brightness.light,
      ),
      useMaterial3: true,
      visualDensity: VisualDensity.standard,
    );
  }

  static void setThemes(ColorScheme? lightColorScheme, ColorScheme? darkColorScheme)
  {
    AppThemes.lightColorScheme = lightColorScheme;
    AppThemes.darkColorScheme = darkColorScheme;
  }
}