import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:vextrack/Services/auth.dart';

import 'package:firebase_core/firebase_core.dart';
import 'package:vextrack/Services/data.dart';
import 'package:vextrack/screen_manager.dart';
import 'firebase_options.dart';

Future<void> main() async {
  WidgetsFlutterBinding.ensureInitialized();
  await Firebase.initializeApp(options: DefaultFirebaseOptions.currentPlatform);
	runApp(const MyApp());
}

class MyApp extends StatelessWidget {
	const MyApp({Key? key}) : super(key: key);

	// This widget is the root of your application.
	@override
	Widget build(BuildContext context) {
    return MultiProvider(
      providers: [
        Provider<AuthService>(
          create: (_) => AuthService(FirebaseAuth.instance),
        ),
        ChangeNotifierProxyProvider<AuthService, DataService>(
          create: (BuildContext context) => DataService(FirebaseFirestore.instance, Provider.of<AuthService>(context, listen: false)),
          update: (BuildContext context, AuthService auth, DataService? data) => DataService(FirebaseFirestore.instance, auth),
        ),
        StreamProvider(
          create: ((context) => context.read<AuthService>().authStateChanges), 
          initialData: null,
        )
      ],
        child: MaterialApp(
        title: 'VexTrack',
        theme: ThemeData(
          primarySwatch: Colors.blue,
        ),
        home: const ScreenManager(),
      )
    );
	}
}