import 'package:flutter/material.dart';
import 'package:vextrack/Components/history_entry.dart';
import 'package:vextrack/Models/History/history_entry.dart';
import 'package:vextrack/Models/History/history_entry_group.dart';

class HistoryEntryGroupWidget extends StatefulWidget
{
  final HistoryEntryGroup model;
  final Function(HistoryEntry)? editHistoryEntry;
  final Function(HistoryEntry)? deleteHistoryEntry;
  
  const HistoryEntryGroupWidget({
    Key? key,
    required this.model,
    required this.editHistoryEntry,
    required this.deleteHistoryEntry,
  }) : super(key: key);
  
  @override
  HistoryEntryGroupWidgetState createState() => HistoryEntryGroupWidgetState();
}

class HistoryEntryGroupWidgetState extends State<HistoryEntryGroupWidget>
{
  @override
  Widget build(BuildContext context)
  {
    return Padding(
      padding: const EdgeInsets.fromLTRB(0, 0, 0, 8),
      child: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(0, 8, 0, 8),
            child: Text(
              widget.model.getFormattedDate(),
              style: TextStyle(
                color: Theme.of(context).colorScheme.onSurfaceVariant,
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),

          ...widget.model.entries.map((entry) => HistoryEntryWidget(
            model: entry,
            editHistoryEntry: widget.editHistoryEntry,
            deleteHistoryEntry: widget.deleteHistoryEntry,
          )),
        ],
      ),
    );
  }
}