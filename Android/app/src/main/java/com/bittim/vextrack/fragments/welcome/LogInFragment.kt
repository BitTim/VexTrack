package com.bittim.vextrack.fragments.welcome

import android.content.Intent
import android.os.Bundle
import android.text.TextUtils
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import com.bittim.vextrack.MainActivity
import com.bittim.vextrack.R
import com.bittim.vextrack.WelcomeActivity
import com.bittim.vextrack.databinding.FragmentLogInBinding
import com.google.firebase.auth.FirebaseAuth

class LogInFragment : Fragment() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
    }

    private var _binding: FragmentLogInBinding? = null
    private val binding get() = _binding!!

    private lateinit var auth: FirebaseAuth

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        _binding = FragmentLogInBinding.inflate(inflater, container, false)
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
        binding.logInForgotPasswordButton.setOnClickListener { (activity as WelcomeActivity).changeFragmentForgot() }
        binding.logInSignUpButton.setOnClickListener { (activity as WelcomeActivity).changeFragmentSignUp() }
        binding.logInLogInButton.setOnClickListener { logInUser() }
    }



    // ================================
    //  Utility
    // ================================

    private fun logInUser()
    {
        val email: String = binding.logInEmailEditText.text.toString()
        val password: String = binding.logInPasswordEditText.text.toString()

        if (TextUtils.isEmpty(email))
        {
            binding.logInEmailEditText.error = "E-Mail cannot be empty"
            binding.logInEmailEditText.requestFocus()
            return
        }

        if (TextUtils.isEmpty(password))
        {
            binding.logInPasswordEditText.error = "Password cannot be empty"
            binding.logInPasswordEditText.requestFocus()
            return
        }

        auth.signInWithEmailAndPassword(email, password).addOnCompleteListener {
            if (it.isSuccessful)
            {
                (activity as WelcomeActivity).startMainActivity()
            }
            else
            {
                Toast.makeText(activity as WelcomeActivity, "Error: " + it.exception?.message, Toast.LENGTH_LONG).show()
            }
        }
    }
}