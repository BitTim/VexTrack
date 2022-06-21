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
  int fragmentID = 0;

  void _changeFragment(int id)
  {
    setState(() {
      fragmentID = id;
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
            
            if(fragmentID == AuthFragments.login.index)
              LoginFragment(notifyParent: _changeFragment),
            if(fragmentID == AuthFragments.signup.index)
              SignupFragment(notifyParent: _changeFragment),
            if(fragmentID == AuthFragments.forgot.index)
              ForgotFragment(notifyParent: _changeFragment),
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