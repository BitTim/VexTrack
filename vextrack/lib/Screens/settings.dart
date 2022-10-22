import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Models/settings_data.dart';
import 'package:vextrack/Models/user_data.dart';
import 'package:vextrack/Services/data.dart';
import 'package:vextrack/Services/settings.dart';
import 'package:vextrack/screen_manager.dart';

class _SettingsState extends State<Settings> {
  late Function(int) _notifyParent;
  late SettingsData sd;
  late List<bool> selectedTheme;

  _SettingsState(Function(int) notifyParent)
  {
    _notifyParent = notifyParent;
  }

  @override
  void initState()
  {
    sd = SettingsService.getSettingsData(widget.user.uid);
    selectedTheme = [ sd.theme == "auto", sd.theme == "light", sd.theme == "dark" ];

    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Settings'),
        actions: [
          IconButton(
            onPressed: () {
              _notifyParent(Screens.home.index);
            },
            icon: const Icon(Icons.check)
          ),
        ],
      ),

      body: Padding(
        padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
        child: SingleChildScrollView(
          child: FutureBuilder(
            future: DataService.getUserData(widget.user.uid),
            builder: (context, snapshot) {
              if(snapshot.hasData == false) return const SizedBox.shrink();
              UserData ud = snapshot.data as UserData;

              return Column(
                children: [
                  // PROFILE_START
                  Padding(
                    padding: const EdgeInsets.fromLTRB(0, 0, 0, 32),
                    child: Column(
                      children: [
                        Row(
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
                        Padding(
                          padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
                          child: AspectRatio(
                            aspectRatio: 4 / 1,
                            child: Row(
                              children: [
                                Flexible(
                                  flex: 1,
                                  child: AspectRatio(
                                    aspectRatio: 1 / 1,
                                    child: Container(
                                      decoration: BoxDecoration(
                                        borderRadius: BorderRadius.circular(8),
                                        color: Colors.red, // TODO: Replace with image
                                      ),
                                    ),
                                  )
                                ),
                                Flexible(
                                  flex: 3,
                                  child: Padding(
                                    padding: const EdgeInsets.fromLTRB(16, 0, 0, 0),
                                    child: Column(
                                      mainAxisSize: MainAxisSize.max,
                                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                      children: [
                                        Column(
                                          children: [
                                            Row(
                                              children: [
                                                Text(
                                                  ud.name,
                                                  style: GoogleFonts.titilliumWeb(
                                                    fontSize: 32,
                                                  ),
                                                ),
                                              ],
                                            ),
                                            Row(
                                              children: [
                                                Text(
                                                  widget.user.email ?? "",
                                                  style: GoogleFonts.titilliumWeb(
                                                    color: Theme.of(context).colorScheme.outline,
                                                  )
                                                ),
                                              ],
                                            ),
                                          ],
                                        ),
                                        Row(
                                          children: [
                                            Padding(
                                              padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                                              child: ElevatedButton(
                                                onPressed: () {},
                                                child: const Text("Edit"),
                                              ),
                                            ),
                                            Padding(
                                              padding: const EdgeInsets.fromLTRB(4, 0, 0, 0),
                                              child: ElevatedButton(
                                                onPressed: () {},
                                                child: const Text("Change Password"),
                                              ),
                                            ),
                                          ],
                                        )
                                      ],
                                    ),
                                  )
                                )
                              ],
                            ),
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
                        Row(
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
                        Padding(
                          padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
                          child: Column(
                            children: [
                              Row(
                                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                children: [
                                  const Text("Buffer Days"),
                                  Text("${sd.bufferDays}"),
                                ],
                              ),
                              Slider(
                                value: sd.bufferDays,
                                max: 14,
                                divisions: 14,
                                onChanged: (value) {
                                  setState(() {
                                    sd.bufferDays = value;
                                  });
                                },
                              )
                            ],
                          ),
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
                                });
                              }
                            )
                          ],
                        ),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            const Text("Ignore init"),
                            Switch(
                              value: sd.ingoreInit,
                              onChanged: (value) {
                                setState(() {
                                  sd.ingoreInit = value;
                                });
                              }
                            )
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
                                });
                              }
                            )
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
                        Row(
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
                        Padding(
                          padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
                          child: Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              const Text("Theme"),
                              ToggleButtons(
                                onPressed: (int index) {
                                  setState(() {
                                    for (int i = 0; i < selectedTheme.length; i++) {
                                      selectedTheme[i] = i == index;
                                    }

                                    if(index == 0) sd.theme = "auto";
                                    if(index == 1) sd.theme = "light";
                                    if(index == 2) sd.theme = "dark";
                                  });
                                },
                                borderRadius: const BorderRadius.all(Radius.circular(8)),
                                // selectedBorderColor: theme.colorScheme.outline,
                                // selectedColor: theme.colorScheme.onPrimaryContainer,
                                // fillColor: theme.colorScheme.primaryContainer,
                                // color: theme.colorScheme.onSurface,
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
                        ),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            const Text("Ignore init"),
                            Switch(
                              value: sd.ingoreInit,
                              onChanged: (value) {
                                setState(() {
                                  sd.ingoreInit = value;
                                });
                              }
                            )
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
                                });
                              }
                            )
                          ],
                        ),
                      ],
                    ),
                  ),

                  // APPEARANCE_END
                ],
              );
            }
          ),
        ),
      ),
    );
  }
}

class Settings extends StatefulWidget
{
  final User user;
  final Function(int) notifyParent;

  const Settings({Key? key, required this.user, required this.notifyParent}) : super(key: key);

  @override
  State createState()
  {
    // ignore: no_logic_in_create_state
    return _SettingsState(notifyParent);
  }
}