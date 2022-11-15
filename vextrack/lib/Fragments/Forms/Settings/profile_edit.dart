import 'package:email_validator/email_validator.dart';
import 'package:flutter/material.dart';

class ProfileEditForm extends StatefulWidget {
  const ProfileEditForm({super.key});

  @override
  ProfileEditFormState createState() => ProfileEditFormState();
}

class ProfileEditFormState extends State<ProfileEditForm> {
  TextEditingController usernameController = TextEditingController();
  TextEditingController emailController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Form(
      autovalidateMode: AutovalidateMode.onUserInteraction,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          Container(
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(8),
              color: Colors.red, // TODO: Replace with image
            ),
          ),
          const Padding(
            padding: EdgeInsets.fromLTRB(0, 8, 0, 4),
            child: Text("Username"),
          ),
          TextFormField(
            controller: usernameController,
            validator: (value) {
              if (value == null || value.isEmpty) return "";
              return null;
            },
            decoration: const InputDecoration(
              border: OutlineInputBorder(),
              errorStyle: TextStyle(height: 0.01),
            ),
            keyboardType: TextInputType.text,
          ),
          const Padding(
            padding: EdgeInsets.fromLTRB(0, 8, 0, 4),
            child: Text("E-Mail"),
          ),
          TextFormField(
            controller: emailController,
            decoration: const InputDecoration(
              border: OutlineInputBorder(),
            ),
            validator: (value) => EmailValidator.validate(value!)
                ? null
                : "Please enter a valid email",
            keyboardType: TextInputType.text,
          ),
        ],
      ),
    );
  }
}
