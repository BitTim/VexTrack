import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/performance_chart.dart';
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
      child: SizedBox(
        height: 512,
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
                padding: const EdgeInsets.all(8.0),
                child: PerformanceChart(
                  model: Performance.fromSeason(season!),
                  showDaily: true,
                ),
              ),

              Wrap(),
            ],
          ),
        ),
      ),
    );
  }
}