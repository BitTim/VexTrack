import 'package:expandable/expandable.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/gradient_progress.dart';
import 'package:vextrack/Components/performance_chart.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Core/history_calc.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/Seasons/performance.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Services/settings.dart';

class SeasonWidget extends StatefulWidget {
  const SeasonWidget({
    Key? key,
    required this.model,
    required this.controller,
  }) : super(key: key);

  final Season model;
  final ExpandableController controller;

  @override
  SeasonWidgetState createState() => SeasonWidgetState();
}

class SeasonWidgetState extends State<SeasonWidget> {
  Gradient getGradient() {
    if (widget.model.hasEpilogue()) return AppColors.epilogueGradient;
    if (widget.model.hasCompleted()) return AppColors.winGradient;
    if (widget.model.isActive()) return AppColors.warnGradient;

    return AppColors.lossGradient;
  }

  IconData getIcon() {
    if (widget.model.hasEpilogue()) return Icons.verified_rounded;
    if (widget.model.hasCompleted()) return Icons.check_rounded;
    if (widget.model.isActive()) return Icons.warning_amber_rounded;

    return Icons.close_rounded;
  }

  Color getColor() {
    if (widget.model.hasEpilogue()) return AppColors.epilogue[0];
    if (widget.model.hasCompleted()) return AppColors.win[0];
    if (widget.model.isActive()) return AppColors.warn[0];

    return AppColors.loss[0];
  }

  @override
  Widget build(BuildContext context) {
    Map<String, dynamic> extremeDays = HistoryCalc.getExtremeDays(
        widget.model.history, SettingsService.data!.ignoreInactiveDays);

    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
      surfaceTintColor: Theme.of(context).colorScheme.surface,
      clipBehavior: Clip.antiAliasWithSaveLayer,
      elevation: 8,
      child: ExpandableNotifier(
        controller: widget.controller,
        child: ExpandablePanel(
          header: SizedBox(
            height: 128,
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
                          widget.model.meta.name,
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
                            segmentStops: [0.0, 1.0, XPCalc.getMaxProgress()],
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
                  Padding(
                    padding: const EdgeInsets.fromLTRB(8, 4, 8, 4),
                    child: Text(
                      widget.model.isActive()
                          ? "${Formatter.formatDate(widget.model.meta.getStartDate())} - ${Formatter.formatDate(widget.model.meta.getEndDate())} (${Formatter.formatDays(widget.model.getRemainingDays())} remaining)"
                          : "${Formatter.formatDate(widget.model.meta.getStartDate())} - ${Formatter.formatDate(widget.model.meta.getEndDate())} (${Formatter.formatDurationDays(widget.model.meta.getDuration())})",
                      style: GoogleFonts.titilliumWeb(
                        fontSize: 14,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
          collapsed: const SizedBox.shrink(),
          expanded: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Padding(
                padding: const EdgeInsets.fromLTRB(8, 4, 8, 0),
                child: Wrap(
                  direction: Axis.horizontal,
                  alignment: WrapAlignment.start,
                  spacing: 16,
                  children: [
                    Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Text(
                          "Daily average: ",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                        Text(
                          widget.model.getFormattedDailyAvg(),
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                    Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Text(
                          "Inactive days: ",
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                        Text(
                          widget.model.getFormattedInactiveDays(),
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                    Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Padding(
                          padding: const EdgeInsets.fromLTRB(0, 0, 8, 0),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text(
                                "Strongest day: ",
                                style: GoogleFonts.titilliumWeb(
                                  fontSize: 14,
                                  fontWeight: FontWeight.w700,
                                ),
                              ),
                              Text(
                                "${Formatter.formatDate(extremeDays['strongestDate'])} (${Formatter.formatXP(extremeDays['strongestXP'])})",
                                style: GoogleFonts.titilliumWeb(
                                  fontSize: 14,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ],
                          ),
                        ),
                        Padding(
                          padding: const EdgeInsets.fromLTRB(8, 0, 0, 0),
                          child: Row(
                            mainAxisSize: MainAxisSize.min,
                            children: [
                              Text(
                                "Weakest day: ",
                                style: GoogleFonts.titilliumWeb(
                                  fontSize: 14,
                                  fontWeight: FontWeight.w700,
                                ),
                              ),
                              Text(
                                "${Formatter.formatDate(extremeDays['weakestDate'])} (${Formatter.formatXP(extremeDays['weakestXP'])})",
                                style: GoogleFonts.titilliumWeb(
                                  fontSize: 14,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ],
                          ),
                        ),
                      ],
                    ),
                  ],
                ),
              ),
              Padding(
                padding: const EdgeInsets.all(8.0),
                child: PerformanceChart(
                  model: Performance.fromSeason(widget.model, true, false),
                  showDaily: false,
                  notifyEpilogueParent: (ep) {
                    return;
                  },
                  notifyCumulativeParent: (cumulative) {
                    return;
                  },
                ),
              ),
            ],
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
    );
  }
}
