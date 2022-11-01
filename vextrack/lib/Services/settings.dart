import 'package:vextrack/Constants/references.dart';
import 'package:vextrack/Models/settings_data.dart';
import 'package:vextrack/Models/user_data.dart';
import 'package:vextrack/Services/data.dart';

class SettingsService // FIXME: This is just a placeholder before Settings get actually implemented
{
  static SettingsData? data;
  static String? uid;

  static setUID(String newUID) {
    uid = newUID;
  }

  static SettingsData getSettingsData() {
    if (data == null) {
      updateSettingsData(null);
      return data!;
    }

    return data!;
  }

  static Future<void> fetchSettingsData() async {
    if (uid == null) {
      updateSettingsData(null);
      return;
    }

    UserData ud = await DataService.getUserData(uid!);
    data = ud.settings;

    if (data == null) {
      updateSettingsData(null);
      return;
    }
  }

  static void updateSettingsData(SettingsData? sd) {
    sd ??= SettingsData.getDefault();
    data = sd;
  }

  static void syncSettingsData() async {
    if (uid == null) return;

    UserData ud = await DataService.getUserData(uid!);
    ud.settings = data ?? SettingsData.getDefault();
    usersRef.doc(uid).update(ud.toMap());
    DataService.userData = ud;
  }
}
