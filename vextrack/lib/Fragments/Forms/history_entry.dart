import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:uuid/uuid.dart';
import 'package:vextrack/Components/history_entry.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Services/data.dart';

class HistoryEntryForm extends StatefulWidget
{
  const HistoryEntryForm({
    super.key,
  });

  @override
  State<HistoryEntryForm> createState() => _HistoryEntryFormState();
}

class _HistoryEntryFormState extends State<HistoryEntryForm>
{
  String selectedModeID = DataService.modes.keys.last;
  String selectedMapID = DataService.maps.keys.first;
  String surrender = "none";
  String timeString = DateTime.now().toString();

  TextEditingController scoreController = TextEditingController();
  TextEditingController enemyScoreController = TextEditingController();
  TextEditingController xpController = TextEditingController();
  TextEditingController descController = TextEditingController();

  HistoryEntry model = HistoryEntry(
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

  @override
  void initState()
  {
    super.initState();
    updateModel();

    scoreController.addListener(updateModel);
    enemyScoreController.addListener(updateModel);
    xpController.addListener(updateModel);
    descController.addListener(updateModel);
  }

  void updateModel()
  {
    int xp = int.tryParse(xpController.text) ?? 0;
    int score = int.tryParse(scoreController.text) ?? 0;
    int enemyScore = int.tryParse(enemyScoreController.text) ?? 0;

    Timestamp time = Timestamp.fromDate(DateTime.tryParse(timeString) ?? DateTime.fromMillisecondsSinceEpoch(0));

    if (selectedModeID == "custom") {
      if(descController.text != model.desc) setState(() {model.desc = descController.text;});
      if(model.score != 0) setState(() {model.score = 0;});
      if(model.enemyScore != 0) setState(() {model.enemyScore = 0;});
    } else {
      if (model.desc != "") setState(() {model.desc = "";});
    }

    if (!DataService.modes[selectedModeID]!.canSurrender && model.surrender != "none") setState(() {model.surrender = "none";});
    
    if (selectedMapID != model.map) setState(() {model.map = selectedMapID;});
    if (selectedModeID != model.mode) setState(() {model.mode = selectedModeID;});
    if (xp != model.xp) setState(() {model.xp = xp;});
    if (score != model.score) setState(() {model.score = score;});
    if (enemyScore != model.enemyScore) setState(() {model.enemyScore = enemyScore;});
    if (time != model.time) setState(() {model.time = time;});
    if (surrender != model.surrender) setState(() {model.surrender = surrender;});
  }

  List<DropdownMenuItem> createModeMenuItems()
  {
    List<DropdownMenuItem> items = [];
    for(String modeID in DataService.modes.keys)
    {
      items.add(DropdownMenuItem(
        value: modeID,
        child: Text(DataService.modes[modeID]!.name),
      ));
    }
    return items;
  }

  List<DropdownMenuItem> createMapMenuItems()
  {
    List<DropdownMenuItem> items = [];
    for(String mapID in DataService.maps.keys)
    {
      items.add(DropdownMenuItem(
        value: mapID,
        child: Text(DataService.maps[mapID]!.name),
      ));
    }
    return items;
  }

  Widget createSurrenderItems()
  {
    return Wrap(
      spacing: 8,
      children: [
        ChoiceChip(
          label: Row(
            mainAxisSize: MainAxisSize.min,
            children: const [
              Icon(Icons.close_rounded),
              Text("None"),
            ],
          ),
          selected: surrender == "none" ? true : false,
          onSelected: (bool selected) {
            setState(() {
              surrender = "none";
              updateModel();
            });
          },
        ),
        ChoiceChip(
          label: Row(
            mainAxisSize: MainAxisSize.min,
            children: const [
              Icon(Icons.person_rounded),
              Text("You"),
            ],
          ),
          selected: surrender == "you" ? true : false,
          onSelected: (bool selected) {
            setState(() {
              surrender = "you";
              updateModel();
            });
          },
        ),
        ChoiceChip(
          label: Row(
            mainAxisSize: MainAxisSize.min,
            children: const [
              Icon(Icons.bolt_rounded),
              Text("Enemy"),
            ],
          ),
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

  Widget showScoreWithCondition()
  {
    List<Widget> children = [];
    String scoreFormat = DataService.modes[selectedModeID]!.scoreFormat;

    if (scoreFormat == "none" || scoreFormat == "") return const SizedBox.shrink();
    if (scoreFormat == "placement" || scoreFormat == "default")
    {
      children.add(Expanded(
        child: TextFormField( // Score
          controller: scoreController,
          inputFormatters: [
            FilteringTextInputFormatter.digitsOnly,
            LengthLimitingTextInputFormatter(3),
          ],
          keyboardType: TextInputType.number,
        ),
      )); 
    }
    if (scoreFormat == "default")
    {
      children.add(const Text("-"));
      children.add(Expanded(
        child: TextFormField( // EnemyScore
          controller: enemyScoreController,
          inputFormatters: [
            FilteringTextInputFormatter.digitsOnly,
            LengthLimitingTextInputFormatter(3),
          ],
          keyboardType: TextInputType.number,
        ),
      ));
    }

    if (children.isEmpty) return const SizedBox.shrink();
    return Expanded(
      flex: 1,
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
    );
  }

  List<Widget> showSurrenderWithCondition()
  {
    List<Widget> widgets = [];

    if(DataService.modes[selectedModeID]!.canSurrender)
    {
      widgets.add(const Text("Surrender"));
      widgets.add(createSurrenderItems());
    }

    return widgets;
  }

  List<Widget> showDescriptionWithCondition()
  {
    List<Widget> widgets = [];

    if(selectedModeID == "custom")
    {
      widgets.add(const Text("Description"));
      widgets.add(TextFormField(
        controller: descController,
        inputFormatters: [
          LengthLimitingTextInputFormatter(22),
        ],
        keyboardType: TextInputType.text,
      ));
    }
    
    return widgets;
  }

  @override
  Widget build(BuildContext context) {
    return Form(
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
                    DropdownButton(
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
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const Text('Map'),
                    DropdownButton(
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
              Expanded(
                flex: 1,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const Text("Amount"),
                    Row(
                      children: [
                        Expanded(
                          child: TextFormField( // XP
                            controller: xpController,
                            inputFormatters: [
                              FilteringTextInputFormatter.digitsOnly,
                              LengthLimitingTextInputFormatter(9),
                            ],
                            keyboardType: TextInputType.number,
                          ),
                        ),
                        const Text("XP"),
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),

          Row(
            mainAxisSize: MainAxisSize.max,
            children: [
              Flexible(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    const Text("Time"),
                    Row(
                      children: [
                        Expanded(
                          flex: 2,
                          child: Text(Formatter.formatDateTime(DateTime.parse(timeString))),
                        ),
                        
                        Expanded(
                          flex: 1,
                          child: ElevatedButton(
                            child: const Text("Change Time"),
                            onPressed: () async {
                              DateTime? pickedDate = await showDatePicker(
                                context: context,
                                //locale: const Locale('en', 'DE'),
                                initialDate: DateTime.now(),
                                firstDate: DateTime(2020),
                                lastDate: DateTime.now().add(const Duration(days: 365)),
                              );

                              if(pickedDate == null) return;
                        
                              TimeOfDay? pickedTime = await showTimePicker(
                                context: context,
                                //locale: const Locale('en', 'DE'),
                                initialTime: TimeOfDay.now()
                              );
                        
                              if (pickedTime == null) return;

                              DateTime picked = DateTime(pickedDate.year, pickedDate.month, pickedDate.day, pickedTime.hour, pickedTime.minute);
                        
                              setState(() {
                                timeString = picked.toString();
                                updateModel();
                              });
                            },
                          ),
                        )
                      ],
                    ),
                  ],
                ),
              ),
            ],
          ),

          const Text("Preview"),
          HistoryEntryWidget(model: model),
        ],
      ),
    );
  }
}