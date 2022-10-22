import 'package:flutter/material.dart';
import 'package:vextrack/Models/settings_data.dart';
import '../Constants/colors.dart';

class SettingsService // FIXME: This is just a placeholder before Settings get actually implemented
{
  static int bufferDays = 8;
  static Color accent = AppColors.defaultAccent;
  static SettingsData? data;

  static SettingsData getSettingsData(String uid)
  {
    if(data == null) return SettingsData.getDefault(); //TODO: Try to fetch before setting default and save defaults after using
    return data!;
  }
}