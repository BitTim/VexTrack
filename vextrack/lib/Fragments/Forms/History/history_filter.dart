import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Services/data.dart';

class HistoryFilterForm extends StatefulWidget
{
  final List<String> seasonIDFilter = [];
  final List<String> gameModeFilter = [];
  final List<String> mapFilter= [];

  HistoryFilterForm({super.key});

  @override
  HistoryFilterFormState createState() => HistoryFilterFormState();
}

class HistoryFilterFormState extends State<HistoryFilterForm>
{
  List<Widget> createSeasonChips()
  {
    List<Widget> chips = [];

    for(String seasonID in DataService.seasons.keys)
    {
      chips.add(FilterChip(
        label: Text(DataService.seasonMetas[seasonID]!.name),
        selected: widget.seasonIDFilter.contains(seasonID),
        onSelected: (bool value) {
          setState(() {
            int idx = widget.seasonIDFilter.indexOf(seasonID);
            if (idx == -1 && value) widget.seasonIDFilter.add(seasonID);
            if (idx > -1 && !value) widget.seasonIDFilter.removeAt(idx);
          });
        },
      ));
    }

    return chips;
  }

  List<Widget> createGameModeChips()
  {
    List<Widget> chips = [];

    for(String mode in DataService.modes.keys)
    {
      chips.add(FilterChip(
        label: Text(DataService.modes[mode]!.name),
        selected: widget.gameModeFilter.contains(mode),
        onSelected: (bool value) {
          setState(() {
            int idx = widget.gameModeFilter.indexOf(mode);
            if (idx == -1 && value) widget.gameModeFilter.add(mode);
            if (idx > -1 && !value) widget.gameModeFilter.removeAt(idx);
          });
        },
      ));
    }

    return chips;
  }

  List<Widget> createMapChips()
  {
    List<Widget> chips = [];

    for(String map in DataService.maps.keys)
    {
      chips.add(FilterChip(
        label: Text(DataService.maps[map]!.name),
        selected: widget.mapFilter.contains(map),
        onSelected: (bool value) {
          setState(() {
            int idx = widget.mapFilter.indexOf(map);
            if (idx == -1 && value) widget.mapFilter.add(map);
            if (idx > -1 && !value) widget.mapFilter.removeAt(idx);
          });
        },
      ));
    }

    return chips;
  }

  @override
  Widget build(BuildContext context) {
    return Form(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(0, 0, 0, 4),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  "Season",
                  style: GoogleFonts.titilliumWeb(
                    fontSize: 18,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.cancel),
                  onPressed: () {
                    setState(() {
                      widget.seasonIDFilter.clear();
                    });
                  },
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
            child: Wrap(
              spacing: 8,
              runSpacing: 4,
              children: [
                ...createSeasonChips(),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(0, 0, 0, 4),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  "Mode",
                  style: GoogleFonts.titilliumWeb(
                    fontSize: 18,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.cancel),
                  onPressed: () {
                    setState(() {
                      widget.gameModeFilter.clear();
                    });
                  },
                ),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
            child: Wrap(
              spacing: 8,
              runSpacing: 4,
              children: [
                ...createGameModeChips(),
              ],
            ),
          ),
          Padding(
            padding: const EdgeInsets.fromLTRB(0, 0, 0, 4),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  "Map",
                  style: GoogleFonts.titilliumWeb(
                    fontSize: 18,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.cancel),
                  onPressed: () {
                    setState(() {
                      widget.mapFilter.clear();
                    });
                  },
                ),
              ],
            ),
          ),
          Wrap(
            spacing: 8,
            runSpacing: 4,
            children: [
              ...createMapChips(),
            ],
          ),
        ],
      ),
    );
  }
}