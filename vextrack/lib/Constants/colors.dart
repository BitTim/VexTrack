import 'package:flutter/material.dart';

class AppColors
{
  static const lightBG = Color(0xffffffff);
  static const lightShade = Color(0xffe5e5e5);
  static const lightShadeSemiTransparent = Color(0xcce5e5e5);
  static const lightText = Color(0xff515151);
  static const lightTextSecondary = Color(0xff626262);

  static const darkBG = Color(0xff404040);
  static const darkShade = Color(0xff333333);
  static const darkShadeSemiTransparent = Color(0xcc333333);
  static const darkText = Color(0xffffffff);
  static const darkTextSecondary = Color(0xffcacaca);




  static const Color defaultAccent = Color(0xffC33149);




  static const win = [
    Color(0xff66c2a9),
    Color(0xff19b2b5),
    Color(0xcc19b2b5),
  ];

  static const warn = [
    Color(0xfff9e500),
    Color(0xfff6c900),
    Color(0xccf6c900),
  ];

  static const loss = [
    Color(0xfff05c57),
    Color(0xffe54c7c),
    Color(0xcce54c7c),
  ];

  static const epilogue = [
    Color(0xff2978a0),
    Color(0xff061a40),
    Color(0xcc061a40),
  ];




  static final winToTransparentGradient = LinearGradient(
    colors: [
      win[0],
      win[2],
      Colors.transparent
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: const [0.0, 0.5, 1.0],
  );

  static final lossToTransparentGradient = LinearGradient(
    colors: [
      loss[0],
      loss[2],
      Colors.transparent
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: const [0.0, 0.5, 1.0],
  );

  static const drawToTransparentGradient = LinearGradient(
    colors: [
      darkBG,
      darkShadeSemiTransparent,
      Colors.transparent,
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: [0.0, 0.5, 1.0],
  );

  static const defaultToTransparentGradient = LinearGradient(
    colors: [
      lightBG,
      lightShadeSemiTransparent,
      Colors.transparent,
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: [0.0, 0.5, 1.0],
  );




  static final winGradient = LinearGradient(
    colors: [
      win[0],
      win[1]
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: const [0.0, 1.0],
  );

  static final warnGradient = LinearGradient(
    colors: [
      warn[0],
      warn[1]
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: const [0.0, 1.0],
  );

  static final lossGradient = LinearGradient(
    colors: [
      loss[0],
      loss[1]
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: const [0.0, 1.0],
  );

  static const drawGradient = LinearGradient(
    colors: [
      darkBG,
      darkShade,
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: [0.0, 1.0],
  );

  static final epilogueGradient = LinearGradient(
    colors: [
      epilogue[0],
      epilogue[1]
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: const [0.0, 1.0],
  );
}