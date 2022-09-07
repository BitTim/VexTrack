import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/contract.dart';
import 'package:vextrack/Components/season.dart';
import 'package:vextrack/Models/Goals/contract.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Services/data.dart';

class ContractsFragment extends StatefulWidget
{
  final String uid;
  const ContractsFragment({Key? key, required this.uid}) : super(key: key);

  @override
  ContractsFragmentState createState() => ContractsFragmentState();
}

class ContractsFragmentState extends State<ContractsFragment>
{
  List<Contract> _contractsActive = [];
  List<Contract> _contractsPaused = [];
  List<Contract> _contractsCompleted = [];
  List<Season> _seasonsActive = [];
  List<Season> _seasonsInactive = [];
  bool _loading = false;

  showInactiveSeasons() {
    List<Widget> seasonList = [];
    
    for(Season s in _seasonsInactive)
    {
      seasonList.add(
        SeasonWidget(model: s)
      );
    }

    return seasonList;
  }

  showActiveSeasons() {
    List<Widget> seasonList = [];
    
    for(Season s in _seasonsActive)
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
    List<Season> seasons = await DataService.getAllSeasons(uid: widget.uid);
    if (mounted) {
      setState(() {
        _seasonsActive = seasons.where((element) => element.isActive()).toList();
        _seasonsInactive = seasons.where((element) => !element.isActive()).toList();
        _loading = false;
      });
    }
  }

  showActiveContracts() {
    List<Widget> contractList = [];

    for(Contract c in _contractsActive)
    {
      contractList.add(
        ContractWidget(model: c)
      );
    }

    return contractList;
  }
  
  showPausedContracts() {
    List<Widget> contractList = [];

    for(Contract c in _contractsPaused)
    {
      contractList.add(
        ContractWidget(model: c)
      );
    }

    return contractList;
  }

  showCompletedContracts() {
    List<Widget> contractList = [];

    for(Contract c in _contractsCompleted)
    {
      contractList.add(
        ContractWidget(model: c)
      );
    }

    return contractList;
  }

  setupContracts() async
  {
    setState(() => _loading = true);
    List<Contract> contracts = await DataService.getAllContracts(uid: widget.uid);
    if (mounted) {
      setState(() {
        _contractsActive = contracts.where((element) => !element.isCompleted() && !element.isPaused()).toList();
        _contractsPaused = contracts.where((element) => !element.isCompleted() && element.isPaused()).toList();
        _contractsCompleted = contracts.where((element) => element.isCompleted()).toList();
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
    setupSeasons();
    setupContracts();
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
      child: RefreshIndicator(
        onRefresh: () async {
          DataService.refresh();
          setupSeasons();
          setupContracts();
        },
        child: ListView(
          physics: const BouncingScrollPhysics(parent: AlwaysScrollableScrollPhysics()),
          children: [
            ExpandablePanel(
              header: Text(
                "Seasons",
                style: GoogleFonts.titilliumWeb(
                  fontSize: 24,
                  fontWeight: FontWeight.w700,
                ),
              ),
              collapsed: Column(
                children: _seasonsActive.isEmpty && _loading == false ? [
                  const Center(child: Text("No active seasons"))
                ] : showActiveSeasons()
              ),
              expanded: Column(
                children: [
                  Text(
                    "Active",
                    style: GoogleFonts.titilliumWeb(
                      fontSize: 18,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  Column(
                    children: _seasonsActive.isEmpty && _loading == false ? [
                      const Center(child: Text("No active seasons"))
                    ] : showActiveSeasons()
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(0, 8, 0, 0),
                    child: Text(
                      "Inactive",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 18,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  Column(
                    children: _seasonsInactive.isEmpty && _loading == false ? [
                      const Center(child: Text("No inactive seasons"))
                    ] : showInactiveSeasons()
                  ),
                ],
              ),
            ),
          
          
          
            ExpandablePanel(
              header: Text(
                "Contracts",
                style: GoogleFonts.titilliumWeb(
                  fontSize: 24,
                  fontWeight: FontWeight.w700,
                ),
              ),
              collapsed: Column(
                children: _contractsActive.isEmpty && _loading == false ? [
                  const Center(child: Text("No active contracts"))
                ] : showActiveContracts()
              ),
              expanded: Column(
                children: [
                  Text(
                    "Active",
                    style: GoogleFonts.titilliumWeb(
                      fontSize: 18,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                  Column(
                    children: _contractsActive.isEmpty && _loading == false ? [
                      const Center(child: Text("No active contracts"))
                    ] : showActiveContracts()
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(0, 8, 0, 0),
                    child: Text(
                      "Paused",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 18,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  Column(
                    children: _contractsPaused.isEmpty && _loading == false ? [
                      const Center(child: Text("No paused contracts"))
                    ] : showPausedContracts()
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(0, 8, 0, 0),
                    child: Text(
                      "Completed",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 18,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ),
                  Column(
                    children: _contractsPaused.isEmpty && _loading == false ? [
                      const Center(child: Text("No completed contracts"))
                    ] : showCompletedContracts()
                  ),
                ],
              ),
            ),
          ],
        ),
      )
    );
  }
}