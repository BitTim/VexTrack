import 'package:flutter/material.dart';
import 'package:vextrack/Components/history_entry_group.dart';
import 'package:vextrack/Fragments/Forms/History/history_filter.dart';
import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';
import 'package:vextrack/Services/data.dart';

class HistoryFragment extends StatefulWidget {
  final String uid;
  const HistoryFragment({Key? key, required this.uid}) : super(key: key);

  @override
  HistoryFragmentState createState() => HistoryFragmentState();
}

class HistoryFragmentState extends State<HistoryFragment>
{
  List<HistoryEntryGroup> _history = [];
  List<HistoryEntryGroup> _filteredHistory = [];
  bool _loading = false;

  List<String> seasonIDFilter = [];
  List<String> gameModeFilter = [];
  List<String> mapFilter = [];

  showHistory() {
    List<Widget> historyEntryGroupList = [];
    
    for(HistoryEntryGroup heg in _filteredHistory)
    {
      historyEntryGroupList.add(
        HistoryEntryGroupWidget(model: heg)
      );
    }

    return historyEntryGroupList;
  }

  setupHistory() async
  {
    setState(() => _loading = true);
    List<HistoryEntryGroup> history = await DataService.getFullHistory(widget.uid);

    if (mounted) {
      setState(() {
        _history = history;
        _loading = false;
      });
    }
  }

  filterHistory()
  {
    List<HistoryEntryGroup> history = [];
    for(String seasonID in DataService.seasonMetas.keys)
    {
      SeasonMeta meta = DataService.seasonMetas[seasonID]!;
      for(HistoryEntryGroup heg in _history)
      {
        if((seasonIDFilter.contains(seasonID) && heg.date.compareTo(meta.startDate) >= 0 && heg.date.compareTo(meta.endDate) < 0) || seasonIDFilter.isEmpty)
        {
          history.add(HistoryEntryGroup(heg.id, heg.day, heg.total, heg.date, heg.entries));
        }
      }
    }

    for(int i = 0; i < history.length; i++)
    {
      if(gameModeFilter.isNotEmpty)
      {
        List<HistoryEntry> entries = [];

        for(HistoryEntry he in history[i].entries)
        {
          if(gameModeFilter.contains(he.mode)) entries.add(he);
        }

        if(entries.isEmpty) {
          history.removeAt(i--);
          continue;
        } else {
          history[i].entries = entries;
        }
      }

      if(mapFilter.isNotEmpty)
      {
        List<HistoryEntry> entries = [];

        for(HistoryEntry he in history[i].entries)
        {
          if(mapFilter.contains(he.map)) entries.add(he);
        }

        if(entries.isEmpty) {
          history.removeAt(i--);
          continue;
        } else {
          history[i].entries = entries;
        }
      }
    }

    setState(() {
      _filteredHistory = history;
    });
  }

  @override
  void initState() {
    super.initState();
    update();
  }

  void update() async
  {
    await setupHistory();
    filterHistory();
  }

  Widget createFilterDialog()
  {
    HistoryFilterForm form = HistoryFilterForm(seasonIDFilter: seasonIDFilter, gameModeFilter: gameModeFilter, mapFilter: mapFilter);

    return AlertDialog(
      scrollable: true,
      title: const Text('Filter history'),
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.all(
          Radius.circular(16),
        ),
      ),
      content: Builder(
        builder: (context) {
          var width = MediaQuery.of(context).size.width;

          return SizedBox(
            width: width - 128, //TODO: Responsive layout (Fullscrenn diag on phones, restrained width on Desktop)
            child: Padding(
              padding: const EdgeInsets.all(8.0),
              child: form,
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
            seasonIDFilter = form.seasonIDFilter; 
            gameModeFilter = form.gameModeFilter;
            mapFilter = form.mapFilter;
            filterHistory();
          },
          child: const Text("Apply"),
        )
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
      child: RefreshIndicator(
        onRefresh: () async {
          DataService.refresh();
          update();
        },
        child: ListView(
          physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
          children: [
            Column(
              children: _history.isEmpty && _loading == false ? [
                const Center(child: Text("No history"))
              ] : showHistory()
            ),
            const SizedBox(
              height: 88,
            ),
          ]
        ),
      ),
    );
  }
}