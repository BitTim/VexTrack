import 'package:flutter/material.dart';
import 'package:vextrack/Components/season.dart';
import 'package:vextrack/Models/season.dart';
import 'package:vextrack/Services/data.dart';

class SeasonsFragment extends StatefulWidget {
  final String uid;
  const SeasonsFragment({Key? key, required this.uid}) : super(key: key);

  @override
  SeasonsFragmentState createState() => SeasonsFragmentState();
}

class SeasonsFragmentState extends State<SeasonsFragment> {
  List<Season> _seasons = [];
  bool _loading = false;

  showSeasons() {
    List<Widget> seasonList = [];
    
    for(Season s in _seasons)
    {
      seasonList.add(
        SeasonWidget(model: s)
      );
    }

    return seasonList;
  }

  setupSeasons() async
  {
    setState(() => _loading = true);
    List<Season> seasons = await DataService.getAllSeasons(widget.uid);
    if (mounted) {
      setState(() {
        _seasons = seasons;
        _loading = false;
      });
    }
  }

  @override
  void initState() {
    super.initState();
    setupSeasons();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
      child: RefreshIndicator(
        onRefresh: () => setupSeasons(),
        child: ListView(
          physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
          children: _seasons.isEmpty && _loading == false ? [
            const Center(child: Text("No seasons"))
          ] : showSeasons()
        ),
      ),
    );
  }
}