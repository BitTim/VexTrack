import 'dart:math';

import 'package:fl_chart/fl_chart.dart';
import 'package:flutter/material.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Models/Seasons/performance.dart';

class PerformanceChart extends StatefulWidget
{
  Performance model;

  PerformanceChart({
    Key? key,
    required this.model,
  }) : super(key: key);

  @override
  State<PerformanceChart> createState() => PerformanceChartState();
}

class PerformanceChartState extends State<PerformanceChart> {
  bool cumulative = true;

  LineChartData getChartData()
  {
    return LineChartData(
      lineBarsData: [
        LineChartBarData( // Avg Ideal
          spots: Performance.mapPointToSpot(widget.model.getAverageIdeal()),
          gradient: AppColors.drawGradient,
          isCurved: true,
          preventCurveOverShooting: true,
          isStrokeCapRound: true,
          barWidth: 3,
          dotData: FlDotData(
            show: false,
          ),
        ),

        LineChartBarData( // User XP
          spots: Performance.mapPointToSpot(widget.model.getUserXP()),
          gradient: AppColors.lossGradient,
          isCurved: true,
          preventCurveOverShooting: true,
          isStrokeCapRound: true,
          barWidth: 3,
          dotData: FlDotData(
            show: false,
          ),
        ),

        LineChartBarData( // User Avg
          spots: Performance.mapPointToSpot(widget.model.getUserAverage()),
          gradient: AppColors.warnGradient,
          isCurved: true,
          preventCurveOverShooting: true,
          isStrokeCapRound: true,
          barWidth: 1,
          dotData: FlDotData(
            show: false,
          ),
          dashArray: [10, 5, 1, 5],
        ),
      ],
      gridData: FlGridData( //TODO: Change to actual lines
        drawVerticalLine: false,
        drawHorizontalLine: false,
      ),
      borderData: FlBorderData( // TODO: Change to actual border
        show: false,
      ),
      titlesData: FlTitlesData( // TODO: Change to actual titles
        show: false,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        AspectRatio(
          aspectRatio: 1.5,
          child: LineChart(
            getChartData()
          ),
        ),
        Row(
          children: [
            IconButton(
              icon: const Icon(Icons.line_axis_rounded),
              onPressed: () {
                setState(() {
                  widget.model.cumulative = !widget.model.cumulative;
                });
              },
            ),
            IconButton(
              icon: const Icon(Icons.rocket_launch_rounded),
              onPressed: () {
                setState(() {
                  widget.model.epilogue = !widget.model.epilogue;
                });
              },
            ),
          ],
        )
      ]
    );
  }
}