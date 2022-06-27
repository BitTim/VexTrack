import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';

import 'Screens/auth.dart';
import 'Screens/home.dart';
import 'Screens/settings.dart';

enum Screens
{
  home,
  auth,
  settings
}

class _ScreenManagerState extends State<ScreenManager>
{
  int _currentScreen = Screens.home.index;

  void changeScreen(int id)
  {
    setState(() {
      _currentScreen = id;
    });
  }

  @override
  Widget build(BuildContext context)
  {
    return StreamBuilder<User?>(
      stream: FirebaseAuth.instance.authStateChanges(),
      builder: (BuildContext context, snapshot) {
        if (snapshot.hasData) {
          if(_currentScreen == Screens.auth.index) _currentScreen = Screens.home.index; // Set current screen to home when user is logged in

          //FIXME: Might be a problem that on every screen change a new instance of the screen is returned
          if(_currentScreen == Screens.settings.index) return Settings(uid: snapshot.data!.uid, notifyParent: changeScreen);
          return Home(uid: snapshot.data!.uid, notifyParent: changeScreen); 
        }
        else
        {
          _currentScreen = Screens.auth.index;
          return Auth(notifyParent: changeScreen);
        }
      }
    );
  }
}

class ScreenManager extends StatefulWidget
{
  const ScreenManager({Key? key}) : super(key: key);

  @override
  State createState()
  {
    return _ScreenManagerState();
  }
}