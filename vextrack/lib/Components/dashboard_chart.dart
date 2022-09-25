import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/performance_chart.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Models/Seasons/performance.dart';
import 'package:vextrack/Models/Seasons/season.dart';
import 'package:vextrack/Models/Seasons/season_meta.dart';
import 'package:vextrack/Services/data.dart';

class DashboardChart extends StatefulWidget
{
  final String uid;
  const DashboardChart({super.key, required this.uid});

  @override
  DashboardChartState createState() => DashboardChartState();
}

class DashboardChartState extends State<DashboardChart>
{
  SeasonMeta? meta;
  Season? season;
  bool epilogue = false;
  GlobalKey<PerformanceChartState> chartKey = GlobalKey<PerformanceChartState>();
  
  @override
  void initState()
  {
    super.initState();
    update();
  }

  void update() async
  {
    SeasonMeta? activeMeta = await DataService.getActiveSeasonMeta();
    setState(() => meta = activeMeta);

    if(activeMeta == null)
    {
      setState(() => season = null);
      return;
    }

    Season? activeSeason = await DataService.getSeason(widget.uid, activeMeta.id);
    setState(() => season = activeSeason);
  }
  
  void updateEpilogue(bool ep)
  {
    PerformanceChart perf = chartKey.currentWidget as PerformanceChart;

    setState(() {
      epilogue = ep;
      perf.model.epilogue = ep;
    });
  }

  @override
  Widget build(BuildContext context)
  {
    if(meta == null || season == null) return const SizedBox.shrink();

    return Card(
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(8),
      ),
      surfaceTintColor: Theme.of(context).colorScheme.surface,
      clipBehavior: Clip.antiAliasWithSaveLayer,
      elevation: 8,
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
                    meta!.name,
                    style: GoogleFonts.titilliumWeb(
                      fontSize: 24,
                      fontWeight: FontWeight.w700,
                    ),
                  ),
                ],
              ),
            ),

            Padding(
              padding: const EdgeInsets.fromLTRB(8, 8, 16, 8),
              child: PerformanceChart(
                key: chartKey,
                model: Performance.fromSeason(season!, true, epilogue),
                showDaily: true,
                notifyEpilogueParent: (ep) => updateEpilogue(ep),
              ),
            ),

            Padding(
              padding: const EdgeInsets.fromLTRB(8, 0, 8, 8),
              child: Wrap(
                spacing: 8,
                runSpacing: 4,
                //alignment: WrapAlignment.spaceEvenly, //TODO: Make Wrap alignment fancier
                children: [
                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Padding(
                        padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                        child: Text(
                          "Deviation (Ideal):",
                          style: GoogleFonts.titilliumWeb(
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ),
                      Text(
                        season!.getFormattedDeviationIdeal(epilogue),
                        style: GoogleFonts.titilliumWeb(
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),

                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Padding(
                        padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                        child: Text(
                          "Deviation (Daily):",
                          style: GoogleFonts.titilliumWeb(
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ),
                      Text(
                        season!.getFormattedDeviationDaily(epilogue),
                        style: GoogleFonts.titilliumWeb(
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),

                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Padding(
                        padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                        child: Text(
                          "Days left:",
                          style: GoogleFonts.titilliumWeb(
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ),
                      Text(
                        Formatter.formatDays(season!.getRemainingDays()),
                        style: GoogleFonts.titilliumWeb(
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),

                  Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Padding(
                        padding: const EdgeInsets.fromLTRB(0, 0, 4, 0),
                        child: Text(
                          "Complete on:",
                          style: GoogleFonts.titilliumWeb(
                            fontWeight: FontWeight.w700,
                          ),
                        ),
                      ),
                      Text(
                        season!.getFormattedCompleteDate(epilogue),
                        style: GoogleFonts.titilliumWeb(
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),

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
                          season!.getFormattedDailyAvg(),
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
                          season!.getFormattedInactiveDays(),
                          style: GoogleFonts.titilliumWeb(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ],
                    ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}