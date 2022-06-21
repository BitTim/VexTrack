import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../Screens/auth.dart';
import '../../Services/auth.dart';
import '../../colors.dart';

class SignupFragment extends StatelessWidget {
  final TextEditingController usernameController = TextEditingController();
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  final Function(int) notifyParent;
  SignupFragment({Key? key, required this.notifyParent}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final authService = Provider.of<AuthService>(context);

    return Column(children: [
      Padding(
        padding: const EdgeInsets.only(bottom: 8.0),
        child: TextFormField(
          controller: usernameController,
          decoration: const InputDecoration(
            labelText: 'Username',
            prefixIcon: Icon(Icons.label),
            fillColor: AppColors.lightBG,
            border: OutlineInputBorder(),
          ),
          keyboardType: TextInputType.text,
          autocorrect: false,
        ),
      ),

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
          authService.signUp(
            username: usernameController.text,
            email: emailController.text,
            password: passwordController.text,
          );
        },
        child: const Text("Submit"),
      ),
      
      ElevatedButton(
        onPressed: () {
          notifyParent(AuthFragments.login.index);
        },
        child: const Text("Back to login"),
      ),
    ]);
  }
}
