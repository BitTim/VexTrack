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

  void editHistoryEntry(HistoryEntry he)
  {

  }

  void deleteHistoryEntry(HistoryEntry he)
  {
    showDialog(
      context: context,
      builder: (context) {
        return AlertDialog(
          icon: const Icon(Icons.delete),
          title: const Text("Delete history entry"),
          content: const Text("This action will permanently delete the selected entry"),
          actions: [
            TextButton(
              child: const Text("Cancel"),
              onPressed: () => Navigator.of(context).pop(),
            ),
            ElevatedButton(
              child: const Text("Delete"),
              onPressed: () async {
                Navigator.of(context).pop();

                SnackBar sb = SnackBar(content: Text("Deleted Entry ${he.getFormattedDesc()}"));
                ScaffoldMessenger.of(context).showSnackBar(sb);

                await DataService.deleteHistoryEntry(widget.uid, he);
                update();
              },
            ),
          ],
        );
      }
    );
  }

  showHistory() {
    List<Widget> historyEntryGroupList = [];
    
    for(HistoryEntryGroup heg in _filteredHistory)
    {
      historyEntryGroupList.add(
        HistoryEntryGroupWidget(model: heg, editHistoryEntry: editHistoryEntry, deleteHistoryEntry: deleteHistoryEntry,)
      );
    }

    return historyEntryGroupList;
  }

  setupHistory() async
  {
    setState(() => _loading = true);
    List<HistoryEntryGroup> history = await DataService.pullFullHistory(widget.uid);

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
    for(HistoryEntryGroup heg in _history)
    {

      if(seasonIDFilter.isEmpty) 
      {
        history.add(HistoryEntryGroup(heg.id, heg.day, heg.total, heg.date, heg.entries));
        continue;
      }

      for(String seasonID in DataService.seasonMetas.keys)
      {
        SeasonMeta meta = DataService.seasonMetas[seasonID]!;
        
        if(seasonIDFilter.contains(seasonID) && heg.date.compareTo(meta.startDate) >= 0 && heg.date.compareTo(meta.endDate) < 0)
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