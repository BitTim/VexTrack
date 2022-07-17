import 'package:flutter/material.dart';
import 'package:vextrack/Constants/colors.dart';

class GradientProgress extends StatefulWidget
{
  final double? width;
  final double? height;
  final double value;
  final double borderRadius;
  final LinearGradient gradient;
  final Color background;
  final int segments;
  final List<double> segmentStops;

  const GradientProgress({Key? key, this.width, this.height, required this.value, this.borderRadius = 8, required this.gradient, this.background = AppColors.lightShade, this.segments = 1, this.segmentStops = const [0, 1]}) : super(key: key);

  @override
  GradientProgressState createState() => GradientProgressState();
}

class GradientProgressState extends State<GradientProgress>
{
  @override
  Widget build(BuildContext context) {
    double width = widget.width ?? MediaQuery.of(context).size.width;
    width -= widget.segments * 4;

    double getWidthMutltiplier(val, min, max)
    {
      double multiplier = (val - min) / (max - min);
      if (multiplier < 0) multiplier = 0;
      if (multiplier > 1) multiplier = 1;

      return multiplier;
    }

    double getSegmentWidthMultiplier(segment)
    {
      double segmentValue = widget.segmentStops[segment] - widget.segmentStops[segment - 1];
      return getWidthMutltiplier(segmentValue, widget.segmentStops[0], widget.segmentStops[widget.segmentStops.length - 1]);
    }

    Widget buildBackgroundSegment(segment)
    {
      return Padding(
        padding: const EdgeInsets.fromLTRB(2, 0, 2, 0),
        child: Container(
          width: width * getWidthMutltiplier(widget.segmentStops[segment], widget.segmentStops[segment - 1], widget.segmentStops[segment]) * getSegmentWidthMultiplier(segment),
          height: widget.height,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(widget.borderRadius),
            color: widget.background,
          ),
        ),
      );
    }

    Widget buildForegroundSegment(segment)
    {
      return Padding(
        padding: const EdgeInsets.fromLTRB(2, 0, 2, 0),
        child: Container(
          width: width * getWidthMutltiplier(widget.value, widget.segmentStops[segment - 1], widget.segmentStops[segment]) * getSegmentWidthMultiplier(segment),
          height: widget.height,
          decoration: BoxDecoration(
            borderRadius: BorderRadius.circular(widget.borderRadius),
            color: Colors.black,
          ),
        )
      );
    }

    Widget buildAllSegments()
    {
      List<Widget> backgroundSegments = [];
      List<Widget> foregroundSegments = [];

      for (int i = 1; i < widget.segments + 1; i++)
      {
        backgroundSegments.add(buildBackgroundSegment(i));
        foregroundSegments.add(buildForegroundSegment(i));
      }

      return Stack(
        children: [
          Row(
            children: backgroundSegments,
          ),
          ShaderMask(
            shaderCallback: ((bounds) {
              return widget.gradient.createShader(bounds);
            }),
            blendMode: BlendMode.srcATop,
            child: Row(
              children: foregroundSegments
            ),
          ),
        ],
      );
    }

    return Padding(
      padding: const EdgeInsets.fromLTRB(6, 0, 6, 0),
      child: buildAllSegments()
    );
  }
}