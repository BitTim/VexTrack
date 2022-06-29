import 'package:flutter/material.dart';

class AppColors
{
  static const lightBG = Color(0xffffffff);
  static const lightShade = Color(0xffe5e5e5);
  static const lightShade_semiTransparent = Color(0xcce5e5e5);
  static const lightText = Color(0xff515151);

  static const darkBG = Color(0xff404040);
  static const darkShade = Color(0xff333333);
  static const darkShade_semiTransparent = Color(0xcc333333);
  static const darkText = Color(0xffffffff);

  static const accBlue = [
    Color(0xff1684fc),
    Color(0xff6975e8),
  ];
  static const accTeal = [
    Color(0xff10a780),
    Color(0xff009e97),
  ];
  static const accGreen = [
    Color(0xff0ac903),
    Color(0xff00bd5c),
  ];
  static const accYellow = [
    Color(0xffefb431),
    Color(0xffde9321),
  ];
  static const accOrange = [
    Color(0xfffc8316),
    Color(0xfff7595f),
  ];
  static const accRed = [
    Color(0xffe2030b),
    Color(0xffd40058),
  ];
  static const accPurple = [
    Color(0xff8a16fc),
    Color(0xffff00b9),
  ];

  static const accCyberpunk1 = [
    Color(0xff1ae699),
    Color(0xffdd0070),
  ];
  static const accCyberpunk2 = [
    Color(0xffe70efc),
    Color(0xff31f96f),
  ];
  static const accLavender = [
    Color(0xff7a13b1),
    Color(0xffd086bb),
  ];
  static const accAqua = [
    Color(0xff463bac),
    Color(0xff17fbe5),
  ];
  static const accNature = [
    Color(0xff18a358),
    Color(0xffc58c73),
  ];
  static const accEmerald = [
    Color(0xff2ea783),
    Color(0xff108321),
  ];
  static const accChocolate = [
    Color(0xff89384c),
    Color(0xffdb6a3a),
  ];

  static const winGradient = LinearGradient(
    colors: [
      Color(0xff66c2a9),
      Color(0xcc19b2b5),
      Colors.transparent
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: [0.0, 0.5, 1.0],
  );

  static const lossGradient = LinearGradient(
    colors: [
      Color(0xfff05c57),
      Color(0xcce54c7c),
      Colors.transparent
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: [0.0, 0.5, 1.0],
  );

  static const drawGradient = LinearGradient(
    colors: [
      darkBG,
      darkShade_semiTransparent,
      Colors.transparent
    ],
    begin: Alignment.topLeft,
    end: Alignment.bottomRight,
    stops: [0.0, 0.5, 1.0],
  );
}