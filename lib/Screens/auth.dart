import 'package:flutter/material.dart';
import 'package:vextrack/Fragments/Auth/login_widget.dart';
import 'package:vextrack/Fragments/Auth/signup_widget.dart';

import '../Fragments/Auth/forgot_widget.dart';

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
              LoginWidget(notifyParent: _changeScreen),
            if(screenID == AuthFragments.signup.index)
              SignupWidget(notifyParent: _changeScreen),
            if(screenID == AuthFragments.forgot.index)
              ForgotWidget(notifyParent: _changeScreen),
          ],
        ),
      )
    );
  }
}

class Auth extends StatefulWidget
{
  const Auth({Key? key}) : super(key: key);

  @override
  State createState()
  {
    return _AuthState();
  }
}