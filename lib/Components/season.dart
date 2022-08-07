import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/gradient_progress.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Services/data.dart';

class SeasonWidget extends StatefulWidget {
  const SeasonWidget({
    Key? key,
    required this.model,
  }) : super(key: key);

  final Season model;

  @override
  SeasonWidgetState createState() => SeasonWidgetState();
}

class SeasonWidgetState extends State<SeasonWidget>
{
  Gradient getGradient()
  {
    if (widget.model.hasEpilogue()) return AppColors.epilogueGradient;
    if (widget.model.hasCompleted()) return AppColors.winGradient;
    if (widget.model.isActive()) return AppColors.warnGradient;
    
    return AppColors.lossGradient;
  }

  IconData getIcon()
  {
    if (widget.model.hasEpilogue()) return Icons.verified_rounded;
    if (widget.model.hasCompleted()) return Icons.check_rounded;
    if (widget.model.isActive()) return Icons.warning_amber_rounded;
    
    return Icons.close_rounded;
  }

  Color getColor()
  {
    if (widget.model.hasEpilogue()) return AppColors.epilogue[0];
    if (widget.model.hasCompleted()) return AppColors.win[0];
    if (widget.model.isActive()) return AppColors.warn[0];
    
    return AppColors.loss[0];
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
      clipBehavior: Clip.antiAliasWithSaveLayer,
      elevation: 8,
      child: ExpandableNotifier(
        child: ExpandablePanel(
          header: SizedBox(
            height: 96,
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
                          DataService.getSeasonName(widget.model.id),
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 24,
                            fontWeight: FontWeight.w700,
                            color: AppColors.lightText,
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
                            segmentStops: [0.0, 1.0, XPCalc.getMaxProgress()],
                            gradient: getGradient() as LinearGradient,
                          ),
                        ),
                        Padding(
                          padding: const EdgeInsets.fromLTRB(4, 0, 0, 0),
                          child: Text(
                            widget.model.getPrecentage(),
                            style: GoogleFonts.titilliumWeb(
                              fontSize: 14,
                              fontWeight: FontWeight.w700,
                              color: AppColors.lightText,
                            ),
                          ),
                        ),
                      ],
                    ),
                  ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 4, 8, 4),
                    child: Text(
                      "${DataService.getSeasonFormattedStartDate(widget.model.id)} - ${DataService.getSeasonFormattedEndDate(widget.model.id)} (${DataService.getSeasonFormattedDuration(widget.model.id)})",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                        color: AppColors.lightText,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),

          collapsed: const SizedBox.shrink(),

          expanded: Column(
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                child: Row(
                  mainAxisSize: MainAxisSize.max,
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      "${widget.model.getXP()} / ${widget.model.getFormattedTotal()}",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                        color: AppColors.lightText,
                      ),
                    ),
                    Text(
                      "Remaining: ${widget.model.getRemaining()}",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                        color: AppColors.lightText,
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}