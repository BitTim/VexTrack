import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'Screens/auth.dart';
import 'Screens/home.dart';
import 'Screens/settings.dart';

enum Screens
{
  home,
  auth,
  settings
}

class _ScreenManagerState extends State
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
    final user = context.watch<User?>();
    // ignore: unnecessary_null_comparison
    if (user == null)
    {
      _currentScreen = Screens.auth.index;
    }
    // ignore: unnecessary_null_comparison
    else if(user != null && _currentScreen == Screens.auth.index)
    {
      _currentScreen = Screens.home.index;
    }
          
    if(_currentScreen == Screens.auth.index) return Auth(notifyParent: changeScreen);
    if(_currentScreen == Screens.settings.index) return Settings(notifyParent: changeScreen);
    return Home(notifyParent: changeScreen);
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