import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Fragments/Forms/Settings/password_change.dart';
import 'package:vextrack/Fragments/Forms/Settings/profile_edit.dart';
import 'package:vextrack/Models/settings_data.dart';
import 'package:vextrack/Models/user_data.dart';
import 'package:vextrack/Services/data.dart';
import 'package:vextrack/Services/settings.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/screen_manager.dart';

class _SettingsState extends State<Settings> {
  late Function(int) _notifyParent;
  late SettingsData sd;
  late List<bool> selectedTheme;
  bool profileImageHovered = false;

  _SettingsState(Function(int) notifyParent) {
    _notifyParent = notifyParent;
  }

  @override
  void initState() {
    init();
    sd = SettingsService.getSettingsData();
    selectedTheme = [
      sd.theme == "auto",
      sd.theme == "light",
      sd.theme == "dark"
    ];

    super.initState();
  }

  void init() async {
    await SettingsService.fetchSettingsData(null);
  }

  void updateTheme(Color accent) {
    setState(() {
      sd.accentColor = accent;
      SettingsService.updateSettingsData(sd);
      widget.onThemeChanged();
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () {
            _notifyParent(Screens.home.index);
            SettingsService.syncSettingsData();
          },
        ),
        title: const Text('Settings'),
        actions: [
          IconButton(
              onPressed: () {
                // TODO: Add about popup
              },
              icon: const Icon(Icons.info_outline)),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
        child: SingleChildScrollView(
          child: FutureBuilder(
              future: DataService.getUserData(widget.user.uid),
              builder: (context, snapshot) {
                if (snapshot.hasData == false) return const SizedBox.shrink();
                UserData ud = snapshot.data as UserData;

                return Column(
                  children: [
                    // PROFILE_START
                    Padding(
                      padding: const EdgeInsets.fromLTRB(0, 0, 0, 32),
                      child: Column(
                        children: [
                          Padding(
                            padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
                            child: Row(
                              children: [
                                Text(
                                  "Profile",
                                  style: GoogleFonts.titilliumWeb(
                                    fontSize: 24,
                                    fontWeight: FontWeight.w700,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          AspectRatio(
                            aspectRatio: 4 / 1,
                            child: Row(
                              children: [
                                Flexible(
                                    flex: 1,
                                    child: MouseRegion(
                                      onEnter: (event) {
                                        setState(
                                            () => profileImageHovered = true);
                                      },
                                      onExit: (event) {
                                        setState(
                                            () => profileImageHovered = false);
                                      },
                                      child: IconButton(
                                        onPressed: () {},
                                        icon: AspectRatio(
                                          aspectRatio: 1 / 1,
                                          child: Container(
                                            decoration: BoxDecoration(
                                              borderRadius:
                                                  BorderRadius.circular(8),
                                              color: Colors
                                                  .red, // TODO: Replace with image
                                            ),
                                            child: Visibility(
                                              visible: profileImageHovered,
                                              child: const Align(
                                                alignment:
                                                    Alignment.bottomRight,
                                                child: Padding(
                                                  padding: EdgeInsets.all(8.0),
                                                  child: Icon(
                                                    Icons.camera_alt,
                                                    color: Colors.white,
                                                    shadows: [
                                                      Shadow(
                                                          color:
                                                              Color(0xcc000000),
                                                          blurRadius: 8)
                                                    ],
                                                  ),
                                                ),
                                              ),
                                            ),
                                          ),
                                        ),
                                      ),
                                    )),
                                Flexible(
                                    flex: 3,
                                    child: Padding(
                                      padding: const EdgeInsets.fromLTRB(
                                          16, 0, 0, 0),
                                      child: Column(
                                        mainAxisSize: MainAxisSize.max,
                                        mainAxisAlignment:
                                            MainAxisAlignment.spaceBetween,
                                        children: [
                                          Column(
                                            children: [
                                              Row(
                                                children: [
                                                  Text(
                                                    ud.name,
                                                    style: GoogleFonts
                                                        .titilliumWeb(
                                                      fontSize: 32,
                                                    ),
                                                  ),
                                                ],
                                              ),
                                              Row(
                                                children: [
                                                  Text(widget.user.email ?? "",
                                                      style: GoogleFonts
                                                          .titilliumWeb(
                                                        color: Theme.of(context)
                                                            .colorScheme
                                                            .outline,
                                                      )),
                                                ],
                                              ),
                                            ],
                                          ),
                                          Row(
                                            children: [
                                              Padding(
                                                padding:
                                                    const EdgeInsets.fromLTRB(
                                                        0, 0, 4, 0),
                                                child: ElevatedButton(
                                                  onPressed: () {
                                                    showEditProfileDialog(
                                                        context);
                                                  },
                                                  child: const Text("Edit"),
                                                ),
                                              ),
                                              Padding(
                                                padding:
                                                    const EdgeInsets.fromLTRB(
                                                        4, 0, 0, 0),
                                                child: ElevatedButton(
                                                  onPressed: () {
                                                    showChangePasswordDialog(
                                                        context);
                                                  },
                                                  child: const Text(
                                                      "Change Password"),
                                                ),
                                              ),
                                            ],
                                          )
                                        ],
                                      ),
                                    ))
                              ],
                            ),
                          ),
                        ],
                      ),
                    ),

                    // PROFILE_END
                    // GENERAL_START

                    Padding(
                      padding: const EdgeInsets.fromLTRB(0, 0, 0, 32),
                      child: Column(
                        children: [
                          Padding(
                            padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
                            child: Row(
                              children: [
                                Text(
                                  "General",
                                  style: GoogleFonts.titilliumWeb(
                                    fontSize: 24,
                                    fontWeight: FontWeight.w700,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Column(
                            children: [
                              Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  const Text("Buffer Days"),
                                  Expanded(
                                    child: Slider(
                                      label: "${sd.bufferDays}",
                                      value: sd.bufferDays,
                                      max: 14,
                                      divisions: 14,
                                      onChanged: (value) {
                                        setState(() {
                                          sd.bufferDays = value;
                                          SettingsService.updateSettingsData(
                                              sd);
                                        });
                                      },
                                    ),
                                  ),
                                ],
                              ),
                            ],
                          ),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text("Ignore inactive days"),
                              Switch(
                                  value: sd.ignoreInactiveDays,
                                  onChanged: (value) {
                                    setState(() {
                                      sd.ignoreInactiveDays = value;
                                      SettingsService.updateSettingsData(sd);
                                    });
                                  })
                            ],
                          ),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text("Hide init"),
                              Switch(
                                  value: sd.hideInit,
                                  onChanged: (value) {
                                    setState(() {
                                      sd.hideInit = value;
                                      SettingsService.updateSettingsData(sd);
                                    });
                                  })
                            ],
                          ),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text("Single season history"),
                              Switch(
                                  value: sd.singleSeasonHistory,
                                  onChanged: (value) {
                                    setState(() {
                                      sd.singleSeasonHistory = value;
                                      SettingsService.updateSettingsData(sd);
                                    });
                                  })
                            ],
                          ),
                        ],
                      ),
                    ),

                    // GENERAL_END
                    // APPEARANCE_START

                    Padding(
                      padding: const EdgeInsets.fromLTRB(0, 0, 0, 32),
                      child: Column(
                        children: [
                          Padding(
                            padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
                            child: Row(
                              children: [
                                Text(
                                  "Appearance",
                                  style: GoogleFonts.titilliumWeb(
                                    fontSize: 24,
                                    fontWeight: FontWeight.w700,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text("Theme"),
                              ToggleButtons(
                                onPressed: (int index) {
                                  setState(() {
                                    for (int i = 0;
                                        i < selectedTheme.length;
                                        i++) {
                                      selectedTheme[i] = i == index;
                                    }

                                    if (index == 0) sd.theme = "auto";
                                    if (index == 1) sd.theme = "light";
                                    if (index == 2) sd.theme = "dark";

                                    SettingsService.updateSettingsData(sd);
                                    widget.onThemeChanged();
                                  });
                                },
                                borderRadius:
                                    const BorderRadius.all(Radius.circular(8)),
                                constraints: const BoxConstraints(
                                  minHeight: 40.0,
                                  minWidth: 40.0,
                                ),
                                isSelected: selectedTheme,
                                children: const [
                                  Icon(Icons.lightbulb),
                                  Icon(Icons.light_mode),
                                  Icon(Icons.dark_mode),
                                ],
                              ),
                            ],
                          ),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text("Override system color"),
                              Switch(
                                  value: sd.overrideSysColor,
                                  onChanged: (value) {
                                    setState(() {
                                      sd.overrideSysColor = value;
                                      SettingsService.updateSettingsData(sd);
                                    });
                                  })
                            ],
                          ),
                          Column(
                            children: [
                              Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  const Text("Accent color"),
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceBetween,
                                    children: [
                                      Radio(
                                        value: AppColors.defaultAccent,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) =>
                                                    AppColors.defaultAccent),
                                        onChanged: (value) {
                                          updateTheme(AppColors.defaultAccent);
                                        },
                                      ),
                                      Radio(
                                        value: AppColors.accent1,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) => AppColors.accent1),
                                        onChanged: (value) {
                                          updateTheme(AppColors.accent1);
                                        },
                                      ),
                                      Radio(
                                        value: AppColors.accent2,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) => AppColors.accent2),
                                        onChanged: (value) {
                                          updateTheme(AppColors.accent2);
                                        },
                                      ),
                                      Radio(
                                        value: AppColors.accent3,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) => AppColors.accent3),
                                        onChanged: (value) {
                                          updateTheme(AppColors.accent3);
                                        },
                                      ),
                                      Radio(
                                        value: AppColors.accent4,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) => AppColors.accent4),
                                        onChanged: (value) {
                                          updateTheme(AppColors.accent4);
                                        },
                                      ),
                                      Radio(
                                        value: AppColors.accent5,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) => AppColors.accent5),
                                        onChanged: (value) {
                                          updateTheme(AppColors.accent5);
                                        },
                                      ),
                                      Radio(
                                        value: AppColors.accent6,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) => AppColors.accent6),
                                        onChanged: (value) {
                                          updateTheme(AppColors.accent6);
                                        },
                                      ),
                                      Radio(
                                        value: AppColors.accent7,
                                        groupValue: sd.accentColor,
                                        fillColor:
                                            MaterialStateColor.resolveWith(
                                                (states) => AppColors.accent7),
                                        onChanged: (value) {
                                          updateTheme(AppColors.accent7);
                                        },
                                      ),
                                    ],
                                  ),
                                ],
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),

                    // APPEARANCE_END
                    // DANGERZONE_START

                    Padding(
                      padding: const EdgeInsets.fromLTRB(0, 0, 0, 32),
                      child: Column(
                        children: [
                          Padding(
                            padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
                            child: Row(
                              children: [
                                Text(
                                  "Danger Zone",
                                  style: GoogleFonts.titilliumWeb(
                                    fontSize: 24,
                                    fontWeight: FontWeight.w700,
                                  ),
                                ),
                              ],
                            ),
                          ),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Expanded(
                                child: Padding(
                                  padding:
                                      const EdgeInsets.fromLTRB(0, 0, 4, 0),
                                  child: ElevatedButton(
                                    onPressed: () {
                                      //TODO: Implement account deletion
                                    },
                                    child: const Text("Delete Account"),
                                  ),
                                ),
                              ),
                              Expanded(
                                child: Padding(
                                  padding:
                                      const EdgeInsets.fromLTRB(4, 0, 0, 0),
                                  child: ElevatedButton(
                                    onPressed: () {
                                      // TODO: Implement data reset
                                    },
                                    child: const Text("Reset Data"),
                                  ),
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),

                    // GENERAL_END
                  ],
                );
              }),
        ),
      ),
    );
  }

  void showEditProfileDialog(BuildContext context) {
    showDialog(
        context: context,
        builder: (context) {
          return AlertDialog(
            scrollable: false,
            title: const Text('Edit profile'),
            shape: const RoundedRectangleBorder(
              borderRadius: BorderRadius.all(
                Radius.circular(16),
              ),
            ),
            content: Builder(
              builder: (context) {
                var width = MediaQuery.of(context).size.width;

                return SizedBox(
                  width: width -
                      128, //TODO: Responsive layout (Fullscrenn diag on phones, restrained width on Desktop)
                  child: const Padding(
                    padding: EdgeInsets.all(8.0),
                    child: ProfileEditForm(),
                  ),
                );
              },
            ),
            actions: [
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: const Text("Cancel"),
              ),
              ElevatedButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: const Text("Apply"),
              )
            ],
          );
        });
  }

  void showChangePasswordDialog(BuildContext context) {
    showDialog(
        context: context,
        builder: (context) {
          return AlertDialog(
            scrollable: false,
            title: const Text('Change Password'),
            shape: const RoundedRectangleBorder(
              borderRadius: BorderRadius.all(
                Radius.circular(16),
              ),
            ),
            content: Builder(
              builder: (context) {
                var width = MediaQuery.of(context).size.width;

                return SizedBox(
                  width: width -
                      128, //TODO: Responsive layout (Fullscrenn diag on phones, restrained width on Desktop)
                  child: const Padding(
                    padding: EdgeInsets.all(8.0),
                    child: PasswordChangeForm(),
                  ),
                );
              },
            ),
            actions: [
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: const Text("Cancel"),
              ),
              ElevatedButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: const Text("Change"),
              )
            ],
          );
        });
  }
}

class Settings extends StatefulWidget {
  final User user;
  final Function(int) notifyParent;
  final Function() onThemeChanged;

  const Settings(
      {Key? key,
      required this.user,
      required this.notifyParent,
      required this.onThemeChanged})
      : super(key: key);

  @override
  State createState() {
    // ignore: no_logic_in_create_state
    return _SettingsState(notifyParent);
  }
}
