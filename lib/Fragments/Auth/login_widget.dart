import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../Screens/auth.dart';
import '../../Services/auth_service.dart';
import '../../colors.dart';

class LoginWidget extends StatelessWidget {
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  final Function(int) notifyParent;
  LoginWidget({Key? key, required this.notifyParent}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Column(children: [
      Padding(
        padding: const EdgeInsets.only(bottom: 8.0),
        child: TextFormField(
          controller: emailController,
          decoration: const InputDecoration(
            labelText: 'Email',
            prefixIcon: Icon(Icons.email),
            fillColor: AppColors.lightBG,
            border: OutlineInputBorder(),
          ),
          keyboardType: TextInputType.emailAddress,
          autocorrect: false,
        ),
      ),

      Padding(
        padding: const EdgeInsets.only(bottom: 8.0),
        child: TextFormField(
          controller: passwordController,
          decoration: const InputDecoration(
              labelText: 'Password',
              prefixIcon: Icon(Icons.key),
              border: OutlineInputBorder()),
          keyboardType: TextInputType.text,
          autocorrect: false,
          obscureText: true,
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

      ElevatedButton(
        onPressed: () {
          notifyParent(AuthFragments.forgot.index);
        },
        child: const Text("Forgot password?"),
      ),

      ElevatedButton(
        onPressed: () {
          notifyParent(AuthFragments.signup.index);
        },
        child: const Text("Need and account?"),
      ),
    ]);
  }
}
