import 'package:flutter/material.dart';
import 'package:vextrack/Components/history_entry_group.dart';
import 'package:vextrack/Core/history_calc.dart';
import 'package:vextrack/Models/History/history_entry.dart';
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

  showHistory() {
    List<Widget> historyEntryGroupList = [];
    
    for(HistoryEntryGroup heg in _history)
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
    List<HistoryEntry> history = await DataService.getFullHistory(widget.uid);
    if (mounted) {
      setState(() {
        _history = HistoryCalc.groupHistoryByDate(history);
        _loading = false;
      });
    }
  }

  @override
  void initState() {
    super.initState();
    setupHistory();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
      child: RefreshIndicator(
        onRefresh: () => setupHistory(),
        child: ListView(
          physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
          children: _history.isEmpty && _loading == false ? [
            const Center(child: Text("No history"))
          ] : showHistory()
        ),
      ),
    );
  }
}