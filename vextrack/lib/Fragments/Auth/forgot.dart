import 'package:flutter/material.dart';

import '../../Screens/auth.dart';
import '../../Services/auth.dart';
import '../../colors.dart';

class ForgotFragment extends StatelessWidget {
  final TextEditingController emailController = TextEditingController();

  final Function(int) notifyParent;
  ForgotFragment({Key? key, required this.notifyParent}) : super(key: key);

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
            border: OutlineInputBorder(),
          ),
          keyboardType: TextInputType.emailAddress,
          autocorrect: false,
        ),
      ),
      
      ElevatedButton(
        onPressed: () {
          AuthService.forgot(
                email: emailController.text,
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
