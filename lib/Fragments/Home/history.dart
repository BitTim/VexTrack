import 'package:flutter/material.dart';
import 'package:vextrack/Components/history_entry.dart';
import 'package:vextrack/Models/history_entry.dart';
import 'package:vextrack/Services/data.dart';

class HistoryFragment extends StatefulWidget {
  final String uid;
  const HistoryFragment({Key? key, required this.uid}) : super(key: key);

  @override
  // ignore: library_private_types_in_public_api
  _HistoryFragmentState createState() => _HistoryFragmentState();
}

class _HistoryFragmentState extends State<HistoryFragment>
{
  List _history = [];
  bool _loading = false;

  showHistory() {
    List<Widget> historyEntryList = [];
    
    for(HistoryEntry he in _history)
    {
      historyEntryList.add(
        HistoryEntryWidget(model: he)
      );
    }

    return historyEntryList;
  }

  setupHistory() async
  {
    setState(() => _loading = true);
    List history = await DataService.getFullHistory(widget.uid);
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
    setupHistory();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.all(8.0),
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