import 'package:dynamic_color/dynamic_color.dart';
import 'package:flutter/material.dart';

import 'package:firebase_core/firebase_core.dart';
import 'package:vextrack/screen_manager.dart';
import 'package:vextrack/themes.dart';
import 'firebase_options.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp(options: DefaultFirebaseOptions.currentPlatform);

  runApp(const MyApp());
}

class MyApp extends StatefulWidget {
  const MyApp({Key? key}) : super(key: key);

  @override
  State<MyApp> createState() => _MyAppState();
}

class _MyAppState extends State<MyApp> {
  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return DynamicColorBuilder(
        builder: (ColorScheme? lightColorScheme, ColorScheme? darkColorScheme) {
      AppThemes.setThemes(lightColorScheme, darkColorScheme);

      return MaterialApp(
        title: 'VexTrack',
        debugShowCheckedModeBanner: false,
        theme: AppThemes.getTheme(forceLightMode: true),
        darkTheme: AppThemes.getTheme(forceDarkMode: true),
        home: ScreenManager(onThemeChanged: () {
          setState(() {});
        }),
      );
    });
  }
}
