import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/goal.dart';
import 'package:vextrack/Components/gradient_progress.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Models/Goals/goal.dart';
import 'package:vextrack/Models/Goals/contract.dart';

class ContractWidget extends StatefulWidget
{
  final Contract model;
  const ContractWidget({Key? key, required this.model}) : super(key: key);

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
        clipBehavior: Clip.antiAliasWithSaveLayer,
        elevation: 8,
        child: ExpandableNotifier(
          child: ExpandablePanel(
            header: SizedBox(
              height: 140,
              child: Align(
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
                              color: AppColors.lightText,
                            ),
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
                                color: AppColors.lightText,
                              ),
                            )
                          )
                        ],
                      )
                    ),
                    Padding(
                      padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                      child: Row(
                        mainAxisSize: MainAxisSize.max,
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            "${widget.model.getFormattedXP()} / ${widget.model.getFormattedTotal()}",
                            style: GoogleFonts.titilliumWeb(
                              fontSize: 14,
                              fontWeight: FontWeight.w500,
                              color: AppColors.lightText,
                            ),
                          ),
                          Text(
                            "Remaining: ${widget.model.getFormattedRemaining()}",
                            style: GoogleFonts.titilliumWeb(
                              fontSize: 14,
                              fontWeight: FontWeight.w500,
                              color: AppColors.lightText,
                            ),
                          ),
                        ],
                      ),
                    ),
                    Padding(
                      padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                      child: Row(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Text(
                            "Next Unlock: ",
                            style: GoogleFonts.titilliumWeb(
                              fontSize: 14,
                              fontWeight: FontWeight.w700,
                              color: AppColors.lightText,
                            ),
                          ),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.end,
                              children: [
                                Text(
                                  "${widget.model.getNextUnlockName()} (${widget.model.getNextUnlockFormattedProgress()})",
                                  style: GoogleFonts.titilliumWeb(
                                    fontSize: 14,
                                    fontWeight: FontWeight.w500,
                                    color: AppColors.lightText,
                                  ),
                                ),
                                Text(
                                  "Remaining: ${widget.model.getNextUnlock()?.getFormattedRemaining()}",
                                  style: GoogleFonts.titilliumWeb(
                                    fontSize: 14,
                                    fontWeight: FontWeight.w500,
                                    color: AppColors.lightText,
                                  ),
                                ),
                              ],
                            ),
                          )
                        ],
                      ),
                    ),
                  ],
                )
              )
            ),
            collapsed: const SizedBox.shrink(),
            expanded: Column(
              children: showGoals(),
            ),
          ),
        ),
      ),
    );
  }
}