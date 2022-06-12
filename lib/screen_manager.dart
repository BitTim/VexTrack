import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/cupertino.dart';
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
  int _currentPage = Screens.home.index;

  void changePage(int id)
  {
    setState(() {
      _currentPage = id;
    });
  }

  @override
  Widget build(BuildContext context)
  {
    final user = context.watch<User?>();
    // ignore: unnecessary_null_comparison
    if (user != null) _currentPage = Screens.auth.index;
    
    if(_currentPage == Screens.auth.index) return Auth(notifyParent: changePage);
    if(_currentPage == Screens.settings.index) return Settings(notifyParent: changePage);

    return Home(notifyParent: changePage);
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