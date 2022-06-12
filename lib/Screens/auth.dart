import 'package:flutter/material.dart';
import 'package:vextrack/Fragments/Auth/login.dart';
import 'package:vextrack/Fragments/Auth/signup.dart';

import '../Fragments/Auth/forgot.dart';

enum AuthFragments
{
  login,
  signup,
  forgot,
}

class _AuthState extends State
{
  int screenID = 0;

  void _changeScreen(int id)
  {
    setState(() {
      screenID = id;
    });
  }

  @override
  Widget build(BuildContext context)
  {
    return Scaffold(
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            const Text('Welcome to VexTrack'),
            
            if(screenID == AuthFragments.login.index)
              LoginFragment(notifyParent: _changeScreen),
            if(screenID == AuthFragments.signup.index)
              SignupFragment(notifyParent: _changeScreen),
            if(screenID == AuthFragments.forgot.index)
              ForgotFragment(notifyParent: _changeScreen),
          ],
        ),
      )
    );
  }
}

class Auth extends StatefulWidget
{
  final Function(int) notifyParent;
  const Auth({Key? key, required this.notifyParent}) : super(key: key);

  @override
  State createState()
  {
    return _AuthState();
  }
}