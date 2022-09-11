import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/history_entry_group.dart';
import 'package:vextrack/Fragments/Forms/History/history_filter.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';
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
  bool _loading = false;

  List<String> seasonIDFilter = [];
  List<String> gameModeFilter = [];
  List<String> mapFilter = [];

  showHistory() {
    List<Widget> historyEntryGroupList = [];
    
    for(HistoryEntryGroup heg in _history)
    {
      // TODO: Implement filtering of individual historx entries aka create new HEG or smth

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

  @override
  void initState() {
    super.initState();
    update();
  }

  void update()
  {
    setupHistory();
  }

  Widget createFilterDialog()
  {
    HistoryFilterForm form = HistoryFilterForm();

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
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  "History",
                  style: GoogleFonts.titilliumWeb(
                    fontSize: 24,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.filter_list),
                  onPressed: () {
                    showDialog(
                      context: context,
                      builder: (BuildContext context) {
                        return createFilterDialog();
                      },
                    );
                  },
                ),
              ],
            ),
            Column(
              children: _history.isEmpty && _loading == false ? [
                const Center(child: Text("No history"))
              ] : showHistory()
            ),
          ]
        ),
      ),
    );
  }
}