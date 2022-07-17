import 'package:flutter/material.dart';
import 'package:vextrack/Components/progression.dart';
import 'package:vextrack/Models/Goals/progression.dart';
import 'package:vextrack/Services/data.dart';

class GoalsFragment extends StatefulWidget
{
  final String uid;
  const GoalsFragment({Key? key, required this.uid}) : super(key: key);

  @override
  GoalsFragmentState createState() => GoalsFragmentState();
}

class GoalsFragmentState extends State<GoalsFragment>
{
  List<Progression> _progressions = [];
  bool _loading = false;

  showProgressions() {
    List<Widget> progressionList = [];

    for(Progression p in _progressions)
    {
      progressionList.add(
        ProgressionWidget(model: p)
      );
    }

    return progressionList;
  }

  setupProgressions() async
  {
    setState(() => _loading = true);
    List<Progression> progressions = await DataService.getAllProgressions(widget.uid);
    if (mounted) {
      setState(() {
        _progressions = progressions;
        _loading = false;
      });
    }
  }

  @override
  void initState() {
    super.initState();
    setupProgressions();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
      child: ListView(
        physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
        children: _progressions.isEmpty && _loading == false ? [
          const Center(child: Text("No Goals"))
        ] : showProgressions()
      )
    );
  }
}