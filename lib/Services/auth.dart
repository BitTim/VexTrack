import 'package:firebase_auth/firebase_auth.dart';

class AuthService{
  final FirebaseAuth _auth;
  AuthService(this._auth);

  Stream<User?> get authStateChanges => _auth.authStateChanges();
  String? get currentUID => _auth.currentUser?.uid;

  Future<void> signOut() async
  {
    await _auth.signOut();
  }

  Future<String> logIn({required String email, required String password}) async
  {
    try
    {
      await _auth.signInWithEmailAndPassword(email: email, password: password);
      return "Login successful";
    }
    catch(e)
    {
      return e.toString();
    }
  }

  Future<String> signUp({required String username, required String email, required String password}) async
  {
    try
    {
      await _auth.createUserWithEmailAndPassword(email: email, password: password);
      _auth.currentUser?.updateDisplayName(username);
      return "Signup successful";
    }
    catch(e)
    {
      return e.toString();
    }
  }

  Future<String> forgot({required String email}) async
  {
    try
    {
      await _auth.sendPasswordResetEmail(email: email);
      return "Password reset email sent";
    }
    catch(e)
    {
      return e.toString();
    }
  }
}