
import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Core/formatter.dart';
import 'package:vextrack/Core/xp_calc.dart';
import 'package:vextrack/Models/Seasons/performance.dart';
import 'package:vextrack/Services/data.dart';
import 'package:vextrack/themes.dart';

class PerformanceChart extends StatefulWidget
{
  final Performance model;
  final bool showDaily;

  const PerformanceChart({
    Key? key,
    required this.model,
    required this.showDaily,
  }) : super(key: key);

  @override
  State<PerformanceChart> createState() => PerformanceChartState();
}

class PerformanceChartState extends State<PerformanceChart> {
  List<HorizontalLine> getBattlepassLines()
  {
    List<HorizontalLine> lines = [];
    if (!widget.model.cumulative) return lines;

    int nLevels = DataService.battlepassParams!.levels;
    if (widget.model.epilogue) nLevels += DataService.battlepassParams!.epilogue;

    int cumulativeSum = 0;
    for (int i = 1; i < nLevels + 1; i++)
    {
      Color col = AppThemes.getTheme().colorScheme.surfaceVariant;
      if (i % 5 == 0) col = AppColors.win[0];
      if (i > DataService.battlepassParams!.levels) col = AppColors.epilogue[0];
      cumulativeSum += XPCalc.getLevelTotal(i + 1) as int;

      if (cumulativeSum < widget.model.activeXP) col = col.withAlpha(16);

      lines.add(HorizontalLine(
        color: col,
        strokeWidth: 1,
        y: cumulativeSum.toDouble(),
      ));
    }

    return lines;
  }

  LineChartData getChartData()
  {
    return LineChartData(
      lineBarsData: [
        LineChartBarData( // Avg Ideal
          spots: Performance.mapPointToSpot(widget.model.getAverageIdeal()),
          color: AppThemes.getTheme().colorScheme.onSurfaceVariant,
          isCurved: false,
          isStrokeCapRound: true,
          barWidth: 3,
          dotData: FlDotData(
            show: false,
          ),
          isStepLineChart: false,
        ),

        LineChartBarData( // User XP
          spots: Performance.mapPointToSpot(widget.model.getUserXP()),
          gradient: AppColors.lossGradient,
          isCurved: false,
          isStrokeCapRound: true,
          barWidth: 3,
          dotData: FlDotData(
            show: false,
          ),
          isStepLineChart: false,
        ),

        LineChartBarData( // User Avg
          spots: Performance.mapPointToSpot(widget.model.getUserAverage()),
          gradient: AppColors.warnGradient,
          isCurved: false,
          isStrokeCapRound: true,
          barWidth: 3,
          dotData: FlDotData(
            show: false,
          ),
          dashArray: [10, 5, 1, 5],
          isStepLineChart: false,
        ),

        if(widget.showDaily) LineChartBarData( // User Ideal
          spots: Performance.mapPointToSpot(widget.model.getUserIdeal()),
          gradient: AppColors.epilogueGradient,
          isCurved: false,
          isStrokeCapRound: true,
          barWidth: 3,
          dotData: FlDotData(
            show: false,
          ),
          dashArray: [5, 5, 5, 5],
          isStepLineChart: false,
        ),
      ],

      gridData: FlGridData(
        drawHorizontalLine: false,
        drawVerticalLine: true,
        verticalInterval: 5,
        getDrawingVerticalLine: (value) => FlLine(
          color: Theme.of(context).colorScheme.surface,
          strokeWidth: 1,
        ),
      ),

      borderData: FlBorderData(
        show: false,
      ),

      titlesData: FlTitlesData(
        bottomTitles: AxisTitles(
          sideTitles: SideTitles(
            showTitles: true,
            interval: 5,
            getTitlesWidget: (value, meta) {
              String text = "";
              if(value % 5 == 0) text = "${value.toInt()}";

              return Text(
                text,
                style: GoogleFonts.titilliumWeb(
                  fontSize: 12,
                ),
              );
            },
          ),
        ),
        leftTitles: AxisTitles(
          sideTitles: SideTitles(
            showTitles: true,
            reservedSize: 48,
            interval: widget.model.getMaxChartXP() / 15,
            getTitlesWidget: (value, meta) {
              String text = Formatter.formatLargeNumber(value.toInt());

              return Text(
                text,
                style: GoogleFonts.titilliumWeb(
                  fontSize: 12,
                ),
              );
            },
          )
        ),
        topTitles: AxisTitles(
          sideTitles: SideTitles(
            showTitles: false,
          ),
        ),
        rightTitles: AxisTitles(
          sideTitles: SideTitles(
            showTitles: false,
          ),
        ),
      ),

      extraLinesData: ExtraLinesData(
        extraLinesOnTop: false,
        horizontalLines: getBattlepassLines(),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {

    return Column(
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          mainAxisSize: MainAxisSize.max,
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            Text(
              "Performance",
              style: GoogleFonts.titilliumWeb(
                fontWeight: FontWeight.w700,
                fontSize: 18,
              ),
            ),
            Row(
              mainAxisSize: MainAxisSize.min,
              children: [
                IconButton(
                  icon: const Icon(Icons.bar_chart),
                  selectedIcon: const Icon(Icons.line_axis),
                  isSelected: widget.model.cumulative,
                  onPressed: () {
                    setState(() {
                      widget.model.cumulative = !widget.model.cumulative;
                    });
                  },
                ),
                IconButton(
                  icon: const Icon(Icons.rocket_launch_outlined),
                  selectedIcon: const Icon(Icons.rocket_launch),
                  isSelected: widget.model.epilogue,
                  onPressed: () {
                    setState(() {
                      widget.model.epilogue = !widget.model.epilogue;
                    });
                  },
                ),
              ],
            ),
          ],
        ),

        AspectRatio(
          aspectRatio: 1.5,
          child: LineChart(
            getChartData(),
            swapAnimationCurve: Curves.easeInOutSine,
            swapAnimationDuration: const Duration(milliseconds: 250),
          ),
        ),
      ]
    );
  }
}