import 'package:firebase_auth/firebase_auth.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:vextrack/Screens/home.dart';

import '../../Screens/auth.dart';

class AuthWrapper extends StatelessWidget
{
  const AuthWrapper({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context)
  {
    final user = context.watch<User?>();
    // ignore: unnecessary_null_comparison
    if (user != null) return const Home();
    return const Auth();
  }
}