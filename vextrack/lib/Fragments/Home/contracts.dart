import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/contract.dart';
import 'package:vextrack/Components/season.dart';
import 'package:vextrack/Models/Contracts/contract.dart';
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

  final List<bool> selectedSeasonType = <bool>[true, false]; 
  final List<bool> selectedContractType = <bool>[true, false, false]; 

  showInactiveSeasons() {
    List<Widget> seasonList = [];
    
    for(Season s in _seasonsInactive)
    {
      seasonList.add(
        SeasonWidget(
          model: s,
          controller: ExpandableController(),
        )
      );
    }

    return seasonList;
  }

  showActiveSeasons() {
    List<Widget> seasonList = [];
    
    for(Season s in _seasonsActive)
    {
      seasonList.add(
        SeasonWidget(
          model: s,
          controller: ExpandableController(),
        )
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

  sortContracts()
  {
    setState(() {
      _contractsActive.sort((a, b) {
        return a.name.toLowerCase().compareTo(b.name.toLowerCase());
      },);
      _contractsPaused.sort((a, b) {
        return a.name.toLowerCase().compareTo(b.name.toLowerCase());
      },);
      _contractsCompleted.sort((a, b) {
        return a.name.toLowerCase().compareTo(b.name.toLowerCase());
      },);
    });
  }

  onContractPaused(Contract contract)
  {
    setState(() {
      if(contract.paused)
      {
        _contractsActive.remove(contract);
        _contractsPaused.add(contract);
      }
      else
      {
        _contractsActive.add(contract);
        _contractsPaused.remove(contract);
      }
      
      sortContracts();
      DataService.updateContract(widget.uid, contract);
    });
  }

  showActiveContracts() {
    List<Widget> contractList = [];

    for(Contract c in _contractsActive)
    {
      contractList.add(
        ContractWidget(
          model: c,
          showPause: true,
          controller: ExpandableController(initialExpanded: false),
          notifyPaused: onContractPaused,
        )
      );
    }

    return contractList;
  }
  
  showPausedContracts() {
    List<Widget> contractList = [];

    for(Contract c in _contractsPaused)
    {
      contractList.add(
        ContractWidget(
          model: c,
          showPause: true,
          controller: ExpandableController(initialExpanded: false),
          notifyPaused: onContractPaused,
        )
      );
    }

    return contractList;
  }

  showCompletedContracts() {
    List<Widget> contractList = [];

    for(Contract c in _contractsCompleted)
    {
      contractList.add(
        ContractWidget(
          model: c,
          showPause: false,
          controller: ExpandableController(initialExpanded: false),
          notifyPaused: onContractPaused,
        )
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
        _contractsCompleted = contracts.where((element) => element.isCompleted()).toList();
        _contractsPaused = contracts.where((element) => element.isPaused()).toList();
        _contractsActive = contracts.where((element) => !element.isCompleted() && !element.isPaused()).toList();
        _loading = false;
      });
    }
  }

  @override
  void initState() {
    super.initState();
    update();
  }

  void update() async
  {
    setupSeasons();
    await setupContracts();
    sortContracts();
  }

  @override
  Widget build(BuildContext context) {
    ThemeData theme = Theme.of(context);

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
            Padding(
              padding: const EdgeInsets.fromLTRB(0, 8, 0, 0),
              child: Column(
                children: [
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        "Seasons",
                        style: GoogleFonts.titilliumWeb(
                          fontSize: 24,
                          fontWeight: FontWeight.w700,
                        ),
                      ),

                      ToggleButtons(
                        onPressed: (int index) {
                          setState(() {
                            for (int i = 0; i < selectedSeasonType.length; i++) {
                              selectedSeasonType[i] = i == index;
                            }
                          });
                        },
                        borderRadius: const BorderRadius.all(Radius.circular(8)),
                        selectedBorderColor: theme.colorScheme.outline,
                        selectedColor: theme.colorScheme.onPrimaryContainer,
                        fillColor: theme.colorScheme.primaryContainer,
                        color: theme.colorScheme.onSurface,
                        constraints: const BoxConstraints(
                          minHeight: 40.0,
                          minWidth: 80.0,
                        ),
                        isSelected: selectedSeasonType,
                        children: const [
                          Text("Active"),
                          Text("Inactive"),
                        ],
                      ),
                    ],
                  ),

                  if(selectedSeasonType[0]) Column(
                    children: _seasonsActive.isEmpty && _loading == false ? [
                      const Center(child: Padding(
                        padding: EdgeInsets.fromLTRB(0, 16, 0, 0),
                        child: Text("No active seasons"),
                      )),
                    ] : showActiveSeasons()
                  ),

                  if(selectedSeasonType[1]) Column(
                    children: _seasonsInactive.isEmpty && _loading == false ? [
                      const Center(child: Padding(
                        padding: EdgeInsets.fromLTRB(0, 16, 0, 0),
                        child: Text("No inactive seasons"),
                      )),
                    ] : showInactiveSeasons()
                  ),
                ],
              ),
            ),
          
          
          
            Padding(
              padding: const EdgeInsets.fromLTRB(0, 8, 0, 8),
              child: Column(
                children: [
                  Column(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            "Contracts",
                            style: GoogleFonts.titilliumWeb(
                              fontSize: 24,
                              fontWeight: FontWeight.w700,
                            ),
                          ),

                          ToggleButtons(
                            onPressed: (int index) {
                              setState(() {
                                for (int i = 0; i < selectedContractType.length; i++) {
                                  selectedContractType[i] = i == index;
                                }
                              });
                            },
                            borderRadius: const BorderRadius.all(Radius.circular(8)),
                            selectedBorderColor: theme.colorScheme.outline,
                            selectedColor: theme.colorScheme.onPrimaryContainer,
                            fillColor: theme.colorScheme.primaryContainer,
                            color: theme.colorScheme.onSurface,
                            constraints: const BoxConstraints(
                              minHeight: 40.0,
                              minWidth: 80.0,
                            ),
                            isSelected: selectedContractType,
                            children: const [
                              Text("Active"),
                              Text("Paused"),
                              Text("Completed"),
                            ],
                          ),
                        ],
                      ),
                    ],
                  ),

                  if(selectedContractType[0]) Column(
                    children: _contractsActive.isEmpty && _loading == false ? [
                      const Center(child: Padding(
                        padding: EdgeInsets.fromLTRB(0, 16, 0, 0),
                        child: Text("No active contracts"),
                      )),
                    ] : showActiveContracts()
                  ),

                  if(selectedContractType[1]) Column(
                    children: _contractsPaused.isEmpty && _loading == false ? [
                      const Center(child: Padding(
                        padding: EdgeInsets.fromLTRB(0, 16, 0, 0),
                        child: Text("No paused contracts"),
                      )),
                    ] : showPausedContracts()
                  ),

                  if(selectedContractType[2]) Column(
                    children: _contractsCompleted.isEmpty && _loading == false ? [
                      const Center(child: Padding(
                        padding: EdgeInsets.fromLTRB(0, 16, 0, 0),
                        child: Text("No completed contracts"),
                      )),
                    ] : showCompletedContracts()
                  ),
                ],
              ),
            ),
            const SizedBox(
              height: 88,
            ),
          ],
        ),
      )
    );
  }
}