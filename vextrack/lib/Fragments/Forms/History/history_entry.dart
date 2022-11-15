import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:uuid/uuid.dart';
import 'package:vextrack/Components/history_entry.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Services/data.dart';

class HistoryEntryForm extends StatefulWidget {
  final GlobalKey<FormState> formKey;
  final HistoryEntry? initEntry;

  final HistoryEntry model = HistoryEntry(
    const Uuid().v4(),
    "",
    DataService.maps.keys.first,
    DataService.modes.keys.last,
    0,
    0,
    0,
    Timestamp.fromMillisecondsSinceEpoch(0),
    "none",
  );

  HistoryEntryForm({
    super.key,
    required this.formKey,
    this.initEntry,
  });

  @override
  State<HistoryEntryForm> createState() => HistoryEntryFormState();
}

class HistoryEntryFormState extends State<HistoryEntryForm> {
  String modelUUID = const Uuid().v4();
  String selectedModeID = DataService.modes.keys.last;
  String selectedMapID = DataService.maps.keys.first;
  String surrender = "none";
  String timeString = DateTime.now().toString();

  TextEditingController scoreController = TextEditingController();
  TextEditingController enemyScoreController = TextEditingController();
  TextEditingController xpController = TextEditingController();
  TextEditingController descController = TextEditingController();
  TextEditingController timeDisplayController = TextEditingController();

  void setTimeDisplayText() {
    setState(() {
      timeDisplayController.text =
          Formatter.formatDateTime(DateTime.parse(timeString));
    });
  }

  @override
  void initState() {
    super.initState();

    if (widget.initEntry != null) {
      initValues(
        widget.initEntry!.uuid,
        widget.initEntry!.mode,
        widget.initEntry!.desc,
        widget.initEntry!.score,
        widget.initEntry!.enemyScore,
        widget.initEntry!.map,
        widget.initEntry!.xp,
        widget.initEntry!.getDateTime().toString(),
      );
    }

    updateModel();

    scoreController.addListener(updateModel);
    enemyScoreController.addListener(updateModel);
    xpController.addListener(updateModel);
    descController.addListener(updateModel);

    setTimeDisplayText();
  }

  void initValues(String uuid, String modeID, String desc, int score,
      int enemyScore, String mapID, int xp, String time) {
    setState(() {
      modelUUID = uuid;
      selectedModeID = modeID;
      descController.text = desc;
      scoreController.text = score.toString();
      enemyScoreController.text = enemyScore.toString();
      selectedMapID = mapID;
      xpController.text = xp.toString();
      timeString = time;
    });
  }

  void updateModel() {
    int xp = int.tryParse(xpController.text) ?? 0;
    int score = int.tryParse(scoreController.text) ?? 0;
    int enemyScore = int.tryParse(enemyScoreController.text) ?? 0;

    Timestamp time = Timestamp.fromDate(DateTime.tryParse(timeString) ??
        DateTime.fromMillisecondsSinceEpoch(0));

    if (modelUUID != widget.model.uuid)
      setState(() {
        widget.model.uuid = modelUUID;
      });
    if (selectedMapID != widget.model.map)
      setState(() {
        widget.model.map = selectedMapID;
      });
    if (selectedModeID != widget.model.mode)
      setState(() {
        widget.model.mode = selectedModeID;
      });
    if (xp != widget.model.xp)
      setState(() {
        widget.model.xp = xp;
      });
    if (score != widget.model.score)
      setState(() {
        widget.model.score = score;
      });
    if (enemyScore != widget.model.enemyScore)
      setState(() {
        widget.model.enemyScore = enemyScore;
      });
    if (time != widget.model.time)
      setState(() {
        widget.model.time = time;
      });
    if (surrender != widget.model.surrender)
      setState(() {
        widget.model.surrender = surrender;
      });

    if (selectedModeID == "custom") {
      if (descController.text != widget.model.desc)
        setState(() {
          widget.model.desc = descController.text;
        });
      if (widget.model.score != 0)
        setState(() {
          widget.model.score = 0;
        });
      if (widget.model.enemyScore != 0)
        setState(() {
          widget.model.enemyScore = 0;
        });
    } else {
      if (widget.model.desc != "")
        setState(() {
          widget.model.desc = "";
        });
    }

    if (!DataService.modes[selectedModeID]!.canSurrender &&
        widget.model.surrender != "none")
      setState(() {
        widget.model.surrender = "none";
      });
  }

  List<DropdownMenuItem> createModeMenuItems() {
    List<DropdownMenuItem> items = [];
    for (String modeID in DataService.modes.keys) {
      items.add(DropdownMenuItem(
        value: modeID,
        child: Text(DataService.modes[modeID]!.name),
      ));
    }
    return items;
  }

  List<DropdownMenuItem> createMapMenuItems() {
    List<DropdownMenuItem> items = [];
    for (String mapID in DataService.maps.keys) {
      items.add(DropdownMenuItem(
        value: mapID,
        child: Text(DataService.maps[mapID]!.name),
      ));
    }
    return items;
  }

  Widget createSurrenderItems() {
    return Wrap(
      spacing: 8,
      children: [
        ChoiceChip(
          label: const Text("None"),
          selected: surrender == "none" ? true : false,
          onSelected: (bool selected) {
            setState(() {
              surrender = "none";
              updateModel();
            });
          },
        ),
        ChoiceChip(
          label: const Text("You"),
          selected: surrender == "you" ? true : false,
          onSelected: (bool selected) {
            setState(() {
              surrender = "you";
              updateModel();
            });
          },
        ),
        ChoiceChip(
          label: const Text("Enemy"),
          selected: surrender == "enemy" ? true : false,
          onSelected: (bool selected) {
            setState(() {
              surrender = "enemy";
              updateModel();
            });
          },
        ),
      ],
    );
  }

  Widget showScoreWithCondition() {
    List<Widget> children = [];
    String scoreFormat = DataService.modes[selectedModeID]!.scoreFormat;
    int scoreLimit = DataService.modes[selectedModeID]!.scoreLimit;

    if (scoreFormat == "none" || scoreFormat == "")
      return const SizedBox.shrink();
    if (scoreFormat == "placement" || scoreFormat == "default") {
      children.add(Expanded(
        child: TextFormField(
          // Score
          controller: scoreController,
          inputFormatters: [
            FilteringTextInputFormatter.digitsOnly,
            LengthLimitingTextInputFormatter(2),
          ],
          validator: (value) {
            if (value == null || value.isEmpty || int.tryParse(value) == null)
              return "";
            if (scoreLimit == -1) return null;
            if (int.parse(value) > scoreLimit) return "";
            return null;
          },
          decoration: InputDecoration(
            border: const OutlineInputBorder(),
            errorStyle: const TextStyle(height: 0.01),
            suffixText: (scoreFormat == "placement" &&
                    int.tryParse(scoreController.text) != null)
                ? widget.model
                    .getPlacementSuffix(int.parse(scoreController.text))
                : "",
          ),
          keyboardType: TextInputType.number,
        ),
      ));
    }
    if (scoreFormat == "default") {
      children.add(const Padding(
        padding: EdgeInsets.fromLTRB(8, 0, 8, 0),
        child: Text("-"),
      ));
      children.add(Expanded(
        child: TextFormField(
          // EnemyScore
          controller: enemyScoreController,
          inputFormatters: [
            FilteringTextInputFormatter.digitsOnly,
            LengthLimitingTextInputFormatter(2),
          ],
          validator: (value) {
            if (value == null || value.isEmpty || int.tryParse(value) == null)
              return "";
            if (scoreLimit == -1) return null;
            if (int.parse(value) > scoreLimit) return "";
            return null;
          },
          decoration: const InputDecoration(
            border: OutlineInputBorder(),
            errorStyle: const TextStyle(height: 0.01),
          ),
          keyboardType: TextInputType.number,
        ),
      ));
    }

    if (children.isEmpty) return const SizedBox.shrink();
    return Expanded(
      flex: 1,
      child: Padding(
        padding: const EdgeInsets.fromLTRB(8, 0, 0, 0),
        child: Column(
          mainAxisSize: MainAxisSize.max,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(scoreFormat == "placement" ? "Placement" : "Score"),
            Row(
              mainAxisSize: MainAxisSize.max,
              children: [
                ...children,
              ],
            ),
          ],
        ),
      ),
    );
  }

  List<Widget> showSurrenderWithCondition() {
    List<Widget> widgets = [];

    if (DataService.modes[selectedModeID]!.canSurrender) {
      widgets.add(const Padding(
        padding: EdgeInsets.fromLTRB(0, 8, 0, 0),
        child: Text("Surrender"),
      ));
      widgets.add(createSurrenderItems());
    }

    return widgets;
  }

  List<Widget> showDescriptionWithCondition() {
    List<Widget> widgets = [];

    if (selectedModeID == "custom") {
      widgets.add(const Padding(
        padding: EdgeInsets.fromLTRB(0, 8, 0, 0),
        child: Text("Description"),
      ));
      widgets.add(TextFormField(
        controller: descController,
        inputFormatters: [
          LengthLimitingTextInputFormatter(22),
        ],
        validator: (value) {
          if (value == null || value.isEmpty) return "";
          return null;
        },
        decoration: const InputDecoration(
          border: OutlineInputBorder(),
          errorStyle: TextStyle(height: 0.01),
        ),
        keyboardType: TextInputType.text,
      ));
    }

    return widgets;
  }

  @override
  Widget build(BuildContext context) {
    return Form(
      autovalidateMode: AutovalidateMode.onUserInteraction,
      key: widget.formKey,
      child: Column(
        mainAxisSize: MainAxisSize.max,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Row(
            mainAxisSize: MainAxisSize.max,
            children: [
              Expanded(
                flex: 2,
                child: Column(
                  mainAxisSize: MainAxisSize.max,
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const Text('Mode'),
                    DropdownButtonFormField(
                      decoration: const InputDecoration(
                        border: OutlineInputBorder(),
                      ),
                      onChanged: (dynamic newValue) {
                        setState(() {
                          selectedModeID = newValue as String;
                          updateModel();
                        });
                      },
                      items: createModeMenuItems(),
                      value: selectedModeID,
                    ),
                  ],
                ),
              ),
              showScoreWithCondition(),
            ],
          ),
          ...showSurrenderWithCondition(),
          ...showDescriptionWithCondition(),
          Row(
            children: [
              Expanded(
                flex: 2,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(0, 8, 0, 0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      const Text('Map'),
                      DropdownButtonFormField(
                        decoration: const InputDecoration(
                          border: OutlineInputBorder(),
                        ),
                        onChanged: (dynamic newValue) {
                          setState(() {
                            selectedMapID = newValue as String;
                            updateModel();
                          });
                        },
                        items: createMapMenuItems(),
                        value: selectedMapID,
                      ),
                    ],
                  ),
                ),
              ),
              Expanded(
                flex: 1,
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(8, 8, 0, 0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      const Text("Amount"),
                      TextFormField(
                        // XP
                        controller: xpController,
                        inputFormatters: [
                          FilteringTextInputFormatter.digitsOnly,
                          LengthLimitingTextInputFormatter(9),
                        ],
                        validator: (value) {
                          if (value == null ||
                              value.isEmpty ||
                              int.tryParse(value) == null) return "";
                          return null;
                        },
                        decoration: const InputDecoration(
                          border: OutlineInputBorder(),
                          errorStyle: TextStyle(height: 0.01),
                          suffixText: "XP",
                        ),
                        keyboardType: TextInputType.number,
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
          Row(
            mainAxisSize: MainAxisSize.max,
            children: [
              Flexible(
                child: Padding(
                  padding: const EdgeInsets.fromLTRB(0, 8, 0, 0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      const Text("Time"),
                      Row(
                        children: [
                          Expanded(
                            flex: 2,
                            child: TextFormField(
                              readOnly: true,
                              enabled: false,
                              controller: timeDisplayController,
                              decoration: const InputDecoration(
                                border: OutlineInputBorder(),
                              ),
                            ),
                          ),
                          Expanded(
                            flex: 1,
                            child: Padding(
                              padding: const EdgeInsets.fromLTRB(8, 0, 0, 0),
                              child: ElevatedButton.icon(
                                icon: const Icon(Icons.edit_calendar),
                                label: const Text("Change"),
                                onPressed: () async {
                                  DateTime? pickedDate = await showDatePicker(
                                    context: context,
                                    //locale: const Locale('en', 'DE'),
                                    initialDate: DateTime.now(),
                                    firstDate: DataService
                                        .seasonMetas.values.first.startDate
                                        .toDate(),
                                    lastDate: DataService
                                        .seasonMetas.values.first.endDate
                                        .toDate()
                                        .subtract(const Duration(days: 1)),
                                  );

                                  if (pickedDate == null) return;

                                  TimeOfDay? pickedTime = await showTimePicker(
                                      context: context,
                                      //locale: const Locale('en', 'DE'),
                                      initialTime: TimeOfDay.now());

                                  if (pickedTime == null) return;

                                  DateTime picked = DateTime(
                                      pickedDate.year,
                                      pickedDate.month,
                                      pickedDate.day,
                                      pickedTime.hour,
                                      pickedTime.minute);

                                  setState(() {
                                    timeString = picked.toString();
                                    setTimeDisplayText();
                                    updateModel();
                                  });
                                },
                              ),
                            ),
                          )
                        ],
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
          const Padding(
            padding: EdgeInsets.fromLTRB(0, 8, 0, 0),
            child: Text("Preview"),
          ),
          HistoryEntryWidget(model: widget.model, showOptions: false),
        ],
      ),
    );
  }
}
