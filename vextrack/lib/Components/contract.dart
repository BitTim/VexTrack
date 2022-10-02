import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/goal.dart';
import 'package:vextrack/Components/gradient_progress.dart';
import 'package:vextrack/Models/Contracts/goal.dart';
import 'package:vextrack/Models/Contracts/contract.dart';

class ContractWidget extends StatefulWidget
{
  final Contract model;
  final bool showPause;
  final ExpandableController controller;
  final Function(Contract)? notifyPaused;
  const ContractWidget({Key? key, required this.model, required this.showPause, required this.controller, this.notifyPaused}) : super(key: key);

  @override
  ContractWidgetState createState() => ContractWidgetState();
}

class ContractWidgetState extends State<ContractWidget>
{
  showGoals() {
    List<Widget> goalList = [];

    for(Goal g in widget.model.goals)
    {
      goalList.add(
        GoalWidget(model: g)
      );
    }

    return goalList;
  }

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(0, 4, 0, 0),
      child: Card(
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(8),
        ),
        surfaceTintColor: Theme.of(context).colorScheme.surface,
        clipBehavior: Clip.antiAliasWithSaveLayer,
        elevation: 8,
        child: ExpandableNotifier(
          controller: widget.controller,
          child: ExpandablePanel(
            header: Align(
              child: Column(
                mainAxisSize: MainAxisSize.max,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                    child: Row(
                      mainAxisSize: MainAxisSize.max,
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          widget.model.name,
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 24,
                            fontWeight: FontWeight.w700,
                          ),
                        ),  

                        if(widget.showPause) IconButton(
                          onPressed: () => setState((() {
                            widget.model.paused = !widget.model.paused;
                            if(widget.notifyPaused != null) widget.notifyPaused!(widget.model);
                          })),
                          isSelected: widget.model.paused,
                          icon: const Icon(Icons.pause),
                        ),
                      ]
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
                    child: Row(
                      mainAxisSize: MainAxisSize.max,
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Padding(
                          padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                          child: GradientProgress(
                            width: MediaQuery.of(context).size.width * 0.684,
                            value: widget.model.getProgress(),
                            height: 8,
                            borderRadius: 4,
                            segments: widget.model.getSegmentCount(),
                            segmentStops: widget.model.getSegmentStops(),
                            gradient: widget.model.getGradient(),
                          ),
                        ),
                        Padding(
                          padding: const EdgeInsets.fromLTRB(4, 0, 0, 0),
                          child: Text(
                            widget.model.getFormattedProgress(),
                            style: GoogleFonts.titilliumWeb(
                              fontSize: 14,
                              fontWeight: FontWeight.w700,
                            ),
                          )
                        )
                      ],
                    )
                  ),
                  Row(
                    children: [
                      Padding(
                        padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                        child: Text(
                          "${widget.model.getFormattedXP()} / ${widget.model.getFormattedTotal()}",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                      Padding(
                        padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                        child: Text(
                          "Remaining: ${widget.model.getFormattedRemaining()}",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                    ],
                  ),

                  Row(
                    children: [
                      Padding(
                        padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                        child: Text(
                          "Next Unlock: ${widget.model.getFormattedNextUnlockName()}",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                      Padding(
                        padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                        child: Text(
                          "Progress: ${widget.model.getFormattedNextUnlockProgress()} (${widget.model.getFormattedNextUnlockRemaining()} remaining)",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                    ],
                  ),

                  Row(
                    children: [
                      Padding(
                        padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                        child: FutureBuilder(
                          future: widget.model.getFormattedCompleteDate(),
                          builder: (context, snapshot)
                          {
                            if(snapshot.hasData && snapshot.data != "") { return Text(
                              "Complete on: ${snapshot.data}",
                              style: GoogleFonts.titilliumWeb(
                                fontSize: 14,
                                fontWeight: FontWeight.w500,
                              ),
                            ); }
                            else { return const SizedBox.shrink(); }
                          }
                        ),
                      )
                    ],
                  ),
                ],
              )
            ),
            collapsed: const SizedBox.shrink(),
            expanded: Column(
              children: showGoals(),
            ),

            theme: ExpandableThemeData.combine(
              ExpandableThemeData(
                iconColor: Theme.of(context).colorScheme.onSurface,
                iconPadding: const EdgeInsets.fromLTRB(8, 0, 8, 0),
              ),
              ExpandableThemeData.defaults,
            ),
          ),
        ),
      ),
    );
  }
}