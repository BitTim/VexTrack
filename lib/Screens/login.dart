import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../Core/auth_service.dart';

class Login extends StatelessWidget
{
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  @override
  Widget build(BuildContext context)
  {
    return Scaffold(
      body: Column(
        children: [
          TextField(
            controller: emailController,
            decoration: const InputDecoration(
              labelText: 'Email',
            ),
          ),
          TextField(
            controller: passwordController,
            decoration: const InputDecoration(
              labelText: 'Password',
            ),
          ),
          ElevatedButton(
            onPressed: () {
              context.read<AuthService>().logIn(
                email: emailController.text,
                password: passwordController.text,
              );
            },
            child: const Text("Log in"),
          ),
        ],
      )
    );
  }
}