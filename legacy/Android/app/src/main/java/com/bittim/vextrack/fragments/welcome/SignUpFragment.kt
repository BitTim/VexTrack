package com.bittim.vextrack.fragments.welcome

import android.os.Bundle
import android.text.TextUtils
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import com.bittim.vextrack.WelcomeActivity
import com.bittim.vextrack.core.Utility
import com.bittim.vextrack.databinding.FragmentWelcomeSignUpBinding
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.UserProfileChangeRequest

class SignUpFragment : Fragment() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
    }

    private var _binding: FragmentWelcomeSignUpBinding? = null
    private val binding get() = _binding!!

    private lateinit var auth: FirebaseAuth

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        _binding = FragmentWelcomeSignUpBinding.inflate(inflater, container, false)
        initButtons()

        auth = FirebaseAuth.getInstance()
        return binding.root
    }

    override fun onDestroyView() {
        super.onDestroyView()
        _binding = null
    }



    // ================================
    //  Initializers
    // ================================

    private fun initButtons()
    {
        binding.signUpLogInButton.setOnClickListener { (activity as WelcomeActivity).changeFragmentLogIn(true) }
        binding.signUpSignUpButton.setOnClickListener { createUser() }
    }



    // ================================
    //  Utility
    // ================================

    private fun createUser()
    {
        val username: String = binding.signUpUsernameEditText.text.toString()
        val email: String = binding.signUpEmailEditText.text.toString()
        val password: String = binding.signUpPasswordEditText.text.toString()

        if (TextUtils.isEmpty(username))
        {
            binding.signUpUsernameEditText.error = "Username cannot be empty"
            binding.signUpUsernameEditText.requestFocus()
            return
        }

        if (TextUtils.isEmpty(email))
        {
            binding.signUpEmailEditText.error = "E-Mail cannot be empty"
            binding.signUpEmailEditText.requestFocus()
            return
        }

        if (TextUtils.isEmpty(password))
        {
            binding.signUpPasswordEditText.error = "Password cannot be empty"
            binding.signUpPasswordEditText.requestFocus()
            return
        }

        auth.createUserWithEmailAndPassword(email, password).addOnCompleteListener { task ->
            if (task.isSuccessful)
            {
                auth.signInWithEmailAndPassword(email, password).addOnCompleteListener {
                    if (it.isSuccessful)
                    {
                        val profileBuilder = UserProfileChangeRequest.Builder()
                            .setDisplayName(username)
                            .setPhotoUri(Utility.genGenericProfilePic(activity as WelcomeActivity, username))
                            .build()

                        auth.currentUser?.updateProfile(profileBuilder)
                        auth.currentUser?.sendEmailVerification()
                        (activity as WelcomeActivity).startMainActivity()
                    }
                    else
                    {
                        Toast.makeText(activity as WelcomeActivity, "Error: " + it.exception?.message, Toast.LENGTH_LONG).show()
                    }
                }
            }
            else
            {
                Toast.makeText(activity as WelcomeActivity, "Error: " + task.exception?.message, Toast.LENGTH_LONG).show()
            }
        }
    }
}