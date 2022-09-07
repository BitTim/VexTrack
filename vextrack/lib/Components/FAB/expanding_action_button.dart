import 'package:flutter/material.dart';

class ExpandingActionButton extends StatelessWidget
{
  const ExpandingActionButton({
    super.key,
    required this.distance,
    required this.progress,
    required this.child,
  });

  final double distance;
  final Animation<double> progress;
  final Widget child;

  @override
  Widget build(BuildContext context)
  {
    return AnimatedBuilder(
      animation: progress,
      builder: (context, child) {
        final offset = Offset(0, distance);
        return Positioned(
          right: 8.0 + offset.dx,
          bottom: 8.0 + offset.dy,
          child: child!,
        );
      },
      child: FadeTransition(
        opacity: progress,
        child: child,
      ),
    );
  }
}