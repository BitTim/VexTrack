import 'package:cloud_firestore/cloud_firestore.dart';
import 'package:firebase_auth/firebase_auth.dart';

class AuthService{
  static final _auth = FirebaseAuth.instance;
  static final _firestore = FirebaseFirestore.instance;

  static void signOut() async
  {
    await _auth.signOut();
  }

  static Future<bool> logIn({required String email, required String password}) async
  {
    try
    {
      await _auth.signInWithEmailAndPassword(email: email, password: password);
      return true;
    }
    catch(e)
    {
      return false;
    }
  }

  static Future<bool> signUp({required String username, required String email, required String password}) async
  {
    try
    {
      UserCredential result = await _auth.createUserWithEmailAndPassword(email: email, password: password);      
      User? signedInUser = result.user;

      if (signedInUser != null) {
        _firestore.collection('users').doc(signedInUser.uid).set({
          'username': username,
          'email': email,
          'profilePicture': ''
        });
        return true;
      }

      return false;
    }
    catch(e)
    {
      return false;
    }
  }

  static Future<bool> forgot({required String email}) async
  {
    try
    {
      await _auth.sendPasswordResetEmail(email: email);
      return true;
    }
    catch(e)
    {
      return false;
    }
  }
}