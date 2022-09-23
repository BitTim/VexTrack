import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/goal.dart';
import 'package:vextrack/Components/gradient_progress.dart';
import 'package:vextrack/Models/Contracts/daily_contract.dart';
import 'package:vextrack/Models/Contracts/goal.dart';
import 'package:vextrack/themes.dart';

import '../Constants/colors.dart';

class DailyContractWidget extends StatefulWidget
{
  final DailyContract model;
  const DailyContractWidget({super.key, required this.model});

  @override
  DailyContractWidgetState createState() => DailyContractWidgetState();
}

class DailyContractWidgetState extends State<DailyContractWidget>
{
  Gradient getGradient()
  {
    if (widget.model.hasEpilogue()) return AppColors.epilogueGradient;
    if (widget.model.hasCompleted()) return AppColors.winGradient;
    
    Color primary = AppThemes.getTheme().colorScheme.primary;
    return LinearGradient(colors: [primary, primary], stops: const [0, 1]);
  }

  IconData getIcon()
  {
    if (widget.model.hasEpilogue()) return Icons.verified_rounded;
    if (widget.model.hasCompleted()) return Icons.check_rounded;
    
    return Icons.clear;
  }

  Color getColor()
  {
    if (widget.model.hasEpilogue()) return AppColors.epilogue[0];
    if (widget.model.hasCompleted()) return AppColors.win[0];
    
    return Colors.transparent;
  }

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
  Widget build(BuildContext context)
  {
    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
      surfaceTintColor: Theme.of(context).colorScheme.surface,
      clipBehavior: Clip.antiAliasWithSaveLayer,
      elevation: 8,
      child: ExpandableNotifier(
        child: ExpandablePanel(
          header: SizedBox(
            height: 96,
            child: Align(
              child: Column(
                children: [
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                    child: Row(
                      mainAxisSize: MainAxisSize.max,
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          "Daily Contract",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 24,
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                        ShaderMask(
                          shaderCallback: (Rect bounds) {
                            return getGradient().createShader(bounds);
                          },
                          child: Icon(
                            getIcon(),
                            color: getColor(),
                          ),
                        ),
                      ],
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(0, 4, 8, 0),
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
                            segments: 2,
                            segmentStops: [0.0, 1.0, widget.model.getMaxProgress()],
                            gradient: getGradient() as LinearGradient,
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
                          ),
                        ),
                      ],
                    ),
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
                ],
              ),
            ),
          ),

          collapsed: const SizedBox.shrink(),
          expanded: Column(
            children: showGoals(),
          ),
        ),
      ),
    );
  }
}