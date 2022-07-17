import 'package:flutter/material.dart';
import 'package:google_fonts/google_fonts.dart';
import 'package:vextrack/Components/gradient_progress.dart';
import 'package:vextrack/Constants/colors.dart';
import 'package:vextrack/Models/Goals/goal.dart';

class GoalWidget extends StatefulWidget
{
  final Goal model;
  const GoalWidget({Key? key, required this.model}) : super(key: key);

  @override
  GoalWidgetState createState() => GoalWidgetState();

}

class GoalWidgetState extends State<GoalWidget>
{
  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.fromLTRB(0, 4, 0, 0),
      child: SizedBox(
        height: 72,
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
                        fontSize: 18,
                        fontWeight: FontWeight.w700,
                        color: AppColors.lightText,
                      ),
                    ),
                  ]
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
                        segments: 1,
                        segmentStops: const [0.0, 1.0],
                        gradient: widget.model.getGradient(),
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
                      )
                    )
                  ],
                )
              )
            ],
          )
        ),
      ),
    );
  }
}