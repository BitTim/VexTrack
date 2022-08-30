import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:flutter/material.dart';
import 'package:vextrack/Components/history_entry.dart';
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
  TextEditingController scoreController = TextEditingController();
  TextEditingController enemyScoreController = TextEditingController();
  String surrender = "none";
  TextEditingController timeController = TextEditingController();
  TextEditingController xpController = TextEditingController();
  TextEditingController descController = TextEditingController();

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
          keyboardType: TextInputType.number,
          onChanged: (value) {
            setState(() {
              scoreController.text = value;
            });
          },
        ),
      )); 
    }
    if (scoreFormat == "default")
    {
      children.add(const Text("-"));
      children.add(Expanded(
        child: TextFormField( // EnemyScore
          keyboardType: TextInputType.number,
          onChanged: (value) {
            setState(() {
              enemyScoreController.text = value;
            });
          },
        ),
      ));
    }

    if (children.isEmpty) return const SizedBox.shrink();
    return Expanded(
      child: Column(
        mainAxisSize: MainAxisSize.max,
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
        keyboardType: TextInputType.text,
        onChanged: (value) {
          setState(() {
            descController.text = value;
          });
        },
      ));
    }
    
    return widgets;
  }

  HistoryEntry createModel()
  {
    int xp = int.tryParse(xpController.text) ?? 0;
    int score = int.tryParse(scoreController.text) ?? 0;
    int enemyScore = int.tryParse(enemyScoreController.text) ?? 0;

    Timestamp time = Timestamp.fromDate(DateTime.tryParse(timeController.text) ?? DateTime.fromMillisecondsSinceEpoch(0));

    return HistoryEntry(
      (selectedModeID == "custom") ? descController.text : "",
      selectedMapID,
      selectedModeID,
      xp,
      score,
      enemyScore,
      time,
      surrender,
    );
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
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Column(
                mainAxisSize: MainAxisSize.max,
                children: [
                  const Text('Mode'),
                  DropdownButton(
                    onChanged: (dynamic newValue) {
                      setState(() {
                        selectedModeID = newValue as String;
                      });
                    },
                    items: createModeMenuItems(),
                    value: selectedModeID,
                  ),
                ],
              ),
          
              showScoreWithCondition(),
            ],
          ),

          ...showSurrenderWithCondition(),
          ...showDescriptionWithCondition(),

          const Text('Map'),
          DropdownButton(
            onChanged: (dynamic newValue) {
              setState(() {
                selectedMapID = newValue as String;
              });
            },
            items: createMapMenuItems(),
            value: selectedMapID,
          ),

          Row(
            mainAxisSize: MainAxisSize.max,
            children: [
              Flexible(
                child: Column(
                  children: [
                    const Text("Time"),
                    TextFormField( // Time
                      controller: timeController,
                      keyboardType: TextInputType.datetime,
                      onChanged: (value) {
                        setState(() {
                          timeController.text = value;
                        });
                      },
                    ),
                  ],
                ),
              ),
              Flexible(
                child: Column(
                  children: [
                    const Text("XP"),
                    TextFormField( // XP
                      controller: xpController,
                      keyboardType: TextInputType.number,
                      onChanged: (value) {
                        setState(() {
                          xpController.text = value;
                        });
                      },
                    ),
                  ],
                ),
              ),
            ],
          ),

          const Text("Preview"),
          HistoryEntryWidget(model: createModel()),
        ],
      ),
    );
  }
}